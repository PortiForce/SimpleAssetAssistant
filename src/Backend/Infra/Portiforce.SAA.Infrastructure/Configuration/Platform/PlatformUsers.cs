namespace Portiforce.SAA.Infrastructure.Configuration.Platform;

public sealed class PlatformUsers
{
	public SqlPlatformUser SqlPlatformUser { get; set; }

	public PlatformUser PlatformOwner { get; set; }

	public PlatformUser PlatformAdmin { get; set; }

	public PlatformUser PlatformBackground { get; set; }

	public PlatformUser DemoBackground { get; set; }

	public PlatformUser DemoAdmin { get; set; }
}