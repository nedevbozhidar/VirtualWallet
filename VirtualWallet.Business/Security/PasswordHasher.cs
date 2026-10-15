using System.Security.Cryptography;

namespace VirtualWallet.Business.Security;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    /// <summary>
    /// Generates a cryptographic salt as a Base64 string.
    /// </summary>
    public static string GenerateSalt()
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        return Convert.ToBase64String(saltBytes);
    }

    /// <summary>
    /// Hashes a password using PBKDF2 with the provided salt.
    /// </summary>
    public static string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, Algorithm, HashSize);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verifies a password against a stored hash and salt.
    /// </summary>
    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        var computedHash = HashPassword(password, storedSalt);
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(computedHash),
            Convert.FromBase64String(storedHash));
    }
}
