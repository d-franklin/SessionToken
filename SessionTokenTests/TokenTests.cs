namespace SessionTokenTests
{
    using System;

    using SessionToken.Models;

    using Xunit;

    public class TokenTests
    {
        [Fact]
        public void Token_IsValid_Should_Return_True_When_DateTime_Greater_Than_UtcNow()
        {
            DateTime now = DateTime.UtcNow.AddDays(1);

            Token tokenOne = new Token
            {
                UserId = 123,
                SessionId = "132",
                ValidToTicks = now.Ticks
            };

            bool isValid = tokenOne.IsValid;

            Assert.True(isValid);
        }

        [Fact]
        public void Token_IsValid_Should_Return_False_When_DateTime_Greater_Than_UtcNow()
        {
            DateTime now = DateTime.UtcNow.AddDays(-1);

            Token tokenOne = new Token
            {
                UserId = 123,
                SessionId = "132",
                ValidToTicks = now.Ticks
            };

            bool isValid = tokenOne.IsValid;

            Assert.False(isValid);
        }

        [Fact]
        public void Token_ValidTo_Should_Return_DateTime_Based_On_ValidTo_Ticks()
        {
            DateTime now = DateTime.UtcNow;

            Token tokenOne = new Token
            {
                UserId = 123,
                SessionId = "132",
                ValidToTicks = now.Ticks
            };

            DateTime validTo = tokenOne.ValidTo;

            Assert.Equal(now, validTo);
        }

        [Fact]
        public void Token_Equals_Should_Return_True_On_Equal_Tokens()
        {
            Token tokenOne = new Token
            {
                UserId = 123,
                SessionId = "132"
            };

            Token tokenTwo = new Token
            {
                UserId = 123,
                SessionId = "132"
            };

            bool isEqual = tokenOne.Equals(tokenTwo);

            Assert.True(isEqual);
        }

        [Fact]
        public void Token_Equals_Should_Return_False_On_Unequal_Tokens()
        {
            Token tokenOne = new Token
            {
                UserId = 321,
                SessionId = "321"
            };

            Token tokenTwo = new Token
            {
                UserId = 123,
                SessionId = "132"
            };

            bool isEqual = tokenOne.Equals(tokenTwo);

            Assert.False(isEqual);
        }

        [Fact]
        public void Token_Equals_Should_Return_False_On_Null_Tokens()
        {
            Token tokenOne = new Token
            {
                UserId = 321,
                SessionId = "321"
            };

            bool isEqual = tokenOne.Equals(null);

            Assert.False(isEqual);
        }

        [Fact]
        public void Token_GetHashCode_Should_Return_Valid_HashCode()
        {
            Token token = new Token
            {
                UserId = 123,
                SessionId = "123",
                ValidToTicks = 123
            };

            int hashCode = token.GetHashCode();

            Assert.Equal(123, hashCode);
        }
    }
}