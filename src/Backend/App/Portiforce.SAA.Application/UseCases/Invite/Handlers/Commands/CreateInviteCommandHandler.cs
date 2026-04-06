using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Common.Security;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Interfaces.Models.Auth;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.Auth;
using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Interfaces.Services.Tenant;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Invite.Projections.Details;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Handlers.Commands;

public sealed class CreateInviteCommandHandler(
	ITokenGenerator tokenGenerator,
	IHashingService hashingService,
	IClock clock,
	ITenantLimitsService tenantLimitsService,
	IAccountIdentifierReadRepository accountIdentifierReadRepository,
	IExternalIdentityReadRepository externalIdentityReadRepository,
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

		DateTimeOffset now = clock.UtcNow;

		if (request.ExpiredAtUtc <= now)
		{
			return TypedResult<CreateInviteResult>.Fail(
				ResultError.Validation("Invite expiration must be in the future."));
		}

		bool isInviteValueOccupied = request.InviteTarget.Channel switch
		{
			InviteChannel.Email => await accountIdentifierReadRepository.ExistsAsync(
				request.TenantId,
				AccountIdentifierKind.Email,
				request.InviteTarget.Value,
				ct),

			InviteChannel.Telegram => await accountIdentifierReadRepository.ExistsAsync(
				request.TenantId,
				AccountIdentifierKind.TelegramUserId,
				request.InviteTarget.Value,
				ct),

			InviteChannel.AppleAccount => await accountIdentifierReadRepository.ExistsAsync(
				request.TenantId,
				request.InviteTarget.Kind == InviteTargetKind.Email
					? AccountIdentifierKind.Email
					: AccountIdentifierKind.Phone,
				request.InviteTarget.Value,
				ct),

			_ => false
		};

		if (isInviteValueOccupied)
		{
			return TypedResult<CreateInviteResult>.Fail(
				ResultError.Conflict("This identifier is already used by an account in this tenant."));
		}

		InviteDetailsRaw? existingInvite =
			await inviteReadRepository.GetByInviteTargetAndTenantAsync(request.InviteTarget, request.TenantId, ct);
		if (existingInvite is not null)
		{
			bool isExpired = existingInvite.ExpiresAtUtc <= now && existingInvite.State != InviteState.Accepted;

			if (!isExpired)
			{
				return TypedResult<CreateInviteResult>.Fail(
					ResultError.Conflict(
						$"Invite already exists in state {existingInvite.State}.",
						new Dictionary<string, object?>
						{
							["state"] = existingInvite.State.ToString(),
							["expiresAtUtc"] = existingInvite.ExpiresAtUtc,
							["relatedAccountId"] = existingInvite.RelatedAccountId?.Value.ToString()
						}));
			}

			// todo: Expired invites - allow creating a new one.
		}

		FlowResult.Result limitChecksResult =
			await tenantLimitsService.EnsureTenantCanInviteOrActivateAccountAsync(request.TenantId, ct);
		if (!limitChecksResult.IsSuccess)
		{
			return TypedResult<CreateInviteResult>.Fail(
				limitChecksResult.Error ?? ResultError.Validation("Tenant has no longer capability to create invites"));
		}

		DateTimeOffset expiresAt = request.ExpiredAtUtc;

		string rawInviteToken = tokenGenerator.GenerateInviteToken();
		byte[] tokenHash = hashingService.HashInviteToken(rawInviteToken);

		TenantInvite invite = TenantInvite.Create(
			request.TenantId,
			request.InviteTarget,
			request.InvitedByAccountId,
			request.Alias,
			request.IntendedRole,
			request.IntendedTier,
			tokenHash,
			now,
			expiresAt);

		try
		{
			await inviteWriteRepository.AddAsync(invite, ct);
			_ = await unitOfWork.SaveChangesAsync(ct);
		}
		catch (UniqueConstraintViolationException)
		{
			return TypedResult<CreateInviteResult>.Fail(
				ResultError.Conflict(
					"Invite could not be created because an active invite already exists or the identifier is already in use."));
		}

		return TypedResult<CreateInviteResult>.Ok(
			new CreateInviteResult(
				invite.Id,
				rawInviteToken,
				expiresAt));
	}
}