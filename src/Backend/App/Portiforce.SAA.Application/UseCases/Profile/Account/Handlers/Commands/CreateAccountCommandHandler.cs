using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.Client;
using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Application.Interfaces.Services.Tenant;
using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Client.Tenant.Projections;
using Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Profile.Account.Handlers.Commands;

public sealed class CreateAccountCommandHandler(
	ITenantLimitsService tenantLimitsService,
	IAccountReadRepository accountReadRepository,
	IAccountWriteRepository accountWriteRepository,
	ITenantReadRepository tenantReadRepository,
		IUnitOfWork unitOfWork)
		: IRequestHandler<CreateAccountCommand, CommandResult<AccountId>>
{
	public async ValueTask<CommandResult<AccountId>> Handle(CreateAccountCommand request, CancellationToken ct)
	{
		if (request.TenantId.IsEmpty)
		{
			throw new DomainValidationException("TenantId is not defined");
		}

		TenantSummary tenantSummary = await tenantReadRepository.GetSummaryByIdAsync(request.TenantId, ct)
		                              ?? throw new NotFoundException("Tenant", request.TenantId);

		AccountDetails? accountDetails = await accountReadRepository.GetByEmailAndTenantAsync(request.Email, request.TenantId, ct);
		if (accountDetails is not null)
		{
			if (accountDetails.State == AccountState.Active || accountDetails.State == AccountState.PendingActivation)
			{
				throw new ConflictException($"Account with email {request.Email.Value} is already active or pending in this tenant.");
			}
			// todo : consider deleted state but technically once email is taken - it is taken
			throw new DomainValidationException($"Account with email: {request.Email.Value} and state: {accountDetails.State} is already added for Tenant");
		}

		await tenantLimitsService.EnsureTenantCanInviteOrActivateUserAsync(tenantSummary, ct);

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

		return new CommandResult<AccountId>()
		{
			Id = account.Id,
			Message = $"{account.Id} registered successfully"
		};
	}
}
