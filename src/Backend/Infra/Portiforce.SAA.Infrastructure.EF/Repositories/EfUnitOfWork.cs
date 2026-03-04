using Microsoft.EntityFrameworkCore;
using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Infrastructure.EF.DbContexts;
using Portiforce.SAA.Infrastructure.EF.Extensions;

namespace Portiforce.SAA.Infrastructure.EF.Repositories;

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
