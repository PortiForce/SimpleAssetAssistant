using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Contracts.Models.Client.User;

namespace Portiforce.SAA.Web.Mappers;

public static class AccountMapper
{
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
