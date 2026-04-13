using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Portiforce.SAA.Application.Interfaces.Services.Invite;
using Portiforce.SAA.Application.Models.Invite;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Infrastructure.Invite;

using EmailAddress = Portiforce.SAA.Core.Primitives.Email;

namespace Portiforce.SAA.Infrastructure.Services.Invite;

public sealed class EmailInviteChannelSender(
	IOptions<InviteEmailOptions> options,
	ILogger<EmailInviteChannelSender> logger) : IInviteChannelSender
{
	private readonly InviteEmailOptions options = options.Value;

	public InviteChannel Channel => InviteChannel.Email;

	public async Task<InviteSendResult> SendAsync(
		TenantInvite invite,
		string rawInviteToken,
		CancellationToken ct)
	{
		ArgumentNullException.ThrowIfNull(invite);
		ArgumentException.ThrowIfNullOrWhiteSpace(rawInviteToken);

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

		string inviteUrl = BuildInviteUrl(this.options.PublicAppBaseUrl, rawInviteToken);
		string subject = BuildSubject(this.options.SubjectPrefix);
		string htmlBody = BuildHtmlBody(invite, inviteUrl);
		string textBody = BuildTextBody(invite, inviteUrl);

		try
		{
			using MailMessage message = CreateMailMessage(
				this.options,
				recipientEmail.Value,
				subject,
				htmlBody,
				textBody);

			using SmtpClient client = CreateSmtpClient(this.options);

			await client.SendMailAsync(message).WaitAsync(ct);

			return InviteSendResult.Success();
		}
		catch (SmtpException ex)
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

	private static string BuildInviteUrl(string publicAppBaseUrl, string rawInviteToken)
	{
		string baseUrl = publicAppBaseUrl.TrimEnd('/');
		string encodedToken = WebUtility.UrlEncode(rawInviteToken);

		return $"{baseUrl}/invite/{encodedToken}";
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
                	<p>
                		Use the link below to review and accept the invitation:
                	</p>
                	<p>
                		<a href="{encodedUrl}">{encodedUrl}</a>
                	</p>
                	<p>
                		If you did not expect this invitation, you can safely ignore this message.
                	</p>
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

	private static MailMessage CreateMailMessage(
		InviteEmailOptions options,
		string recipientEmail,
		string subject,
		string htmlBody,
		string textBody)
	{
		MailMessage message = new()
		{
			From = new MailAddress(options.FromEmail, options.FromDisplayName),
			Subject = subject,
			Body = textBody,
			IsBodyHtml = false
		};

		_ = message.To.Add(recipientEmail);

		AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
		AlternateView textView = AlternateView.CreateAlternateViewFromString(textBody, null, "text/plain");

		_ = message.AlternateViews.Add(textView);
		_ = message.AlternateViews.Add(htmlView);

		return message;
	}

	private static SmtpClient CreateSmtpClient(InviteEmailOptions options)
	{
		SmtpClient client = new(options.Host, options.Port)
		{
			EnableSsl = options.EnableSsl,
			DeliveryMethod = SmtpDeliveryMethod.Network
		};

		if (!string.IsNullOrWhiteSpace(options.UserName))
		{
			client.Credentials = new NetworkCredential(options.UserName, options.Password);
		}
		else
		{
			client.UseDefaultCredentials = true;
		}

		return client;
	}
}