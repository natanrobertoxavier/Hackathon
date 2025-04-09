using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Doctor.Application.Extensions;

public static class UsefulExtensions
{
    public static string NormalizeText(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        string normalized = text.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();

        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        string result = Regex.Replace(sb.ToString(), @"[^a-zA-Z0-9]", "").ToUpper();
        return result;
    }
}