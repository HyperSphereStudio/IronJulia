using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

public partial struct Base {
    public interface Number : Any;
    public interface Real : Number;

    public interface AbstractFloat : Real;
    public interface Integer : Real;
    public interface Signed : Real;
    public interface Unsigned : Real;
    
    public readonly record struct Bool(bool Value) : Integer {
        public static readonly Any True = new Bool(true);
        public static readonly Any False = new Bool(false);
        
        public static implicit operator Bool(bool value) => new(){Value=value};
        public static implicit operator bool(Bool value) => value.Value;
        public override string ToString() => Value.ToString();
    }
    
    public interface Val<T> : Any{
        public static abstract T Value { get; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = 0)]
    public struct Nothing : Any {
        public static readonly Any Instance = new Nothing();
    }

    public interface Any : IDynamicMetaObjectProvider {
        private static readonly Any DefaultProvider = new Int(0);

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter) {
        
            return new JuliaMetaObject(parameter, BindingRestrictions.Empty, this);
        }
        
        public class JuliaMetaObject(Expression expression, BindingRestrictions restrictions, object value)
            : DynamicMetaObject(expression, restrictions, value) {
        }
    }
    
    
}