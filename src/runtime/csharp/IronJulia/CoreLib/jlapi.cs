using IronJulia.Compiler;

namespace IronJulia.CoreLib;

public static class jlapi
{
    public static Base.Symbol jl_create_sym(string symbol) => symbol;
    public static unsafe Base.Symbol jl_create_sym(char* symbol, int n) => jl_create_sym(new string(symbol, 0, n));
    
    public static Base.Any jl_alloc_array_1d(System.Type elType, int nr) {
        return (Base.Any) CachedJuliaInfos.jl_alloc_array_1d_1.MakeGenericMethod(elType).Invoke(null, [nr])!;
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
}