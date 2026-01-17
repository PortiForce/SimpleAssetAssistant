using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;

public sealed class ExternalIdentity : Entity<ExternalIdentityId>
{
	private ExternalIdentity(
		ExternalIdentityId id,
		AccountId accountId,
		AuthProvider provider,
		string providerSubject,
		bool isPrimary) : base(id)
	{
		if (id.IsEmpty)
		{
			throw new DomainValidationException("ExternalIdentity.Id must be defined.");
		}

		if (accountId.IsEmpty)
		{
			throw new DomainValidationException("AccountId must be defined.");
		}

		string providerSubjectValue = NormalizeAndValidateProviderSubject(providerSubject);

		AccountId = accountId;
		Provider = provider;
		ProviderSubject = providerSubjectValue;
		IsPrimary = isPrimary;
	}

	public AccountId AccountId { get; }
	public AuthProvider Provider { get; }
	public string ProviderSubject { get; }

	public bool IsPrimary { get; private set; }

	public static ExternalIdentity Create(
		AccountId accountId,
		AuthProvider provider,
		string providerSubject,
		bool isPrimary = false,
		ExternalIdentityId id = default)
		=> new(
			id.IsEmpty ? ExternalIdentityId.New() : id,
			accountId,
			provider,
			providerSubject,
			isPrimary);

	public void MarkPrimary() => IsPrimary = true;
	public void UnmarkPrimary() => IsPrimary = false;

	private static string NormalizeAndValidateProviderSubject(string providerSubject)
	{
		if (string.IsNullOrWhiteSpace(providerSubject))
		{
			throw new DomainValidationException("ProviderSubject is required.");
		}

		// Google 'sub' is short, passkey subjects can vary
		if (providerSubject.Length > LimitationRules.Lengths.ProviderSubjectMaxLength)
		{
			throw new DomainValidationException($"ProviderSubject is too long (max {LimitationRules.Lengths.ProviderSubjectMaxLength}).");
		}

		return providerSubject.Trim();
	}
}