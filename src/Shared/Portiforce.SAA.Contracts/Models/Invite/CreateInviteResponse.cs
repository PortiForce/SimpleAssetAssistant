namespace Portiforce.SAA.Contracts.Models.Invite;

public sealed class CreateInviteResponse
{
	public Guid InviteId { get; set; }

	public string AcceptInviteUrl { get; set; } = default!;

	public string DeclineInviteUrl { get; set; } = default!;
	
	public DateTimeOffset ExpiresAtUtc { get; set; }
}
