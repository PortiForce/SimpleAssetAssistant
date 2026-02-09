using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Core.Identity.Models.Profile;

public sealed record ContactInfo
{
	public ContactInfo(
		Email email,
		PhoneNumber? phone = null,
		Email? backupEmail = null)
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
