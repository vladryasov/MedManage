using MedManage.Application.Interfaces;
using MedManage.Domain.Common;

namespace MedManage.Application.Services;

public class PasswordService : IPasswordService
{
    private const string ReadableChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";

    public string Hash(string password) => PasswordHelper.Hash(password);

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string GeneratePassword()
    {
        return GenerateRandomString(16);
    }

    public string GenerateUserName()
    {
        return GenerateRandomString(10);
    }

    private static string GenerateRandomString(int length)
    {
        var chars = new char[length];
        for (var i = 0; i < length; i++)
            chars[i] = ReadableChars[Random.Shared.Next(ReadableChars.Length)];
        return new string(chars);
    }
}
