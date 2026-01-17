using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Profile.Account;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Actions.Queries;

public sealed record GetAccountListRequest(
	TenantId TenantId,
	PageRequest PageRequest
) : IQuery<PagedResult<AccountListItemDto>>;
