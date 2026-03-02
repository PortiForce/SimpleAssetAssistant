namespace Portiforce.SAA.Web.Security;

public sealed class TenancyOptions
{
	public const string SectionName = "Tenancy";

	public required string BaseDomain { get; init; } = "portiforce.ai";
	public string[] LandingHosts { get; init; } = ["portiforce.ai", "www.portiforce.ai"];

	public bool IsLandingHost(string host)
		=> LandingHosts.Any(h => string.Equals(h, host, StringComparison.OrdinalIgnoreCase));

	public static string? TryGetPrefix(string host, string baseDomain)
	{
		// host = app.portiforce.ai, baseDomain = portiforce.ai
		var suffix = "." + baseDomain;
		if (!host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
			return null;

		var prefix = host[..^suffix.Length];
		if (string.IsNullOrWhiteSpace(prefix) || prefix.Contains('.'))
			return null;

		return prefix;
	}
}