namespace SpinorCompiler.Utils;

public struct RandomAccessStack<T> {
    private readonly List<T> _items = new();

    public RandomAccessStack(){}
    
    public int Count => _items.Count;

    public void Push(T item) => _items.Add(item);

    public T Pop() {
        if (_items.Count == 0)
            throw new InvalidOperationException("Empty stack.");

        int index = _items.Count - 1;
        T item = _items[index];
        _items.RemoveAt(index);
        return item;
    }

    public T Peek() => _items[^1];

    public T this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }
}
