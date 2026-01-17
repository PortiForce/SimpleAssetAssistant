using Portiforce.SimpleAssetAssistant.Core.Interfaces;

namespace Portiforce.SimpleAssetAssistant.Core.Models;

/// <summary>
/// Immutable persisted fact. Equality is identity-based (Id only),
/// not record "value equality".
/// </summary>
public abstract record Fact<TId>(TId Id) : IEntity<TId>
	where TId : struct, IEquatable<TId>
{
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

		if (other.GetType() != GetType())
		{
			return false;
		}

		// Treat default Id as transient => never equal (consistent with Entity<TId>)
		if (EqualityComparer<TId>.Default.Equals(Id, default))
		{
			return false;
		}

		return EqualityComparer<TId>.Default.Equals(Id, other.Id);
	}
	
	public override int GetHashCode()
	{
		if (EqualityComparer<TId>.Default.Equals(Id, default))
		{
			return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
		}

		return EqualityComparer<TId>.Default.GetHashCode(Id);
	}
}