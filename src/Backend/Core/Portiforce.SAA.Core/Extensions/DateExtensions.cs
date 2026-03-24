using System.Globalization;

namespace Portiforce.SAA.Core.Extensions;

public static class DateExtensions
{
	public static DateTimeOffset FromUnixSeconds(long unixSeconds)
		=> DateTimeOffset.FromUnixTimeSeconds(unixSeconds);

	public static bool TryFromUnixSeconds(string unixRawDate, out DateTimeOffset value)
	{
		if (long.TryParse(unixRawDate, NumberStyles.Integer, CultureInfo.InvariantCulture, out long seconds))
		{
			try
			{
				value = DateTimeOffset.FromUnixTimeSeconds(seconds);
				return true;
			}
			catch (ArgumentOutOfRangeException)
			{
			}
		}

		value = default;
		return false;
	}
}