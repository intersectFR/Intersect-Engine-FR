namespace Intersect.Framework.Memory.Pooling;

public class Pool<T>
{
    private readonly IComparer<T> _comparer;
    private readonly List<T> _pool;

    public Pool(IComparer<T> comparer)
    {
        _comparer = comparer;
        _pool = new List<T>();
    }

    public void Release(T item)
    {
        var index = 0;
        while (index < _pool.Count)
        {
            var itemAtIndex = _pool[index];
            var comparison = _comparer.Compare(itemAtIndex, item);
            if (comparison > 0)
            {
                _pool.Insert(index, item);
                return;
            }

            ++index;
        }

        _pool.Add(item);
    }

    public bool Take(IPoolAllocator<T> poolAllocator, out T item)
    {
        if (_pool.Count > 0)
        {
            var index = 0;
            while (index < _pool.Count)
            {
                var itemAtIndex = _pool[index];
                var selectionResult = poolAllocator.Select(itemAtIndex);
                switch (selectionResult)
                {
                    case SelectionResult.Abort:
                        goto DoAllocate;

                    case SelectionResult.Continue:
                        ++index;
                        break;

                    case SelectionResult.Select:
                        _pool.RemoveAt(index);
                        item = itemAtIndex;
                        return true;

                    default:
                        throw new IndexOutOfRangeException();
                }

                ++index;
            }
        }

DoAllocate:
        item = poolAllocator.Allocate();
        return false;
    }
}
