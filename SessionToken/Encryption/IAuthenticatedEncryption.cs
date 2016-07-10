namespace SessionToken.Encryption
{
    public interface IAuthenticatedEncryption
    {
        string Encrypt(byte[] data);

        bool Decrypt(string base64, out byte[] data);

        bool Decrypt(byte[] encrypted, out byte[] data);

        bool Authenticate(string encrypted);

        bool Authenticate(byte[] encrypted);
    }
}