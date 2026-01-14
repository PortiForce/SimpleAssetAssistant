using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Core.Interfaces;

public interface IAuditedEntity
{
	DateTimeOffset CreatedAt { get; }
	DateTimeOffset? UpdatedAt { get; }

	AccountId? UpdatedByActorId { get; }
	string? UpdatedBySource { get; }
}
