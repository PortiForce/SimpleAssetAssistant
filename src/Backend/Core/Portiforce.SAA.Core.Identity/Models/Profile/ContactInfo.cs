using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Core.Identity.Models.Profile;

public sealed record ContactInfo
{
	public ContactInfo(
		Email? email = null,
		PhoneNumber? phone = null)
	{
		this.Email = email;
		this.Phone = phone;
	}

	// Private Empty Constructor for EF Core
	private ContactInfo()
	{
	}

	public Email? Email { get; init; }

	public PhoneNumber? Phone { get; init; }
}