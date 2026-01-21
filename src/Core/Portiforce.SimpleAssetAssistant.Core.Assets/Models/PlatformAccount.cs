using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Interfaces;
using Portiforce.SimpleAssetAssistant.Core.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Core.Assets.Models;

public sealed class PlatformAccount : Entity<PlatformAccountId>, IAggregateRoot
{
	private PlatformAccount(
		PlatformAccountId id,
		TenantId tenantId,
		AccountId accountId,
		PlatformId platformId,
		string name,
		PlatformAccountState state,
		string? externalAccountId,
		string? externalUserId): base(id)
	{
		if (id.IsEmpty)
		{
			throw new DomainValidationException("PlatformAccountId must be defined.");
		}

		if (tenantId.IsEmpty)
		{
			throw new DomainValidationException("TenantId must be defined.");
		}

		if (accountId.IsEmpty)
		{
			throw new DomainValidationException("AccountId must be defined.");
		}

		if (platformId.IsEmpty)
		{
			throw new DomainValidationException("PlatformId must be defined.");
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new DomainValidationException("PlatformAccount Name is required.");
		}
		name = name.Trim();

		if (name.Length > EntityConstraints.CommonSettings.NameMaxLength)
		{
			throw new ArgumentException(
				$"Name value exceeds max length of: {EntityConstraints.CommonSettings.NameMaxLength}",
				nameof(name));
		}

		TenantId = tenantId;
		AccountId = accountId;
		PlatformId = platformId;
		Name = name;
		State = state;

		ExternalAccountId = string.IsNullOrWhiteSpace(externalAccountId) ? null : externalAccountId.Trim();
		ExternalUserId = string.IsNullOrWhiteSpace(externalUserId) ? null : externalUserId.Trim();
	}

	public TenantId TenantId { get; }
	public AccountId AccountId { get; }
	public PlatformId PlatformId { get; }

	public string Name { get; private set; }
	public PlatformAccountState State { get; private set; }

	public string? ExternalAccountId { get; private set; }
	public string? ExternalUserId { get; private set; }

	public static PlatformAccount Create(
		TenantId tenantId,
		AccountId accountId,
		PlatformId platformId,
		string name,
		PlatformAccountState state = PlatformAccountState.Active,
		string? externalAccountId = null,
		string? externalUserId = null,
		PlatformAccountId id = default)
		=> new(
			id.IsEmpty ? PlatformAccountId.New() : id,
			tenantId,
			accountId,
			platformId,
			name,
			state,
			externalAccountId,
			externalUserId);

	public void Rename(string name)
	{
		EnsureEditable();

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new DomainValidationException("PlatformAccount Name is required.");
		}

		name = name.Trim();

		if (name.Length > EntityConstraints.CommonSettings.NameMaxLength)
		{
			throw new ArgumentException(
				$"Name value exceeds max length of: {EntityConstraints.CommonSettings.NameMaxLength}",
				nameof(name));
		}

		Name = name;
	}

	public void ChangeState(PlatformAccountState state)
	{
		EnsureEditable();
		State = state;
	}

	public void LinkExternal(string? externalAccountId, string? externalUserId)
	{
		EnsureEditable();
		ExternalAccountId = string.IsNullOrWhiteSpace(externalAccountId) ? null : externalAccountId.Trim();
		ExternalUserId = string.IsNullOrWhiteSpace(externalUserId) ? null : externalUserId.Trim();
	}

	private void EnsureEditable()
	{
		if (State is PlatformAccountState.ReadOnly or PlatformAccountState.PendingDeletion or PlatformAccountState.Deleted)
		{
			throw new DomainValidationException($"It is not possible to update readonly entity, state: {State}, id: {Id}");
		}
	}
}
