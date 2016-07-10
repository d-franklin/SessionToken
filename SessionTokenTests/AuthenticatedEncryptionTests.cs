namespace SessionTokenTests
{
    using System;

    using SessionToken.Encryption;

    using Xunit;

    public class AuthenticatedEncryptionTests
    {
        [Fact]
        public void Encrypt_Should_Return_Base64_Encoded_String()
        {
            string key = Guid.NewGuid().ToString();
            byte[] data = Guid.NewGuid().ToByteArray();

            IAuthenticatedEncryption encryption = new AuthenticatedEncryption(key);

            string encrypted = encryption.Encrypt(data);

            Assert.IsType<string>(encrypted);

            // This will throw if the string is not Base64
            Convert.FromBase64String(encrypted);
        }

        [Fact]
        public void Decrypt_Should_Return_True_And_Equal_Data_On_Success()
        {
            string key = Guid.NewGuid().ToString();
            byte[] data = Guid.NewGuid().ToByteArray();

            IAuthenticatedEncryption encryption = new AuthenticatedEncryption(key);

            string encrypted = encryption.Encrypt(data);

            byte[] outData;
            bool success = encryption.Decrypt(encrypted, out outData);

            Assert.True(success);
            Assert.Equal(data, outData);
        }

        [Fact]
        public void Decrypt_Should_Return_True_And_Equal_Data_On_Success_With_Large_Key_And_Data()
        {
            byte[] data = new byte[250];
            byte[] key = new byte[250];

            Random random = new Random();
            random.NextBytes(data);
            random.NextBytes(key);

            IAuthenticatedEncryption encryption = new AuthenticatedEncryption(key);

            string encrypted = encryption.Encrypt(data);

            byte[] outData;
            bool success = encryption.Decrypt(encrypted, out outData);

            Assert.True(success);
            Assert.Equal(data, outData);
        }

        [Fact]
        public void Decrypt_Should_Return_False_On_Bad_Key()
        {
            byte[] key = Guid.NewGuid().ToByteArray();
            byte[] keyNew = Guid.NewGuid().ToByteArray();
            byte[] data = Guid.NewGuid().ToByteArray();

            IAuthenticatedEncryption encryption = new AuthenticatedEncryption(key);
            IAuthenticatedEncryption decryption = new AuthenticatedEncryption(keyNew);

            string encrypted = encryption.Encrypt(data);

            byte[] outData;
            bool success = decryption.Decrypt(encrypted, out outData);

            Assert.False(success);
            Assert.Null(outData);
        }

        [Fact]
        public void Decrypt_Should_Return_False_On_Invalid_Data()
        {
            byte[] key = Guid.NewGuid().ToByteArray();
            byte[] data = Guid.NewGuid().ToByteArray();

            IAuthenticatedEncryption encryption = new AuthenticatedEncryption(key);

            string encrypted = encryption.Encrypt(data);

            byte[] bytes = Convert.FromBase64String(encrypted);
            bytes[61]++;
            encrypted = Convert.ToBase64String(bytes);

            byte[] outData;
            bool success = encryption.Decrypt(encrypted, out outData);

            Assert.False(success);
            Assert.Null(outData);
        }

        [Fact]
        public void Decrypt_Should_Return_False_On_Invalid_Base64_String()
        {
            byte[] key = Guid.NewGuid().ToByteArray();

            IAuthenticatedEncryption encryption = new AuthenticatedEncryption(key);

            byte[] outData;
            bool success = encryption.Decrypt("jsdfh8r3uihf8iahf835oaho8a3th8o3tghddogaw83", out outData);

            Assert.False(success);
            Assert.Null(outData);
        }

        [Fact]
        public void Authenticate_Should_Return_True_On_Valid_Data()
        {
            byte[] key = Guid.NewGuid().ToByteArray();
            byte[] data = Guid.NewGuid().ToByteArray();

            IAuthenticatedEncryption encryption = new AuthenticatedEncryption(key);

            string encrypted = encryption.Encrypt(data);

            bool success = encryption.Authenticate(encrypted);

            Assert.True(success);
        }

        [Fact]
        public void Authenticate_Should_Return_False_On_Bad_Key()
        {
            byte[] key = Guid.NewGuid().ToByteArray();
            byte[] keyNew = Guid.NewGuid().ToByteArray();
            byte[] data = Guid.NewGuid().ToByteArray();

            IAuthenticatedEncryption encryption = new AuthenticatedEncryption(key);
            IAuthenticatedEncryption decryption = new AuthenticatedEncryption(keyNew);

            string encrypted = encryption.Encrypt(data);

            bool success = decryption.Authenticate(encrypted);

            Assert.False(success);
        }

        [Fact]
        public void Authenticate_Should_Return_False_On_Invalid_Data()
        {
            byte[] key = Guid.NewGuid().ToByteArray();
            byte[] data = Guid.NewGuid().ToByteArray();

            IAuthenticatedEncryption encryption = new AuthenticatedEncryption(key);

            string encrypted = encryption.Encrypt(data);

            byte[] bytes = Convert.FromBase64String(encrypted);
            bytes[61]++;
            encrypted = Convert.ToBase64String(bytes);

            bool success = encryption.Authenticate(encrypted);

            Assert.False(success);
        }

        [Fact]
        public void Authenticate_Should_Return_False_On_Invalid_Base64_String()
        {
            byte[] key = Guid.NewGuid().ToByteArray();

            IAuthenticatedEncryption encryption = new AuthenticatedEncryption(key);

            bool success = encryption.Authenticate("jsdfh8r3uihf8iahf835oaho8a3th8o3tghddogaw83");

            Assert.False(success);
        }
    }
}