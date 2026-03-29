using Portiforce.SAA.Core.StaticResources;

namespace Portiforce.SAA.Core.Primitives;

public sealed record PhoneNumber
{
	// Private Empty Constructor for EF Core
	private PhoneNumber()
	{
	}

	private PhoneNumber(string value)
	{
		this.Value = value;
	}

	public string Value { get; init; } = null!;

	public static bool TryCreate(string rawData, out PhoneNumber phoneNumber)
	{
		phoneNumber = default;
		try
		{
			phoneNumber = Create(rawData);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static PhoneNumber Create(string rawData)
	{
		if (string.IsNullOrWhiteSpace(rawData))
		{
			throw new ArgumentException("Phone number cannot be empty", nameof(rawData));
		}

		rawData = rawData.Trim();

		if (!rawData.StartsWith('+'))
		{
			throw new ArgumentException("Phone number must be in E.164 international format (+...).", nameof(rawData));
		}

		string digitsOnly = new(rawData.Skip(1).Where(char.IsDigit).ToArray());
		rawData = "+" + digitsOnly;

		if (!IsValid(rawData))
		{
			throw new ArgumentException("Invalid phone number format", nameof(rawData));
		}

		return new PhoneNumber(rawData);
	}

	public override string ToString() => this.Value;

	private static bool IsValid(string value)
	{
		// E.164: + and 8–15 digits
		return value.Length is >= EntityConstraints.CommonSettings.PhoneNumberMinLength
				   and <= EntityConstraints.CommonSettings.PhoneNumberMaxLength &&
			   value[0] == '+' &&
			   value.Skip(1).All(char.IsDigit);
	}
}