using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;

public sealed record LoginWithGoogleCommand(string IdToken, TenantId TenantId) : ICommand<AuthResponse>;
