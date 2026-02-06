using System.Globalization;
using System.Text;

namespace AlumniApi.Helpers
{
    public static class StringHelper
    {
        // "Čačak" u "cacak", "Foča" u "foca"
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }

        // cuvam u bazi "foca|bih"
        public static string GenerateSearchKey(string city, string country)
        {
            var cleanCity = city != null ? RemoveDiacritics(city) : "";
            var cleanCountry = country != null ? RemoveDiacritics(country) : "";

            return $"{cleanCity}|{cleanCountry}";
        }
    }
}
