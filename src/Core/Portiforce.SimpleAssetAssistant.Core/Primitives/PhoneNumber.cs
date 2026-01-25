namespace Portiforce.SimpleAssetAssistant.Core.Primitives;

public sealed record PhoneNumber
{
	// Private Empty Constructor for EF Core
	private PhoneNumber()
	{

	}

	public string Value { get; init; } = null!;

	private PhoneNumber(string value)
	{
		Value = value;
	}

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

		var normalized = Normalize(rawData);

		if (!IsValid(normalized))
		{
			throw new ArgumentException("Invalid phone number format", nameof(rawData));
		}

		return new PhoneNumber(normalized);
	}

	public override string ToString() => Value;

	private static string Normalize(string input)
	{
		input = input.Trim();

		if (!input.StartsWith('+'))
		{
			throw new ArgumentException("Phone number must be in E.164 international format (+...).", nameof(input));
		}

		string digitsOnly = new string(input.Skip(1).Where(char.IsDigit).ToArray());
		return "+" + digitsOnly;
	}

	private static bool IsValid(string value)
	{
		// E.164: + and 8–15 digits
		return value.Length is >= 9 and <= 16 &&
			   value[0] == '+' &&
			   value.Skip(1).All(char.IsDigit);
	}
}
