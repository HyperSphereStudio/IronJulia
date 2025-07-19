using System.Reflection;
using IronJulia.CoreLib;

namespace IronJulia.Compiler;

public class CachedJuliaInfos {
    private static ConstructorInfo? sSymbolConstructor;
    public static ConstructorInfo Symbol_Cctor => sSymbolConstructor ??= typeof(Base.Symbol).GetConstructor(BindingFlags.Public, [typeof(string)])!;

    private static MethodInfo? sjl_alloc_array_1d_1;
    public static MethodInfo jl_alloc_array_1d_1 = jl_alloc_array_1d_1 ??= typeof(jlapi).GetMethod("jl_alloc_array_1d", BindingFlags.Static | BindingFlags.Public, [typeof(int)])!;
}