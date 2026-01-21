using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Interfaces;
using Portiforce.SimpleAssetAssistant.Core.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Core.Assets.Models;

public sealed class Platform : Entity<PlatformId>, IAggregateRoot
{
	private Platform(
		PlatformId id,
		string name,
		string code,
		PlatformKind kind,
		PlatformState state): base(id)
	{
		if (id.IsEmpty)
		{
			throw new DomainValidationException("PlatformId must be defined.");
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new DomainValidationException("Platform Name is required.");
		}

		if (string.IsNullOrWhiteSpace(code))
		{
			throw new DomainValidationException("Platform Code is required.");
		}

		Name = name.Trim();
		Code = code.Trim();
		Kind = kind;
		State = state;
	}
	
	public string Name { get; private set; }
	public string Code { get; private set; }
	public PlatformKind Kind { get; }
	public PlatformState State { get; private set; }

	public static Platform Create(
		string name,
		string code,
		PlatformKind kind,
		PlatformState state = PlatformState.Active,
		PlatformId id = default)
		=> new(
			id.IsEmpty ? PlatformId.New() : id,
			name,
			code,
			kind,
			state);

	public void Rename(string name)
	{
		EnsureEditable();

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new DomainValidationException("Platform Name is required.");
		}

		if (name.Trim().Length > EntityConstraints.CommonSettings.NameMaxLength)
		{
			throw new ArgumentException($"Name value exceeds max length of: {EntityConstraints.CommonSettings.NameMaxLength}", nameof(name));
		}
		Name = name.Trim();
	}

	public void ChangeState(PlatformState state)
	{
		EnsureEditable();
		State = state;
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
		return State is PlatformState.ReadOnly or PlatformState.Deleted;
	}
}
