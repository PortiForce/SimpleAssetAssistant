using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Models;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;

namespace Portiforce.SAA.Core.Identity.Models.Client;

public sealed class Tenant : Entity<TenantId>, IAggregateRoot
{
	private readonly HashSet<AssetId> _restrictedAssets = [];

	private readonly HashSet<PlatformId> _restrictedPlatforms = [];

	private readonly List<TenantRestrictedAsset> _restrictedTenantAssets = [];

	private readonly List<TenantRestrictedPlatform> _restrictedTenantPlatforms = [];

	private Tenant(
		TenantId id,
		string name,
		string code,
		string? brandName,
		string domainPrefix,
		TenantState state,
		TenantPlan plan,
		TenantSettings settings)
		: base(id)
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

		this.Name = name;
		this.Code = code;
		this.BrandName = brandName;
		this.DomainPrefix = domainPrefix;
		this.State = state;
		this.Plan = plan;
		this.Settings = settings ?? throw new DomainValidationException("TenantSettings is required.");
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

	/// <summary>
	///     Company/tenant related country specific list of restricted assets
	/// </summary>
	public IReadOnlyCollection<TenantRestrictedAsset> RestrictedAssets => this._restrictedTenantAssets;

	/// <summary>
	///     Company/tenant related country specific list of restricted platforms
	/// </summary>
	public IReadOnlyCollection<TenantRestrictedPlatform> RestrictedPlatforms => this._restrictedTenantPlatforms;

	public static Tenant Create(
		string name,
		string code,
		string brandName,
		string domainPrefix,
		TenantState state,
		TenantPlan plan,
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
		this.EnsureEditable();

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new DomainValidationException("Tenant name is required.");
		}

		name = name.Trim();
		if (name.Length is < 2 or > 100)
		{
			throw new DomainValidationException("Tenant name must be 2..100 characters.");
		}

		this.Name = name;
	}

	public void ChangeState(TenantState newState)
	{
		this.EnsureEditable();

		// todo: keep state machine minimal for now
		this.State = newState;
	}

	public void ChangePlan(TenantPlan plan)
	{
		this.EnsureEditable();

		this.Plan = plan;
	}

	public void UpdateSettings(TenantSettings newSettings)
	{
		this.EnsureEditable();

		this.Settings = newSettings ?? throw new DomainValidationException("Settings cannot be null.");
	}

	private void SyncRestrictedAssetsFromEf()
	{
		this._restrictedAssets.Clear();
		foreach (TenantRestrictedAsset r in this._restrictedTenantAssets)
		{
			_ = this._restrictedAssets.Add(r.AssetId);
		}
	}

	private void SyncRestrictedPlatformsFromEf()
	{
		this._restrictedPlatforms.Clear();
		foreach (TenantRestrictedPlatform r in this._restrictedTenantPlatforms)
		{
			_ = this._restrictedPlatforms.Add(r.PlatformId);
		}
	}

	public void UpdateRestrictedAssetList(IReadOnlyCollection<AssetId> assetIds, bool isRestricted)
	{
		this.EnsureEditable();

		if (assetIds.Count == 0)
		{
			return;
		}

		if (isRestricted)
		{
			foreach (AssetId assetId in assetIds)
			{
				if (this._restrictedAssets.Add(assetId))
				{
					this._restrictedTenantAssets.Add(new TenantRestrictedAsset(this.Id, assetId));
				}
			}
		}
		else
		{
			foreach (AssetId assetId in assetIds)
			{
				if (this._restrictedAssets.Remove(assetId))
				{
					_ = this._restrictedTenantAssets.RemoveAll(x => x.AssetId == assetId);
				}
			}
		}
	}

	public void UpdateRestrictedPlatformList(IReadOnlyCollection<PlatformId> platformIds, bool isRestricted)
	{
		this.EnsureEditable();

		if (platformIds.Count == 0)
		{
			return;
		}

		if (isRestricted)
		{
			foreach (PlatformId platformId in platformIds)
			{
				if (this._restrictedPlatforms.Add(platformId))
				{
					this._restrictedTenantPlatforms.Add(new TenantRestrictedPlatform(this.Id, platformId));
				}
			}
		}
		else
		{
			foreach (PlatformId platformId in platformIds)
			{
				if (this._restrictedPlatforms.Remove(platformId))
				{
					_ = this._restrictedTenantPlatforms.RemoveAll(x => x.PlatformId == platformId);
				}
			}
		}
	}

	private void EnsureEditable()
	{
		if (this.State is TenantState.Deleted)
		{
			throw new DomainValidationException($"Tenant is deleted and cannot be changed. TenantId: {this.Id}");
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