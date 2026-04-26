using System.Text.Encodings.Web;

using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;

using Portiforce.SAA.Application.Interfaces.Services.Invite;
using Portiforce.SAA.Application.Models.Invite;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Infrastructure.Invite;

using EmailAddress = Portiforce.SAA.Core.Primitives.Email;

namespace Portiforce.SAA.Notifications.Worker.Invite;

public sealed class EmailInviteChannelSender(
	IOptions<InviteEmailOptions> options,
	ILogger<EmailInviteChannelSender> logger) : IInviteChannelSender
{
	private readonly InviteEmailOptions options = options.Value;

	public InviteChannel Channel => InviteChannel.Email;

	public async Task<InviteSendResult> SendAsync(
		TenantInvite invite,
		string inviteLink,
		CancellationToken ct)
	{
		ArgumentNullException.ThrowIfNull(invite);
		ArgumentException.ThrowIfNullOrWhiteSpace(inviteLink);

		if (invite.InviteTarget.Channel != InviteChannel.Email ||
			invite.InviteTarget.Kind != InviteTargetKind.Email)
		{
			return InviteSendResult.Failure(
				"invite_channel_not_supported",
				"Invite channel is not supported by this sender.");
		}

		if (!this.options.IsValid())
		{
			logger.LogError("Invite email sender is not configured correctly.");
			return InviteSendResult.Failure(
				"invite_sender_not_configured",
				"Invite delivery is not available right now.");
		}

		EmailAddress recipientEmail;
		try
		{
			recipientEmail = EmailAddress.Create(invite.InviteTarget.Value);
		}
		catch (ArgumentException)
		{
			return InviteSendResult.Failure(
				"invite_recipient_invalid",
				"Invite recipient is invalid.");
		}

		string subject = BuildSubject(this.options.SubjectPrefix);
		string htmlBody = BuildHtmlBody(invite, inviteLink);
		string textBody = BuildTextBody(invite, inviteLink);

		try
		{
			using MimeMessage message = CreateMessage(
				this.options,
				recipientEmail.Value,
				subject,
				htmlBody,
				textBody);

			using SmtpClient client = new();

#if DEBUG

			// todo: NOT FOR PRODUCTION USE - This is to allow sending emails even if the SMTP server has a self-signed certificate or other certificate issues.
			client.ServerCertificateValidationCallback = static (_, _, _, _) => true;
#endif

			await client.ConnectAsync(
				this.options.Host,
				this.options.Port,
				GetSecureSocketOptions(this.options),
				ct);

			if (!string.IsNullOrWhiteSpace(this.options.UserName))
			{
				await client.AuthenticateAsync(this.options.UserName, this.options.Password, ct);
			}

			_ = await client.SendAsync(message, ct);
			await client.DisconnectAsync(true, ct);

			return InviteSendResult.Success();
		}
		catch (CommandException ex)
		{
			logger.LogWarning(
				ex,
				"Invite email sending failed for invite {InviteId}.",
				invite.Id);

			return InviteSendResult.Failure(
				"invite_email_delivery_failed",
				"Invite email could not be delivered.");
		}
		catch (ProtocolException ex)
		{
			logger.LogWarning(
				ex,
				"Invite email sending failed for invite {InviteId}.",
				invite.Id);

			return InviteSendResult.Failure(
				"invite_email_delivery_failed",
				"Invite email could not be delivered.");
		}
		catch (InvalidOperationException ex)
		{
			logger.LogWarning(
				ex,
				"Invite email sending failed for invite {InviteId}.",
				invite.Id);

			return InviteSendResult.Failure(
				"invite_email_delivery_failed",
				"Invite email could not be delivered.");
		}
	}

	private static string BuildSubject(string? subjectPrefix)
	{
		string prefix = string.IsNullOrWhiteSpace(subjectPrefix)
			? string.Empty
			: $"{subjectPrefix.Trim()} ";

		return $"{prefix}You have been invited";
	}

	private static string BuildHtmlBody(TenantInvite invite, string inviteUrl)
	{
		string encodedAlias = HtmlEncoder.Default.Encode(invite.Alias);
		string encodedUrl = HtmlEncoder.Default.Encode(inviteUrl);
		string encodedRole = HtmlEncoder.Default.Encode(invite.IntendedRole.ToString());
		string encodedTier = HtmlEncoder.Default.Encode(invite.IntendedTier.ToString());

		return $"""
                <html>
                <body>
                	<p>Hello {encodedAlias},</p>
                	<p>You have been invited to join a tenant in Portiforce SAA.</p>
                	<p><strong>Role:</strong> {encodedRole}<br />
                	<strong>Tier:</strong> {encodedTier}</p>
                	<p>Use the link below to review and accept the invitation:</p>
                	<p><a href="{encodedUrl}">{encodedUrl}</a></p>
                	<p>If you did not expect this invitation, you can safely ignore this message.</p>
                </body>
                </html>
                """;
	}

	private static string BuildTextBody(TenantInvite invite, string inviteUrl) =>
		$"""
         Hello {invite.Alias},

         You have been invited to join a tenant in Portiforce SAA.

         Role: {invite.IntendedRole}
         Tier: {invite.IntendedTier}

         Open this link to review and accept the invitation:
         {inviteUrl}

         If you did not expect this invitation, you can safely ignore this message.
         """;

	private static MimeMessage CreateMessage(
		InviteEmailOptions options,
		string recipientEmail,
		string subject,
		string htmlBody,
		string textBody)
	{
		MimeMessage message = new();

		message.From.Add(new MailboxAddress(options.FromDisplayName, options.FromEmail));
		message.To.Add(MailboxAddress.Parse(recipientEmail));
		message.Subject = subject;

		BodyBuilder bodyBuilder = new() { TextBody = textBody, HtmlBody = htmlBody };

		message.Body = bodyBuilder.ToMessageBody();

		return message;
	}

	private static SecureSocketOptions GetSecureSocketOptions(InviteEmailOptions options)
	{
		if (!options.EnableSsl)
		{
			return SecureSocketOptions.None;
		}

		return options.Port switch
		{
			465 => SecureSocketOptions.SslOnConnect,
			_ => SecureSocketOptions.StartTls
		};
	}
}