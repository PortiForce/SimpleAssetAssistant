using System.Globalization;

namespace Portiforce.SimpleAssetAssistant.Core.Extensions;

public static class DateExtensions
{
	public static DateTimeOffset FromUnixSeconds(long unixSeconds)
		=> DateTimeOffset.FromUnixTimeSeconds(unixSeconds);

	public static bool TryFromUnixSeconds(string unixRawDate, out DateTimeOffset value)
	{
		if (long.TryParse(unixRawDate, NumberStyles.Integer, CultureInfo.InvariantCulture, out var seconds))
		{
			value = DateTimeOffset.FromUnixTimeSeconds(seconds);
			return true;
		}

		value = default;
		return false;
	}
}