using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using IronJulia.CoreLib.Interop;

public partial struct Core {
    public interface Module : IBinding, Base.Any {
        public Module? Parent { get; }
        Core.Module IBinding.Module => this;
        Type IBinding.BindingType => typeof(NetRuntimeNamespaceModule);
        FieldAttributes IBinding.Attributes => FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly;
        
        public object? this[params Span<Base.Symbol> names] {
            get => getglobal(names);
            set => setglobal(value, names);
        }
        
        public object? getglobal(params Span<Base.Symbol> names) {
            if (TryGetBinding(out var b, FieldAttributes.Public | FieldAttributes.Static, names))
                return b.GetValue(null);
            throw new KeyNotFoundException(string.Join(".", names.ToArray()));
        }
        
        public void setglobal(object? value, params Span<Base.Symbol> names) {
            if (!TryGetBinding(out var b, FieldAttributes.Public | FieldAttributes.Static, names))
                throw new KeyNotFoundException(string.Join(".", names.ToArray()));
            b.SetValue(null, value);
        }
        
        public bool TryGetBinding([NotNullWhen(true)] out IBinding? value, FieldAttributes fattr, params Span<Base.Symbol> names);
        object? IBinding.GetValue(object? instance) => this;
        void IBinding.SetValue(object? instance, object? value) => throw new NotSupportedException();
    }
}

namespace IronJulia.CoreLib {
    public class JulianRuntimeModule(Base.Symbol name, Core.Module? parent) : Core.Module {
        public readonly Dictionary<Base.Symbol, Core.Module> Imports = new();
        public readonly HashSet<Core.Module> Usings = [];
        private readonly Dictionary<Base.Symbol, IBinding> _bindings = new();
        public Base.Symbol Name { get; } = name;
        public Core.Module? Parent { get; } = parent;

        public void AddImport(Core.Module module, Base.Symbol? name = null) {
            name ??= module.Name;
            Imports[name.Value] = module;
        }
        
        public void AddUsing(Core.Module module) {
            Usings.Add(module);
        }
        
        public T AddBinding<T>(Base.Symbol name, T binding) where T : IBinding {
            _bindings.Add(name, binding);
            return binding;
        }

        public bool TryGetBinding([NotNullWhen(true)] out IBinding? value, FieldAttributes fattr, params Span<Base.Symbol> names) {
            value = null;
            if (names.Length < 0)
                return false;

            if (names.Length == 1) {
                if (_bindings.TryGetValue(names[0], out value))
                    return true;
                foreach (var u in Usings) {
                    if (u.Name == names[0]) {
                        value = u;
                        return true;
                    }
                }
                return false;
            }

            if (Imports.TryGetValue(names[0], out var imp)) {
                if (imp.TryGetBinding(out value, fattr, names[1..]))
                    return true;
            }

            foreach (var u in Usings) {
                if(u.TryGetBinding(out value, fattr, names))
                    return true;
            }
            
            return false;
        }
    }
}


