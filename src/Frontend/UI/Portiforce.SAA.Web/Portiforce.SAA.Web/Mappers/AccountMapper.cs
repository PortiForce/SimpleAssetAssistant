using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Contracts.Enums;
using Portiforce.SAA.Contracts.Models.Client.Account;
using Portiforce.SAA.Core.Identity.Enums;

namespace Portiforce.SAA.Web.Mappers;

public static class AccountMapper
{
	public static UiAccountTier ToPresentation(this AccountTier domainTier) => domainTier switch
	{
		AccountTier.Observer => UiAccountTier.Observer,
		AccountTier.Investor => UiAccountTier.Investor,
		AccountTier.Strategist => UiAccountTier.Strategist,
		_ => UiAccountTier.None
	};

	public static AccountTier? ToBusiness(this UiAccountTier presentationTier) => presentationTier switch
	{
		UiAccountTier.Observer => AccountTier.Observer,
		UiAccountTier.Investor => AccountTier.Investor,
		UiAccountTier.Strategist => AccountTier.Strategist,
		_ => null
	};

	public static UiAccountState ToPresentation(this AccountState domainState) => domainState switch
	{
		AccountState.Active => UiAccountState.Active,
		AccountState.PendingActivation => UiAccountState.PendingActivation,
		AccountState.Suspended => UiAccountState.Suspended,
		AccountState.Disabled => UiAccountState.Disabled,
		AccountState.PendingDeletion => UiAccountState.PendingDeletion,
		_ => UiAccountState.None
	};

	public static AccountState? ToBusiness(this UiAccountState presentationState) => presentationState switch
	{
		UiAccountState.Active => AccountState.Active,
		UiAccountState.PendingActivation => AccountState.PendingActivation,
		UiAccountState.Suspended => AccountState.Suspended,
		UiAccountState.Disabled => AccountState.Disabled,
		UiAccountState.PendingDeletion => AccountState.PendingDeletion,
		_ => null
	};

	public static AccountListResponse MapToAccountList(this PagedResult<AccountListItem> model)
	{
		AccountListItemResponse[] items = model.Items
			.Select(MapAccountListItem)
			.ToArray();

		return new AccountListResponse(
			items,
			model.TotalCount,
			model.PageNumber,
			model.PageSize);
	}

	private static AccountListItemResponse MapAccountListItem(this AccountListItem model)
	{
		throw new NotImplementedException("User list is not yet implemented");

		//return new UserListItemResponse(
		//	model.Id,
		//	model.TenantId.Value,
		//	model.Alias);
	}

	public static AccountDetailsResponse MapToAccountDetails(this AccountDetails model)
	{
		throw new NotImplementedException("User details is not yet implemented");

		//return new UserDetailsResponse(
		//	model.Id,
		//	model.TenantId.Value,
		//	model.Alias);
	}
}
