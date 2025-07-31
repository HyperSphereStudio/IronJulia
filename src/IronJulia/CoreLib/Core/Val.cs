using System.Reflection;
using System.Reflection.Emit;
using IronJulia.Compiler;
using IronJulia.Emit;
using static Base;
using static System.Reflection.MethodAttributes;

namespace IronJulia.CoreLib;

public static class Vals {
    public struct AtomicSymbol : Val<Symbol> {
        public static Symbol Value => "atomic";
    }

    public struct NotAtomicSymbol : Val<Symbol> {
        public static Symbol Value => "not_atomic";
    }
    
    public struct Int0 : Val<Int> {
        public static Int Value => 0;
    }

    public struct Int1 : Val<Int> {
        public static Int Value => 1;
    }

    public struct Int2 : Val<Int> {
        public static Int Value => 2;
    }

    public struct Int3 : Val<Int> {
        public static Int Value => 3;
    }

    private static readonly Dictionary<object, Type> ValMap = new();
    
    public static Type GetFixedVal(Any value) => GetFixedVal(value, value.GetType());
    
    public static Type GetFixedVal(Any value, Type valueType) {
        if (!ValMap.TryGetValue(value, out var t)) {
            //TODO: Make sure to check if its unmanaged or string type for now
            
            var ty = global::Core.Package.Module.DefineType("Val" + ValMap.Count, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.SequentialLayout,
                typeof(ValueType));
            var valType = typeof(Val<>).MakeGenericType(valueType);
            ty.AddInterfaceImplementation(valType);
            var method = ty.DefineMethod("get_Value", Public | Static | HideBySig | SpecialName, valueType, Type.EmptyTypes);
            ty.DefineMethodOverride(method, valType.GetMethod("get_Value")!);
            
            var cg = new CodeGenerator(method);
            
            if (!ILSerializer.Serialize(ty, cg, value)) 
                throw new Exception("Value:" + value + " is not able to be used by Val");
            cg.il.Emit(OpCodes.Ret);
            
            t = ty.CreateType();
            
            ValMap[value] = t;
        }
            
        return t;
    }
    
    static Vals() {
        ValMap.Add(AtomicSymbol.Value, typeof(AtomicSymbol));
        ValMap.Add(NotAtomicSymbol.Value, typeof(NotAtomicSymbol));
        ValMap.Add(Int0.Value, typeof(Int0));
        ValMap.Add(Int1.Value, typeof(Int1));
        ValMap.Add(Int2.Value, typeof(Int2));
        ValMap.Add(Int3.Value, typeof(Int3));
    }
}