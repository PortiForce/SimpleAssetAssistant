using Portiforce.SimpleAssetAssistant.Core.Interfaces;

namespace Portiforce.SimpleAssetAssistant.Core.Models;

/// <summary>
/// Abstract base class for all Domain Entities.
/// Implements equality based on the ID (Identity Pattern).
/// </summary>
public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>
	where TId : struct, IEquatable<TId>
{
	public TId Id { get; protected set; }

	protected Entity(TId id)
	{
		Id = id;
	}

	// Required for EF Core or serialization where constructor might be bypassed
	protected Entity() { }

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

		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((Entity<TId>)obj);
	}

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

		// If IDs are default (empty), they are not equal unless it's the exact same instance
		if (EqualityComparer<TId>.Default.Equals(Id, default))
		{
			return false;
		}

		return EqualityComparer<TId>.Default.Equals(Id, other.Id);
	}

	public override int GetHashCode()
	{
		// ReSharper disable once NonReadonlyMemberInGetHashCode
		return EqualityComparer<TId>.Default.GetHashCode(Id);
	}

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

	public static bool operator !=(Entity<TId>? a, Entity<TId>? b)
	{
		return !(a == b);
	}
}