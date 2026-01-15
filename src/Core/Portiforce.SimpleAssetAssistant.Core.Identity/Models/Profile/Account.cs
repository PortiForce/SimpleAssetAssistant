using System.ComponentModel.DataAnnotations;

using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Interfaces;
using Portiforce.SimpleAssetAssistant.Core.Models;
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
		ContactInfo? contact,
		AccountSettings settings): base(id)
	{
		if (id.IsEmpty)
		{
			throw new ValidationException("AccountId must be defined.");
		}

		if (tenantId.IsEmpty)
		{
			throw new ValidationException("TenantId must be defined.");
		}

		alias = NormalizeAndValidateAlias(alias);

		TenantId = tenantId;
		Alias = alias;

		Role = role;
		State = state;
		Tier = tier;

		Contact = contact;
		Settings = settings ?? throw new ValidationException("AccountSettings is required.");
	}

	public TenantId TenantId { get; }
	public string Alias { get; }

	public Role Role { get; private set; }
	public AccountState State { get; private set; }
	public AccountTier Tier { get; private set; }

	public ContactInfo? Contact { get; private set; }
	public AccountSettings Settings { get; private set; }

	public static Account Create(
		TenantId tenantId,
		string alias,
		Role role = Role.TenantUser,
		AccountState state = AccountState.NotVerified,
		AccountTier tier = AccountTier.Demo,
		ContactInfo? contact = null,
		AccountSettings? settings = null,
		AccountId? id = null)
	{
		settings ??= AccountSettings.Default();

		return new Account(
			id is {IsEmpty: false} ? id.Value : AccountId.New(),
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

	public void UpdateContact(ContactInfo? contact)
	{
		EnsureEditable();
		Contact = contact;
	}

	public void UpdateSettings(AccountSettings settings)
	{
		EnsureEditable();
		Settings = settings ?? throw new ValidationException("AccountSettings is required.");
	}

	private void EnsureEditable()
	{
		if (State is AccountState.Disabled or AccountState.Deleted)
		{
			throw new ValidationException($"Account is not editable in state: {State}. AccountId: {Id}");
		}
	}

	private static string NormalizeAndValidateAlias(string alias)
	{
		if (string.IsNullOrWhiteSpace(alias))
		{
			throw new ValidationException("Alias is required.");
		}

		alias = alias.Trim().ToLowerInvariant();

		int min = LimitationRules.Lengths.Account.MinAliasLength;
		int max = LimitationRules.Lengths.Account.MaxAliasLength;

		if (alias.Length < min || alias.Length > max)
		{
			throw new ValidationException($"Alias must be {min}..{max} characters.");
		}

		foreach (char c in alias)
		{
			if (!char.IsLetterOrDigit(c) && c is not '_' and not '-')
			{
				throw new ValidationException("Alias can only contain letters, digits, '_' and '-'.");
			}
		}

		return alias;
	}
}