using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

public partial struct Base {
    
    public readonly record struct Bool(bool Value) : Integer {
        public static readonly object True = new Bool(true);
        public static readonly object False = new Bool(false);
        public static implicit operator Bool(bool value) => new(){Value=value};
        public static implicit operator bool(Bool value) => value.Value;
        public override string ToString() => Value.ToString();
    }
    
    public interface Val<T> : IAny{
        public static abstract T Value { get; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 0)]
    public struct Nothing : IAny {
        public static readonly IAny Instance = new Nothing();
    }
    
    public interface IAny : IDynamicMetaObjectProvider {
        private static readonly IAny DefaultProvider = new Int(0);

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter) {
        
            return new JuliaMetaObject(parameter, BindingRestrictions.Empty, this);
        }
        
        public class JuliaMetaObject(Expression expression, BindingRestrictions restrictions, object value)
            : DynamicMetaObject(expression, restrictions, value) {
        }
    }
}