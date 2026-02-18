namespace Portiforce.SAA.Contracts.Models.Invite;

public sealed class CreateInviteRequest
{
	public string Email { get; set; } = string.Empty;

	public string IntendedRole { get; set; } = "TenantUser";

	public string IntendedTier { get; set; } = "Observer";
}
