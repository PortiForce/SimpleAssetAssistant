using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Notification;

public interface IOutboxMessageWriter
{
	ValueTask AddAsync<TMessage>(
		TenantId tenantId,
		string messageType,
		TMessage message,
		CancellationToken ct);
}