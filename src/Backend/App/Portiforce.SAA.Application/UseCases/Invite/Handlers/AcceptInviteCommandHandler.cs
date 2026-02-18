using System.Linq.Expressions;
using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Common.Security;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Application.Interfaces.Services.Tenant;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Application.UseCases.Invite.Handlers;

public sealed class AcceptInviteCommandHandler(
	IHashingService hashing,
	IClock clock,
	ITenantLimitsService tenantLimitsService,
	IInviteReadRepository inviteReadRepository,
	IInviteWriteRepository inviteWriteRepository,
	IAccountReadRepository accountReadRepository,
	IAccountWriteRepository accountWriteRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<AcceptInviteCommand, TypedResult<AcceptInviteResult>>
{
	public async ValueTask<TypedResult<AcceptInviteResult>> Handle(AcceptInviteCommand request, CancellationToken ct)
	{
		byte[] tokenHash;
		try
		{
			tokenHash = hashing.HashInviteToken(request.RawToken);
		}
		catch (ArgumentException)
		{
			return TypedResult<AcceptInviteResult>.Fail(ResultError.NotFound("Invite not found.", request.RawToken));
		}

		TenantInvite? invite = await inviteReadRepository.GetByTenantAndTokenHashAsync(request.TenantId, tokenHash, ct);
		if (invite is null)
		{
			return TypedResult<AcceptInviteResult>.Fail(ResultError.NotFound("Invite not found.", request.RawToken));
		}

		var now = clock.UtcNow;
		if (invite.IsExpired(now))
		{
			return TypedResult<AcceptInviteResult>.Fail(ResultError.Conflict("Invite expired."));
		}

		if (invite.State == InviteState.RevokedByTenant)
		{
			return TypedResult<AcceptInviteResult>.Fail(ResultError.Conflict("Invite revoked."));
		}

		if (invite.State == InviteState.DeclinedByUser)
		{
			return TypedResult<AcceptInviteResult>.Fail(ResultError.Conflict("Invite declined."));
		}

		if (invite.State == InviteState.Accepted)
		{
			return TypedResult<AcceptInviteResult>.Fail(ResultError.Conflict("Invite already accepted."));
		}

		var role = MapRole(invite.IntendedRole);
		var tier = MapTier(invite.IntendedTier);
		if (role == Role.None || tier == AccountTier.None)
		{
			return TypedResult<AcceptInviteResult>.Fail(ResultError.Validation("Invalid invite role or tier."));
		}

		FlowResult.Result limitChecksResult = await tenantLimitsService.EnsureTenantCanInviteOrActivateAccountAsync(request.TenantId, ct);
		if (!limitChecksResult.IsSuccess)
		{
			return TypedResult<AcceptInviteResult>.Fail(limitChecksResult.Error ?? ResultError.Validation("Tenant has no longer capability to accept invites"));
		}
		
		var inviteTarget = invite.InviteTarget;
		if (inviteTarget.Type == InviteChannel.Email)
		{
			AccountDetails? existingAccount = await accountReadRepository.GetByEmailAndTenantAsync(Email.Create(inviteTarget.Value), request.TenantId, ct);
			if (existingAccount is not null)
			{
				return TypedResult<AcceptInviteResult>.Fail(ResultError.Conflict("An account with this email already exists in this tenant."));
			}
		}
		else
		{
			// todo tech: here I need to handle other invite channels when they are implemented (e.g. SSO) - for now I just want to make sure that the system is ready for that and doesn't allow accepting invites with unsupported channels
			return TypedResult<AcceptInviteResult>.Fail(ResultError.NotSupported($"Invite by provided channel: '{inviteTarget.Type}' is not yet supported"));
		}

		var newAccount = Account.Create(
			request.TenantId,
			request.AutoGenAlias ?? (invite.InviteTarget.Value.Split('@')[0] + new Random(1).Next(999)),
			new ContactInfo(Email.Create(inviteTarget.Value)), // todo : fix me
			role,
			AccountState.PendingActivation,
			tier
		);

		invite.Accept(newAccount.Id, now);

		try
		{
			await accountWriteRepository.AddAsync(newAccount, ct);
			await inviteWriteRepository.UpdateAsync(invite, ct);

			// todo : here I already should have an externalId that is referenced to the already logged in account -> but not sure how to handle it yet

			await unitOfWork.SaveChangesAsync(ct);
		}
		catch (UniqueConstraintViolationException)
		{
			return TypedResult<AcceptInviteResult>.Fail(ResultError.Conflict("Invite already accepted or account with provided email already exists."));
		}

		await unitOfWork.SaveChangesAsync(ct);

		return TypedResult<AcceptInviteResult>.Ok(new AcceptInviteResult(invite.Id, newAccount.Id));
	}

	private static Role MapRole(InviteTenantRole inviteRole) => inviteRole switch
	{
		InviteTenantRole.TenantUser => Role.TenantUser,
		InviteTenantRole.TenantAdmin => Role.TenantAdmin,
		_ => Role.None
	};

	private static AccountTier MapTier(InviteAccountTier inviteTier) => inviteTier switch
	{
		// here I need to throw an exception in case of fail as this is not a standard business rule violation
		InviteAccountTier.Observer => AccountTier.Observer,
		InviteAccountTier.Investor => AccountTier.Investor,
		InviteAccountTier.Strategist => AccountTier.Strategist,
		_ => AccountTier.None
	};
}
