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
		string code,
		string? brandName,
		string domainPrefix,
		TenantState state,
		TenantPlan plan,
		TenantSettings settings) :base(id)
	{
		if (id.IsEmpty)
		{
			throw new DomainValidationException("TenantId must be defined.");
		}

		if (string.IsNullOrWhiteSpace(code))
		{
			throw new DomainValidationException("Code should be defined");
		}

		name = NormalizeAndValidateTenantName(name);

		Name = name;
		Code = code;
		BrandName = brandName;
		DomainPrefix = domainPrefix;
		State = state;
		Plan = plan;
		Settings = settings ?? throw new DomainValidationException("TenantSettings is required.");
	}

	// Private Empty Constructor for EF Core
	private Tenant()
	{
		
	}

	public string Name { get; private set; }
	public string Code { get; init; }
	public string? BrandName { get; private set; }
	public string DomainPrefix { get; init; }
	public TenantState State { get; private set; }
	public TenantSettings Settings { get; private set; }
	public TenantPlan Plan { get; private set; }

	private readonly HashSet<AssetId> _restrictedAssets = new();

	private readonly HashSet<PlatformId> _restrictedPlatforms = new();

	private readonly List<TenantRestrictedAsset> _restrictedTenantAssets = new();

	private readonly List<TenantRestrictedPlatform> _restrictedTenantPlatforms = new();

	/// <summary>
	/// Company/tenant related country specific list of restricted assets
	/// </summary>
	public IReadOnlyCollection<TenantRestrictedAsset> RestrictedAssets => _restrictedTenantAssets;

	/// <summary>
	/// Company/tenant related country specific list of restricted platforms
	/// </summary>
	public IReadOnlyCollection<TenantRestrictedPlatform> RestrictedPlatforms => _restrictedTenantPlatforms;

	public static Tenant Create(
		string name,
		string code,
		string brandName,
		string domainPrefix,
		TenantState state = TenantState.Provisioning,
		TenantPlan plan = TenantPlan.Demo,
		TenantSettings? settings = null,
		TenantId id = default)
		=> new(
			id.IsEmpty ? TenantId.New() : id,
			name,
			code,
			brandName,
			domainPrefix,
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

	private void SyncRestrictedAssetsFromEf()
	{
		_restrictedAssets.Clear();
		foreach (var r in _restrictedTenantAssets)
		{
			_restrictedAssets.Add(r.AssetId);
		}
	}

	private void SyncRestrictedPlatformsFromEf()
	{
		_restrictedPlatforms.Clear();
		foreach (var r in _restrictedTenantPlatforms)
		{
			_restrictedPlatforms.Add(r.PlatformId);
		}
	}

	public void UpdateRestrictedAssetList(IReadOnlyCollection<AssetId> assetIds, bool isRestricted)
	{
		EnsureEditable();

		if (assetIds.Count == 0)
		{
			return;
		}

		if (isRestricted)
		{
			foreach (var assetId in assetIds)
			{
				if (_restrictedAssets.Add(assetId))
				{
					_restrictedTenantAssets.Add(new TenantRestrictedAsset(Id, assetId));
				}
			}
		}
		else
		{
			foreach (var assetId in assetIds)
			{
				if (_restrictedAssets.Remove(assetId))
				{
					_restrictedTenantAssets.RemoveAll(x => x.AssetId == assetId);
				}
			}
		}
	}

	public void UpdateRestrictedPlatformList(IReadOnlyCollection<PlatformId> platformIds, bool isRestricted)
	{
		EnsureEditable();

		if (platformIds.Count == 0)
		{
			return;
		}

		if (isRestricted)
		{
			foreach (var platformId in platformIds)
			{
				if (_restrictedPlatforms.Add(platformId))
				{
					_restrictedTenantPlatforms.Add(new TenantRestrictedPlatform(Id, platformId));
				}
			}
		}
		else
		{
			foreach (var platformId in platformIds)
			{
				if (_restrictedPlatforms.Remove(platformId))
				{
					_restrictedTenantPlatforms.RemoveAll(x => x.PlatformId == platformId);
				}
			}
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

		int min = EntityConstraints.Domain.Tenant.NameMinLength;
		int max = EntityConstraints.Domain.Tenant.NameMaxLength;

		name = name.Trim();
		if (name.Length < min || name.Length > max)
		{
			throw new DomainValidationException($"Tenant name must be {min}..{max} characters.");
		}

		return name;
	}
}