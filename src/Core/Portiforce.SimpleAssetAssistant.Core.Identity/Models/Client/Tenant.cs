using System.ComponentModel.DataAnnotations;

using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Models.Client;

public sealed class Tenant
{
	private Tenant(
		TenantId id,
		string name,
		TenantState state,
		TenantPlan tenantPlan,
		TenantSettings settings)
	{
		if (id.IsEmpty)
		{
			throw new ValidationException("TenantId must be defined.");
		}

		name = NormalizeAndValidateTenantName(name);

		Id = id;
		Name = name;
		State = state;
		Plan = tenantPlan;
		Settings = settings ?? throw new ValidationException("TenantSettings is required.");
	}

	public TenantId Id { get; }
	public string Name { get; private set; }
	public TenantState State { get; private set; }
	public TenantSettings Settings { get; private set; }
	public TenantPlan Plan { get; private set; } = TenantPlan.Demo;

	public static Tenant Create(
		string name,
		TenantSettings? settings = null,
		TenantState state = TenantState.Provisioning,
		TenantPlan plan = TenantPlan.Demo,
		TenantId? id = null)
		=> new(
			id is { IsEmpty: false } ? id.Value : TenantId.New(),
			name,
			state,
			plan,
			settings ?? TenantSettings.Default());

	public void Rename(string name)
	{
		EnsureEditable();

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ValidationException("Tenant name is required.");
		}

		name = name.Trim();
		if (name.Length is < 2 or > 100)
		{
			throw new ValidationException("Tenant name must be 2..100 characters.");
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

	private void EnsureEditable()
	{
		if (State is TenantState.Deleted)
		{
			throw new ValidationException($"Tenant is deleted and cannot be changed. TenantId: {Id}");
		}
	}

	private static string NormalizeAndValidateTenantName(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ValidationException("Tenant name is required.");
		}

		int min = LimitationRules.Lengths.Tenant.MinNameLength;
		int max = LimitationRules.Lengths.Tenant.MaxNameLength;

		name = name.Trim();
		if (name.Length < min || name.Length > max)
		{
			throw new ValidationException($"Tenant name must be {min}..{max} characters.");
		}

		return name;
	}
}