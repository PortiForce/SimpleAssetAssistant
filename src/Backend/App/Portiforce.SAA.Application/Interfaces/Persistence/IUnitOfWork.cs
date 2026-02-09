namespace Portiforce.SAA.Application.Interfaces.Persistence;

public interface IUnitOfWork
{
	/// <summary>
	/// Commits all changes tracked by Repositories to the database.
	/// </summary>
	Task<int> SaveChangesAsync(CancellationToken ct);
}
