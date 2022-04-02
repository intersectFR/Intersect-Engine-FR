namespace Intersect.Framework.Memory.Pooling;

public class ArrayPoolAllocator<T> : IPoolAllocator<T[]>
{
    private readonly int _capacity;

    public ArrayPoolAllocator(int capacity)
    {
        _capacity = capacity;
    }

    public T[] Allocate() => new T[_capacity];

    public SelectionResult Select(T[] item)
    {
        if (item.Length >= (_capacity << 8))
        {
            return SelectionResult.Abort;
        }

        if (item.Length < _capacity)
        {
            return SelectionResult.Continue;
        }

        return SelectionResult.Select;
    }
}
