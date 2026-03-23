using System.Net.Mail;

namespace Portiforce.SAA.Core.Primitives;

public sealed record Email
{
	// Private Empty Constructor for EF Core
	private Email()
	{
	}

	private Email(string value)
	{
		this.Value = value;
	}

	public string Value { get; init; } = null!;

	public static bool TryCreate(string rawData, out Email email)
	{
		email = default;
		try
		{
			email = Create(rawData);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static Email Create(string rawData)
	{
		if (string.IsNullOrWhiteSpace(rawData))
		{
			throw new ArgumentException("Email cannot be empty.", nameof(rawData));
		}

		string normalized = rawData.Trim().ToLowerInvariant();

		if (normalized.Length > 255)
		{
			throw new ArgumentException("Email is too long (max 255).", nameof(rawData));
		}

		if (!IsValid(normalized))
		{
			throw new ArgumentException("Invalid email format.", nameof(rawData));
		}

		return new Email(normalized);
	}

	public override string ToString() => this.Value;

	private static bool IsValid(string email)
	{
		try
		{
			MailAddress addr = new(email);

			// This prevents some edge cases where MailAddress accepts but normalizes unexpectedly.
			// It also ensures we don't accept values with display names etc.
			return string.Equals(addr.Address, email, StringComparison.OrdinalIgnoreCase);
		}
		catch
		{
			return false;
		}
	}
}