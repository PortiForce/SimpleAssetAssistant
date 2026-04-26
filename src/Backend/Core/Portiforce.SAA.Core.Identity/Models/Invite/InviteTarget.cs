using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Core.Identity.Models.Invite;

public sealed record InviteTarget
{
	// Private empty constructor for EF Core
	private InviteTarget()
	{
		this.Value = null!;
	}

	private InviteTarget(
		string value,
		InviteChannel channel,
		InviteTargetKind kind,
		string locale = "en-GB")
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException("Value is required.", nameof(value));
		}

		ValidateCombination(channel, kind);

		this.Channel = channel;
		this.Kind = kind;
		this.Value = Normalize(kind, value);
		this.Locale = locale;
	}

	public string Value { get; init; }

	public bool IsEmpty => string.IsNullOrWhiteSpace(this.Value) || this.Kind == InviteTargetKind.None;

	public InviteChannel Channel { get; init; }

	public InviteTargetKind Kind { get; init; }

	public string Locale { get; init; } = "en-GB";

	public static InviteTarget Email(string email) =>
		new(email, InviteChannel.Email, InviteTargetKind.Email);

	public static InviteTarget TelegramUserName(string username) =>
		new(username, InviteChannel.Telegram, InviteTargetKind.TelegramUserName);

	public static InviteTarget TelegramUserId(string userId) =>
		new(userId, InviteChannel.Telegram, InviteTargetKind.TelegramUserId);

	public static InviteTarget AppleEmail(string email) =>
		new(email, InviteChannel.AppleAccount, InviteTargetKind.Email);

	public static InviteTarget ApplePhone(string phone) =>
		new(phone, InviteChannel.AppleAccount, InviteTargetKind.Phone);

	public static InviteTarget Restore(
		string value,
		InviteChannel channel,
		InviteTargetKind kind) =>
		new(value, channel, kind);

	private static void ValidateCombination(
		InviteChannel channel,
		InviteTargetKind kind)
	{
		bool valid = channel switch
		{
			InviteChannel.Email =>
				kind == InviteTargetKind.Email,

			InviteChannel.Telegram =>
				kind is InviteTargetKind.TelegramUserName or
					InviteTargetKind.TelegramUserId,

			InviteChannel.AppleAccount =>
				kind is InviteTargetKind.Email or
					InviteTargetKind.Phone,

			_ => false
		};

		if (!valid)
		{
			throw new ArgumentException($"Invite channel '{channel}' does not support target kind '{kind}'.");
		}
	}

	private static string Normalize(InviteTargetKind kind, string value) =>
		kind switch
		{
			InviteTargetKind.Email => Primitives.Email.Create(value).Value,
			InviteTargetKind.Phone => PhoneNumber.Create(value).Value,
			InviteTargetKind.TelegramUserName => NormalizeTelegramUserName(value),
			InviteTargetKind.TelegramUserId => NormalizeTelegramUserId(value),
			_ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported invite target kind.")
		};

	private static string NormalizeTelegramUserName(string username) =>
		username.Trim().TrimStart('@');

	private static string NormalizeTelegramUserId(string userId) =>
		userId.Trim();
}