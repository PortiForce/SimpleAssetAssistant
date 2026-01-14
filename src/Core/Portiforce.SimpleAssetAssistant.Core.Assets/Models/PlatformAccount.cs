using System.ComponentModel.DataAnnotations;

using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Core.Assets.Models;

public sealed class PlatformAccount
{
	private PlatformAccount(
		PlatformAccountId id,
		TenantId tenantId,
		AccountId accountId,
		PlatformId platformId,
		string name,
		PlatformAccountState state,
		string? externalAccountId,
		string? externalUserId)
	{
		if (id.IsEmpty)
		{
			throw new ValidationException("PlatformAccountId must be defined.");
		}

		if (tenantId.IsEmpty)
		{
			throw new ValidationException("TenantId must be defined.");
		}

		if (accountId.IsEmpty)
		{
			throw new ValidationException("AccountId must be defined.");
		}

		if (platformId.IsEmpty)
		{
			throw new ValidationException("PlatformId must be defined.");
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ValidationException("PlatformAccount Name is required.");
		}
		name = name.Trim();

		if (name.Length > LimitationRules.Lengths.NameMaxLength)
		{
			throw new ArgumentException(
				$"Name value exceeds max length of: {LimitationRules.Lengths.NameMaxLength}",
				nameof(name));
		}

		Id = id;
		TenantId = tenantId;
		AccountId = accountId;
		PlatformId = platformId;
		Name = name;
		State = state;

		ExternalAccountId = string.IsNullOrWhiteSpace(externalAccountId) ? null : externalAccountId.Trim();
		ExternalUserId = string.IsNullOrWhiteSpace(externalUserId) ? null : externalUserId.Trim();
	}

	public PlatformAccountId Id { get; }
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
		PlatformAccountId? id = null)
		=> new(
			id is { IsEmpty: false } ? id.Value : PlatformAccountId.New(),
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
			throw new ValidationException("PlatformAccount Name is required.");
		}

		name = name.Trim();

		if (name.Length > LimitationRules.Lengths.NameMaxLength)
		{
			throw new ArgumentException(
				$"Name value exceeds max length of: {LimitationRules.Lengths.NameMaxLength}",
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
		if (State is PlatformAccountState.ReadOnly or PlatformAccountState.Deleted)
		{
			throw new ValidationException($"It is not possible to update readonly entity, state: {State}, id: {Id}");
		}
	}
}
