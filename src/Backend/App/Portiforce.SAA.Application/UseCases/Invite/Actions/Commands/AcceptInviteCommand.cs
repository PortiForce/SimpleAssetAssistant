using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;

/// <summary>
/// Invite user flow
/// </summary>
/// <param name="TenantId">current tenant</param>
/// <param name="RawToken">Invite token info</param>
/// <param name="ExternalId">reference to the external id that was used to authenticate as a user with valid token</param>
/// <param name="AutoGenAlias">if provided: auto generated alias</param>
public sealed record AcceptInviteCommand(
	TenantId TenantId,
	string RawToken,
	// reference to the external id that was used to authenticate as a user with valid token
	string ExternalId,
	string? AutoGenAlias) : ICommand<TypedResult<AcceptInviteResult>>;
