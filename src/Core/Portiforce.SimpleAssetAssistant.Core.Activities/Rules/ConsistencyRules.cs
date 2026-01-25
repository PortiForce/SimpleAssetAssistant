using Portiforce.SimpleAssetAssistant.Core.Exceptions;

namespace Portiforce.SimpleAssetAssistant.Core.Activities.Rules;

public static class ConsistencyRules
{
	/// <summary>
	/// Ensures that the decimal precision of a value does not exceed
	/// the maximum allowed scale for the related asset.
	/// </summary>
	/// <param name="value">Actual value to validate</param>
	/// <param name="maxDecimals">Maximum allowed decimal places</param>
	/// <param name="paramName">Name of the field being validated</param>
	/// <exception cref="DomainValidationException">
	/// Thrown when the value exceeds allowed precision.
	/// </exception>
	public static void EnsureScaleDoesNotExceed(
		decimal value,
		byte maxDecimals,
		string paramName)
	{
		var bits = decimal.GetBits(value);
		var scale = (bits[3] >> 16) & 0x7F;

		if (scale > maxDecimals)
		{
			throw new DomainValidationException(
				$"{paramName} has {scale} decimal places, but max allowed is {maxDecimals}.");
		}
	}
}
