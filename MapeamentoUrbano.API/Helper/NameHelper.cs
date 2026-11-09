using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MapeamentoUrbano.API.Helper
{
    public static partial class NameHelper
    {
        [GeneratedRegex("^[0-9A-Za-zÀ-ÖØ-öø-ÿ '-]+$")]
        private static partial Regex MyRegex();

        public static bool VerifyIfNameContainsSpecialCharacters(string? name) =>
            !string.IsNullOrEmpty(name) && !MyRegex().IsMatch(name);

        public static string? StringNormalization(this string formattedString) =>
            formattedString != null ? formattedString.ToUpper().RemoveAccentsAndCedilla() : formattedString;

        public static string RemoveAccentsAndCedilla(this string formattedString) =>
            new(formattedString.Normalize(NormalizationForm.FormD)
                .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark && !"´~^¸¨".Contains(ch))
                .ToArray());
    }
}
