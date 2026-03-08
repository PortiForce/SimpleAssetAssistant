namespace Portiforce.SAA.Contracts.Models.Client.Invite;

public sealed record CreateInviteResponse(
	Guid InviteId,
	string RawToken,
	DateTimeOffset ExpiresAtUtc);
