using Portiforce.SimpleAssetAssistant.Core.Primitives;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;

public sealed record ContactInfo
{
	public ContactInfo(
		Email email,
		PhoneNumber? phone,
		Email? backupEmail)
	{
		Email = email;
		Phone = phone;
		BackupEmail = backupEmail;
	}

	// Private Empty Constructor for EF Core
	private ContactInfo()
	{

	}

	public Email Email { get; init; }

	public PhoneNumber? Phone { get; init; }

	public Email? BackupEmail { get; init; }
}
