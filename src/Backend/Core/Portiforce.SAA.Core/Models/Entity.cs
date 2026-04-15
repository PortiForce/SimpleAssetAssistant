using System.Runtime.CompilerServices;

using Portiforce.SAA.Core.Interfaces;

namespace Portiforce.SAA.Core.Models;

/// <summary>
///     Abstract base class for all Domain Entities.
///     Implements equality based on the ID (Identity Pattern).
/// </summary>
public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>
	where TId : struct, IEquatable<TId>
{
	protected Entity(TId id)
	{
		this.Id = id;
	}

	/// <summary>
	///     For serializers / tooling. Domain invariants enforced by factories/methods.
	/// </summary>
	protected Entity()
	{
	}

	public TId Id { get; protected set; }

	public bool Equals(Entity<TId>? other)
	{
		if (other is null)
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		if (this.GetType() != other.GetType())
		{
			return false;
		}

		// If IDs are default (empty), they are not equal unless it's the exact same instance
		if (EqualityComparer<TId>.Default.Equals(this.Id, default))
		{
			return false;
		}

		if (EqualityComparer<TId>.Default.Equals(other.Id, default))
		{
			return false;
		}

		return EqualityComparer<TId>.Default.Equals(this.Id, other.Id);
	}

	public override bool Equals(object? obj)
	{
		if (obj is null)
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != this.GetType())
		{
			return false;
		}

		return this.Equals((Entity<TId>)obj);
	}

	public override int GetHashCode()
	{
		if (EqualityComparer<TId>.Default.Equals(this.Id, default))
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		return HashCode.Combine(this.GetType(), this.Id);
	}

#pragma warning disable SA1201 // Elements should appear in the correct order
	public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
	{
		if (a is null && b is null)
		{
			return true;
		}

		if (a is null || b is null)
		{
			return false;
		}

		return a.Equals(b);
	}
#pragma warning restore SA1201 // Elements should appear in the correct order

	public static bool operator !=(Entity<TId>? a, Entity<TId>? b) => !(a == b);
}