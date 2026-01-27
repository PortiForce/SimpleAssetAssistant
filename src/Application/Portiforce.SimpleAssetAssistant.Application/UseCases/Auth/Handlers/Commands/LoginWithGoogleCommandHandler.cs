using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Auth;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Auth;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;
using Portiforce.SimpleAssetAssistant.Application.Models.Auth;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Handlers.Commands;

public sealed class LoginWithGoogleCommandHandler(
	IGoogleAuthProvider googleProvider,
	IAccountReadRepository accountReadRepository,
	ITokenGenerator tokenGenerator,
	IExternalIdentityReadRepository externalIdentityReadRepository,
	IExternalIdentityWriteRepository externalIdentityWriteRepository
	) : IRequestHandler<LoginWithGoogleCommand, AuthResponse>
{
	public async ValueTask<AuthResponse> Handle(LoginWithGoogleCommand request, CancellationToken ct)
	{
		// 1. Verify Token with Google
		GoogleUserInfo googleUser = await googleProvider.VerifyAsync(request.IdToken, ct);

		// 2. Find Account by Google Link
		ExternalIdentityDetails? identity = await externalIdentityReadRepository.FindGoogleIdentityAsync(googleUser.ExternalId, ct);

		AccountDetails? accountDetails;
		if (identity != null)
		{
			// User has logged in before
			accountDetails = await accountReadRepository.GetByIdAsync(identity.AccountId, ct);
			if (accountDetails != null && accountDetails.TenantId != request.TenantId)
			{
				// additional check that account belongs to a tenant that is used at the moment
				throw new NotFoundException($"AccountDetails with Tenant: {request.TenantId}", identity.AccountId);
			}
		}
		else
		{
			// 3. Auto-Link or Fail?
			// For MVP: Check if an account with this EMAIL exists. 
			// If yes, create the link (ExternalIdentity). If no, throw "User not found".
			// (Self-signup is a complex topic, usually separated).

			// simplified lookup method
			accountDetails = await accountReadRepository.GetByEmailAndTenantAsync(googleUser.Email, request.TenantId, ct);

			if (accountDetails == null)
			{
				throw new NotFoundException("AccountDetails", googleUser.Email);
			}

			// Link them for next time
			ExternalIdentity newIdentity = ExternalIdentity.Create(
				accountDetails.Id,
				AuthProvider.Google,
				googleUser.ExternalId);

			await externalIdentityWriteRepository.AddAsync(newIdentity, ct);
		}

		if (accountDetails == null)
		{
			throw new NotFoundException("AccountDetails", googleUser.Email);
		}

		Account account = Account.Create(
			accountDetails.TenantId,
			accountDetails.Alias,
			contact: null,
			accountDetails.Role,
			accountDetails.State,
			accountDetails.Tier,
			settings: null,
			accountDetails.Id);
		

		// 4. Generate JWT
		var token = tokenGenerator.Generate(account!);

		return new AuthResponse(token, "dummy-refresh-token", 3600);
	}
}
