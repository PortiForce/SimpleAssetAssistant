namespace Portiforce.SAA.Application.Models.Invite;

public sealed record InviteSendResult(
	bool IsSuccess,
	string? ErrorCode = null,
	string? ErrorMessage = null)
{
	public static InviteSendResult Success() => new(true);

	public static InviteSendResult Failure(
		string errorCode,
		string errorMessage) =>
		new(false, errorCode, errorMessage);
}