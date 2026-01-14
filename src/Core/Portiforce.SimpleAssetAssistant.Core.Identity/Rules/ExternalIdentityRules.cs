using System.ComponentModel.DataAnnotations;

using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;

namespace Portiforce.SimpleAssetAssistant.Core.Identity.Rules;

public static class ExternalIdentityRules
{
	public static void EnsureSinglePrimary(IEnumerable<ExternalIdentity> identities)
	{
		if (identities.Count(i => i.IsPrimary) > 1)
		{
			throw new ValidationException("Only one external identity can be primary per account.");
		}
	}
}