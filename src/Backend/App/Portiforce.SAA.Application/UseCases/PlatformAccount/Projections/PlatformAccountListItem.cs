using Portiforce.SAA.Application.Interfaces.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.PlatformAccount.Projections;

public sealed record PlatformAccountListItem(
	PlatformAccountId Id,
	TenantId TenantId,
	AccountId AccountId,
	PlatformId PlatformId,
	string AccountName,
	string? ExternalUserId) : IListItemProjection;
