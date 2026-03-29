using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Common.Security;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.Auth;
using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Application.Interfaces.Services.Tenant;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Application.UseCases.Invite.Handlers.Commands;

public sealed class AcceptInviteWithGoogleCommandHandler(
	IHashingService hashing,
	IClock clock,
	ITenantLimitsService tenantLimitsService,
	IInviteReadRepository inviteReadRepository,
	IInviteWriteRepository inviteWriteRepository,
	IAccountIdentifierReadRepository accountIdentifierReadRepository,
	IAccountIdentifierWriteRepository accountIdentifierWriteRepository,
	IAccountReadRepository accountReadRepository,
	IAccountWriteRepository accountWriteRepository,
	IExternalIdentityReadRepository externalIdentityReadRepository,
	IExternalIdentityWriteRepository externalIdentityWriteRepository,
	IUnitOfWork unitOfWork)
	: IRequestHandler<AcceptInviteWithGoogleCommand, TypedResult<AcceptInviteResult>>
{
	public async ValueTask<TypedResult<AcceptInviteResult>> Handle(
		AcceptInviteWithGoogleCommand request,
		CancellationToken ct)
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

		DateTimeOffset now = clock.UtcNow;
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

		FlowResult.Result limitChecksResult =
			await tenantLimitsService.EnsureTenantCanInviteOrActivateAccountAsync(request.TenantId, ct);
		if (!limitChecksResult.IsSuccess)
		{
			return TypedResult<AcceptInviteResult>.Fail(
				limitChecksResult.Error ?? ResultError.Validation("Tenant has no longer capability to accept invites"));
		}

		InviteTarget inviteTarget = invite.InviteTarget;
		if (inviteTarget.Channel == InviteChannel.Email)
		{
			Email invitedEmail = Email.Create(inviteTarget.Value);
			Email googleEmail = Email.Create(request.Email);

			if (!request.Verified)
			{
				return TypedResult<AcceptInviteResult>.Fail(ResultError.Conflict("Google email must be verified."));
			}

			if (invitedEmail != googleEmail)
			{
				return TypedResult<AcceptInviteResult>.Fail(
					ResultError.Conflict("The signed-in Google account does not match the invited email."));
			}

			bool emailOccupied = await accountIdentifierReadRepository.ExistsAsync(
				request.TenantId,
				AccountIdentifierKind.Email,
				invitedEmail.Value,
				ct);

			if (emailOccupied)
			{
				return TypedResult<AcceptInviteResult>.Fail(
					ResultError.Conflict("An account with this email already exists in this tenant."));
			}

			bool googleIdentityOccupied = await externalIdentityReadRepository.ExistsAsync(
				request.TenantId,
				AuthProvider.Google,
				request.GoogleSubjectId,
				ct);

			if (googleIdentityOccupied)
			{
				return TypedResult<AcceptInviteResult>.Fail(
					ResultError.Conflict("This Google account is already linked in this tenant."));
			}

			// check contact info references - if another account has the same email as contact info, we should prevent accepting the invite to avoid confusion in tenant's admin
			AccountDetails? existingAccount = await accountReadRepository.GetByEmailAndTenantAsync(
				Email.Create(inviteTarget.Value),
				request.TenantId,
				ct);
			if (existingAccount is not null)
			{
				return TypedResult<AcceptInviteResult>.Fail(
					ResultError.Conflict("An account with this email already exists in this tenant."));
			}
		}
		else
		{
			return TypedResult<AcceptInviteResult>.Fail(
				ResultError.NotSupported($"Invite by provided channel: '{inviteTarget.Channel}' is not yet supported"));
		}

		// create account entity
		Account newAccount = Account.Create(
			request.TenantId,
			invite.InviteTarget.Value,
			new ContactInfo(Email.Create(inviteTarget.Value)),
			invite.IntendedRole,
			AccountState.Active,
			invite.IntendedTier);

		// link account entity to External Identity
		ExternalIdentity newExternalUser = ExternalIdentity.Create(
			newAccount.Id,
			newAccount.TenantId,
			AuthProvider.Google,
			request.GoogleSubjectId,
			true);

		// link account entity to AccountIdentifier
		AccountIdentifier accountIdentifier = AccountIdentifier.Create(
			newAccount.TenantId,
			newAccount.Id,
			AccountIdentifierKind.Email,
			request.Email,
			true,
			true);

		invite.Accept(newAccount.Id, now);

		try
		{
			await accountWriteRepository.AddAsync(newAccount, ct);
			await externalIdentityWriteRepository.AddAsync(newExternalUser, ct);
			await accountIdentifierWriteRepository.AddAsync(accountIdentifier, ct);
			await inviteWriteRepository.UpdateAsync(invite, ct);

			_ = await unitOfWork.SaveChangesAsync(ct);
		}
		catch (UniqueConstraintViolationException)
		{
			return TypedResult<AcceptInviteResult>.Fail(
				ResultError.Conflict("Invite already accepted or account with provided email already exists."));
		}
		catch (Exception)
		{
			return TypedResult<AcceptInviteResult>.Fail(
				ResultError.Conflict("Server error while accepting an invite."));
		}

		return TypedResult<AcceptInviteResult>.Ok(
			new AcceptInviteResult(invite.Id, newAccount.Id, newAccount.TenantId, newAccount.Role, newAccount.State));
	}
}