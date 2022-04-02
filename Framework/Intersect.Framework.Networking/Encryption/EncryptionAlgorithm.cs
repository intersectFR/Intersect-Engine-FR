using System.Runtime.InteropServices;
using System.Security;

namespace Intersect.Framework.Networking.Encryption;

public abstract class EncryptionAlgorithm : IDisposable
{
    private bool disposedValue;

    private SecureString _key;

    public ReadOnlySpan<byte> Key
    {
        get => DecodeFromSecureString(_key);
        set => _key = EncodeToSecureString(value);
    }

    protected EncryptionAlgorithm(byte[] key)
    {
        Key = key;
    }

    public abstract Span<byte> Decrypt(ReadOnlySpan<byte> ciphertext);

    public abstract Span<byte> Decrypt(ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> nonce);

    public abstract Span<byte> Encrypt(ReadOnlySpan<byte> plaintext);

    public abstract Span<byte> Encrypt(ReadOnlySpan<byte> plaintext, ReadOnlySpan<byte> nonce);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~NetEncryption()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public static unsafe SecureString EncodeToSecureString(in ReadOnlySpan<byte> data)
    {
        var base64Chars = new char[(int)Math.Ceiling(data.Length * 4 / 3.0)];
        var charCount = Convert.ToBase64CharArray(data.ToArray(), 0, data.Length, base64Chars, 0);
        fixed (char* base64CharsPtr = base64Chars)
        {
            return new SecureString(base64CharsPtr, charCount);
        }
    }

    public static ReadOnlySpan<byte> DecodeFromSecureString(in SecureString secureString)
    {
        var secureStringPtr = Marshal.SecureStringToBSTR(secureString);
        var base64Chars = new char[secureString.Length];
        Marshal.Copy(secureStringPtr, base64Chars, 0, base64Chars.Length);
        return Convert.FromBase64CharArray(base64Chars, 0, secureString.Length);
    }
}
