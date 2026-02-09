using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Auth.Actions.Commands;

public sealed record LogoutCommand(string RefreshToken, AccountId AccountId, TenantId TenantId, string? IpAddress) : ICommand<Unit>;

