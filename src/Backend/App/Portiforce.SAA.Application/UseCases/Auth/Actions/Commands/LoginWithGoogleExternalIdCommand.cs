using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Auth.Result;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Auth.Actions.Commands;

public sealed record LoginWithGoogleExternalIdCommand(
	TenantId TenantId,
	string GoogleSubjectId,
	string FirstName,
	string LastName) : ICommand<TypedResult<LoginWithGoogleResult>>;

