namespace SessionToken.Factory
{
    using System;

    using Encryption;

    using Models;

    using Protobuf;

    public class TokenFactory : ITokenFactory
    {
        private readonly IAuthenticatedEncryption _encryption;

        private readonly TimeSpan _expirationTime;

        public TokenFactory(IAuthenticatedEncryption encryption, TimeSpan expirationTime = default(TimeSpan))
        {
            if (expirationTime < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(expirationTime));
            }

            this._expirationTime = expirationTime == default(TimeSpan) ? TimeSpan.FromDays(1) : expirationTime;

            if (encryption == null)
            {
                throw new ArgumentNullException(nameof(encryption));
            }

            this._encryption = encryption;
        }

        public string GenerateToken(int userId, string sessionId)
        {
            Token token = new Token
            {
                UserId = userId,
                SessionId = sessionId,
                ValidToTicks = DateTime.UtcNow.Add(this._expirationTime).Ticks
            };

            return this.Encrypt(token);
        }

        public bool RenewToken(string encrypted, out string token)
        {
            if (string.IsNullOrWhiteSpace(encrypted))
            {
                throw new ArgumentNullException(nameof(encrypted));
            }

            Token tokenObject;
            if (this.DencryptToken(encrypted, out tokenObject) == false)
            {
                token = null;
                return false;
            }

            tokenObject.ValidToTicks = DateTime.UtcNow.Add(this._expirationTime).Ticks;
            
            token = this.Encrypt(tokenObject);
            return true;
        }

        public bool DencryptToken(string encrypted, out Token token)
        {
            byte[] tokenData;
            if (this._encryption.Decrypt(encrypted, out tokenData) == false)
            {
                token = null;
                return false;
            }

            // We're using ProtoBuf here since it fast and small
            token = Serializer.Deserialize(tokenData);
            return true;
        }

        private string Encrypt(Token token)
        {
            // We're using ProtoBuf here since it fast and small
            byte[] data = Serializer.Serialize(token);
            return this._encryption.Encrypt(data);
        }
    }
}