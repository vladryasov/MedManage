using System.Security.Cryptography;
using System.Text;

namespace MedManage.Domain.Common;

public static class PasswordHelper
{
    public static string Hash(string password)
    {
        var sha256 = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        var sha256Hex = Convert.ToHexString(sha256).ToLowerInvariant();
        return BCrypt.Net.BCrypt.HashPassword(sha256Hex, workFactor: 12);
    }
}
