using System.Reflection;

namespace IronJulia.CoreLib.Interop;

public interface IBinding {
    public Base.Symbol Name { get; }
    public Core.Module Module { get; }
    public FieldAttributes Attributes { get; }
    public Type BindingType { get; }
    public object? GetValue(object? instance);
    public void SetValue(object? instance, object? value);
}
    
public record RuntimeGlobalBinding<T>(Base.Symbol Name, Core.Module Module, FieldAttributes Attributes, Type BindingType, T? SpecializedValue = default) : IBinding {
    public T? SpecializedValue = SpecializedValue;

    public object? GetValue(object? instance) => SpecializedValue;

    public void SetValue(object? instance, object? value) {
        if (Attributes.HasFlag(FieldAttributes.InitOnly))
            throw new NotSupportedException(Name + " is readonly!");
        SpecializedValue = (T?) value;
    }
}

public class FieldBinding : IBinding {
    public Base.Symbol Name => Field.Name;
    public Core.Module Module { get; }
    public FieldAttributes Attributes => Field.Attributes;
    public Type BindingType => Field.FieldType;
    public readonly FieldInfo Field;
    
    public object? GetValue(object? instance) => Field.GetValue(instance);
    public void SetValue(object? instance, object? value) => Field.SetValue(instance, value);
    
    public FieldBinding(FieldInfo field, Core.Module module) {
        Field = field;
        Module = module;
    }
}

public class PropertyBinding : IBinding
{
    public Base.Symbol Name => Property.Name;
    public Core.Module Module { get; }

    public FieldAttributes Attributes {
        get {
            FieldAttributes fa = 0;
            MethodInfo? method = Property.GetGetMethod(true) ?? Property.GetSetMethod(true);
            if (method == null)
                return fa;
            
            if(method.IsPrivate)
                fa |= FieldAttributes.Private;
            if(method.IsPublic)
               fa |= FieldAttributes.Public;
            if(method.IsStatic)
                fa |= FieldAttributes.Static;
            if(!Property.CanWrite)
                fa |= FieldAttributes.InitOnly;
            if(!Property.CanRead)
                fa |= FieldAttributes.Private;
            return fa;
        }
    }
    public Type BindingType => Property.PropertyType;
    public PropertyInfo Property;
    public object? GetValue(object? instance) => Property.GetValue(instance);
    public void SetValue(object? instance, object? value) => Property.SetValue(instance, value);

    public PropertyBinding(PropertyInfo property, Core.Module module) {
        Property = property;
        Module = module;
    }   
}



