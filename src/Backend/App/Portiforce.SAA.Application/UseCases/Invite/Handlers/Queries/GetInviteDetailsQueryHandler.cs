using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Invite.Flow.Mappers;
using Portiforce.SAA.Application.UseCases.Invite.Projections.Details;

namespace Portiforce.SAA.Application.UseCases.Invite.Handlers.Queries;

public sealed class GetInviteDetailsQueryHandler(
	IClock clock,
	IInviteReadRepository inviteReadRepository)
	: IRequestHandler<GetInviteDetailsQuery, TypedResult<AdminInviteDetails>>
{
	public async ValueTask<TypedResult<AdminInviteDetails>> Handle(
		GetInviteDetailsQuery request,
		CancellationToken ct)
	{
		InviteDetailsRaw? inviteDetailsRaw = await inviteReadRepository.GetByIdAsync(
			request.InviteId,
			ct);

		if (inviteDetailsRaw is null)
		{
			return TypedResult<AdminInviteDetails>.Fail(
				ResultError.NotFound(
					"Invite",
					request.InviteId));
		}

		DateTimeOffset now = clock.UtcNow;

		AdminInviteDetails inviteDetails = InviteProjectionMapper.ToAdminDetails(inviteDetailsRaw, now);

		return TypedResult<AdminInviteDetails>.Ok(inviteDetails);
	}
}