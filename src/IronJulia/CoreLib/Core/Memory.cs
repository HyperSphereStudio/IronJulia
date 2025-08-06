using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IronJulia.CoreLib;
using IronJulia.Utils;
using SpinorCompiler.Boot;
using SpinorCompiler.Utils;
using static Base;

public partial struct Core
{
    public interface AddrSpace : Any;
    public interface AddrSpace<Backend> : AddrSpace;
    public struct CPU : AddrSpace<CPU>;
    
    public interface AbstractArray : Any;
    public interface AbstractArray<T> : AbstractArray, IEnumerable<T>;
    public interface AbstractArray<T, N> : AbstractArray<T> where N : Val<Int>;
    public interface AbstractVector : AbstractArray;
    public interface AbstractVector<T> : AbstractVector, AbstractArray<T, Vals.Int1>;
    
    public interface DenseArray : AbstractArray;
    public interface DenseArray<T> : DenseArray, AbstractArray<T>;
    public interface DenseArray<T, N> : DenseArray<T>, AbstractArray<T, N> where N : Val<Int>;
    
    public interface GenericMemory : DenseArray {
        public int length { get; }
        public Ptr<byte> ptr { get; }
    }
    public interface GenericMemory<Kind> : GenericMemory where Kind : Val<Symbol>{}
    public interface GenericMemory<Kind, T> : GenericMemory<Kind>, DenseArray<T> where Kind : Val<Symbol> {
        public Span<T> Span { get; }
    }
    public unsafe struct GenericMemory<Kind, T, AddressSpace> : DenseArray
        where Kind : Val<Symbol> where AddressSpace : Any {
        public Ptr<byte> ptr => new((byte*) Unsafe.AsPointer(ref Ref));
        public T[]? Array;
        public readonly T* ptr_v;
        public Int length;

        public GenericMemoryScopedSlice<Kind, T, AddressSpace> Slice {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(ref Ref, length);
        }

        public GenericMemoryScopedSlice<Kind, T, AddressSpace> ArraySlice {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(ref ArrayRef, length);
        }
        
        public ref T ArrayRef
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Array!.GetArrayRef(-1);
        }

        public ref T ExternalRef {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<T>(ptr_v);
        }
        
