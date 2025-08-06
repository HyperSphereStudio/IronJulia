using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpinorCompiler.Utils;

public static class ArrayUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T UnsafeArrayAccess<T>(this T[] ar, int index) {
        ref var tableRef = ref MemoryMarshal.GetArrayDataReference(ar);
        return ref Unsafe.Add(ref tableRef, (nint)index);
    }
    
}