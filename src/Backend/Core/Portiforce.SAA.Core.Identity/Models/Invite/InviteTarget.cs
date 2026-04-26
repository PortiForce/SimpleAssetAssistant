using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Core.Identity.Models.Invite;

public sealed record InviteTarget
{
	private const int MaxLocaleLength = 6;

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

		string trimmedLocale = (locale ?? string.Empty).Trim();

		if (string.IsNullOrEmpty(trimmedLocale))
		{
			throw new ArgumentException("Locale is required.", nameof(locale));
		}

		if (trimmedLocale.Length > MaxLocaleLength)
		{
			throw new ArgumentException(
				$"Locale cannot exceed {MaxLocaleLength} characters.",
				nameof(locale));
		}

		ValidateCombination(channel, kind);

		this.Channel = channel;
		this.Kind = kind;
		this.Value = Normalize(kind, value);
		this.Locale = trimmedLocale;
	}

	public string Value { get; init; }

	public bool IsEmpty => string.IsNullOrWhiteSpace(this.Value) || this.Kind == InviteTargetKind.None;

	public InviteChannel Channel { get; init; }

	public InviteTargetKind Kind { get; init; }

	public string Locale { get; init; } = "en-GB";

	public static InviteTarget Email(string email, string locale = "en-GB") =>
		new(email, InviteChannel.Email, InviteTargetKind.Email, locale);

	public static InviteTarget TelegramUserName(string username, string locale = "en-GB") =>
		new(username, InviteChannel.Telegram, InviteTargetKind.TelegramUserName, locale);

	public static InviteTarget TelegramUserId(string userId, string locale = "en-GB") =>
		new(userId, InviteChannel.Telegram, InviteTargetKind.TelegramUserId, locale);

	public static InviteTarget AppleEmail(string email, string locale = "en-GB") =>
		new(email, InviteChannel.AppleAccount, InviteTargetKind.Email, locale);

	public static InviteTarget ApplePhone(string phone, string locale = "en-GB") =>
		new(phone, InviteChannel.AppleAccount, InviteTargetKind.Phone, locale);

	public static InviteTarget Restore(
		string value,
		InviteChannel channel,
		InviteTargetKind kind,
		string locale = "en-GB") =>
		new(value, channel, kind, locale);

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