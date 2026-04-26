namespace Portiforce.SAA.Infrastructure.Invite;

public sealed class InviteEmailOptions
{
	public string Host { get; set; } = string.Empty;

	public int Port { get; set; } = 587;

	public bool EnableSsl { get; set; } = true;

	public string FromEmail { get; set; } = string.Empty;

	public string FromDisplayName { get; set; } = "Portiforce SAA";

	public string? UserName { get; set; }

	public string? Password { get; set; }

	public string? SubjectPrefix { get; set; }

	public bool IsValid() =>
		!string.IsNullOrWhiteSpace(this.Host) &&
		this.Port > 0 &&
		!string.IsNullOrWhiteSpace(this.FromEmail);
}