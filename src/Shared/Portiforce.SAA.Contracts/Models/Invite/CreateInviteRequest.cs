using System.ComponentModel.DataAnnotations;
using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Invite;

public sealed record CreateInviteRequest : IValidatableObject
{
	[Required]
	public InviteChannel Channel { get; set; } = InviteChannel.Email;

	[Required]
	[StringLength(256, MinimumLength = 2)]
	public string TargetValue { get; set; } = string.Empty;

	[Required]
	public InviteTenantRole IntendedRole { get; set; } = InviteTenantRole.TenantUser;

	[Required]
	public InviteAccountTier IntendedTier { get; set; } = InviteAccountTier.Investor;

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (string.IsNullOrWhiteSpace(TargetValue))
		{
			yield return new ValidationResult("Value is required.", [nameof(TargetValue)]);
			yield break;
		}

		var rawValue = TargetValue.Trim();

		switch (Channel)
		{
			case InviteChannel.Email:
				// Let server be the source of truth, but catch obvious issues here.
				if (!new EmailAddressAttribute().IsValid(rawValue))
				{
					yield return new ValidationResult("Please enter a valid email address.", [nameof(TargetValue)]);
				}
				break;

			case InviteChannel.Telegram:
				// Accept "@nick" or "nick"
				var nick = rawValue.StartsWith("@") ? rawValue[1..] : rawValue;
				if (nick.Length is < 5 or > 32)
				{
					yield return new ValidationResult("Telegram username should be 5–32 characters.", [nameof(TargetValue)]);
				}
				// basic charset check (Telegram usernames are letters/digits/underscore)
				if (nick.Any(c => !(char.IsLetterOrDigit(c) || c == '_')))
				{
					yield return new ValidationResult(
						"Telegram username may contain letters, digits, and underscore only.",
						[nameof(TargetValue)]);
				}
				break;

			case InviteChannel.AppleId:
				// Usually an email, but could be a stable Apple subject on server side.
				// In UI we’ll treat it as an email-like identifier.
				if (!new EmailAddressAttribute().IsValid(rawValue))
				{
					yield return new ValidationResult(
						"Apple ID is typically an email address. Please enter a valid value.",
						[nameof(TargetValue)]);
				}
				break;

			default:
				yield return new ValidationResult("Unknown invite channel.", [nameof(Channel)]);
				break;
		}

		if (IntendedRole == InviteTenantRole.None)
		{
			yield return new ValidationResult("Please select an intended role.", [nameof(IntendedRole)]);
		}

		if (IntendedTier == InviteAccountTier.None)
		{
			yield return new ValidationResult("Please select an account tier.", [nameof(IntendedTier)]);
		}
	}
}
