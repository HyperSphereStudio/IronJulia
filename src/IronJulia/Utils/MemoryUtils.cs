using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IronJulia.CoreLib;

namespace IronJulia.Utils;

public static class MemoryUtils
{
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
    public static extern void Memmove<K>([UnsafeAccessorType("System.Buffer")] object? _, ref K destination, ref K source, nuint elementCount);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetArrayRef<T>(this T[] array, nint offset = 0) => ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), offset);
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Core.GenericMemoryScopedSlice<Vals.NotAtomicSymbol, T, Core.CPU> ToGenericMemorySlice<T>(this T[] array) => 
        new(ref array.GetArrayRef(-1), array.Length);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
    public static extern void ClearWithReferences([UnsafeAccessorType("System.SpanHelpers")] object? _, ref IntPtr ip, nuint pointerSizeLength);
    
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
    public static extern void ClearWithoutReferences([UnsafeAccessorType("System.SpanHelpers")] object? _, ref byte dest, nuint len);
}