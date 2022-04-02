namespace Intersect.Framework.Memory.Pooling;

public interface IPoolAllocator<T>
{
    T Allocate();

    SelectionResult Select(T item);
}
