namespace Portiforce.SAA.Web.Security;

public sealed class TenancyOptions
{
	public const string SectionName = "Tenancy";

	public required string BaseDomain { get; init; } = "dev.localhost";
	public string[] LandingHosts { get; init; } = ["dev.localhost", "www.dev.localhost"];

	public bool IsLandingHost(string host)
		=> LandingHosts.Any(h => string.Equals(h, host, StringComparison.OrdinalIgnoreCase));

	public static string? TryGetPrefix(string host, string baseDomain)
	{
		// host = app.dev.localhost, baseDomain = dev.localhost
		var suffix = "." + baseDomain;
		if (!host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
			return null;

		var prefix = host[..^suffix.Length];
		if (string.IsNullOrWhiteSpace(prefix) || prefix.Contains('.'))
			return null;

		return prefix;
	}
}