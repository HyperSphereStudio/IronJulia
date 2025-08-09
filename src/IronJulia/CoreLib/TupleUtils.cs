using System.Buffers;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Scripting.Utils;
using SpinorCompiler.Utils;
using DynamicMethod = System.Reflection.Emit.DynamicMethod;

namespace SpinorCompiler.Boot;

using Type = System.Type;

public readonly struct TypeTuple : IEquatable<TypeTuple>
{
    public readonly Type[] Types;
    private readonly int _hash;

    public TypeTuple(params Type[] types) {
        Types = types;
        foreach (var t in Types) {
            _hash = HashCode.Combine(t, _hash);
        }
    }
    
    public bool Equals(TypeTuple other) => Types.SequenceEqual(other.Types);

    public override int GetHashCode() => _hash;
}

public static class JuliaTupleUtils {
    private static readonly Type[] NativeTuple_types = [
        typeof(ValueTuple), typeof(ValueTuple<>), typeof(ValueTuple<,>), typeof(ValueTuple<,,>), 
        typeof(ValueTuple<,,,>), typeof(ValueTuple<,,,,>), typeof(ValueTuple<,,,,,>), typeof(ValueTuple<,,,,,,>),
        typeof(ValueTuple<,,,,,,,>)
    ];
    internal static Dictionary<TypeTuple, Type> _tuples2type = new();
    internal static Dictionary<Type, TypeTuple> _type2tuples = new();
    private static Dictionary<TypeTuple, Delegate> _tuplectors = new();
    public static Core.Tuple<ValueTuple> NewTuple() => new();
    public static Core.Tuple<ValueTuple<T1>> NewTuple<T1>(T1 t1) => new (new ValueTuple<T1>(t1));
    public static Core.Tuple<(T1, T2)> NewTuple<T1, T2>(T1 t1, T2 t2) => new ((t1, t2));
    public static Core.Tuple<(T1, T2, T3)> NewTuple<T1, T2, T3>(T1 t1, T2 t2, T3 t3) => new ((t1, t2, t3));
    public static Core.Tuple<(T1, T2, T3, T4)> NewTuple<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) => new ((t1, t2, t3, t4));
    public static Core.Tuple<(T1, T2, T3, T4, T5)> NewTuple<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => new ((t1, t2, t3, t4, t5));
    public static Core.Tuple<(T1, T2, T3, T4, T5, T6)> NewTuple<T1, T2, T3, T4, T5, T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => new ((t1, t2, t3, t4, t5, t6));
    public static Core.Tuple<(T1, T2, T3, T4, T5, T6, T7)> NewTuple<T1, T2, T3, T4, T5, T6, T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) => new ((t1, t2, t3, t4, t5, t6, t7));

    public static Type GetTupleType(Type[] tupleOfTypes) {
        if (!_tuples2type.TryGetValue(new TypeTuple(tupleOfTypes), out var tt)) {
            tt = typeof(Core.Tuple<>).MakeGenericType(GetTupleNetType(tupleOfTypes));
            RuntimeHelpers.RunClassConstructor(tt.TypeHandle);        //Sets the tuples2type stuff
        }
        return tt;
    }

    public static Delegate GetTupleCtor(Type[] tupleOfTypes) {
        var tt = new TypeTuple(tupleOfTypes);
        if (!_tuplectors.TryGetValue(tt, out var mi)) {
            mi = EmitTupleMaker(tupleOfTypes, GetTupleNetType(tupleOfTypes));
            _tuplectors[tt] = mi;
        }
        return mi;
    }

    private static Type GetTupleNetType(Span<Type> types) {
        if (types.Length == 0)
            return typeof(ValueTuple);
        var tybuffer = new Type[Math.Min(types.Length, 8)];
        for(int i = 0, n = types.Length > 7 ? 7 : types.Length; i < n; i++)
            tybuffer[i] = types[i];
        if (types.Length > 7)
            tybuffer[7] = GetTupleNetType(types[7..]);
        return NativeTuple_types[tybuffer.Length].MakeGenericType(tybuffer);
    }

    private static void EmitTuple(Type tt, ILGenerator ilg, int pOffset) {
        if (tt != typeof(ValueTuple)) {
            var targs = tt.GetGenericArguments();
            for(int i = 0, n = Math.Min(7, targs.Length); i < n; i++, pOffset++) {
                ilg.Emit(OpCodes.Ldarg, pOffset);
            }
            if (targs.Length == 8)
                EmitTuple(targs[7], ilg, pOffset);
        
            ilg.Emit(OpCodes.Newobj, tt.GetConstructors()[0]);
        }
        else {
            ilg.Emit(OpCodes.Ldloc, ilg.DeclareLocal(typeof(ValueTuple)));
        }
    }
    
    private static Delegate EmitTupleMaker(Type[] parameters, Type tupleType) {
        Type[] tptyargs = [tupleType];
        var jlType = typeof(Core.Tuple<>).MakeGenericType(tptyargs);
        var met = new DynamicMethod("CreateTuple",
            MethodAttributes.Public | MethodAttributes.Static, 
            CallingConventions.Standard, 
            jlType, parameters, typeof(JuliaTupleUtils),
            true);
        
        var ilg = met.GetILGenerator();
        EmitTuple(tupleType, ilg, 0);
        ilg.Emit(OpCodes.Newobj, jlType.GetConstructor(tptyargs)!);
        ilg.Emit(OpCodes.Ret);
        
        return met.CreateDelegate(Expression.GetFuncType([..parameters, jlType]));
    }
    
    public static Core.JTuple NewTuple(object[] values, Type tupleType) {
        RuntimeHelpers.RunClassConstructor(tupleType.TypeHandle);
        return Unsafe.As<Core.JTuple>(GetTupleCtor(_type2tuples[tupleType].Types).DynamicInvoke(Unsafe.As<object?[]>(values)))!;
    }

    public static Core.JTuple NewTuple(object[] values, params Type[] types) => NewTuple(values, GetTupleType(types));
    
    public static Core.JTuple NewTuple(params object[] values) {
        var tybuffer = new Type[values.Length];
        for(int i = 0, n = values.Length; i < n; i++)
            tybuffer[i] = values[i].GetType();
        return Unsafe.As<Core.JTuple>(GetTupleCtor(tybuffer).DynamicInvoke(Unsafe.As<object?[]>(values)))!;    
    }
    
    public static Span<T> NTupleSpan<T, K>(ref K t) where K: Core.JTuple {
        return MemoryMarshal.CreateSpan(ref Unsafe.As<K, T>(ref t), t.Length);
    }
}
