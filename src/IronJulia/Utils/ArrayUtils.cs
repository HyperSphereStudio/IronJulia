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

    public static int SeqHashCode<T>(this T[] ar) {
        var value = 0;
        foreach (var t in ar) {
            value = HashCode.Combine(t,value);
        }
        return value;
    }
}