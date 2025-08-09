using System.Buffers;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using IronJulia.CoreLib.Interop;
using IronJulia.Utils;
using Microsoft.Extensions.ObjectPool;
using SpinorCompiler.Boot;

public static class JuliaTypeSpecializer
{
    private static readonly Dictionary<Type, int> TypeToSpecialization = new();

    public static int GetTypeSpecialization(Type type)
    {
        if (TypeToSpecialization.TryGetValue(type, out var s))
            return s;

        int score;

        if (type.IsInterface)
            score = GetInterfaceDepth(type);
        else
            score = GetInheritanceDepth(type);

        if (type.IsValueType)
            score += 1;

        if (type is { IsGenericType: true, ContainsGenericParameters: false })
            score += 1; // Closed generics more specific

        if (type.IsInterface)
            score -= 1; // Slightly less specific than a concrete class

        TypeToSpecialization[type] = score;

        return score;
    }

    private static int GetInterfaceDepth(Type type)
    {
        if (!type.IsInterface) return 0;
        return GetInterfaceDepthRecursive(type, new HashSet<Type>());
    }

    private static int GetInterfaceDepthRecursive(Type type, HashSet<Type> visited)
    {
        if (!visited.Add(type)) return 0;
        var parents = type.GetInterfaces();
        if (parents.Length == 0) return 0;
        int maxDepth = 0;
        foreach (var parent in parents)
        {
            int parentDepth = 1 + GetInterfaceDepthRecursive(parent, visited);
            if (parentDepth > maxDepth)
                maxDepth = parentDepth;
        }

        return maxDepth;
    }

    private static int GetInheritanceDepth(Type type)
    {
        int depth = 0;
        Type? current = type;
        while (current != null && current.BaseType != null)
        {
            depth++;
            current = current.BaseType;
        }

        return depth;
    }
}

public enum MethodToCallSiteMatch
{
    Partial,
    Exact,
    NoMatch
}

public partial struct Core
{
    public sealed class Function : IBinding
    {
        public Base.Symbol Name { get; }
        public Core.Module Module { get; }
        public FieldAttributes Attributes => FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly;
        public Type BindingType => typeof(Function);
        internal SortedSet<Method> MethodSet { get; }
        public ICollection<Method> Methods => MethodSet;
        private int _uniqueMethodCnt = 0;

        public Function(Base.Symbol name, Module module)
        {
            Name = name;
            Module = module;
            var mts = new SortedSet<Method>(MethodComparer.Instance);
            MethodSet = mts;
        }

        public void AddMethod(Method m)
        {
            lock (MethodSet)
            {
                m.uniqueID = _uniqueMethodCnt++;
                MethodSet.Add(m);
            }
        }

        internal class MethodComparer : IComparer<Method>
        {
            public static readonly MethodComparer Instance = new();

            public int Compare(Method? x, Method? y)
            {
                var c = x!.sig.Specialization.CompareTo(y!.sig.Specialization);
                if (c == 0 && !ReferenceEquals(x, y))
                    return x.uniqueID.CompareTo(y.uniqueID);
                return c;
            }
        }

        public object? GetValue(object? instance) => this;
        public void SetValue(object? instance, object? value) => throw new NotSupportedException();

        public object? Invoke(JulianRuntimeCallsite site) {
            var nm = SelectMethod(site);
            if (nm == null)
                throw new Exception("Unable to find method for " + Name + " given callsite");
            return nm.Invoke(site);
        }

        public MethodInstance? SelectMethod(JulianRuntimeCallsite site) {
            lock (MethodSet) {
                using var il = new InlinedList<Type?, PoolAllocator>();
                foreach (var m in MethodSet) {
                    il.ResizeBuffer(m.nspecargs);
                    il.Clear();
                    if (m.Match(site, il.Span) == MethodToCallSiteMatch.Exact)
                        return m.GetMethodInstance(site, new(Unsafe.As<Type[]>(il.Compact())));
                }
            }
            return null;
        }
    }

    public readonly record struct ParamInfo(
        Base.Symbol Name,
        Type ParameterType,
        object? DefaultValue,
        bool IsOptional)
    {
        public ParamInfo(ParameterInfo info) : this(info.Name, info.ParameterType, info.DefaultValue, info.IsOptional)
        {
        }
    }

