using System.Collections;
using System.Runtime.CompilerServices;
using IronJulia.Compiler;

namespace IronJulia.CoreLib;

public static class jlapi
{
    private static readonly Dictionary<Type, object> _constantCache = new();
    
    public static Base.Symbol jl_create_sym(string symbol) => symbol;
    public static unsafe Base.Symbol jl_create_sym(char* symbol, int n) => jl_create_sym(new string(symbol, 0, n));
    public static object jl_box<T>(T value) {
        if (typeof(T) == typeof(object))
            return value;
        if (_constantCache.TryGetValue(typeof(T), out var cd)) {
            if (cd is Dictionary<T, object> d && d.TryGetValue(value, out var k))
                return k;
            return (T) cd;
        }
        return value;
    }
    
    public static object jl_alloc_array_1d(System.Type elType, int nr) {
        return CachedJuliaInfos.jl_alloc_array_1d_1.MakeGenericMethod(elType).Invoke(null, [nr])!;
    }
 
    public static Core.Array<T, Vals.Int1, ValueTuple<Base.Int>> jl_alloc_array_1d<T>(int nr) {
        return new(new ValueTuple<Base.Int>(nr));
    }

    public static void jl_array_ptr_1d_push<T>(Core.Array<T, Vals.Int1, ValueTuple<Base.Int>> array, T value) {
        array.Add(value);
    }
    
    public static void jl_array_ptr_1d_push(dynamic array, dynamic value) {
        array.Add(value);
    }

    static jlapi() {
        var nintv = new Dictionary<Base.Int, object>();
        for (var i = -10; i < 11; i++)
            nintv[i] = new Base.Int(i);
        _constantCache[typeof(Base.Int)] = nintv;
        
        var intv = new Dictionary<Base.Int, object>();
        for (var i = -10; i < 11; i++)
            intv[i] = new Base.Int(i);
        _constantCache[typeof(Base.Int)] = intv;
        
        var boolv = new Dictionary<Base.Bool, object> {
            [false] = Base.Bool.False,
            [true] = Base.Bool.True
        };
        _constantCache[typeof(Base.Bool)] = boolv;
        
        _constantCache[typeof(Base.Nothing)] = Base.Nothing.Instance;
    }
}