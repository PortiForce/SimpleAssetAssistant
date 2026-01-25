using Microsoft.AspNetCore.Connections;
using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Interfaces;
using Portiforce.SimpleAssetAssistant.Core.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;

public sealed class Account : Entity<AccountId>, IAggregateRoot
{
	private Account(
		AccountId id,
		TenantId tenantId,
		string alias,
		Role role,
		AccountState state,
		AccountTier tier,
		ContactInfo contact,
		AccountSettings settings): base(id)
	{
		if (id.IsEmpty)
		{
			throw new DomainValidationException("AccountId must be defined.");
		}

		if (tenantId.IsEmpty)
		{
			throw new DomainValidationException("TenantId must be defined.");
		}

		alias = NormalizeAndValidateAlias(alias);

		TenantId = tenantId;
		Alias = alias;

		Role = role;
		State = state;
		Tier = tier;

		Contact = contact;
		Settings = settings ?? throw new DomainValidationException("AccountSettings is required.");
	}

	private Account(
		AccountId id,
		TenantId tenantId,
		string alias,
		Role role,
		AccountState state,
		AccountTier tier)
		: this(
			id,
			tenantId,
			alias,
			role,
			state,
			tier,
			new ContactInfo(Email.Create("test@portiforce.com"), null, null),
			AccountSettings.Default()) 
	{
		
	}

	public TenantId TenantId { get; private set; }
	public string Alias { get; private set; }

	public Role Role { get; private set; }
	public AccountState State { get; private set; }
	public AccountTier Tier { get; private set; }

	public ContactInfo Contact { get; private set; }
	public AccountSettings Settings { get; private set; }

	public static Account Create(
		TenantId tenantId,
		string alias,
		ContactInfo contact,
		Role role = Role.TenantUser,
		AccountState state = AccountState.NotVerified,
		AccountTier tier = AccountTier.Demo,
		AccountSettings? settings = null,
		AccountId id = default)
	{
		settings ??= AccountSettings.Default();

		return new Account(
			id.IsEmpty ? AccountId.New() : id,
			tenantId,
			alias,
			role,
			state,
			tier,
			contact,
			settings);
	}

	public void ChangeRole(Role role)
	{
		EnsureEditable();
		Role = role;
	}

	public void ChangeTier(AccountTier tier)
	{
		EnsureEditable();
		Tier = tier;
	}

	public void Suspend()
	{
		EnsureEditable();
		State = AccountState.Suspended;
	}

	public void Disable()
	{
		EnsureEditable();
		State = AccountState.Disabled;
	}

	public void UpdateContact(ContactInfo contact)
	{
		EnsureEditable();
		Contact = contact;
	}

	public void UpdateSettings(AccountSettings settings)
	{
		EnsureEditable();
		Settings = settings ?? throw new DomainValidationException("AccountSettings is required.");
	}

	private void EnsureEditable()
	{
		if (State is AccountState.Disabled or AccountState.Deleted)
		{
			throw new DomainValidationException($"Account is not editable in state: {State}. AccountId: {Id}");
		}
	}

	private static string NormalizeAndValidateAlias(string alias)
	{
		if (string.IsNullOrWhiteSpace(alias))
		{
			throw new DomainValidationException("Alias is required.");
		}

		alias = alias.Trim().ToLowerInvariant();

		int min = EntityConstraints.Domain.Account.AliasMinLength;
		int max = EntityConstraints.Domain.Account.AliasMaxLength;

		if (alias.Length < min || alias.Length > max)
		{
			throw new DomainValidationException($"Alias must be {min}..{max} characters.");
		}

		foreach (char c in alias)
		{
			if (!char.IsLetterOrDigit(c) && c is not '_' and not '-')
			{
				throw new DomainValidationException("Alias can only contain letters, digits, '_' and '-'.");
			}
		}

		return alias;
	}
}