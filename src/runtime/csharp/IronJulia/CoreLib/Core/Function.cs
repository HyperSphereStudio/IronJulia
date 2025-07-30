using System.Buffers;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using IronJulia.CoreLib.Interop;
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

        public object? Invoke(Callsite site) {
            var nm = SelectMethod(site);
            if (nm == null)
                throw new Exception("Unable to find method for " + Name + " given callsite");
            return nm.Invoke(site.Values.Span, site.KeyValue.Span, site.KeyArgNames.Span);
        }

        public Method? SelectMethod(Callsite site) {
            var at = site.ValueTypes.Span;
            var kat = site.KeyValueTypes.Span;
            var kn = site.KeyArgNames.Span;
            lock (MethodSet) {
                foreach (var m in MethodSet) {
                    if (m.Match(at, kat, kn) == MethodToCallSiteMatch.Exact)
                        return m;
                }
            }
            return null;
        }
    }

    public abstract class Method(Function function) : Base.Any {
        public readonly Function Function = function;
        public int Specialization { get; protected set; }
        internal int UniqueID { get; set; }
        public abstract Type ReturnType { get; }
        public abstract MethodToCallSiteMatch Match(Span<Type> argTypes, Span<Type> kargTypes, Span<string> knames);
        public abstract object? Invoke(Span<object?> args, Span<object?> kargs, Span<string> knames);
    }
}

public class Callsite {
    internal InlinedList<string> KeyArgNames = new();
    internal InlinedList<object?> KeyValue = new();
    internal InlinedList<object?> Values = new();
    internal InlinedList<Type> ValueTypes = new();
    internal InlinedList<Type> KeyValueTypes = new();
    
    public void ApplyKeyArgs(Dictionary<string, object?> kargs) {
        foreach (var m in kargs)
            AddKeyArg(m.Key, m.Value);
    }
    
    public void AddArg(object? value, Type? valueType = null) {
        Values.Add(value);
        ValueTypes.Add(value?.GetType() ?? valueType ?? typeof(object));
    }

    public void AddKeyArg(string name, object? value, Type? keyArgType = null) {
        KeyArgNames.Add(name);
        KeyValue.Add(value);
        KeyValueTypes.Add(value?.GetType() ?? keyArgType ?? typeof(object));
    }

    public void Reset() {
        KeyArgNames.Clear();
        KeyValue.Clear();
        Values.Clear();
        ValueTypes.Clear();
        KeyValueTypes.Clear();
    }
}


