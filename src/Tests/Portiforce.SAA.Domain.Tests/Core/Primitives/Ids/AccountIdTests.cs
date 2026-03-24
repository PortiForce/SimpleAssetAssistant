using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Domain.Tests.Core.Primitives.Ids.Common;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives.Ids;

public sealed class AccountIdTests : StronglyTypedGuidIdTests<AccountId>
{
	protected override AccountId Empty => AccountId.Empty;

	protected override AccountId New() => AccountId.New();

	protected override AccountId From(Guid value) => AccountId.From(value);

	protected override AccountId Parse(string raw) => AccountId.Parse(raw);

	protected override bool TryParse(string? raw, out AccountId id) => AccountId.TryParse(raw, out id);

	protected override Guid GetValue(AccountId id) => id.Value;

	protected override bool IsEmpty(AccountId id) => id.IsEmpty;
}
