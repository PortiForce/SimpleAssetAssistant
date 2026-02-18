using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Invite;

internal sealed class InviteWriteRepository(AssetAssistantDbContext db) : IInviteWriteRepository
{
	public Task AddAsync(TenantInvite entity, CancellationToken ct)
	{
		return db.Invites.AddAsync(entity, ct).AsTask();
	}

	public Task UpdateAsync(TenantInvite tenantInvite, CancellationToken ct)
	{
		db.Invites.Update(tenantInvite);
		return Task.CompletedTask;
	}
}
