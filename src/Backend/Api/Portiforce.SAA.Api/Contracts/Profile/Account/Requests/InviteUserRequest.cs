using Portiforce.SAA.Core.Identity.Enums;

namespace Portiforce.SAA.Api.Contracts.Profile.Account.Requests;

public sealed record InviteUserRequest(
	string Email,
	string Alias,
	Role Role, 
	AccountTier Tier
);