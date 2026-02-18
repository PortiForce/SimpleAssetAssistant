using Portiforce.SAA.Core.Identity.Enums;

namespace Portiforce.SAA.Core.Identity.Models.Invite;

public sealed record InviteTarget
{
	// Private Empty Constructor for EF Core
	private InviteTarget()
	{

	}

	private InviteTarget(string value, InviteChannel type)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException("Value is required.", nameof(value));
		}

		Value = value;
		Type = type;
	}

	public string Value { get; init; }
	public InviteChannel Type { get; init; }

	public static InviteTarget Email(string email)
		=> new(email, InviteChannel.Email);

	public static InviteTarget Telegram(string username)
		=> new(NormalizeTelegram(username), InviteChannel.Telegram);

	public static InviteTarget AppleId(string sub)
		=> new(sub, InviteChannel.AppleId);

	// EF Core mapping helper if needed
	public static InviteTarget Restore(string value, InviteChannel type)
		=> new(value, type);

	private static string NormalizeTelegram(string u) =>
		u.Trim().TrimStart('@');
}