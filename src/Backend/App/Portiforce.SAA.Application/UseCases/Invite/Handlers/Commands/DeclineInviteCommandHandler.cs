using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Common.Security;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;

namespace Portiforce.SAA.Application.UseCases.Invite.Handlers.Commands;

public sealed class DeclineInviteCommandHandler(
	IHashingService hashing,
	IClock clock,
	IInviteReadRepository inviteReadRepository,
	IInviteWriteRepository inviteWriteRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<DeclineInviteCommand, TypedResult<DeclineInviteResult>>
{
	public async ValueTask<TypedResult<DeclineInviteResult>> Handle(
		DeclineInviteCommand request,
		CancellationToken ct)
	{
		byte[] tokenHash;
		try
		{
			tokenHash = hashing.HashInviteToken(request.RawToken);
		}
		catch (ArgumentException)
		{
			return TypedResult<DeclineInviteResult>.Fail(ResultError.NotFound("Invite not found.", request.RawToken));
		}

		TenantInvite? invite = await inviteReadRepository.GetByTenantAndTokenHashAsync(request.TenantId, tokenHash, ct);
		if (invite is null)
		{
			return TypedResult<DeclineInviteResult>.Fail(ResultError.NotFound("Invite not found.", request.RawToken));
		}

		DateTimeOffset now = clock.UtcNow;
		if (invite.State == InviteState.DeclinedByUser)
		{
			return TypedResult<DeclineInviteResult>.Fail(ResultError.Conflict("Invite already declined."));
		}

		if (invite.State == InviteState.Accepted)
		{
			return TypedResult<DeclineInviteResult>.Fail(ResultError.Conflict("Invite already accepted."));
		}

		invite.Decline(now);
		try
		{
			await inviteWriteRepository.UpdateAsync(invite, ct);

			_ = await unitOfWork.SaveChangesAsync(ct);
		}
		catch (UniqueConstraintViolationException)
		{
			return TypedResult<DeclineInviteResult>.Fail(ResultError.Conflict("Invite already declined"));
		}
		catch (Exception)
		{
			return TypedResult<DeclineInviteResult>.Fail(
				ResultError.Conflict("Server error while declining an invite."));
		}

		return TypedResult<DeclineInviteResult>.Ok(new DeclineInviteResult(invite.Id, request.TenantId));
	}
}