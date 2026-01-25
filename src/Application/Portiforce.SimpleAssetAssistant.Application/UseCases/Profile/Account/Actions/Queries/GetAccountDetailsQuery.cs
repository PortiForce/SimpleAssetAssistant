using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Actions.Queries;

public sealed record GetAccountDetailsQuery(AccountId Id) : IQuery<AccountDetails>
{
}
