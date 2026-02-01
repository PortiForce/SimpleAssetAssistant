using Microsoft.EntityFrameworkCore;

using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Extensions;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Auth;

internal sealed class ExternalIdentityWriteRepository(AssetAssistantDbContext db) : IExternalIdentityWriteRepository
{
	public async Task AddAsync(ExternalIdentity externalIdentity, CancellationToken ct)
	{
		try
		{
			await db.ExternalIdentities.AddAsync(externalIdentity, ct);
			await db.SaveChangesAsync(ct);
		}
		catch (DbUpdateException ex)
		{
			if (DbUpdateExceptionClassifier.IsUniqueConstraintViolation(ex))
			{
				throw new ConflictException(
					"This Google account is already linked to a user.");
			}

			// If it's something else (e.g. Foreign Key fail), let it bubble up as 500
			throw;
		}
	}
}
