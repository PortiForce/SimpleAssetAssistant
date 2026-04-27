using Microsoft.Extensions.Caching.Memory;

using Portiforce.SAA.Contracts.Contexts;

namespace Portiforce.SAA.Web.Infrastructure;

public sealed class ContactMessageDailyRateLimitFilter(
	IMemoryCache memoryCache,
	ITenantContext tenantContext,
	ILogger<ContactMessageDailyRateLimitFilter> logger) : IEndpointFilter
{
	private const int DailyPermitLimit = 10;

	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		string partitionKey = GetPartitionKey(context.HttpContext, tenantContext);
		string cacheKey = $"contact-message:daily:{DateTimeOffset.UtcNow:yyyyMMdd}:{partitionKey}";

		ContactMessageDailyCounter counter = memoryCache.GetOrCreate(
			cacheKey,
			static entry =>
			{
				entry.AbsoluteExpiration = DateTimeOffset.UtcNow.Date.AddDays(1);
				return new ContactMessageDailyCounter();
			}) ?? new ContactMessageDailyCounter();

		int attemptCount = counter.Increment();
		if (attemptCount > DailyPermitLimit)
		{
			logger.LogWarning(
				"Contact message daily rate limit exceeded for partition {PartitionKey}.",
				partitionKey);

			return TypedResults.Problem(
				title: "Too many contact messages.",
				detail: "Please wait before sending another message.",
				statusCode: StatusCodes.Status429TooManyRequests);
		}

		return await next(context);
	}

	private static string GetPartitionKey(HttpContext httpContext, ITenantContext tenantContext)
	{
		string tenantPart = tenantContext.TenantId?.ToString("N") ?? "landing";
		string clientPart = httpContext.Connection.RemoteIpAddress?.ToString()
							?? httpContext.Request.Headers.Host.ToString()
							?? "anonymous";

		return $"{tenantPart}:{clientPart}";
	}

	private sealed class ContactMessageDailyCounter
	{
		private int _messagesCount;

		public int Increment() => Interlocked.Increment(ref this._messagesCount);
	}
}