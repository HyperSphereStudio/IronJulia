using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IronJulia.CoreLib.Interop;

public class NetRuntimeNamespaceModule : Core.Module {
    public Base.Symbol Name { get; }
    public Core.Module? Parent { get; }
    public readonly string Namespace;
    public readonly Dictionary<Base.Symbol, NetRuntimeNamespaceModule> ChildrenNameSpaces = new();
    private readonly Dictionary<Base.Symbol, IBinding> _bindingCache = new();

    public NetRuntimeNamespaceModule(Base.Symbol name, string @namespace, Core.Module? parent) {
        Parent = parent;
        Namespace = @namespace;
        Name = name;
        ChildrenNameSpaces.Add(name, this);
    }
    
    public bool TryGetBinding([NotNullWhen(true)] out IBinding? value, FieldAttributes fattr, params Span<Base.Symbol> names) {
        lock (_bindingCache) {
            value = null;
            if (names.Length < 1)
                return false;
        
            var child = ChildrenNameSpaces.TryGetValue(names[0], out var ns);
        
            if (names.Length == 1 && child) {
                value = ns;
#pragma warning disable CS8762
                return true;
#pragma warning restore CS8762
            }
        
            if (child)
                return ns!.TryGetBinding(out value, fattr, names[1..]);

            if (_bindingCache.TryGetValue(names[0], out value))
                return true;
        
            if (names.Length == 1) {
                var ty = Type.GetType(Namespace + "." + names[0]);
                if (ty != null) {
                    value = new NetRuntimeType(ty, this);
                    _bindingCache.Add(names[0], value);
                    return true;
                }
            }
          
            return false;
        }
    }
}

public static class NetType {
    private static readonly ConditionalWeakTable<Type, Core.Module> Type2Module = new();

    public static Core.Module RegisterType2Module(Type t, Core.Module m) {
        if (TryGetModuleFromType(t, out _) || !Type2Module.TryAdd(t, m)) {
            throw new NotSupportedException($"Type {t.FullName} is already registered.");
        }
        return m;
    }

    public static bool TryGetModuleFromType(Type t, out Core.Module? m) => Type2Module.TryGetValue(t, out m);

    public static Core.Module GetOrCreateModuleForType(Type t) {
        if (TryGetModuleFromType(t, out var m))
            return m!;
        var parent = t.DeclaringType != null ? GetOrCreateModuleForType(t.DeclaringType) : null;
        return RegisterType2Module(t, new NetRuntimeType(t, parent));
    }
}

public class NetRuntimeType : Core.Module {
    public readonly Type Source;
    public Base.Symbol Name => Source.Name;
    public Core.Module Module => this;
    public Core.Module? Parent { get; }
    private readonly Dictionary<Base.Symbol, IBinding> _bindingCache = new();

    private static readonly Dictionary<string, Core.Function> NetOps = new() {
        ["op_Addition"] =  Base.op_Add,
        ["op_Subtraction"] =  Base.op_Sub,
        ["op_Multiplication"] =  Base.op_Mul,
        ["op_Division"] =  Base.op_Div,
        ["op_LessThan"] =  Base.op_LessThan,
        ["op_GreaterThan"] =  Base.op_GreaterThan,
        ["op_LessThanOrEqual"] =  Base.op_LessThanOrEqual,
        ["op_GreaterThanOrEqual"] =  Base.op_GreaterThanOrEqual,
        ["op_Equality"] = Base.op_Equality,
        ["op_Inequality"] = Base.op_InEquality
    };
    
    internal NetRuntimeType(Type source, Core.Module? parent) {
        Source = source;
        Parent = parent;
        
        //Load certain .net methods into ecosystem that are hard to discover
        foreach (var m in source.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
            if (m.IsSpecialName && NetOps.TryGetValue(m.Name, out var bop)) {
                bop.AddMethod(new NetMethod(bop, m));
            }
        }
    }

    public bool TryGetBinding([NotNullWhen(true)] out IBinding? value, FieldAttributes fattr, params Span<Base.Symbol> names) {
        lock (_bindingCache) {
            value = null;
            if (names.Length is 0 or > 1) 
                return false;
        
            if (_bindingCache.TryGetValue(names[0], out value))
                return true;
        
            var name = names[0].Value;
        
            value = null;
        
            var bf = BindingFlags.FlattenHierarchy;

            if (fattr.HasFlag(FieldAttributes.Public))
                bf |= BindingFlags.Public;
        
            if(fattr.HasFlag(FieldAttributes.Private))
                bf |= BindingFlags.NonPublic;
            
            if(fattr.HasFlag(FieldAttributes.Static))
                bf |= BindingFlags.Static;

            List<MethodInfo?>? methods = null;
            
            foreach (var w in Source.GetMember(name, bf)) {
                switch (w) {
                    case FieldInfo fi:
                        value = new FieldBinding(fi, this);
                        break;
                    case PropertyInfo pi:
                        value = new PropertyBinding(pi, this);
                        break;
                    case MethodInfo mi:
                        methods ??= new();
                        methods.Add(mi);
                        break;
                }
            }
        
            if (methods != null) {
                var f = new Core.Function(name, this);
                value = f;
                foreach (var m in methods)
                    f.Methods.Add(new NetMethod(f, m!));
            }
            
            if(value != null)
                _bindingCache.Add(name, value);

            return value != null;
        }
    }
}
