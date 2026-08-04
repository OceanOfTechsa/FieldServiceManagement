using FieldServiceManagement.Business.Configuration;
using System.Net;

namespace FieldServiceManagement.Business.UserBusiness
{
    public static class AvatarHelper
    {
        /// <summary>
        /// Generates a UI Avatars URL from a name and optional surname.
        /// Handles one-word names, missing surnames, and URL-encodes safely.
        /// </summary>
        public static string GetAvatar(string? name, string? surname = null, string background = "random")
        {
            var fullName = string.Join(" ", new[] { name, surname }
                .Where(part => !string.IsNullOrWhiteSpace(part)));

            if (string.IsNullOrWhiteSpace(fullName))
            {
                fullName = "Unknown User";
            }

            var encodedName = WebUtility.UrlEncode(fullName);

            return $"{AppSettings.AvatarBaseUrl}?name={encodedName}&background={background}";
        }
    }
}
