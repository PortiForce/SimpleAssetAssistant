using Portiforce.SAA.Core.Assets.Enums;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Models;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;

namespace Portiforce.SAA.Core.Assets.Models;

public sealed class Platform : Entity<PlatformId>, IAggregateRoot
{
	private Platform(
		PlatformId id,
		string name,
		string code,
		PlatformKind kind,
		PlatformState state)
		: base(id)
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

		this.Name = name.Trim();
		this.Code = code.Trim();
		this.Kind = kind;
		this.State = state;
	}

	// Private Empty Constructor for EF Core
	private Platform()
	{
	}

	public string Name { get; private set; }

	public string Code { get; private set; }

	public PlatformKind Kind { get; init; }

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
		this.EnsureEditable();

		if (string.IsNullOrWhiteSpace(name))
		{
			throw new DomainValidationException("Platform Name is required.");
		}

		if (name.Trim().Length > EntityConstraints.CommonSettings.NameMaxLength)
		{
			throw new ArgumentException(
				$"Name value exceeds max length of: {EntityConstraints.CommonSettings.NameMaxLength}",
				nameof(name));
		}

		this.Name = name.Trim();
	}

	public void ChangeState(PlatformState state)
	{
		this.EnsureEditable();
		this.State = state;
	}

	private void EnsureEditable()
	{
		if (this.IsReadonly())
		{
			throw new DomainValidationException(
				$"It is not possible to update Readonly entity, state: {this.State}, id: {this.Id}");
		}
	}

	private bool IsReadonly() => this.State is PlatformState.ReadOnly or PlatformState.Deleted;
}