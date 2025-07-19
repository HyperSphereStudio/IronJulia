using System.Diagnostics.CodeAnalysis;
using System.Reflection;

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

public class NetRuntimeType : Core.Module {
    public readonly Type Source;
    public Base.Symbol Name => Source.Name;
    public Core.Module Module => this;
    public Core.Module? Parent { get; }
    private readonly Dictionary<Base.Symbol, IBinding> _bindingCache = new();
    
    public NetRuntimeType(Type source, Core.Module? parent) {
        Source = source;
        Parent = parent;
        
        //Load certain .net methods into ecosystem that are hard to discover
        foreach (var m in source.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
            if (m.IsSpecialName && m.Name.StartsWith("op_")) {
                switch (m.Name) {
                    case "op_Addition":
                        Base.op_Add.Methods.Add(new NetMethod(Base.op_Add, m));
                        break;
                    case "op_Subtraction":
                        Base.op_Sub.Methods.Add(new NetMethod(Base.op_Sub, m));
                        break;
                    case "op_Multiply":    
                        Base.op_Mul.Methods.Add(new NetMethod(Base.op_Mul, m));
                        break;
                    case "op_Divide":    
                        Base.op_Div.Methods.Add(new NetMethod(Base.op_Div, m));
                        break;
                }
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
        
            var bf = BindingFlags.FlattenHierarchy;

            if (fattr.HasFlag(FieldAttributes.Public))
                bf |= BindingFlags.Public;
        
            if(fattr.HasFlag(FieldAttributes.Private))
                bf |= BindingFlags.NonPublic;
            
            if(fattr.HasFlag(FieldAttributes.Static))
                bf |= BindingFlags.Static;

            List<MethodInfo>? methods = null;
            
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
                    f.Methods.Add(new NetMethod(f, m));
            }
        
            if(value != null)
                _bindingCache.Add(name, value);

            return value != null;
        }
    }
}
