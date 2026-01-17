using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Interfaces;
using Portiforce.SimpleAssetAssistant.Core.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;

public sealed class Tenant : Entity<TenantId>, IAggregateRoot
{
	private Tenant(
		TenantId id,
		string name,
		TenantState state,
		TenantPlan tenantPlan,
		TenantSettings settings) :base(id)
	{
		if (id.IsEmpty)
		{
			throw new DomainValidationException("TenantId must be defined.");
		}

		name = NormalizeAndValidateTenantName(name);

		Name = name;
		State = state;
		Plan = tenantPlan;
		Settings = settings ?? throw new DomainValidationException("TenantSettings is required.");
	}

	public string Name { get; private set; }
	public TenantState State { get; private set; }
	public TenantSettings Settings { get; private set; }
	public TenantPlan Plan { get; private set; } = TenantPlan.Demo;

	private readonly HashSet<AssetId> _restrictedAssets = new();

	/// <summary>
	/// Company/tenant related country specific list of restricted assets
	/// </summary>
	public IReadOnlySet<AssetId> RestrictedAssets => _restrictedAssets;

	public static Tenant Create(
		string name,
		TenantSettings? settings = null,
		TenantState state = TenantState.Provisioning,
		TenantPlan plan = TenantPlan.Demo,
		TenantId id = default)
		=> new(
			id.IsEmpty ? TenantId.New() : id,
			name,
			state,
			plan,
			settings ?? TenantSettings.Default());

	public void Rename(string name)
	{
		EnsureEditable();

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new DomainValidationException("Tenant name is required.");
		}

		name = name.Trim();
		if (name.Length is < 2 or > 100)
		{
			throw new DomainValidationException("Tenant name must be 2..100 characters.");
		}

		Name = name;
	}

	public void ChangeState(TenantState newState)
	{
		EnsureEditable();

		// todo: keep state machine minimal for now
		State = newState;
	}

	public void ChangePlan(TenantPlan plan)
	{
		EnsureEditable();

		Plan = plan;
	}

	public void UpdateRestrictedAssetList(AssetId assetId, bool isRestricted)
	{
		EnsureEditable();

		// todo : consider RestrictionAction enum instead of bool (if necessary)
		if (isRestricted)
		{
			_restrictedAssets.Add(assetId);
		}
		else
		{
			_restrictedAssets.Remove(assetId);
		}
	}

	private void EnsureEditable()
	{
		if (State is TenantState.Deleted)
		{
			throw new DomainValidationException($"Tenant is deleted and cannot be changed. TenantId: {Id}");
		}
	}

	private static string NormalizeAndValidateTenantName(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new DomainValidationException("Tenant name is required.");
		}

		int min = LimitationRules.Lengths.Tenant.MinNameLength;
		int max = LimitationRules.Lengths.Tenant.MaxNameLength;

		name = name.Trim();
		if (name.Length < min || name.Length > max)
		{
			throw new DomainValidationException($"Tenant name must be {min}..{max} characters.");
		}

		return name;
	}
}