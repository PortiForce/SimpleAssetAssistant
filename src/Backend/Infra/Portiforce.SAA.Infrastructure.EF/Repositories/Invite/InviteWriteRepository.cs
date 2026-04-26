using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Invite;

internal sealed class InviteWriteRepository(AssetAssistantDbContext db) : IInviteWriteRepository
{
	public async Task AddAsync(TenantInvite entity, CancellationToken ct) => await db.Invites.AddAsync(entity, ct);

	public async Task UpdateAsync(TenantInvite tenantInvite, CancellationToken ct)
	{
		_ = db.Invites.Update(tenantInvite);
		await Task.CompletedTask;
	}
}