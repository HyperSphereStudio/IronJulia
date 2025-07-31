using System.Collections;

namespace SpinorCompiler.Utils;

public struct InlinedList<T> : IList<T> {
    private T[] _buf;
    private int _count;
    public Span<T> Span => _buf.AsSpan(0, _count);
    public int Count => _count;
    public bool IsReadOnly => false;

    public InlinedList() {
        _buf = [];
    }

    public InlinedList(int capacity) {
        _buf = new T[capacity];
    }
    
    public void Add(T item) {
        EnsureRoom(_count + 1);
        _buf[_count++] = item;
    }

    private void EnsureRoom(int n) {
        if(n > _buf.Length)
            Array.Resize(ref _buf, (int) (n * 1.5f));
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
        Array.Clear(_buf, 0, _buf.Length);
        _count = 0;
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
}