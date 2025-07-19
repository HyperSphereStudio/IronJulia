using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using static IronJulia.Emit.CachedReflectionInfo;

namespace IronJulia.Compiler;

public static class CompilerUtils
{
    public static string GetNestedMethodName(MethodBase parentMethod, string methodName, int index) {
        return methodName;
    }

    public static void EmitCompilerGeneratedAttribute(this MethodBuilder mb) {
        mb.SetCustomAttribute(new CustomAttributeBuilder(CompilerGeneratedCctor, []));
    }

    public static void EmitUnsafeAccessorAttribute(this MethodBuilder mb, UnsafeAccessorKind kind, string name) {
        mb.SetCustomAttribute(new CustomAttributeBuilder(UnsafeAccessorCCtor, [kind], [UnsafeAccessorNameField], [name]));
    }
        
    public static MethodBuilder EmitGetFieldRefUnsafeAccessor(this TypeBuilder tb, string methodName, FieldInfo field) {
        var m = tb.DefineMethod(methodName,
            MethodAttributes.Assembly | MethodAttributes.Static | MethodAttributes.HideBySig,
            field.FieldType.MakeByRefType(), [field.DeclaringType.MakeByRefType()]);
            
        m.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.PreserveSig); //Sets flag for runtime
        EmitCompilerGeneratedAttribute(m);
        EmitUnsafeAccessorAttribute(m, UnsafeAccessorKind.Field, field.Name);
            
        return m;
    }
}