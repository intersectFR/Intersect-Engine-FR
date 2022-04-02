namespace Intersect.Framework.Networking.Encryption;

internal class NoopEncryptionAlgorithm : EncryptionAlgorithm
{
    public NoopEncryptionAlgorithm(byte[] key) : base(key)
    {
    }

    public override Span<byte> Decrypt(ReadOnlySpan<byte> ciphertext) => ciphertext.ToArray().AsSpan();

    public override Span<byte> Decrypt(ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> nonce) => ciphertext.ToArray().AsSpan();

    public override Span<byte> Encrypt(ReadOnlySpan<byte> plaintext) => plaintext.ToArray().AsSpan();

    public override Span<byte> Encrypt(ReadOnlySpan<byte> plaintext, ReadOnlySpan<byte> nonce) => plaintext.ToArray().AsSpan();
}
