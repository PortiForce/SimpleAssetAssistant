namespace Portiforce.SAA.Application.Exceptions;

public sealed record ResultError(
	string Code,
	string Message,
	IReadOnlyDictionary<string, object?>? Details = null)
{
	public static ResultError InvalidTenantId(string message = "Invalid tenant ID.")
		=> new("invalid_tenant_id", message);

	public static ResultError Validation(string message, IReadOnlyDictionary<string, object?>? details = null)
		=> new("validation_error", message, details);

	public static ResultError Conflict(string message, IReadOnlyDictionary<string, object?>? details = null)
		=> new("conflict", message, details);

	public static ResultError NotFound(string entity, object key)
		=> new("not_found", $"{entity} was not found.", new Dictionary<string, object?> { ["key"] = key });

	public static ResultError Unauthorized(string message = "Unauthorized.")
		=> new("unauthorized", message);

	public static ResultError Forbidden(string message = "Forbidden.")
		=> new("forbidden", message);

	public static ResultError NotSupported(string message = "Not supported.")
		=> new("not_supported", message);
}