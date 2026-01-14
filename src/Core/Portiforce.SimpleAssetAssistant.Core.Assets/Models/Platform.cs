using System.ComponentModel.DataAnnotations;

using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;

namespace Portiforce.SimpleAssetAssistant.Core.Assets.Models;

public sealed class Platform
{
	private Platform(
		PlatformId id,
		string name,
		string code,
		PlatformKind kind,
		PlatformState state)
	{
		if (id.IsEmpty)
		{
			throw new ValidationException("PlatformId must be defined.");
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ValidationException("Platform Name is required.");
		}

		if (string.IsNullOrWhiteSpace(code))
		{
			throw new ValidationException("Platform Code is required.");
		}

		Id = id;
		Name = name.Trim();
		Code = code.Trim();
		Kind = kind;
		State = state;
	}

	public PlatformId Id { get; }
	public string Name { get; private set; }
	public string Code { get; private set; }
	public PlatformKind Kind { get; }
	public PlatformState State { get; private set; }

	public static Platform Create(
		string name,
		string code,
		PlatformKind kind,
		PlatformState state = PlatformState.Active,
		PlatformId? id = null)
		=> new(
			id is { IsEmpty: false } ? id.Value : PlatformId.New(),
			name,
			code,
			kind,
			state);

	public void Rename(string name)
	{
		EnsureEditable();

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ValidationException("Platform Name is required.");
		}

		if (name.Trim().Length > LimitationRules.Lengths.NameMaxLength)
		{
			throw new ArgumentException($"Name value exceeds max length of: {LimitationRules.Lengths.NameMaxLength}", nameof(name));
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
			throw new ValidationException($"It is not possible to update Readonly entity, state: {State}, id: {Id}");
		}
	}

	private bool IsReadonly()
	{
		return State is PlatformState.ReadOnly or PlatformState.Deleted;
	}
}
