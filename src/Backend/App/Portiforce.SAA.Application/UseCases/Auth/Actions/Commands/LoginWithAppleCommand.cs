using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Auth.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Auth.Actions.Commands;

/// <summary>
/// 
/// </summary>
/// <param name="IdToken"></param>
/// <param name="TenantId">if tenantId is null : this is global Login lookup flow </param>
public sealed record LoginWithAppleCommand(
	string IdToken,
	TenantId? TenantId) : ICommand<AuthResponse>;
