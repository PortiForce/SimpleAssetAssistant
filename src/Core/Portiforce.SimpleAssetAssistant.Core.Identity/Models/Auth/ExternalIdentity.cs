using System.ComponentModel.DataAnnotations;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;

public sealed class ExternalIdentity : Entity<Guid>
{
	private ExternalIdentity(
		Guid id,
		AccountId accountId,
		AuthProvider provider,
		string providerSubject,
		bool isPrimary)
	{
		if (id == Guid.Empty)
		{
			throw new ValidationException("ExternalIdentity.Id must be defined.");
		}

		if (accountId.IsEmpty)
		{
			throw new ValidationException("AccountId must be defined.");
		}

		string providerSubjectValue = NormalizeAndValidateProviderSubject(providerSubject);

		Id = id;
		AccountId = accountId;
		Provider = provider;
		ProviderSubject = providerSubjectValue;
		IsPrimary = isPrimary;
	}

	public Guid Id { get; }
	public AccountId AccountId { get; }
	public AuthProvider Provider { get; }
	public string ProviderSubject { get; }

	public bool IsPrimary { get; private set; }

	public static ExternalIdentity Create(
		AccountId accountId,
		AuthProvider provider,
		string providerSubject,
		bool isPrimary = false,
		Guid? id = null)
		=> new(
			id ?? Guid.CreateVersion7(),
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
			throw new ValidationException("ProviderSubject is required.");
		}

		// Google 'sub' is short, passkey subjects can vary
		if (providerSubject.Length > LimitationRules.Lengths.ProviderSubjectMaxLength)
		{
			throw new ValidationException($"ProviderSubject is too long (max {LimitationRules.Lengths.ProviderSubjectMaxLength}).");
		}

		return providerSubject.Trim();
	}
}