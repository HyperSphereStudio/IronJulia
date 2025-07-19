using System.Reflection;
using System.Runtime.CompilerServices;

namespace IronJulia.Emit;

public class CachedReflectionInfo
{
    private static ConstructorInfo? s_CompilerGeneratedCctor;
    public static ConstructorInfo CompilerGeneratedCctor => s_CompilerGeneratedCctor ??= typeof(CompilerGeneratedAttribute).GetConstructor([])!;
    
    private static ConstructorInfo? s_UnsafeAccessorCttor;

    public static ConstructorInfo UnsafeAccessorCCtor => s_UnsafeAccessorCttor ??=
        typeof(UnsafeAccessorAttribute).GetConstructor([typeof(UnsafeAccessorKind)])!;

    private static PropertyInfo? s_UnsafeAccessorNameField;
    public static PropertyInfo UnsafeAccessorNameField => s_UnsafeAccessorNameField ??=
        typeof(UnsafeAccessorAttribute).GetProperty("Name", BindingFlags.Public|BindingFlags.Instance)!;

    
    private static MethodInfo? s_runtimeHelper_isOrContainsReferences;

    public static MethodInfo RuntimeHelper_isOrContainsReferences =>
        s_runtimeHelper_isOrContainsReferences ??=
            typeof(RuntimeHelpers).GetMethod("IsReferenceOrContainsReferences`1")!;

    private static MethodInfo? s_nullableHasValueGetter;

    public static MethodInfo Nullable_Getter_HasValue => s_nullableHasValueGetter ??=
        typeof(Nullable<>).GetMethod("get_HasValue", BindingFlags.Instance | BindingFlags.Public)!;

    private static MethodInfo? s_nullableValueGetter;

    public static MethodInfo Nullable_Getter_Value => s_nullableValueGetter ??=
        typeof(Nullable<>).GetMethod("get_Value", BindingFlags.Instance | BindingFlags.Public)!;

    private static MethodInfo? s_nullableGetValueOrDefault;

    public static MethodInfo Nullable_GetValueOrDefault => s_nullableGetValueOrDefault ??=
        typeof(Nullable<>).GetMethod("GetValueOrDefault", Type.EmptyTypes)!;

    private static ConstructorInfo? s_Nullable_Boolean_Ctor;

    public static ConstructorInfo Nullable_Boolean_Ctor
        => s_Nullable_Boolean_Ctor ??= typeof(bool?).GetConstructor([typeof(bool)])!;

    private static ConstructorInfo? s_Decimal_Ctor_Int32;

    public static ConstructorInfo Decimal_Ctor_Int32 =>
        s_Decimal_Ctor_Int32 ??= typeof(decimal).GetConstructor([typeof(int)])!;

    private static ConstructorInfo? s_Decimal_Ctor_UInt32;

    public static ConstructorInfo Decimal_Ctor_UInt32 =>
        s_Decimal_Ctor_UInt32 ??= typeof(decimal).GetConstructor([typeof(uint)])!;

    private static ConstructorInfo? s_Decimal_Ctor_Int64;

    public static ConstructorInfo Decimal_Ctor_Int64 =>
        s_Decimal_Ctor_Int64 ??= typeof(decimal).GetConstructor([typeof(long)])!;

    private static ConstructorInfo? s_Decimal_Ctor_UInt64;

    public static ConstructorInfo Decimal_Ctor_UInt64 =>
        s_Decimal_Ctor_UInt64 ??= typeof(decimal).GetConstructor([typeof(ulong)])!;

    private static ConstructorInfo? s_Decimal_Ctor_Int32_Int32_Int32_Bool_Byte;

