using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Application.Interfaces.Services.Tenant;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Application.UseCases.Profile.Result;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Profile;

namespace Portiforce.SAA.Application.UseCases.Profile.Account.Handlers.Commands;

/// <summary>
/// Not an ordinary user onboarding flow - but some internal case then Account can be created without an Invite
/// </summary>
/// <param name="tenantLimitsService"></param>
/// <param name="accountReadRepository"></param>
/// <param name="accountWriteRepository"></param>
/// <param name="unitOfWork"></param>
public sealed class CreateAccountCommandHandler(
	ITenantLimitsService tenantLimitsService,
	IAccountReadRepository accountReadRepository,
	IAccountWriteRepository accountWriteRepository,
		IUnitOfWork unitOfWork)
		: IRequestHandler<CreateAccountCommand, TypedResult<CreateAccountResult>>
{
	public async ValueTask<TypedResult<CreateAccountResult>> Handle(CreateAccountCommand request, CancellationToken ct)
	{
		if (request.TenantId.IsEmpty)
		{
			throw new DomainValidationException("TenantId is not defined");
		}

		AccountDetails? accountDetails = await accountReadRepository.GetByEmailAndTenantAsync(request.Email, request.TenantId, ct);
		if (accountDetails is not null)
		{
			if (accountDetails.State is AccountState.Active or AccountState.PendingActivation)
			{
				throw new ConflictException($"Account with email {request.Email.Value} is already active or pending in this tenant.");
			}
			// todo tech: consider deleted state but technically once email is taken - it is taken
			throw new DomainValidationException($"Account with email: {request.Email.Value} and state: {accountDetails.State} is already added for Tenant");
		}

		FlowResult.Result limitChecksResult = await tenantLimitsService.EnsureTenantCanInviteOrActivateAccountAsync(request.TenantId, ct);
		if (!limitChecksResult.IsSuccess)
		{
			return TypedResult<CreateAccountResult>.Fail(limitChecksResult.Error ?? ResultError.Validation("Tenant has no longer capability to create accounts"));
		}

		Core.Identity.Models.Profile.Account account = Core.Identity.Models.Profile.Account.Create(
			request.TenantId,
			request.Alias,
			new ContactInfo(request.Email),
			request.Role,
			AccountState.PendingActivation,
			request.Tier);

		try
		{
			await accountWriteRepository.AddAsync(account, ct);
			await unitOfWork.SaveChangesAsync(ct);
		}
		catch (UniqueConstraintViolationException)
		{
			throw new ConflictException($"Account with email: {request.Email.Value} already created.");
		}

		return TypedResult<CreateAccountResult>.Ok(new CreateAccountResult(
			AccountId: account.Id,
			TenantId: request.TenantId
		));
	}
}
