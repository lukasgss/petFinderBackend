using System.Globalization;
using System.Text;

namespace Application.Extensions;

public static class StringExtensions
{
	public static string? ToStrWithoutDiacritics(this string? accentedStr)
	{
		if (accentedStr is null)
		{
			return null;
		}

		string normalizedString = accentedStr.Normalize(NormalizationForm.FormD);
		StringBuilder stringBuilder = new(capacity: normalizedString.Length);

		foreach (char character in normalizedString)
		{
			UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
			if (unicodeCategory != UnicodeCategory.NonSpacingMark)
			{
				stringBuilder.Append(character);
			}
		}

		return stringBuilder
			.ToString()
			.Normalize(NormalizationForm.FormC);
	}
}