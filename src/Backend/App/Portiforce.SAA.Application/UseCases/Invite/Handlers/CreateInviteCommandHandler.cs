using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Common.Security;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Interfaces.Models.Auth;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Interfaces.Services.Tenant;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Handlers;

public sealed class CreateInviteCommandHandler(
	ITokenGenerator tokenGenerator,
	IHashingService hashingService,
	IClock clock,
	ITenantLimitsService tenantLimitsService,
	IInviteReadRepository inviteReadRepository,
	IInviteWriteRepository inviteWriteRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<CreateInviteCommand, TypedResult<CreateInviteResult>>
{
	public async ValueTask<TypedResult<CreateInviteResult>> Handle(CreateInviteCommand request, CancellationToken ct)
	{
		if (request.TenantId.IsEmpty)
		{
			return TypedResult<CreateInviteResult>.Fail(ResultError.Validation("TenantId is not defined."));
		}

		if (request.InvitedByAccountId == AccountId.Empty)
		{
			return TypedResult<CreateInviteResult>.Fail(ResultError.Validation("InvitedByAccountId is not defined."));
		}

		var now = clock.UtcNow;

		var existing = await inviteReadRepository.GetByInviteTargetAndTenantAsync(request.InviteTarget, request.TenantId, ct);
		if (existing is not null)
		{
			var isExpired = existing.ExpiresAtUtc <= now && existing.State != InviteState.Accepted;

			if (!isExpired)
			{
				return existing.State switch
				{
					InviteState.Accepted =>
						TypedResult<CreateInviteResult>.Fail(ResultError.Conflict(
							$"Invite already accepted for target '{request.InviteTarget.Value}' and channel: '{request.InviteTarget.Type}'.",
							details: new Dictionary<string, object?>
							{
								["state"] = existing.State.ToString(),
								["relatedAccountId"] = existing.RelatedAccountId?.Value.ToString()
							})),

					InviteState.RevokedByTenant =>
						TypedResult<CreateInviteResult>.Fail(ResultError.Conflict(
							$"Invite for '{request.InviteTarget.Value}' and channel: '{request.InviteTarget.Type}' was revoked by tenant.",
							details: new Dictionary<string, object?> { ["state"] = existing.State.ToString() })),

					InviteState.DeclinedByUser =>
						TypedResult<CreateInviteResult>.Fail(ResultError.Conflict(
							$"Invite for '{request.InviteTarget.Value}' and channel: '{request.InviteTarget.Type}' was declined by user.",
							details: new Dictionary<string, object?> { ["state"] = existing.State.ToString() })),

					// Created-Sent-AcceptAttemptFailed -> treat as pending
					_ =>
						TypedResult<CreateInviteResult>.Fail(ResultError.Conflict(
							$"Invite already exists in state {existing.State}.",
							details: new Dictionary<string, object?>
							{
								["state"] = existing.State.ToString(),
								["expiresAtUtc"] = existing.ExpiresAtUtc
							}))
				};
			}

			// Expired invites - allow creating a new one.
		}

		FlowResult.Result limitChecksResult = await tenantLimitsService.EnsureTenantCanInviteOrActivateAccountAsync(request.TenantId, ct);
		if (!limitChecksResult.IsSuccess)
		{
			return TypedResult<CreateInviteResult>.Fail(limitChecksResult.Error ?? ResultError.Validation("Tenant has no longer capability to create invites"));
		}

		//todo tech: move to settings
		var expiresAt = now.AddHours(48);

		var rawInviteToken = tokenGenerator.GenerateInviteToken();
		var tokenHash = hashingService.HashInviteToken(rawInviteToken);

		var invite = TenantInvite.Create(
			request.TenantId,
			request.InviteTarget,
			request.InvitedByAccountId,
			request.IntendedRole,
			request.IntendedTier,
			tokenHash,
			now,
			expiresAtUtc: expiresAt
		);

		try
		{
			await inviteWriteRepository.AddAsync(invite, ct);
			await unitOfWork.SaveChangesAsync(ct);
		}
		catch (UniqueConstraintViolationException)
		{
			return TypedResult<CreateInviteResult>.Fail(ResultError.Conflict(
				$"Invite with email '{request.InviteTarget.Value}' and channel: '{request.InviteTarget.Type}' already exists."));
		}

		return TypedResult<CreateInviteResult>.Ok(new CreateInviteResult(
			InviteId: invite.Id,
			Token: rawInviteToken,
			ExpiresAtUtc: expiresAt
		));
	}
}
