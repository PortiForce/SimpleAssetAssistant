using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Assets.Extensions;
using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Interfaces;
using Portiforce.SimpleAssetAssistant.Core.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Core.Assets.Models;

public sealed class Asset : Entity<AssetId>, IAggregateRoot
{
	private readonly HashSet<AssetCode> _synonyms = new();

	private Asset(
		AssetId id,
		AssetCode code,
		AssetKind kind,
		AssetLifecycleState state,
		string? name,
		byte nativeDecimals) : base(id)
	{
		if (id.IsEmpty)
		{
			throw new DomainValidationException("AssetId must be defined.");
		}
		if (string.IsNullOrWhiteSpace(code.Value))
		{
			throw new DomainValidationException("AssetCode must be defined.");
		}
		if (nativeDecimals > EntityConstraints.Domain.Asset.NativeDecimalsMaxLength)
		{
			throw new DomainValidationException($"NativeDecimals must be <= {EntityConstraints.Domain.Asset.NativeDecimalsMaxLength}.");
		}
		if (state == AssetLifecycleState.Deleted)
		{
			throw new DomainValidationException("State  cannot be Deleted for new Entity");
		}

		if (name is {Length: > EntityConstraints.CommonSettings.NameMaxLength})
		{
			throw new ArgumentException($"Name value exceeds max length of: {EntityConstraints.CommonSettings.NameMaxLength}", nameof(name));
		}

		Code = code;
		Kind = kind;
		Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
		NativeDecimals = nativeDecimals;
		State = state;
	}

	// Private Empty Constructor for EF Core
	private Asset()
	{
		_synonyms = new HashSet<AssetCode>();
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
		EnsureEditable();

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Name should be not empty", nameof(name));
		}

		if (name.Length > EntityConstraints.CommonSettings.NameMaxLength)
		{
			throw new ArgumentException($"Name value exceeds max length of: {EntityConstraints.CommonSettings.NameMaxLength}", nameof(name));
		}
		Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
	}

	public bool RegisterSynonym(AssetCode code)
	{
		EnsureEditable();

		if (code.Value == Code.Value)
		{
			return false;
		}

		return _synonyms.Add(code);
	}

	public IReadOnlySet<AssetCode> GetSynonyms()
	{
		return _synonyms;
	}

	public bool Deactivate()
	{
		EnsureEditable();

		if (State == AssetLifecycleState.Disabled)
		{
			return false;
		}
		State = AssetLifecycleState.Disabled;
		return true;
	}

	private void EnsureEditable()
	{
		if (IsReadonly())
		{
			throw new DomainValidationException($"It is not possible to update Readonly entity, state: {State}, id: {Id}");
		}
	}

	private bool IsReadonly()
	{
		return State is
			AssetLifecycleState.Disabled 
			or AssetLifecycleState.ReadOnly 
			or AssetLifecycleState.Deleted;
	}

	/// <summary>
	/// Verifies that Asset is Fiat or StableCoin related
	/// </summary>
	/// <returns>true if asset belongs to Fiat or StableCoin kinds</returns>
	public bool IsFiatOrStableKind()
	{
		return AssetExtensions.IsFiatOrStableKind(Kind);
	}
}