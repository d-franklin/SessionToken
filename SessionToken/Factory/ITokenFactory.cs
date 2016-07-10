namespace SessionToken.Factory
{
    using Models;

    public interface ITokenFactory
    {
        string GenerateToken(int userId, string sessionId);

        bool RenewToken(string encrypted, out string token);

        bool DencryptToken(string encrypted, out Token token);
    }
}