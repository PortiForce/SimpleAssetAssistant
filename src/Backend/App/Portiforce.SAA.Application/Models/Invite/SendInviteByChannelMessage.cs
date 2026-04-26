using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Models.Invite;

public sealed record SendInviteByChannelMessage
{
	public TenantId TenantId { get; init; }

	public Guid InviteId { get; init; }

	public InviteChannel Channel { get; init; }

	public string Recipient { get; init; } = string.Empty;

	public string? Alias { get; init; }

	public string InviteUrl { get; init; } = string.Empty;

	public DateTimeOffset ExpiresAtUtc { get; init; }

	public static SendInviteByChannelMessage Create(
		TenantId tenantId,
		Guid inviteId,
		InviteChannel channel,
		string recipient,
		string? alias,
		string inviteUrl,
		DateTimeOffset expiresAtUtc,
		DateTimeOffset now)
	{
		if (tenantId.IsEmpty)
		{
			throw new ArgumentException("TenantId is not defined.", nameof(tenantId));
		}

		if (inviteId == Guid.Empty)
		{
			throw new ArgumentException("InviteId is not defined.", nameof(inviteId));
		}

		if (string.IsNullOrWhiteSpace(recipient))
		{
			throw new ArgumentException("Recipient is not defined.", nameof(recipient));
		}

		if (string.IsNullOrWhiteSpace(inviteUrl))
		{
			throw new ArgumentException("InviteUrl is not defined.", nameof(inviteUrl));
		}

		if (expiresAtUtc <= now)
		{
			throw new ArgumentException("Invite expiration must be in the future.", nameof(expiresAtUtc));
		}

		return new SendInviteByChannelMessage
		{
			TenantId = tenantId,
			InviteId = inviteId,
			Channel = channel,
			Recipient = recipient.Trim(),
			Alias = string.IsNullOrWhiteSpace(alias) ? null : alias.Trim(),
			InviteUrl = inviteUrl.Trim(),
			ExpiresAtUtc = expiresAtUtc
		};
	}
}