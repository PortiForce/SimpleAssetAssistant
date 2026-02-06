using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;

public sealed record LogoutCommand(string RefreshToken, AccountId AccountId, TenantId TenantId) : ICommand<Unit>;

