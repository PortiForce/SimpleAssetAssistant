using System.Net;

using Microsoft.Extensions.Options;

using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Persistence.Client;
using Portiforce.SAA.Application.Interfaces.Services.Invite;
using Portiforce.SAA.Application.UseCases.Client.Tenant.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Infrastructure.Invite;

public sealed class InviteLinkBuilder(
	ITenantReadRepository tenantReadRepository,
	IOptions<InviteLinkOptions> options) : IInviteLinkBuilder
{
	private readonly InviteLinkOptions options = options.Value;

	public async ValueTask<TypedResult<string>> BuildInviteOverviewUrlAsync(
		TenantId tenantId,
		string rawInviteToken,
		CancellationToken ct)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(rawInviteToken);

		if (tenantId.IsEmpty)
		{
			return TypedResult<string>.Fail(ResultError.InvalidTenantId());
		}

		if (string.IsNullOrWhiteSpace(this.options.PublicAppBaseUrl))
		{
			return TypedResult<string>.Fail(ResultError.Validation("Invite public app base URL is not configured."));
		}

		TenantSummary? tenant = await tenantReadRepository.GetSummaryByIdAsync(tenantId, ct);
		if (tenant is null)
		{
			return TypedResult<string>.Fail(ResultError.NotFound("Tenant", tenantId));
		}

		if (tenant.State != TenantState.Active)
		{
			return TypedResult<string>.Fail(ResultError.Validation("Tenant is not active"));
		}

		if (string.IsNullOrWhiteSpace(tenant.DomainPrefix))
		{
			return TypedResult<string>.Fail(ResultError.Validation("Tenant domain prefix is missing."));
		}

		string encodedToken = WebUtility.UrlEncode(rawInviteToken);

		return TypedResult<string>.Ok(
			$"https://{tenant.DomainPrefix}.{this.options.PublicAppBaseUrl.TrimEnd('/')}/invite/{encodedToken}");
	}
}