using System.Text.RegularExpressions;

namespace VirtualWallet.Business.Security;

public static partial class PasswordValidator
{
    private const int MinLength = 8;

    /// <summary>
    /// Validates that a password meets the project requirements:
    /// at least 8 characters, contains a capital letter, a digit, and a special symbol.
    /// Returns null if valid, or an error message string if invalid.
    /// </summary>
    public static string? Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return "Password is required.";

        if (password.Length < MinLength)
            return $"Password must be at least {MinLength} characters long.";

        if (!UppercaseRegex().IsMatch(password))
            return "Password must contain at least one uppercase letter.";

        if (!DigitRegex().IsMatch(password))
            return "Password must contain at least one digit.";

        if (!SpecialCharRegex().IsMatch(password))
            return "Password must contain at least one special character (+, -, *, &, ^, !, @, #, etc.).";

        return null;
    }

    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex UppercaseRegex();

    [GeneratedRegex(@"\d")]
    private static partial Regex DigitRegex();

    [GeneratedRegex(@"[+\-*&^!@#$%~]")]
    private static partial Regex SpecialCharRegex();
}
