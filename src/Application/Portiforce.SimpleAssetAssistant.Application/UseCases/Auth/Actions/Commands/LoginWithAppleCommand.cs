using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;

/// <summary>
/// 
/// </summary>
/// <param name="IdToken"></param>
/// <param name="TenantId">if tenantId is null : this is global Login lookup flow </param>
public sealed record LoginWithAppleCommand(
	string IdToken,
	TenantId? TenantId) : ICommand<AuthResponse>;
