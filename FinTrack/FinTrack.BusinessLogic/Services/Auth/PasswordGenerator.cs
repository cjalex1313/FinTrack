using Microsoft.AspNetCore.Identity;
using System.Text;

namespace FinTrack.BusinessLogic.Services.Auth
{
    public static class PasswordGenerator
    {
        public static string GeneratePassword(IdentityOptions options, int length = 12)
        {
            var random = new Random();
            var password = new StringBuilder();

            // Character pools
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string nonAlphanumeric = "!@$?_-" + "\"#%&()*+,:;<=>[]^`{|}~";

            // Ensure required characters
            if (options.Password.RequireUppercase)
                password.Append(uppercase[random.Next(uppercase.Length)]);

            if (options.Password.RequireLowercase)
                password.Append(lowercase[random.Next(lowercase.Length)]);

            if (options.Password.RequireDigit)
                password.Append(digits[random.Next(digits.Length)]);

            if (options.Password.RequireNonAlphanumeric)
                password.Append(nonAlphanumeric[random.Next(nonAlphanumeric.Length)]);

            // Fill the rest with random characters from all pools
            var allChars = "";
            if (options.Password.RequireUppercase) allChars += uppercase;
            if (options.Password.RequireLowercase) allChars += lowercase;
            if (options.Password.RequireDigit) allChars += digits;
            if (options.Password.RequireNonAlphanumeric) allChars += nonAlphanumeric;

            // If no requirements are set, use all pools
            if (string.IsNullOrEmpty(allChars))
                allChars = uppercase + lowercase + digits + nonAlphanumeric;

            while (password.Length < length)
                password.Append(allChars[random.Next(allChars.Length)]);

            // Shuffle the result to avoid predictable positions
            return new string([.. password.ToString().OrderBy(_ => random.Next())]);
        }
    }
}