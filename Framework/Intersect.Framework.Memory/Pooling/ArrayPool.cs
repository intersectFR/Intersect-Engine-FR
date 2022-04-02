namespace Intersect.Framework.Memory.Pooling;

public class ArrayPool<T> : Pool<T[]>
{
    public ArrayPool() : base(new ArrayComparer<T>())
    {
    }

    public bool Take(int capacity, out T[] item)
    {
        var poolAllocator = new ArrayPoolAllocator<T>(capacity);
        return Take(poolAllocator, out item);
    }
}
