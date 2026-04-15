using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Domain.Tests.Core.Primitives.Ids.Common;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives.Ids;

public sealed class AccountIdentifierIdTests : StronglyTypedGuidIdTests<AccountIdentifierId>
{
	protected override AccountIdentifierId Empty => AccountIdentifierId.Empty;

	protected override AccountIdentifierId New() => AccountIdentifierId.New();

	protected override AccountIdentifierId From(Guid value) => AccountIdentifierId.From(value);

	protected override AccountIdentifierId Parse(string raw) => AccountIdentifierId.Parse(raw);

	protected override bool TryParse(string? raw, out AccountIdentifierId id) =>
		AccountIdentifierId.TryParse(raw, out id);

	protected override Guid GetValue(AccountIdentifierId id) => id.Value;

	protected override bool IsEmpty(AccountIdentifierId id) => id.IsEmpty;
}