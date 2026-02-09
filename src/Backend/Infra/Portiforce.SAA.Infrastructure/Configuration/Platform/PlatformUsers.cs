namespace Portiforce.SAA.Infrastructure.Configuration.Platform;

public sealed class PlatformUsers
{
	public SqlPlatformUser SqlPlatformUser { get; set; }

	public PlatformUser PlatformOwner { get; set; }

	public PlatformUser PlatformAdmin { get; set; }
}
