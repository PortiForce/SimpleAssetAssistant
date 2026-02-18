using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Profile.Result;

public sealed record CreateAccountResult(AccountId AccountId, TenantId TenantId);
