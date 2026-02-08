using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Profile.Account.Requests;

public sealed record InviteUserRequest(
	string Email,
	string Alias,
	Role Role, 
	AccountTier Tier
);