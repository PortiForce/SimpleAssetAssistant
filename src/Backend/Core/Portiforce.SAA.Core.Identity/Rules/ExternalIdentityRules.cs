using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Identity.Models.Auth;

namespace Portiforce.SAA.Core.Identity.Rules;

public static class ExternalIdentityRules
{
	public static void EnsureSinglePrimary(IEnumerable<ExternalIdentity> identities)
	{
		if (identities.Count(i => i.IsPrimary) > 1)
		{
			throw new DomainValidationException("Only one external identity can be primary per account.");
		}
	}
}