    public static ConstructorInfo Decimal_Ctor_Int32_Int32_Int32_Bool_Byte =>
        s_Decimal_Ctor_Int32_Int32_Int32_Bool_Byte ??= typeof(decimal).GetConstructor([typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte)
        ])!;

    private static FieldInfo? s_Decimal_One;

    public static FieldInfo Decimal_One =>
        s_Decimal_One ??= typeof(decimal).GetField(nameof(decimal.One))!;

    private static FieldInfo? s_Decimal_MinusOne;

    public static FieldInfo Decimal_MinusOne =>
        s_Decimal_MinusOne ??= typeof(decimal).GetField(nameof(decimal.MinusOne))!;

    private static FieldInfo? s_Decimal_MinValue;

    public static FieldInfo Decimal_MinValue =>
        s_Decimal_MinValue ??= typeof(decimal).GetField(nameof(decimal.MinValue))!;

    private static FieldInfo? s_Decimal_MaxValue;

    public static FieldInfo Decimal_MaxValue =>
        s_Decimal_MaxValue ??= typeof(decimal).GetField(nameof(decimal.MaxValue))!;

    private static FieldInfo? s_Decimal_Zero;

    public static FieldInfo Decimal_Zero =>
        s_Decimal_Zero ??= typeof(decimal).GetField(nameof(decimal.Zero))!;

    private static FieldInfo? s_DateTime_MinValue;

    public static FieldInfo DateTime_MinValue =>
        s_DateTime_MinValue ??= typeof(DateTime).GetField(nameof(DateTime.MinValue))!;

    private static MethodInfo? s_MethodBase_GetMethodFromHandle_RuntimeMethodHandle;

    public static MethodInfo MethodBase_GetMethodFromHandle_RuntimeMethodHandle =>
        s_MethodBase_GetMethodFromHandle_RuntimeMethodHandle ??=
            typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle),
                new[] { typeof(RuntimeMethodHandle) })!;

    private static MethodInfo? s_MethodBase_GetMethodFromHandle_RuntimeMethodHandle_RuntimeTypeHandle;

    public static MethodInfo MethodBase_GetMethodFromHandle_RuntimeMethodHandle_RuntimeTypeHandle =>
        s_MethodBase_GetMethodFromHandle_RuntimeMethodHandle_RuntimeTypeHandle ??= typeof(MethodBase).GetMethod(
            nameof(MethodBase.GetMethodFromHandle), [typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle)])!;

    private static MethodInfo? s_MethodInfo_CreateDelegate_Type_Object;

    public static MethodInfo MethodInfo_CreateDelegate_Type_Object =>
        s_MethodInfo_CreateDelegate_Type_Object ??= typeof(MethodInfo).GetMethod(nameof(MethodInfo.CreateDelegate),
            new[] { typeof(Type), typeof(object) })!;

    private static MethodInfo? s_String_op_Equality_String_String;

    public static MethodInfo String_op_Equality_String_String =>
        s_String_op_Equality_String_String ??=
            typeof(string).GetMethod("op_Equality", [typeof(string), typeof(string)])!;

    private static MethodInfo? s_String_Equals_String_String;

    public static MethodInfo String_Equals_String_String =>
        s_String_Equals_String_String ??= typeof(string).GetMethod("Equals", [typeof(string), typeof(string)])!;

    private static MethodInfo? s_DictionaryOfStringInt32_Add_String_Int32;

    public static MethodInfo DictionaryOfStringInt32_Add_String_Int32 =>
        s_DictionaryOfStringInt32_Add_String_Int32 ??=
            typeof(Dictionary<string, int>).GetMethod(nameof(Dictionary<string, int>.Add),
                new[] { typeof(string), typeof(int) })!;

    private static ConstructorInfo? s_DictionaryOfStringInt32_Ctor_Int32;

    public static ConstructorInfo DictionaryOfStringInt32_Ctor_Int32 =>
        s_DictionaryOfStringInt32_Ctor_Int32 ??= typeof(Dictionary<string, int>).GetConstructor([typeof(int)])!;

    private static MethodInfo? s_Type_GetTypeFromHandle;

    public static MethodInfo Type_GetTypeFromHandle =>
        s_Type_GetTypeFromHandle ??= typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle))!;

    private static MethodInfo? s_Object_GetType;

    public static MethodInfo Object_GetType =>
        s_Object_GetType ??= typeof(object).GetMethod(nameof(object.GetType))!;

    private static MethodInfo? s_Decimal_op_Implicit_Byte;

    public static MethodInfo Decimal_op_Implicit_Byte =>
        s_Decimal_op_Implicit_Byte ??= typeof(decimal).GetMethod("op_Implicit", [typeof(byte)])!;

    private static MethodInfo? s_Decimal_op_Implicit_SByte;

    public static MethodInfo Decimal_op_Implicit_SByte =>
        s_Decimal_op_Implicit_SByte ??= typeof(decimal).GetMethod("op_Implicit", [typeof(sbyte)])!;

    private static MethodInfo? s_Decimal_op_Implicit_Int16;

    public static MethodInfo Decimal_op_Implicit_Int16 =>
        s_Decimal_op_Implicit_Int16 ??= typeof(decimal).GetMethod("op_Implicit", [typeof(short)])!;

    private static MethodInfo? s_Decimal_op_Implicit_UInt16;

    public static MethodInfo Decimal_op_Implicit_UInt16 =>
        s_Decimal_op_Implicit_UInt16 ??= typeof(decimal).GetMethod("op_Implicit", [typeof(ushort)])!;

    private static MethodInfo? s_Decimal_op_Implicit_Int32;

    public static MethodInfo Decimal_op_Implicit_Int32 =>
        s_Decimal_op_Implicit_Int32 ??= typeof(decimal).GetMethod("op_Implicit", [typeof(int)])!;

    private static MethodInfo? s_Decimal_op_Implicit_UInt32;

    public static MethodInfo Decimal_op_Implicit_UInt32 =>
        s_Decimal_op_Implicit_UInt32 ??= typeof(decimal).GetMethod("op_Implicit", [typeof(uint)])!;

    private static MethodInfo? s_Decimal_op_Implicit_Int64;

    public static MethodInfo Decimal_op_Implicit_Int64 =>
        s_Decimal_op_Implicit_Int64 ??= typeof(decimal).GetMethod("op_Implicit", [typeof(long)])!;

    private static MethodInfo? s_Decimal_op_Implicit_UInt64;

    public static MethodInfo Decimal_op_Implicit_UInt64 =>
        s_Decimal_op_Implicit_UInt64 ??= typeof(decimal).GetMethod("op_Implicit", [typeof(ulong)])!;

    private static MethodInfo? s_Decimal_op_Implicit_Char;

    public static MethodInfo Decimal_op_Implicit_Char =>
        s_Decimal_op_Implicit_Char ??= typeof(decimal).GetMethod("op_Implicit", [typeof(char)])!;

    private static MethodInfo? s_Math_Pow_Double_Double;

    public static MethodInfo Math_Pow_Double_Double =>
        s_Math_Pow_Double_Double ??=
            typeof(Math).GetMethod(nameof(Math.Pow), new[] { typeof(double), typeof(double) })!;
}