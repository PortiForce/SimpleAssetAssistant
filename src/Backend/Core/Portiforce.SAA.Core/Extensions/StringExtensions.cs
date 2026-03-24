namespace Portiforce.SAA.Core.Extensions;

public static class StringExtensions
{
	public static string Truncate(this string? value, int maxTotalLength = 64)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(maxTotalLength);

		if (string.IsNullOrEmpty(value))
		{
			return "<null-or-empty>";
		}

		if (value.Length <= maxTotalLength)
		{
			return value;
		}

		if (maxTotalLength <= 3)
		{
			return new string('.', maxTotalLength);
		}

		return value[..(maxTotalLength - 3)] + "...";
	}
}