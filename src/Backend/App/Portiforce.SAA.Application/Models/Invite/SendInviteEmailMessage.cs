using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Models.Invite;

public sealed record SendInviteEmailMessage
{
	private SendInviteEmailMessage(
		TenantId tenantId,
		Guid inviteId,
		string recipientEmail,
		string? alias,
		string inviteUrl,
		DateTimeOffset expiresAtUtc)
	{
		this.TenantId = tenantId;
		this.InviteId = inviteId;
		this.RecipientEmail = recipientEmail;
		this.Alias = alias;
		this.InviteUrl = inviteUrl;
		this.ExpiresAtUtc = expiresAtUtc;
	}

	public TenantId TenantId { get; }

	public Guid InviteId { get; }

	public string RecipientEmail { get; }

	public string? Alias { get; }

	public string InviteUrl { get; }

	public DateTimeOffset ExpiresAtUtc { get; }

	public static SendInviteEmailMessage Create(
		TenantId tenantId,
		Guid inviteId,
		string recipientEmail,
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

		if (string.IsNullOrWhiteSpace(recipientEmail))
		{
			throw new ArgumentException("Recipient email is not defined.", nameof(recipientEmail));
		}

		if (string.IsNullOrWhiteSpace(inviteUrl))
		{
			throw new ArgumentException("InviteUrl is not defined.", nameof(inviteUrl));
		}

		if (expiresAtUtc <= now)
		{
			throw new ArgumentException("Invite expiration must be in the future.", nameof(expiresAtUtc));
		}

		return new SendInviteEmailMessage(
			tenantId,
			inviteId,
			recipientEmail.Trim(),
			string.IsNullOrWhiteSpace(alias) ? null : alias.Trim(),
			inviteUrl.Trim(),
			expiresAtUtc);
	}
}