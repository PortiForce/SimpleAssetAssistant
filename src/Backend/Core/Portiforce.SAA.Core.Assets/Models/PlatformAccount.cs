using Portiforce.SAA.Core.Assets.Enums;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Models;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;

namespace Portiforce.SAA.Core.Assets.Models;

public sealed class PlatformAccount : Entity<PlatformAccountId>, IAggregateRoot
{
	private PlatformAccount(
		PlatformAccountId id,
		TenantId tenantId,
		AccountId accountId,
		PlatformId platformId,
		string accountName,
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

		if (string.IsNullOrWhiteSpace(accountName))
		{
			throw new DomainValidationException("PlatformAccount Name is required.");
		}
		accountName = accountName.Trim();

		if (accountName.Length > EntityConstraints.CommonSettings.NameMaxLength)
		{
			throw new ArgumentException(
				$"AccountName value exceeds max length of: {EntityConstraints.CommonSettings.NameMaxLength}",
				nameof(accountName));
		}

		TenantId = tenantId;
		AccountId = accountId;
		PlatformId = platformId;
		AccountName = accountName;
		State = state;

		ExternalAccountId = string.IsNullOrWhiteSpace(externalAccountId) ? null : externalAccountId.Trim();
		ExternalUserId = string.IsNullOrWhiteSpace(externalUserId) ? null : externalUserId.Trim();
	}

	// Private Empty Constructor for EF Core
	private PlatformAccount() { }

	public TenantId TenantId { get; init; }
	public AccountId AccountId { get; init; }
	public PlatformId PlatformId { get; init; }
	public string AccountName { get; private set; }
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

		AccountName = name;
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
