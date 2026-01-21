using Microsoft.EntityFrameworkCore;

using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Extensions;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories;

internal sealed class EfUnitOfWork(AssetAssistantDbContext db) : IUnitOfWork
{
	public async Task<int> SaveChangesAsync(CancellationToken ct)
	{
		try
		{
			return await db.SaveChangesAsync(ct).ConfigureAwait(false);
		}
		catch (DbUpdateException ex) when (DbUpdateExceptionClassifier.IsUniqueConstraintViolation(ex))
		{
			// surface specifics via ProblemDetails code mapping.
			throw new UniqueConstraintViolationException("Unique constraint violation");
		}
	}
}
