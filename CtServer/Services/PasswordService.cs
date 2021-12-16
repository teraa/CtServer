using System;
using System.Security.Cryptography;

namespace CtServer.Services;

public class PasswordService
{
    // https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
    private const int s_saltSize = 128 / 8;
    private const int s_hashSize = 256 / 8;
    private const int s_iterations = 310_000;
    private static readonly HashAlgorithmName s_hashAlgorithm = HashAlgorithmName.SHA256;

    public (byte[] hash, byte[] salt) Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(s_saltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, s_iterations, s_hashAlgorithm, s_hashSize);
        return (hash, salt);
    }

    public bool Test(string password, ReadOnlySpan<byte> expectedHash, ReadOnlySpan<byte> salt)
    {
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, s_iterations, s_hashAlgorithm, s_hashSize);
        return expectedHash.SequenceEqual(hash);
    }
}
