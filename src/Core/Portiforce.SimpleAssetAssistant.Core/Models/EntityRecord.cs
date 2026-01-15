using Portiforce.SimpleAssetAssistant.Core.Interfaces;

namespace Portiforce.SimpleAssetAssistant.Core.Models;

/// <summary>
/// Abstract base class for all Domain Entities.
/// Implements equality based on the ID (Identity Pattern).
/// </summary>
/// <summary>
/// Abstract base record for Immutable Entities (like Activities).
/// Uses standard Record "Value Equality" (checks all properties + ID), 
/// which is safer for immutable facts than reference equality.
/// </summary>
public abstract record EntityRecord<TId>(TId Id) : IEntity<TId>
	where TId : struct, IEquatable<TId> // alaf: Keep this constraint!
{
	// No explicit Equals or GetHashCode needed.
	// The compiler synthesizes them to check Id + All Other Properties.
}