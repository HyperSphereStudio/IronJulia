using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using IronJulia.CoreLib;
using SpinorCompiler.Boot;
using static Base;

public partial struct Core
{
    public interface AddrSpace : Any;
    public interface AddrSpace<Backend> : AddrSpace;
    public struct CPU : AddrSpace<CPU>;
    
    public interface AbstractArray : Any;
    public interface AbstractArray<T> : AbstractArray, ICollection<T>;
    public interface AbstractArray<T, N> : AbstractArray<T> where N : Val<Int>;

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
        public Ptr<byte> ptr { get; set; }
        public T[]? Array;
        public Int length { get; set; }
        
        public GenericMemory(T[] array) {
            length = array.Length;
            Array = array;
        }
        
        public GenericMemory(Ptr<byte> unmanagedMemory, Int length) {
            this.length = length;
            ptr = unmanagedMemory;
        }  
        
        public Span<T> Span => Array != null ? Array.AsSpan() : new Span<T>(ptr.Value, length);

        public void Resize(Int newSize)
        {
            if (Array == null)
                throw new NotSupportedException("Cannot Resize External Memory Reference!");
            System.Array.Resize(ref Array, newSize);
            length = newSize;
        }

        public void Clear() => Resize(4);
    }

    public interface GenericMemoryRef : Any;
    public interface GenericMemoryRef<Kind> : GenericMemoryRef where Kind : Val<Symbol>;

    public struct GenericMemoryRef<Kind, T, AddressSpace> : Ref<T> where Kind : Val<Symbol>
        where AddressSpace : Any {
        public VoidPtr ptr_or_offset;
        public GenericMemory<Kind, T, AddressSpace> mem;
        public ref T Value => ref mem.Span.GetPinnableReference();
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
        
        public T this[int idx] {
            get => Mem.Span[idx - 1];
            set => Mem.Span[idx - 1] = value;
        }
        
        public void Add(T item) {
            GrowCapacity(Length + 1);
            Mem.Span[Length] = item;
            Length++;
        }

        public T Pop() {
            return Mem.Span[Length--];
        }
        
        private void GrowCapacity(long newCap) {
            Debug.Assert(newCap < int.MaxValue);
            if (Mem.length < newCap)
                Mem.Resize(Math.Max((int) newCap, Mem.length * 2));
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator() {
            for (Int i = 0, n = Length; i < n; i++)
                yield return Mem.Span[i];
        }

        public void Clear() {
            Length = 0;
            Mem.Clear();
        }

        public bool Contains(T item) {
            return this.Any(k => EqualityComparer<T>.Default.Equals(k, item));
        }

        public void CopyTo(T[] array, int arrayIndex) {
            for (Int len = Length - 1; len >= 0; len -= 1)
                array[arrayIndex++] = Mem.Span[len];
        }

        public bool Remove(T item) => throw new NotSupportedException();
        public int Count => Length;
        public bool IsReadOnly => false;
    }

    public interface Array : DenseArray, IEnumerable;
    public interface Array<T> : DenseArray<T>, Array;
    public class Array<T, N, TT> : Array<T>, DenseArray<T, N>
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
        public int Count => Length;
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