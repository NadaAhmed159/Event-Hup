using System.Linq;
using System.Text.RegularExpressions;

namespace EventHub.BLL.Validation
{
    public static class CredentialValidation
    {
        public const int PasswordMinLength = 8;

        public const string PasswordPolicyMessage =
            "Password must be at least 8 characters and include at least one uppercase letter, " +
            "one lowercase letter, and one special character (anything that is not a letter or a digit, for example @ # ! or a space).";

        public const string EmailFormatMessage =
            "Enter a valid email address (for example, name@example.com).";

        /// <summary>
        /// Optional phone: empty is valid. If provided, must contain 10–15 digits (spaces, dashes, parentheses, and a leading + are allowed).
        /// </summary>
        public const string PhoneFormatMessage =
            "Phone number must contain 10 to 15 digits. You may use spaces, dashes, parentheses, or a leading +.";

        /// <summary>Practical format check: local@domain with a TLD of at least two letters.</summary>
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled,
            TimeSpan.FromMilliseconds(250));

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Length > 256)
                return false;
            try
            {
                return EmailRegex.IsMatch(email.Trim());
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < PasswordMinLength)
                return false;
            // Use Unicode categories so Latin letters from any keyboard layout count (ASCII-only ranges miss look-alikes).
            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasSpecial = password.Any(static c => !char.IsLetter(c) && !char.IsDigit(c));
            return hasUpper && hasLower && hasSpecial;
        }

        /// <summary>Null or whitespace is valid (optional phone).</summary>
        public static bool IsValidPhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true;
            var digitCount = phone.Count(char.IsDigit);
            return digitCount is >= 10 and <= 15;
        }
    }
}
