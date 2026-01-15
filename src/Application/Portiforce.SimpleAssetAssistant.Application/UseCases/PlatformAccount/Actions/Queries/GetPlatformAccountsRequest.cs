using Portiforce.SimpleAssetAssistant.Application.Models.DTOs.PlatformAccount;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.PlatformAccount.Actions.Queries;

// Returns the user's connected accounts
public sealed record GetMyPlatformAccountsRequest(
	AccountId AccountId
) : IQuery<List<PlatformAccountDto>>;
