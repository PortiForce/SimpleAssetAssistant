using Portiforce.SimpleAssetAssistant.Core.Primitives;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;

public sealed record ContactInfo
{
	public Email Email { get; init; }

	public PhoneNumber? Phone { get; init; }

	public Email? BackupEmail { get; init; }
}
