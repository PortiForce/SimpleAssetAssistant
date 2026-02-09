using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Portiforce.SAA.Api.Interfaces;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Api.Services;

internal sealed class TenantIdServiceResolver(
	IHttpContextAccessor httpContextAccessor, 
	ILogger<TenantIdServiceResolver> logger) : ITenantIdServiceResolver
{
	private const string TenantHeaderName = "X-Tenant-ID";

	/// <summary>
	/// Helper to safely extract and parse the Tenant ID from the X-Tenant-ID header.
	/// </summary>
	public TenantId? GetTenantFromHeader(out ProblemDetails? problem)
	{
		problem = null;

		var context = httpContextAccessor.HttpContext;

		// fpr a background job or outside a request, Context is null
		if (context is null)
		{
			return null;
		}

		if (!context.Request.Headers.TryGetValue(TenantHeaderName, out StringValues values))
		{
			return null;
		}

		var headerValue = values.ToString();
		if (string.IsNullOrWhiteSpace(headerValue))
		{
			return null;
		}

		try
		{
			TenantId tenantId = TenantId.From(Guid.Parse(headerValue));
			return tenantId;
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to parse X-Tenant-ID header: {HeaderValue}", headerValue);

			problem = new ProblemDetails
			{
				Title = "Invalid tenant header",
				Detail = $"{TenantHeaderName} must be a valid GUID.",
				Status = StatusCodes.Status400BadRequest,
				Extensions =
				{
					["code"] = "PF-400-INVALID_TENANT_HEADER"
				}
			};
			return null;
		}
	}
}