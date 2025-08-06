global using RuntimeJulianCallsite = JulianCallsite<RuntimeValue, object?>;

using System.Reflection;
using IronJulia.CoreLib.Interop;
using Microsoft.Extensions.ObjectPool;
using SpinorCompiler.Utils;

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

public enum MethodToCallSiteMatch {
    Partial,
    Exact,
    NoMatch
}

public partial struct Core
{
    public sealed class Function : IBinding {
        public Base.Symbol Name { get; }
        public Core.Module Module { get; }
        public FieldAttributes Attributes => FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly;
        public Type BindingType => typeof(Function);
        internal SortedSet<Method> MethodSet { get; }
        public ICollection<Method> Methods => MethodSet;
        
        public Function(Base.Symbol name, Module module) {
            Name = name;
            Module = module;
            var mts = new SortedSet<Method>(MethodComparer.Instance);
            MethodSet = mts;
        }

        public void AddMethod(Method m) {
            lock (MethodSet) {
                m.UniqueID = MethodSet.Count;
                MethodSet.Add(m);
            }
        }

        internal class MethodComparer : IComparer<Method> {
            public static readonly MethodComparer Instance = new();
            public int Compare(Method? x, Method? y) {
                var c = x!.Specialization.CompareTo(y!.Specialization);
                if(c == 0 && !ReferenceEquals(x, y))
                    return x.UniqueID.CompareTo(y.UniqueID);
                return c;
            }
        }

        public object? GetValue(object? instance) => this;
        public void SetValue(object? instance, object? value) => throw new NotSupportedException();

        public object? Invoke(JulianCallsite<RuntimeValue, object?> site) {
            var nm = SelectMethod(site);
            if (nm == null)
                throw new Exception("Unable to find method for " + Name + " given callsite");
            return nm.Invoke(site);
        }

        public Method? SelectMethod<TCVal, TVal>(JulianCallsite<TCVal, TVal> site) where TCVal : ICallsiteValue<TCVal, TVal> {
            var at = site.Values.Span;
            var kat = site.KeyValues.Span;
            var kn = site.KeyArgNames.Span;
            lock (MethodSet) {
                foreach (var m in MethodSet) {
                    if (m.Match<TCVal, TVal>(at, kat, kn) == MethodToCallSiteMatch.Exact)
                        return m;
                }
            }
            return null;
        }
        
    }

    public abstract class Method(Function function) : Base.IAny {
        public readonly Function Function = function;
        public int Specialization { get; protected set; }
        internal int UniqueID { get; set; }
        public abstract Type ReturnType { get; }
        public abstract MethodToCallSiteMatch Match<TCVal, TVal>(Span<TCVal> args, Span<TCVal> kargs, Span<Base.Symbol> knames) where TCVal : ICallsiteValue<TCVal, TVal>;
        public abstract object? Invoke(Span<RuntimeValue> args, Span<RuntimeValue> kargs, Span<Base.Symbol> knames);
        public object? Invoke(RuntimeJulianCallsite site) => Invoke(site.Values.Span, site.KeyValues.Span, site.KeyArgNames.Span);
        public abstract Delegate CreateDelegate(Type t);
        public abstract T CreateDelegate<T>() where T: Delegate;
    }
}

public interface ICallsiteValue<TCVal, TValue> where TCVal : ICallsiteValue<TCVal, TValue> {
    public TValue Value { get; }
    public Type Type { get; }
}

public record struct RuntimeValue(object? Value, Type Type) : ICallsiteValue<RuntimeValue, object?> {
    public RuntimeValue(object? value) : this(value, value?.GetType() ?? typeof(object)){}
}

public class JulianCallsite<TCVal, TVal> where TCVal: ICallsiteValue<TCVal, TVal> {
    public static readonly DefaultObjectPool<JulianCallsite<TCVal, TVal>> SharedPool = new(new DefaultPooledObjectPolicy<JulianCallsite<TCVal, TVal>>());
    internal InlinedList<Base.Symbol> KeyArgNames = new();
    internal InlinedList<TCVal> KeyValues = new();
    internal InlinedList<TCVal> Values = new();

    public static JulianCallsite<TCVal, TVal> Get() => SharedPool.Get();

    public void Return()
    {
        Reset();
        SharedPool.Return(this);
    }
    
    public JulianCallsite<TCVal, TVal> ApplyKeyArgs(Dictionary<Base.Symbol, TCVal> kargs) {
        foreach (var m in kargs)
            AddKeyArg(m.Key, m.Value);
        return this;
    }
    
    public JulianCallsite<TCVal, TVal> AddArg(TCVal value) {
        Values.Add(value);
        return this;
    }

    public JulianCallsite<TCVal, TVal> AddKeyArg(Base.Symbol name, TCVal value) {
        KeyArgNames.Add(name);
        KeyValues.Add(value);
        return this;
    }

    public JulianCallsite<TCVal, TVal> Reset() {
        KeyArgNames.Clear();
        KeyValues.Clear();
        Values.Clear();
        return this;
    }
}
