namespace SessionTokenTests
{
    using System;

    using Moq;

    using SessionToken.Encryption;
    using SessionToken.Factory;
    using SessionToken.Models;

    using Xunit;

    public class TokenFactoryTests
    {
        ///// <summary>
        ///// Outputs a serialzed Token as 'new byte[] { 8, 123, 18, 4, 78, 97, 109, 101, 26, 4, 77, 105, 115, 99 };'
        ///// </summary>
        ///// <returns></returns>
        //private string HelperTokenSerializer()
        //{
        //    byte[] data;
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        ProtoBuf.Serializer.Serialize(stream, new Token { Id = 123, Name = "Name", Misc = "Misc" });
        //        data = stream.ToArray();
        //    }

        //    string holder = data.Aggregate(string.Empty, (current, value) => current + (value + ", "));

        //    const string output = "new byte[] {{ {0} }};";
        //    return string.Format(output, holder.Substring(0, holder.Length - 2));
        //}

        [Fact]
        public void TokenFactory_Should_Throw_ArgumentNullException_On_Null_IAuthenticatedEncryption()
        {
            Assert.Throws<ArgumentNullException>(() => new TokenFactory(null));
        }

        [Fact]
        public void TokenFactory_Should_Throw_ArgumentOutOfRangeException_ExpirationTime_Less_Than_TimeSpanZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TokenFactory(new AuthenticatedEncryption("key"), TimeSpan.MinValue));
        }

        [Fact]
        public void TokenFactory_Should_Return_Token_With_Default_Expiry_Of_One_Day_From_Now()
        {
            IAuthenticatedEncryption authenticatedEncryption = new AuthenticatedEncryption("key");

            ITokenFactory tokenFactory = new TokenFactory(authenticatedEncryption);

            string token = tokenFactory.GenerateToken(1, "1");

            Token tokenOut;
            tokenFactory.DencryptToken(token, out tokenOut);

            DateTime dateTime = DateTime.UtcNow.Add(TimeSpan.FromDays(1));

            Assert.InRange(tokenOut.ValidTo, dateTime.Subtract(TimeSpan.FromMinutes(1)), dateTime.Add(TimeSpan.FromMinutes(1)));
        }

        [Fact]
        public void GenerateToken_Should_Return_Encrypted_Base64_Token()
        {
            IAuthenticatedEncryption authenticatedEncryption = new AuthenticatedEncryption("key");

            ITokenFactory tokenFactory = new TokenFactory(authenticatedEncryption);
            
            string tokenEncrypted = tokenFactory.GenerateToken(123, "123");

            // This will throw if the string is not Base64
            Convert.FromBase64String(tokenEncrypted);
        }

        [Fact]
        public void GenerateToken_Should_Call_Encrypt_Only_Once_In_IAuthenticatedEncryption()
        {
            Mock<IAuthenticatedEncryption> authenticatedEncryption = new Mock<IAuthenticatedEncryption>();

            ITokenFactory tokenFactory = new TokenFactory(authenticatedEncryption.Object);

            tokenFactory.GenerateToken(123, "123");

            authenticatedEncryption.Verify(m => m.Encrypt(It.IsAny<byte[]>()), Times.Exactly(1));
        }

        [Fact]
        public void RenewToken_Should_Throw_ArgumentNullException_On_Null_Or_Empty_Encrypted_String()
        {
            IAuthenticatedEncryption authenticatedEncryption = new AuthenticatedEncryption("key");

            ITokenFactory tokenFactory = new TokenFactory(authenticatedEncryption);

            string token;
            Assert.Throws<ArgumentNullException>(() => tokenFactory.RenewToken(null, out token));
            Assert.Throws<ArgumentNullException>(() => tokenFactory.RenewToken(string.Empty, out token));
            Assert.Throws<ArgumentNullException>(() => tokenFactory.RenewToken("             ", out token));
        }

        [Fact]
        public void RenewToken_Should_Return_Null_Token_And_False_On_Decryption_Failure()
        {
            IAuthenticatedEncryption authenticatedEncryption = new AuthenticatedEncryption("key");

            ITokenFactory tokenFactory = new TokenFactory(authenticatedEncryption);

            string token;
            bool success = tokenFactory.RenewToken("fake", out token);

            Assert.False(success);
            Assert.Null(token);
        }

        [Fact]
        public void RenewToken_Should_Return_Valid_Token_And_True_On_Decryption_Success()
        {
            long tickes = DateTime.UtcNow.Ticks;

            IAuthenticatedEncryption authenticatedEncryption = new AuthenticatedEncryption("key");

            ITokenFactory tokenFactory = new TokenFactory(authenticatedEncryption);

            string tokenTemp = tokenFactory.GenerateToken(123, "132");

            string tokenString;
            bool success = tokenFactory.RenewToken(tokenTemp, out tokenString);

            Token token;
            tokenFactory.DencryptToken(tokenString, out token);

            Assert.True(success);
            Assert.NotNull(token);
            Assert.True(tickes < token.ValidToTicks);
        }

        [Fact]
        public void DencryptToken_Should_Return_Decrypted_Token_And_True_On_Success()
        {
            IAuthenticatedEncryption authenticatedEncryption = new AuthenticatedEncryption("key");

            ITokenFactory tokenFactory = new TokenFactory(authenticatedEncryption);

            Token token = new Token
            {
                UserId = 123,
                SessionId = "132"
            };

            string tokenEncrypted = tokenFactory.GenerateToken(token.UserId, token.SessionId);

            Token tokenOut;
            bool tokenDecrypted = tokenFactory.DencryptToken(tokenEncrypted, out tokenOut);

            Assert.True(tokenDecrypted);
            Assert.Equal(token.UserId, tokenOut.UserId);
            Assert.Equal(token.SessionId, tokenOut.SessionId);
        }

        [Fact]
        public void DencryptToken_Should_Return_False_On_Bad_Input()
        {
            IAuthenticatedEncryption authenticatedEncryption = new AuthenticatedEncryption("key");

            ITokenFactory tokenFactory = new TokenFactory(authenticatedEncryption);

            Token tokenOut;
            bool tokenDecrypted = tokenFactory.DencryptToken("fake", out tokenOut);

            Assert.False(tokenDecrypted);
            Assert.Null(tokenOut);
        }

        [Fact]
        public void DencryptToken_Should_Call_Decrypt_Only_Once_In_IAuthenticatedEncryption()
        {
            Mock<IAuthenticatedEncryption> authenticatedEncryptionMock = new Mock<IAuthenticatedEncryption>();

            // Define mocked 'out' values
            // This is a valid ProtoBuf serialized Token
            // ReSharper disable once RedundantAssignment
            byte[] tokenBytes = { 8, 123, 18, 4, 78, 97, 109, 101, 24, 131, 129, 13 };

            authenticatedEncryptionMock.Setup(m => m.Decrypt(It.IsAny<string>(), out tokenBytes)).Returns(true);

            ITokenFactory tokenFactory = new TokenFactory(authenticatedEncryptionMock.Object);

            Token tokenOut;
            tokenFactory.DencryptToken("fake", out tokenOut);

            authenticatedEncryptionMock.Verify(m => m.Decrypt(It.IsAny<string>(), out tokenBytes), Times.Exactly(1));
        }
    }
}