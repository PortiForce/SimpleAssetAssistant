namespace Portiforce.SimpleAssetAssistant.Core.Extensions;

internal static class StringExtensions
{
	public static string Truncate(this string? value, int maxLength = 64)
	{
		if (string.IsNullOrEmpty(value))
		{
			return "<null-or-empty>";
		}

		return value.Length <= maxLength
			? value
			: value[..maxLength] + "...";
	}
}
