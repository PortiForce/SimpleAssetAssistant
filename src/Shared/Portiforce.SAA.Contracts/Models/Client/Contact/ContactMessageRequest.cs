using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Resources;

using Portiforce.SAA.Contracts.Resources;

namespace Portiforce.SAA.Contracts.Models.Client.Contact;

public sealed record ContactMessageRequest : IValidatableObject
{
	private const int MaxNameLength = 100;
	private const int MaxEmailAddressLength = 254;
	private const int MaxSubjectLength = 160;
	private const int MinSubjectLength = 2;
	private const int MaxMessageLength = 4000;
	private const int MinMessageLength = 10;

	private static readonly EmailAddressAttribute EmailAddressValidator = new();

	public string Name { get; set; } = string.Empty;

	public string EmailAddress { get; set; } = string.Empty;

	public string Subject { get; set; } = string.Empty;

	public string Message { get; set; } = string.Empty;

	public bool AgreeToBeContacted { get; set; }

	public string? Website { get; set; }

	public DateTimeOffset FormStartedAtUtc { get; set; } = DateTimeOffset.UtcNow;

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (string.IsNullOrWhiteSpace(this.Name))
		{
			yield return CreateValidationResult(nameof(this.Name), "NameRequired");
		}
		else if (this.Name.Trim().Length > MaxNameLength)
		{
			yield return CreateValidationResult(nameof(this.Name), "NameTooLong", MaxNameLength);
		}

		if (string.IsNullOrWhiteSpace(this.EmailAddress))
		{
			yield return CreateValidationResult(nameof(this.EmailAddress), "EmailAddressRequired");
		}
		else if (this.EmailAddress.Trim().Length > MaxEmailAddressLength)
		{
			yield return CreateValidationResult(nameof(this.EmailAddress), "EmailAddressTooLong", MaxEmailAddressLength);
		}
		else if (!EmailAddressValidator.IsValid(this.EmailAddress.Trim()))
		{
			yield return CreateValidationResult(nameof(this.EmailAddress), "EmailAddressInvalid");
		}

		if (string.IsNullOrWhiteSpace(this.Subject))
		{
			yield return CreateValidationResult(nameof(this.Subject), "SubjectRequired");
		}
		else if (this.Subject.Trim().Length < MinSubjectLength)
		{
			yield return CreateValidationResult(nameof(this.Subject), "SubjectTooShort", MinSubjectLength);
		}
		else if (this.Subject.Trim().Length > MaxSubjectLength)
		{
			yield return CreateValidationResult(nameof(this.Subject), "SubjectTooLong", MaxSubjectLength);
		}

		if (string.IsNullOrWhiteSpace(this.Message))
		{
			yield return CreateValidationResult(nameof(this.Message), "MessageRequired");
		}
		else if (this.Message.Trim().Length < MinMessageLength)
		{
			yield return CreateValidationResult(nameof(this.Message), "MessageTooShort", MinMessageLength);
		}
		else if (this.Message.Trim().Length > MaxMessageLength)
		{
			yield return CreateValidationResult(nameof(this.Message), "MessageTooLong", MaxMessageLength);
		}

		if (!this.AgreeToBeContacted)
		{
			yield return CreateValidationResult(nameof(this.AgreeToBeContacted), "AgreeToBeContactedRequired");
		}

		if (!string.IsNullOrWhiteSpace(this.Website))
		{
			yield return CreateValidationResult(nameof(this.Website), "MessageCouldNotBeAccepted");
		}

		if (this.FormStartedAtUtc == default)
		{
			yield return CreateValidationResult(nameof(this.FormStartedAtUtc), "MessageCouldNotBeAccepted");
		}

		if (DateTimeOffset.UtcNow - this.FormStartedAtUtc < TimeSpan.FromSeconds(3))
		{
			yield return CreateValidationResult(nameof(this.Message), "PleaseWaitBeforeSending");
		}
	}

	private static ValidationResult CreateValidationResult(
		string memberName,
		string resourceKey,
		params object[] args) =>
		new(
			GetLocalizedValidationMessage(resourceKey, args),
			[memberName]);

	private static string GetLocalizedValidationMessage(
		string resourceKey,
		params object[] args)
	{
		ResourceManager resourceManager = new(
			"Portiforce.SAA.Contracts.Resources.ContactMessageValidationResources",
			typeof(ContactMessageValidationResources).GetTypeInfo().Assembly);

		string format = resourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture)
						?? resourceKey;

		return args.Length == 0
			? format
			: string.Format(CultureInfo.CurrentCulture, format, args);
	}
}
