using System.Net.Mail;

using Portiforce.SAA.Core.StaticResources;

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
		catch (ArgumentException)
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

		if (normalized.Length > EntityConstraints.CommonSettings.EmailAddressMaxLength)
		{
			throw new ArgumentException(
				$"Email is too long (max {EntityConstraints.CommonSettings.EmailAddressMaxLength}).",
				nameof(rawData));
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

			if (!string.Equals(addr.Address, email, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			int atIndex = email.LastIndexOf('@');
			if (atIndex <= 0 || atIndex == email.Length - 1)
			{
				return false;
			}

			string domain = email[(atIndex + 1)..];

			return domain.Contains('.');
		}
		catch
		{
			return false;
		}
	}
}