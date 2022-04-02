namespace Intersect.Framework.Memory.Pooling;

public class ArrayComparer<T> : IComparer<T[]>
{
    public int Compare(T[] x, T[] y) => x.Length - y.Length;
}
