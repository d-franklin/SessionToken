namespace SessionToken.Encryption
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    using SecurityDriven.Inferno;

    public class AuthenticatedEncryption : IAuthenticatedEncryption
    {
        private readonly byte[] _keyBytes;

        public AuthenticatedEncryption(byte[] keyBytes)
        {
            this._keyBytes = keyBytes;
        }

        public AuthenticatedEncryption(string key) : this(Encoding.UTF8.GetBytes(key))
        {
        }

        public string Encrypt(byte[] data)
        {
            using (MemoryStream originalStream = new MemoryStream(data))
            using (MemoryStream encryptedStream = new MemoryStream())
            {
                using (EtM_EncryptTransform encTransform = new EtM_EncryptTransform(this._keyBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, encTransform, CryptoStreamMode.Write))
                    {
                        originalStream.CopyTo(cryptoStream);
                    }
                }

                return Convert.ToBase64String(encryptedStream.ToArray());
            }
        }

        public bool Decrypt(string base64, out byte[] data)
        {
            try
            {
                byte[] encryptedData = Convert.FromBase64String(base64);
                return this.Decrypt(encryptedData, out data);
            }
            catch (FormatException)
            {
                data = null;
                return false;
            }
        }

        public bool Decrypt(byte[] encrypted, out byte[] data)
        {
            try
            {
                using (MemoryStream encryptedStream = new MemoryStream(encrypted))
                using (MemoryStream decryptedStream = new MemoryStream())
                using (EtM_DecryptTransform decTransform = new EtM_DecryptTransform(this._keyBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, decTransform, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(decryptedStream);
                    }

                    data = decryptedStream.ToArray();

                    return decTransform.IsComplete;
                }
            }
            catch (Exception)
            {
                data = null;
                return false;
            }
        }

        public bool Authenticate(string base64)
        {
            try
            {
                byte[] encryptedData = Convert.FromBase64String(base64);
                return this.Authenticate(encryptedData);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool Authenticate(byte[] encrypted)
        {
            try
            {
                using (MemoryStream encryptedStream = new MemoryStream(encrypted))
                using (EtM_DecryptTransform decTransform = new EtM_DecryptTransform(this._keyBytes, authenticateOnly: true))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, decTransform, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(Stream.Null);
                    }

                    return decTransform.IsComplete;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}