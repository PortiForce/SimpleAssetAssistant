namespace Portiforce.SimpleAssetAssistant.Core.Primitives;

public sealed record Email
{
	// Private Empty Constructor for EF Core
	private Email()
	{

	}

	public string Value { get; init; } = null!;

	private Email(string value) => Value = value;

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

	public override string ToString() => Value;

	private static bool IsValid(string email)
	{
		try
		{
			var addr = new System.Net.Mail.MailAddress(email);

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