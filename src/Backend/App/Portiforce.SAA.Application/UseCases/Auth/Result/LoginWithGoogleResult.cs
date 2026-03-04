using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Auth.Result;

public sealed record LoginWithGoogleResult(
	AccountId AccountId,
	TenantId TenantId,
	Role Role,
	AccountState State);