using Microsoft.EntityFrameworkCore;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Extensions;

internal static class DbUpdateExceptionClassifier
{
	public static bool IsUniqueConstraintViolation(DbUpdateException ex)
	{
		// todo tech: for production check SQL error numbers per provider.
		var msg = ex.InnerException?.Message ?? ex.Message;
		return msg.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase)
		       || msg.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
		       || msg.Contains("duplicate key", StringComparison.OrdinalIgnoreCase);
	}
}
