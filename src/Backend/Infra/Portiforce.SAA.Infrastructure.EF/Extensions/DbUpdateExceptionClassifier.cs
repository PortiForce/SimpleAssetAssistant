using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Portiforce.SAA.Infrastructure.EF.Extensions;

internal static class DbUpdateExceptionClassifier
{
	public static bool IsUniqueConstraintViolation(DbUpdateException ex)
	{
		// 1. Unwrap the InnerException to find the driver-specific error
		if (ex.InnerException is SqlException sqlEx)
		{
			// MSSQL Error Codes:
			// 2601: Cannot insert duplicate key row in object with unique index.
			// 2627: Violation of %ls constraint. Cannot insert duplicate key in object.
			return sqlEx.Number == 2601 || sqlEx.Number == 2627;
		}

		// 2. (Optional) Support for SQLite (Local/Testing)
		// If you use SQLite for unit tests, you need this, otherwise tests fail.
		if (ex.InnerException is Microsoft.Data.Sqlite.SqliteException sqliteEx)
		{
			// SQLite Error Code 19: Constraint violation
			// Extended Code 2067: UNIQUE constraint failed
			return sqliteEx.SqliteErrorCode == 19;
		}

		// 3. Fallback (The "Dirty" Check)
		// Only use this if the specific drivers failed to match, as a last resort.
		// It helps if you accidentally swap DB providers without updating this method.
		var msg = ex.InnerException?.Message ?? ex.Message;
		return msg.Contains("IX_", StringComparison.OrdinalIgnoreCase)
		       && (msg.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
		           msg.Contains("unique", StringComparison.OrdinalIgnoreCase));
	}
}
