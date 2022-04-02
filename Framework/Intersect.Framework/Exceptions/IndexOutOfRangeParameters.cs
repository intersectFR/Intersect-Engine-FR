namespace Intersect.Framework.Exceptions;

public struct IndexOutOfRangeParameters
{
    public int ContainerLength { get; }
    
    public int Length { get; }

    public int Offset { get; }

    public IndexOutOfRangeParameters(int bufferLength, int offset, int length)
    {
        ContainerLength = bufferLength;
        Offset = offset;
        Length = length;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is IndexOutOfRangeParameters other &&
               ContainerLength == other.ContainerLength &&
               Offset == other.Offset &&
               Length == other.Length;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(ContainerLength, Offset, Length);
    }
}
