using System.Buffers;
using System.Collections;

namespace IronJulia.Utils;

public interface IMemoryAllocator {
    public static abstract T[] Allocate<T>(int n);
    public static abstract void Free<T>(T[] memory);
}

public struct DefaultAllocator : IMemoryAllocator {
    public static T[] Allocate<T>(int n) => n == 0 ? [] : new T[n];
    public static void Free<T>(T[] memory){}
}

public struct PoolAllocator : IMemoryAllocator {
    public static T[] Allocate<T>(int n) {
        if (n == 0)
            return [];
        return ArrayPool<T>.Shared.Rent(n);
    }

    public static void Free<T>(T[] memory) {
        if (memory.Length != 0)
           ArrayPool<T>.Shared.Return(memory);
    }
}

public struct InlinedList<T, Allocator>(int capacity) : IList<T>, IDisposable
    where Allocator : IMemoryAllocator {
    private T[] _buf = Allocator.Allocate<T>(capacity);
    private int _count = 0;
    public Span<T> Span => _buf.AsSpan(0, _count);
    public int Count => _count;
    public bool IsReadOnly => false;

    public InlinedList() : this(0){}

    public void Add(T item) {
        EnsureRoom(_count + 1);
        _buf[_count] = item;
        _count++;
    }

    private void EnsureRoom(int n) {
        if(n > _buf.Length)
            ResizeBuffer((int) (n * 1.5f));
    }

    public int IndexOf(T item) => throw new NotImplementedException();
    public void Insert(int index, T item) => throw new NotImplementedException();
    public void RemoveAt(int index) => throw new NotImplementedException();

    public T this[int index] {
        get {
            if ((uint)index >= (uint)_count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _buf[index];   
        }
        set {
            if ((uint)index >= (uint)_count)
                throw new ArgumentOutOfRangeException(nameof(index));
            _buf[index] = value;
        }
    }

    public void Clear() {
        MemoryUtils.Clear(ref _buf.GetArrayRef(), (nuint) _count);
        _count = 0;
    }

    public void ResizeBuffer(int n) {
        if (n < 0)
            return;
        if (_buf.Length != n) {
            var m = Allocator.Allocate<T>(n);
            MemoryUtils.Memmove(ref m.GetArrayRef(), ref _buf.GetArrayRef(), (nuint) Math.Min(_count, n));
            Allocator.Free(_buf);
            _buf = m;
        }
    }

    public T[] Compact() {
        ResizeBuffer(Count);
        return _buf;
    }

    public bool Contains(T item) {
        var comparer = EqualityComparer<T>.Default;
        for (var i = 0; i < _count; i++) {
            if (comparer.Equals(this[i], item))
                return true;
        }
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex) {
        for (var i = 0; i < _count; i++)
            array[arrayIndex + i] = this[i];
    }

    public bool Remove(T item) {
        var comparer = EqualityComparer<T>.Default;
        for (var i = 0; i < _count; i++) {
            if (!comparer.Equals(this[i], item)) continue;
            for (var j = i + 1; j < _count; j++)
                this[j - 1] = this[j];
            _count--;
            return true;
        }
        return false;
    }

    public IEnumerator<T> GetEnumerator() {
        for (var i = 0; i < _count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Dispose() => Allocator.Free(_buf);
}