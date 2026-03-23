using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Domain.Tests.Core.Primitives.Ids.Common;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives.Ids;

public sealed class PlatformPlatformAccountIdTests : StronglyTypedGuidIdTests<PlatformAccountId>
{
	protected override PlatformAccountId Empty => PlatformAccountId.Empty;

	protected override PlatformAccountId New() => PlatformAccountId.New();

	protected override PlatformAccountId From(Guid value) => PlatformAccountId.From(value);

	protected override PlatformAccountId Parse(string raw) => PlatformAccountId.Parse(raw);

	protected override bool TryParse(string? raw, out PlatformAccountId id) => PlatformAccountId.TryParse(raw, out id);

	protected override Guid GetValue(PlatformAccountId id) => id.Value;

	protected override bool IsEmpty(PlatformAccountId id) => id.IsEmpty;
}

