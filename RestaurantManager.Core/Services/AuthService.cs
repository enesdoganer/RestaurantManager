using System.Security.Cryptography;
using System.Text;

namespace RestaurantManager.Core.Services;

public static class AuthService
{
    // To generate a new hash: run GenerateHash("yourtoken") and paste the result here
    private static string _validTokenHash = string.Empty;
    
    public static void Initialize(string tokenHash)
    {
        _validTokenHash = tokenHash;
    }
    
    public static bool ValidateToken(string input)
    {
        var hash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(input)));
        return hash.Equals(_validTokenHash, StringComparison.OrdinalIgnoreCase);
    }

    public static string GenerateHash(string input)
    {
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(input)));
    }
    
}