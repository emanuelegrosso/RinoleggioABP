using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Rinoleggio.Contracts;

public static class ContractSlugGenerator
{
    public static string Generate(string title, Guid id)
    {
        var slug = Normalize(title);
        if (string.IsNullOrWhiteSpace(slug))
        {
            slug = "annuncio";
        }

        return $"annunci/{slug}-{id:N}".ToLowerInvariant();
    }

    private static string Normalize(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        var result = builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        result = Regex.Replace(result, @"[^a-z0-9\s-]", string.Empty);
        result = Regex.Replace(result, @"\s+", "-");
        result = Regex.Replace(result, @"-+", "-").Trim('-');
        return result.Length > 80 ? result[..80].Trim('-') : result;
    }
}
