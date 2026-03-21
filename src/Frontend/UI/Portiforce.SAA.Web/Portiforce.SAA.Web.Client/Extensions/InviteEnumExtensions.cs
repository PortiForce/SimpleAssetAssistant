using Microsoft.Extensions.Localization;

using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Web.Client.Extensions;

public static class InviteEnumExtensions
{
	public static readonly IReadOnlyList<InviteChannel> ChannelOptions =
	[
		InviteChannel.Email,
		InviteChannel.Telegram,
		InviteChannel.AppleId
	];

	public static readonly IReadOnlyList<InviteStatus> StatusOptions =
	[
		InviteStatus.Pending,
		InviteStatus.Accepted,
		InviteStatus.Revoked,
		InviteStatus.Expired,
		InviteStatus.Declined,
		InviteStatus.Failed
	];

	public static readonly IReadOnlyList<InviteTenantRole> RoleOptions =
	[
		InviteTenantRole.TenantUser,
		InviteTenantRole.TenantAdmin
	];

	public static readonly IReadOnlyList<InviteAccountTier> TierOptions =
	[
		InviteAccountTier.Observer,
		InviteAccountTier.Investor,
		InviteAccountTier.Strategist
	];

	public static string ToDisplayName(this InviteChannel channel, IStringLocalizer localizer) => channel switch
	{
		InviteChannel.Email => localizer["ChannelEmail"],
		InviteChannel.Telegram => localizer["ChannelTelegram"],
		InviteChannel.AppleId => localizer["ChannelAppleId"],
		InviteChannel.None => localizer["None"],
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

	public static string ToDisplayName(this InviteTenantRole role, IStringLocalizer localizer) => role switch
	{
		InviteTenantRole.TenantUser => localizer["RoleTenantUser"],
		InviteTenantRole.TenantAdmin => localizer["RoleTenantAdmin"],
		InviteTenantRole.None => localizer["None"],
		_ => role.ToString()
	};

	public static string ToDisplayName(this InviteAccountTier tier, IStringLocalizer localizer) => tier switch
	{
		InviteAccountTier.Observer => localizer["TierObserver"],
		InviteAccountTier.Investor => localizer["TierInvestor"],
		InviteAccountTier.Strategist => localizer["TierStrategist"],
		_ => tier.ToString()
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