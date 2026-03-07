namespace Portiforce.SAA.Contracts.Models.Invite;

public sealed record CreateInviteResponse(
	Guid InviteId,
	string RawToken,
	DateTimeOffset ExpiresAtUtc);
