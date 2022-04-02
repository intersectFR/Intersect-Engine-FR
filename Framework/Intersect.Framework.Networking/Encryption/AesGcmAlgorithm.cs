using System.Buffers.Binary;
using System.Security.Cryptography;


namespace Intersect.Framework.Networking.Encryption;

public class AesGcmAlgorithm : EncryptionAlgorithm
{
    private const int NonceSize = 12;
    private const int TagSize = 16;

    public AesGcmAlgorithm(byte[] key) : base(key)
    {
    }

    public override Span<byte> Decrypt(ReadOnlySpan<byte> ciphertext)
    {
        if (ciphertext.Length < sizeof(int) * 2)
        {
            throw new ArgumentOutOfRangeException(nameof(ciphertext), "Expected the ciphertext to be at least 8 bytes long.");
        }

        var offset = 0;
        var nonceLength = BinaryPrimitives.ReadInt32BigEndian(ciphertext[offset..(offset += sizeof(int))]);
        if (nonceLength < AesGcm.NonceByteSizes.MinSize || AesGcm.NonceByteSizes.MaxSize < nonceLength)
        {
            throw new ArgumentOutOfRangeException(nameof(ciphertext), $"Received a nonce length of {nonceLength} bytes, which is outside of the supported range of {AesGcm.NonceByteSizes.MinSize} to {AesGcm.NonceByteSizes.MaxSize} bytes.");
        }

        var tagLength = BinaryPrimitives.ReadInt32BigEndian(ciphertext[offset..(offset += sizeof(int))]);
        if (tagLength < AesGcm.TagByteSizes.MinSize || AesGcm.TagByteSizes.MaxSize < tagLength)
        {
            throw new ArgumentOutOfRangeException(nameof(ciphertext), $"Received a tag length of {tagLength} bytes, which is outside of the supported range of {AesGcm.TagByteSizes.MinSize} to {AesGcm.TagByteSizes.MaxSize} bytes.");
        }

        var minLength = offset + nonceLength + tagLength;
        if (ciphertext.Length < minLength)
        {
            throw new ArgumentOutOfRangeException(nameof(ciphertext), $"Expected the ciphertext to be at least {minLength} bytes long.");
        }

        var nonce = ciphertext[offset..(offset += nonceLength)];
        var ciphertextBuffer = ciphertext[offset..^tagLength];
        var tag = ciphertext[..^tagLength];
        return Decrypt(nonce, ciphertextBuffer, tag);
    }

    public override Span<byte> Decrypt(ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> nonce)
    {
        if (nonce.Length < AesGcm.NonceByteSizes.MinSize || AesGcm.NonceByteSizes.MaxSize < nonce.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(nonce), $"Received a nonce length of {nonce.Length} bytes, which is outside of the supported range of {AesGcm.NonceByteSizes.MinSize} to {AesGcm.NonceByteSizes.MaxSize} bytes.");
        }

        if (ciphertext.Length < AesGcm.TagByteSizes.MinSize)
        {
            throw new ArgumentOutOfRangeException(nameof(ciphertext), $"Received a ciphertext of {ciphertext.Length} bytes, which is less than the minimum length of {AesGcm.TagByteSizes.MinSize} bytes.");
        }

        return Decrypt(nonce, ciphertext[..^TagSize], ciphertext[^TagSize..]);
    }

    private Span<byte> Decrypt(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> tag) {
        using var aesGcm = new AesGcm(Key);

        var plaintext = new Span<byte>(new byte[ciphertext.Length]);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
        return plaintext;
    }

    public override Span<byte> Encrypt(ReadOnlySpan<byte> plaintext) => Encrypt(plaintext, GenerateNonce(12));

    public override Span<byte> Encrypt(ReadOnlySpan<byte> plaintext, ReadOnlySpan<byte> nonce)
    {
        if (plaintext.IsEmpty)
        {
            return new Span<byte>();
        }

        if (nonce.Length < AesGcm.NonceByteSizes.MinSize || AesGcm.NonceByteSizes.MaxSize < nonce.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(nonce));
        }

        using var aesGcm = new AesGcm(Key);

        var destinationBuffer = new Span<byte>(new byte[sizeof(int) * 2 + plaintext.Length + nonce.Length + TagSize]);
        var offset = 0;

        BinaryPrimitives.WriteInt32BigEndian(destinationBuffer[offset..(offset += sizeof(int))], nonce.Length);
        BinaryPrimitives.WriteInt32BigEndian(destinationBuffer[offset..(offset += sizeof(int))], TagSize);

        var nonceBuffer = destinationBuffer[offset..(offset += nonce.Length)];
        var ciphertextBuffer = destinationBuffer[offset..(offset += plaintext.Length)];
        var tagBuffer = destinationBuffer[offset..(offset + TagSize)];

        nonce.CopyTo(nonceBuffer);
        aesGcm.Encrypt(destinationBuffer[(sizeof(int) * 2)..offset], plaintext, ciphertextBuffer, tagBuffer);
        return destinationBuffer;
    }

    private static Span<byte> GenerateNonce(int length)
    {
        var nonce = new byte[length].AsSpan();
        RandomNumberGenerator.Fill(nonce);
        return nonce;
    }
}
