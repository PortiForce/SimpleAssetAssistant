namespace Portiforce.SAA.Application.UseCases.Invite.Result;

public sealed record CreateInviteResult(
	Guid InviteId,
	string Token,
	DateTimeOffset ExpiresAtUtc);
