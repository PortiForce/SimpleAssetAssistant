using Portiforce.SAA.Core.Identity.Models.Invite;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Invite;

public interface IInviteWriteRepository : IWriteRepository<TenantInvite, Guid>
{
	Task UpdateAsync(TenantInvite entity, CancellationToken ct);
}
