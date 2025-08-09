using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IronJulia.CoreLib;

namespace IronJulia.Utils;

public static class MemoryUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Memmove<K>(ref K destination, ref K source, nuint elementCount) => Memmove(null, ref destination, ref source, elementCount);
    
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
    private static extern void Memmove<K>([UnsafeAccessorType("System.Buffer")] object? _, ref K destination, ref K source, nuint elementCount);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetArrayRef<T>(this T[] array, nint offset = 0) => ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), offset);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Core.GenericMemoryScopedSlice<Vals.NotAtomicSymbol, T, Core.CPU> ToGenericMemorySlice<T>(this T[] array) => 
        new(ref array.GetArrayRef(-1), array.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Clear<T>(ref T sp, nuint length) {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            ClearWithReferences(null, ref Unsafe.As<T, IntPtr>(ref sp), length * (nuint)(sizeof(T) / sizeof(nuint)));
        }
        else{
            ClearWithoutReferences(null, ref Unsafe.As<T, byte>(ref sp), (nuint) length * (nuint)sizeof(T));
        }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    }

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
    private static extern void ClearWithReferences([UnsafeAccessorType("System.SpanHelpers")] object? _, ref IntPtr ip, nuint pointerSizeLength);
    
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
    private static extern void ClearWithoutReferences([UnsafeAccessorType("System.SpanHelpers")] object? _, ref byte dest, nuint len);
}