    public readonly struct MethodSignature : IEquatable<MethodSignature>
    {
        public readonly ParamInfo[] Parameters;
        public int Specialization { get; }
        public int HashCode { get; }

        public MethodSignature(ParamInfo[] parameters) {
            Parameters = parameters;

            foreach (var p in parameters) {
                Specialization += JuliaTypeSpecializer.GetTypeSpecialization(p.ParameterType);
                HashCode = System.HashCode.Combine(Specialization, HashCode);
            }
        }


        [UnsafeAccessor(UnsafeAccessorKind.Method)]
        private static extern ReadOnlySpan<ParameterInfo> GetParametersAsSpan(MethodBase? info);

        public MethodSignature(MethodInfo info)
        {
            var ps = GetParametersAsSpan(info);
            Parameters = new ParamInfo[ps.Length];
            for (var i = 0; i < ps.Length; i++)
            {
                Parameters[i] = new ParamInfo(ps[i]);
                Specialization += JuliaTypeSpecializer.GetTypeSpecialization(ps[i].ParameterType);
            }

            HashCode = System.HashCode.Combine(Specialization, HashCode);
        }

        public bool Equals(MethodSignature other) {
            if (other.HashCode != HashCode)
                return false;
            for (var i = 0; i < Parameters.Length; i++) {
                var p = Parameters[i];
                if (p != other.Parameters[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode() => HashCode;
    }

    public abstract class Method : Base.IAny {
        public readonly Base.Symbol name;
        public readonly Module module;
        public readonly MethodSignature sig;
        public object source;
        public readonly int nspecargs;
        public readonly int primary_world;
        public int uniqueID { get; internal set; }
        public readonly Dictionary<TypeTuple, MethodInstance> specializations = new();

        public Method(Base.Symbol name, Module module, MethodSignature sig, object source, int nspecargs, int primary_world) {
            this.name = name;
            this.module = module;
            this.sig = sig;
            this.source = source;
            this.nspecargs = nspecargs;
            this.primary_world = primary_world;
        }

        public MethodInstance GetMethodInstance(JulianCompilerCallsite callsite, TypeTuple specArgs) {
            if (!specializations.TryGetValue(specArgs, out var m)) {
                m = CreateMethodInstance(callsite, specArgs);
                specializations.Add(specArgs, m);
            }
            return m;
        }
        protected abstract MethodInstance CreateMethodInstance(JulianCompilerCallsite callsite, TypeTuple specArgs);
        protected abstract bool CanAssignTo(Type from, Type to, Span<Type?> specArgs);
        public MethodToCallSiteMatch Match(JulianCompilerCallsite callsite, Span<Type?> specArgs) {
            var knames = callsite.KeyArgNames.Span;
            var kargs = callsite.KArgTypes.Span;
            var args = callsite.ArgTypes.Span;

            foreach (var p in sig.Parameters) {
                var pidx = knames.IndexOf(p.Name!);
                if (pidx != -1) {
                    if (!CanAssignTo(kargs[pidx], p.ParameterType, specArgs))
                        return MethodToCallSiteMatch.NoMatch;
                }
                else
                {
                    if (args.Length > 0) {
                        if (!CanAssignTo(args[0], p.ParameterType, specArgs))
                            return MethodToCallSiteMatch.NoMatch;
                        args = args[1..]; //Next
                    }
                    else if (!p.IsOptional)
                        return MethodToCallSiteMatch.NoMatch;
                }
            }

            //To many args remaining
            if (args.Length != 0)
                return MethodToCallSiteMatch.NoMatch;

            return MethodToCallSiteMatch.Exact;
        }
    }

    public class ConcreteMethod(Base.Symbol name, Module module, MethodSignature sig, object source, int primary_world, MethodInstance methodInstance) : 
        Method(name, module, sig, source, 0, primary_world) {
        public readonly MethodInstance MethodInstance = methodInstance;
        protected override MethodInstance CreateMethodInstance(JulianCompilerCallsite callsite, TypeTuple specArgs) => MethodInstance;
        protected override bool CanAssignTo(Type from, Type to, Span<Type?> specArgs) => from.IsAssignableFrom(to);

        internal static ConcreteMethod FromInfo(MethodInfo info) {
            var sig = new MethodSignature(info);
            var mi = new RuntimeNetMethodInstance(info, null);
            var cm = new ConcreteMethod(info.Name, NetType.GetOrCreateModuleForType(info.DeclaringType!), sig, null!, 0, mi);
            mi.Method = cm;
            return cm;
        }
    }

    public class NetRuntimeGenericMethod : Method {
        public readonly MethodInfo GenericMethodDef;
        public readonly Type[] GenericArguments;


        public NetRuntimeGenericMethod(Base.Symbol name, Module module, MethodSignature sig, object source, int primary_world, 
            MethodInfo genericMethodDef, Type[] genericArguments) : 
            base(name, module, sig, source, genericArguments.Length, primary_world) {
            Debug.Assert(genericMethodDef.IsGenericMethodDefinition == true);
            GenericMethodDef = genericMethodDef;
            GenericArguments = genericMethodDef.GetGenericArguments();
        }

        protected override MethodInstance CreateMethodInstance(JulianCompilerCallsite callsite, TypeTuple specArgs) {
            return new RuntimeNetMethodInstance(GenericMethodDef.MakeGenericMethod(specArgs.Types), this);
        }

        protected override bool CanAssignTo(Type from, Type to, Span<Type?> specArgs) {
            throw new NotImplementedException();
        }
        
    }

    public abstract class MethodInstance(Method method) {
        public Method Method { get; internal set; } = method;
        public abstract object? Invoke(JulianRuntimeCallsite callsite);
    }

    public class RuntimeNetMethodInstance : MethodInstance {
        public readonly MethodInfo Info;

        public RuntimeNetMethodInstance(MethodInfo info, Method? method) : base(method!) {
            Debug.Assert(info.IsGenericMethodDefinition == false);
            Info = info;
        }

        public override object? Invoke(JulianRuntimeCallsite callsite)
        {
            var pspan = Method.sig.Parameters.AsSpan();
            var args = callsite.ArgValues.Span;
            var knames = callsite.KeyArgNames.Span;
            var kargs = callsite.KArgValues.Span;
            var fullArgs = new object?[pspan.Length];
            var fargs = fullArgs.AsSpan();

            foreach (var p in pspan)
            {
                var pidx = knames!.IndexOf(p.Name!);
                if (pidx != -1)
                    fargs[0] = kargs[pidx];
                else
                {
                    if (args.Length > 0)
                    {
                        fargs[0] = args[0];
                        args = args[1..]; //Next
                    }
                    else if (p.IsOptional)
                        fargs[0] = p.DefaultValue;
                    else
                        throw new Exception();
                }

                fargs = fargs[1..]; //Next
            }

            return Info.Invoke(null, fullArgs);
        }
    }
}

public class JulianCompilerCallsite
{
    internal InlinedList<Base.Symbol, DefaultAllocator> KeyArgNames = new();
    internal InlinedList<Type, DefaultAllocator> ArgTypes = new();
    internal InlinedList<Type, DefaultAllocator> KArgTypes = new();

    public JulianCompilerCallsite()
    {
    }

    public JulianCompilerCallsite AddArgType(Type argType)
    {
        ArgTypes.Add(argType);
        return this;
    }

    public JulianCompilerCallsite AddKeyArgType(Base.Symbol name, Type argType)
    {
        KeyArgNames.Add(name);
        KArgTypes.Add(argType);
        return this;
    }

    public virtual void Reset()
    {
        KeyArgNames.Clear();
        ArgTypes.Clear();
        KArgTypes.Clear();
    }
}

public class JulianRuntimeCallsite : JulianCompilerCallsite
{
    private static readonly DefaultObjectPool<JulianRuntimeCallsite> _pool = new(new DefaultPooledObjectPolicy<JulianRuntimeCallsite>());
    internal InlinedList<object?, DefaultAllocator> ArgValues = new();
    internal InlinedList<object?, DefaultAllocator> KArgValues = new();

    public JulianRuntimeCallsite() {}
    
    public static JulianRuntimeCallsite GetFromPool() => _pool.Get();

    public static void ReturnToPool(JulianRuntimeCallsite callsite) {
        callsite.Reset();
        _pool.Return(callsite);
    }
    
    public JulianRuntimeCallsite ApplyKeyArgs(Dictionary<Base.Symbol, object> kargs)
    {
        foreach (var m in kargs)
        {
            AddKeyArg(m.Key, m.Value);
        }

        return this;
    }

    public JulianRuntimeCallsite AddArg(object? value, Type? type = null)
    {
        ArgValues.Add(value);
        type ??= value?.GetType() ?? typeof(object);
        AddArgType(type);
        return this;
    }

    public JulianRuntimeCallsite AddKeyArg(Base.Symbol name, object? value, Type? type = null)
    {
        type ??= value?.GetType() ?? typeof(object);
        KArgValues.Add(value);
        AddKeyArgType(name, type);
        return this;
    }

    public override void Reset() {
        base.Reset();
        ArgValues.Clear();
        KArgValues.Clear();
    }
}