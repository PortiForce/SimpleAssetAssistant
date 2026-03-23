using Portiforce.SAA.Core.Assets.Enums;
using Portiforce.SAA.Core.Assets.Extensions;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Models;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;

namespace Portiforce.SAA.Core.Assets.Models;

public sealed class Asset : Entity<AssetId>, IAggregateRoot
{
	private readonly HashSet<AssetCode> _synonyms = [];

	private Asset(
		AssetId id,
		AssetCode code,
		AssetKind kind,
		AssetLifecycleState state,
		string? name,
		byte nativeDecimals)
		: base(id)
	{
		if (id.IsEmpty)
		{
			throw new DomainValidationException("AssetId must be defined.");
		}

		if (code == null || string.IsNullOrWhiteSpace(code.Value))
		{
			throw new DomainValidationException("AssetCode must be defined.");
		}

		if (nativeDecimals > EntityConstraints.Domain.Asset.NativeDecimalsMaxLength)
		{
			throw new DomainValidationException(
				$"NativeDecimals must be <= {EntityConstraints.Domain.Asset.NativeDecimalsMaxLength}.");
		}

		if (state == AssetLifecycleState.Deleted)
		{
			throw new DomainValidationException("State  cannot be Deleted for new Entity");
		}

		if (name is { Length: > EntityConstraints.CommonSettings.NameMaxLength })
		{
			throw new ArgumentException(
				$"Name value exceeds max length of: {EntityConstraints.CommonSettings.NameMaxLength}",
				nameof(name));
		}

		this.Code = code;
		this.Kind = kind;
		this.Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
		this.NativeDecimals = nativeDecimals;
		this.State = state;
	}

	// Private Empty Constructor for EF Core
	private Asset()
	{
		this._synonyms = [];
	}

	public AssetCode Code { get; init; }

	public AssetKind Kind { get; init; }

	public string? Name { get; private set; }

	public byte NativeDecimals { get; private set; }

	public AssetLifecycleState State { get; private set; } = AssetLifecycleState.Draft;

	public static Asset Create(
		AssetCode code,
		AssetKind kind,
		AssetLifecycleState state,
		string? name = null,
		byte nativeDecimals = 4,
		AssetId id = default)
		=> new(
			id.IsEmpty ? AssetId.New() : id,
			code,
			kind,
			state,
			name,
			nativeDecimals);

	public void Rename(string? name)
	{
		this.EnsureEditable();

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Name should be not empty", nameof(name));
		}

		if (name.Length > EntityConstraints.CommonSettings.NameMaxLength)
		{
			throw new ArgumentException(
				$"Name value exceeds max length of: {EntityConstraints.CommonSettings.NameMaxLength}",
				nameof(name));
		}

		this.Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
	}

	public bool RegisterSynonym(AssetCode code)
	{
		this.EnsureEditable();

		if (code.Value == this.Code.Value)
		{
			return false;
		}

		return this._synonyms.Add(code);
	}

	public IReadOnlySet<AssetCode> GetSynonyms() => this._synonyms;

	public bool Deactivate()
	{
		this.EnsureEditable();

		if (this.State == AssetLifecycleState.Disabled)
		{
			return false;
		}

		this.State = AssetLifecycleState.Disabled;
		return true;
	}

	private void EnsureEditable()
	{
		if (this.IsReadonly())
		{
			throw new DomainValidationException(
				$"It is not possible to update Readonly entity, state: {this.State}, id: {this.Id}");
		}
	}

	private bool IsReadonly()
	{
		return this.State is
			AssetLifecycleState.Disabled
			or AssetLifecycleState.ReadOnly
			or AssetLifecycleState.Deleted;
	}

	/// <summary>
	///     Verifies that Asset is Fiat or StableCoin related
	/// </summary>
	/// <returns>true if asset belongs to Fiat or StableCoin kinds</returns>
	public bool IsFiatOrStableKind() => AssetExtensions.IsFiatOrStableKind(this.Kind);
}