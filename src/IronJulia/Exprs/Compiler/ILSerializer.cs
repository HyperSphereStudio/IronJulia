using System.Reflection;
using System.Reflection.Emit;
using IronJulia.Emit;

namespace IronJulia.Compiler;

public class ILSerializer {
    public static bool Serialize(TypeBuilder tb, CodeGenerator cg, object? value) {
        switch (value) {
            case Base.Symbol s:
                cg.il.EmitString(s.Value);
                cg.il.EmitNew(CachedJuliaInfos.Symbol_Cctor);
                return true;
            case Base.IAny: {
                if (!value.GetType().IsValueType)
                    return false;
                
                var lb = cg.GetLocal(value.GetType());
                foreach (var f in value.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance |
                                                            BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)) {
                    if (f.IsPublic) {
                        cg.il.Emit(OpCodes.Ldloca, lb);
                        if (!Serialize(tb, cg, f.GetValue(value)))
                            return false;
                        
                        cg.il.EmitFieldSet(f);
                    }else {
                        cg.il.Emit(OpCodes.Ldloca, lb);
                        var fieldAccess = tb.EmitGetFieldRefUnsafeAccessor(CompilerUtils.GetNestedMethodName(cg.InternalMethod, "field", 1), f);
                        cg.il.EmitCall(OpCodes.Call, fieldAccess, null);
                        if (!Serialize(tb, cg, f.GetValue(value)))
                            return false;
                        cg.il.EmitStoreValueIndirect(f.FieldType);
                    }
                }
                cg.il.Emit(OpCodes.Ldloc, lb);
                cg.FreeLocal(lb);
                
                return true;
            }
            default:
                return cg.il.TryEmitConstant(value, value?.GetType() ?? typeof(Base.Nothing), cg);
        }
    }
}