using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Invite.Flow.Mappers;
using Portiforce.SAA.Application.UseCases.Invite.Projections;

namespace Portiforce.SAA.Application.UseCases.Invite.Handlers.Queries;

public sealed class GetInviteDetailsQueryHandler(
    IClock clock,
    IInviteReadRepository inviteReadRepository) : IRequestHandler<GetInviteDetailsQuery, TypedResult<InviteDetails>>
{
    public async ValueTask<TypedResult<InviteDetails>> Handle(
        GetInviteDetailsQuery request,
        CancellationToken ct)
    {
        InviteDetailsRaw? inviteDetailsRaw = await inviteReadRepository.GetByIdAsync(
            request.InviteId,
            ct);

        if (inviteDetailsRaw is null)
        {
            return TypedResult<InviteDetails>.Fail(
                ResultError.NotFound(
                    "Invite",
                    request.InviteId));
        }

        DateTimeOffset now = clock.UtcNow;

        InviteDetails inviteDetails = InviteProjectionMapper.ToDetails(inviteDetailsRaw, now);

        return TypedResult<InviteDetails>.Ok(inviteDetails);
    }
}