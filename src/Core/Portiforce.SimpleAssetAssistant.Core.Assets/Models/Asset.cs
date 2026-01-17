using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Enums;
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
		if (nativeDecimals > 18)
		{
			throw new DomainValidationException("NativeDecimals must be <= 18.");
		}

		if (name is {Length: > LimitationRules.Lengths.NameMaxLength})
		{
			throw new ArgumentException($"Name value exceeds max length of: {LimitationRules.Lengths.NameMaxLength}", nameof(name));
		}

		Code = code;
		Kind = kind;
		Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
		NativeDecimals = nativeDecimals;
	}

	public AssetCode Code { get; }
	public AssetKind Kind { get; }

	public string? Name { get; private set; }
	public byte NativeDecimals { get; }

	public EntityLifecycleState EntityState { get; private set; } = EntityLifecycleState.Active;

	public IReadOnlySet<AssetCode> Synonyms => _synonyms;

	public static Asset Create(
		AssetCode code,
		AssetKind kind,
		string? name = null,
		byte nativeDecimals = 4,
		AssetId id = default)
		=> new(
			id.IsEmpty ? AssetId.New() : id,
			code,
			kind,
			name,
			nativeDecimals);

	public void Rename(string? name)
	{
		EnsureEditable();

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Name should be not empty", nameof(name));
		}

		if (name.Length > LimitationRules.Lengths.NameMaxLength)
		{
			throw new ArgumentException($"Name value exceeds max length of: {LimitationRules.Lengths.NameMaxLength}", nameof(name));
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

	public bool Deactivate()
	{
		EnsureEditable();

		if (EntityState == EntityLifecycleState.Disabled)
		{
			return false;
		}
		EntityState = EntityLifecycleState.Disabled;
		return true;
	}

	private void EnsureEditable()
	{
		if (IsReadonly())
		{
			throw new DomainValidationException($"It is not possible to update Readonly entity, state: {EntityState}, id: {Id}");
		}
	}

	private bool IsReadonly()
	{
		return EntityState is 
			EntityLifecycleState.Disabled 
			or EntityLifecycleState.ReadOnly 
			or EntityLifecycleState.Deleted;
	}
}