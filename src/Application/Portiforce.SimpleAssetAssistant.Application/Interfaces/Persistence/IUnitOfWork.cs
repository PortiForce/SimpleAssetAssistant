namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;

public interface IUnitOfWork
{
	/// <summary>
	/// Commits all changes tracked by Repositories to the database.
	/// </summary>
	Task SaveChangesAsync(CancellationToken ct);
}