        public ref T Ref {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (Array != null)
                    return ref Array!.GetArrayRef(-1);
                return ref Unsafe.AsRef<T>(ptr_v);
            }
        }
        
        public GenericMemory(T[] array) {
            length = array.Length;
            Array = array;
        }
        
        public GenericMemory(Ptr<byte> unmanagedMemory, Int length) {
            this.length = length;
            ptr_v = (T*) unmanagedMemory.Value;
        }

        public void Resize(Int newSize) {
            if (Array == null)
                throw new NotSupportedException("Cannot Resize External Memory Reference!");
            var newArr = new T[newSize];
            Array.ToGenericMemorySlice().CopyTo(newArr.ToGenericMemorySlice());
            Array = newArr;
            length = newSize;
        }

        public void Clear() => Slice.Clear();
    }
    
    public readonly ref struct GenericMemoryScopedSlice<Kind, T, AddressSpace>(ref T ptr, Int length) : Ref<T> where Kind : Val<Symbol>
        where AddressSpace : Any {
        public ref T JuliaPtr => ref Ptr;
        public ref T NetPtr => ref Unsafe.Add(ref Ptr, 1);
        public readonly ref T Ptr = ref ptr;
        public readonly Int Length = length;
        public ref T Value => ref Ptr;
        public DynamicMetaObject GetMetaObject(Expression parameter) => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GenericMemoryScopedSlice<Kind, T, AddressSpace> Slice(Int offset, Int length) => new(ref Unsafe.Add(ref JuliaPtr, offset), length);
        
        public ref T this[Int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if ((nuint)(nint)index > (nuint)(nint)Length || index == 0)
                    throw new IndexOutOfRangeException();
                return ref Unsafe.Add(ref Ptr, (nuint)(nint)index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(GenericMemoryScopedSlice<Kind, T, AddressSpace> dest) {
            if ((nuint)(nint)dest.Length < (nuint)(nint)Length)
                throw new IndexOutOfRangeException();
            MemoryUtils.Memmove(null, ref dest.NetPtr, ref NetPtr, (nuint)(nint)Length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Clear() {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
                MemoryUtils.ClearWithReferences(null, ref Unsafe.As<T, IntPtr>(ref NetPtr), (nuint) length.Value * (nuint)(sizeof(T) / sizeof(nuint)));
            }
            else{
                MemoryUtils.ClearWithoutReferences(null, ref Unsafe.As<T, byte>(ref NetPtr), (nuint) length.Value * (nuint)sizeof(T));
            }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        }
    }

    public interface GenericMemoryRef : Any;
    public interface GenericMemoryRef<Kind> : GenericMemoryRef where Kind : Val<Symbol>;

    public struct GenericMemoryRef<Kind, T, AddressSpace> : Ref<T> where Kind : Val<Symbol>
        where AddressSpace : Any {
        public VoidPtr ptr_or_offset;
        public GenericMemory<Kind, T, AddressSpace> mem;
        public ref T Value => ref mem.Ref;
    }

    public struct SimpleVector<T> : DenseArray<T, Vals.Int1>{
        public GenericMemory<Vals.NotAtomicSymbol, T, CPU> Mem;
        public Int Length;
        
        public SimpleVector(Int size, Bool isCapacity) {
            Length = isCapacity ? 0 : (nint) size;
            Mem = new(new T[size]);
        }

        public SimpleVector(GenericMemory<Vals.NotAtomicSymbol, T, CPU> gmem, Int length) {
            Mem = gmem;
            Length = (nint) length;
        }

        public ref T this[Int idx] => ref Mem.Slice[idx];

        public void Add(T item) {
            GrowCapacity(Length + 1);
            Length += 1;
            Mem.ArraySlice[Length] = item;
        }

        public T Pop() {
            return Mem.ArraySlice[Length--];
        }
        
        private void GrowCapacity(long newCap) {
            Debug.Assert(newCap < int.MaxValue);
            if (Mem.length < newCap)
                Mem.Resize(Math.Max((int) newCap, Mem.length * 2));
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator() => new FastEnumerator(Mem.Array, Mem.ptr, Length);
        public void Clear() {
            Length = 0;
            Mem.Clear();
        }
        public bool Contains(T item) {
            return this.Any(k => EqualityComparer<T>.Default.Equals(k, item));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int offset) {
            Mem.Slice.Slice(0, Length).CopyTo(array.ToGenericMemorySlice().Slice(offset, Length - offset));
        }
        
        public int Count => (int) Length;
        
        public unsafe struct FastEnumerator(T[]? array, IntPtr ptr, nint length) : IEnumerator<T> {
            private int index = -1;
            private readonly T[]? array = array;
            private readonly IntPtr ptr = ptr;
            private readonly nint length = length;
            
            public T Current {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => array != null ? array.UnsafeArrayAccess(index) : Unsafe.Read<T>(Unsafe.Add<T>(ptr.ToPointer(), index + 1));
            }
            object IEnumerator.Current => Current!;
            public bool MoveNext() {
                index++;
                return index < length;
            }
            public void Reset() => index = -1;
            public void Dispose() { }
        }
    }

    public interface Array : DenseArray, IEnumerable;
    public interface Array<T> : DenseArray<T>, Array;
    public class Array<T, N, TT> : Array<T>, DenseArray<T, N>,  ICollection<T>
        where N : Val<Int> where TT : ITuple {
        
        public SimpleVector<T> vec;
        public Tuple<TT> size;
        public Int Length => vec.Count;
        public Span<Int> sizespan => JuliaTupleUtils.NTupleSpan<Int, Tuple<TT>>(ref size);

        public Array(int capacity = 3) {
            sizespan.Fill(0);
            vec = new(capacity, true);
        }
        
        public Array(Tuple<TT> size) {
            this.size = size;
            long len = 1;
            foreach (var s in sizespan)
                len *= s;
            vec = new((nint) len, false);
        }

        public unsafe Array(void* ptr, Tuple<TT> size) {
            this.size = size;
            Int len = 1;
            foreach (var t in sizespan)
                len *= t;
            vec = new(new(new Ptr<byte>((byte*)ptr), len), len);
            this.size = size;
        }

        private int GetArrayIndex(Tuple<TT> idx) {
            long cartesianIdx = 0;
            var idxSpan = JuliaTupleUtils.NTupleSpan<Base.Int, Tuple<TT>>(ref idx);
            for (var i = 1; i < sizespan.Length; i++) {
                cartesianIdx += sizespan[i - 1] * (idxSpan[i] - 1);
            }
            if(idx.Length > 0)
                cartesianIdx += idxSpan[0];
            return (int)cartesianIdx;
        }

        public void Add(T item) {
            Debug.Assert(sizespan.Length == 1, "Expected Rank 1 Array");
            vec.Add(item);
            sizespan[0] = vec.Length;
        }

        public T this[Tuple<TT> idx] {
            get => vec[GetArrayIndex(idx)];
            set => vec[GetArrayIndex(idx)] = value;
        }
        
        public T this[Int idx] {
            get => vec[idx];
            set => vec[idx] = value;
        }

        public override string ToString() {
            return $"Array{{{typeof(T).Name}, {N.Value}}}[{string.Join(", ", this)}]";
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator() => vec.GetEnumerator();
        public void Clear() {
            sizespan.Fill(0);
            vec.Clear();
        }
        public bool Contains(T item) => vec.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => vec.CopyTo(array, arrayIndex);
        public bool Remove(T item) => throw new NotSupportedException();
        public int Count => (int) Length;
        public bool IsReadOnly => false;
    }

    public interface Ref : Any;
    public interface Ref<T> : Ref {
        public ref T Value { get; }
    }

    public interface Ptr : Any;
    public readonly unsafe struct Ptr<T>(T* value = null) : Ptr where T : unmanaged {
        public T* Value { get; init; } = value;
        public static implicit operator IntPtr(Ptr<T> v) => new(v.Value);
    }
    
}
