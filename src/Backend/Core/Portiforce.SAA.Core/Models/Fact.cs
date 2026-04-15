using System.Runtime.CompilerServices;

using Portiforce.SAA.Core.Interfaces;

namespace Portiforce.SAA.Core.Models;

/// <summary>
///     Immutable persisted fact. Equality is identity-based (Id only),
///     not record "value equality".
/// </summary>
public abstract record Fact<TId> : IEntity<TId>
	where TId : struct, IEquatable<TId>
{
	// Protected Constructor for Domain Logic
	protected Fact(TId id)
	{
		this.Id = id;
	}

	// Protected Empty Constructor for EF Core Chain
	// Using default! suppresses null warnings, EF fixes this at runtime.
	protected Fact()
	{
	}

	public TId Id { get; }

	// IMPORTANT: this method REPLACES the record-generated one.
	// Make sure you have it ONLY ONCE in the type.
	public virtual bool Equals(Fact<TId>? other)
	{
		if (ReferenceEquals(this, other))
		{
			return true;
		}

		if (other is null)
		{
			return false;
		}

		if (other.GetType() != this.GetType())
		{
			return false;
		}

		// Treat default Id as transient => never equal (consistent with Entity<TId>)
		if (EqualityComparer<TId>.Default.Equals(this.Id, default))
		{
			return false;
		}

		return EqualityComparer<TId>.Default.Equals(this.Id, other.Id);
	}

	public override int GetHashCode()
	{
		if (EqualityComparer<TId>.Default.Equals(this.Id, default))
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		return EqualityComparer<TId>.Default.GetHashCode(this.Id);
	}
}