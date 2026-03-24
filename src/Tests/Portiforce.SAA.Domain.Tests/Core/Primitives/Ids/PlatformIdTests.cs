using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Domain.Tests.Core.Primitives.Ids.Common;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives.Ids;

public sealed class PlatformIdTests : StronglyTypedGuidIdTests<PlatformId>
{
	protected override PlatformId Empty => PlatformId.Empty;

	protected override PlatformId New() => PlatformId.New();

	protected override PlatformId From(Guid value) => PlatformId.From(value);

	protected override PlatformId Parse(string raw) => PlatformId.Parse(raw);

	protected override bool TryParse(string? raw, out PlatformId id) => PlatformId.TryParse(raw, out id);

	protected override Guid GetValue(PlatformId id) => id.Value;

	protected override bool IsEmpty(PlatformId id) => id.IsEmpty;
}

