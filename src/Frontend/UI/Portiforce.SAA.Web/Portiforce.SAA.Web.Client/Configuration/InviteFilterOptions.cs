using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Web.Client.Configuration;

public static class InviteFilterOptions
{
	public static readonly IReadOnlyList<InviteChannel> Channels =
	[
		InviteChannel.Email,
		InviteChannel.Telegram,
		InviteChannel.AppleAccount
	];

	public static readonly IReadOnlyList<InviteStatus> Statuses =
	[
		InviteStatus.Pending,
		InviteStatus.Accepted,
		InviteStatus.Revoked,
		InviteStatus.Expired,
		InviteStatus.Declined,
		InviteStatus.Failed
	];
}