namespace Portiforce.SAA.Web.Client.Extensions;

public static class DateTimeExtensions
{
	public static string ToUiDateTime(this DateTime value)
		=> value.ToLocalTime().ToString("yyyy-MM-dd HH:mm");

	public static string ToUiDateTime(this DateTimeOffset value)
		=> value.ToLocalTime().ToString("yyyy-MM-dd HH:mm");

	public static string ToUiDate(this DateTime value)
		=> value.ToLocalTime().ToString("yyyy-MM-dd");

	public static string ToUiDate(this DateTimeOffset value)
		=> value.ToLocalTime().ToString("yyyy-MM-dd");
}
