using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Services.Invite;

public interface IInviteLinkBuilder
{
	ValueTask<TypedResult<string>> BuildInviteOverviewUrlAsync(
		TenantId tenantId,
		string rawInviteToken,
		CancellationToken ct);
}