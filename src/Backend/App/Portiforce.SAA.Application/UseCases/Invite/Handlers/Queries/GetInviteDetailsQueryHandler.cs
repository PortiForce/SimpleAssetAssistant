using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Invite.Projections;

namespace Portiforce.SAA.Application.UseCases.Invite.Handlers.Queries;

public sealed class GetInviteDetailsQueryHandler(
	IInviteReadRepository inviteReadRepository
) : IRequestHandler<GetInviteDetailsQuery, TypedResult<InviteDetails>>
{
	public async ValueTask<TypedResult<InviteDetails>> Handle(GetInviteDetailsQuery request, CancellationToken ct)
	{
		InviteDetails? inviteDetails = await inviteReadRepository.GetByIdAsync(
			request.InviteId,
			ct);

		if (inviteDetails is null)
		{
			return TypedResult<InviteDetails>.Fail(
				ResultError.NotFound("Invite",
				request.InviteId));
		}

		return TypedResult<InviteDetails>.Ok(inviteDetails);
	}
}
