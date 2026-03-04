using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Core.Interfaces;

public interface IAuditedEntity
{
	DateTimeOffset CreatedAt { get; }
	DateTimeOffset? UpdatedAt { get; }

	AccountId? UpdatedByActorId { get; }
	string? UpdatedBySource { get; }
}
