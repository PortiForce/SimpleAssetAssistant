using Microsoft.Extensions.Localization;

using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Web.Client.Extensions;

public static class InviteEnumExtensions
{
	public static string ToDisplayName(this InviteChannel channel, IStringLocalizer localizer) => channel switch
	{
		InviteChannel.Email => localizer["ChannelEmail"],
		InviteChannel.Telegram => localizer["ChannelTelegram"],
		InviteChannel.AppleId => localizer["ChannelAppleId"],
		_ => channel.ToString()
	};

	public static string ToDisplayName(this InviteStatus status, IStringLocalizer localizer) => status switch
	{
		InviteStatus.Pending => localizer["StatusPending"],
		InviteStatus.Accepted => localizer["StatusAccepted"],
		InviteStatus.Revoked => localizer["StatusRevoked"],
		InviteStatus.Expired => localizer["StatusExpired"],
		InviteStatus.Declined => localizer["StatusDeclined"],
		InviteStatus.Failed => localizer["StatusFailed"],
		_ => status.ToString()
	};

	public static string ToCssBadgeClass(this InviteStatus status) => status switch
	{
		InviteStatus.Pending => "bg-warning text-dark",
		InviteStatus.Accepted => "bg-success",
		InviteStatus.Revoked => "bg-danger",
		InviteStatus.Expired => "bg-dark",
		_ => "bg-secondary"
	};
}