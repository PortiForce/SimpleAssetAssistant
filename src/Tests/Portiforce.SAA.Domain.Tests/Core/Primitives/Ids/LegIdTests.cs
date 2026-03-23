using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Domain.Tests.Core.Primitives.Ids.Common;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives.Ids;

public sealed class LegIdTests : StronglyTypedGuidIdTests<LegId>
{
	protected override LegId Empty => LegId.Empty;

	protected override LegId New() => LegId.New();

	protected override LegId From(Guid value) => LegId.From(value);

	protected override LegId Parse(string raw) => LegId.Parse(raw);

	protected override bool TryParse(string? raw, out LegId id) => LegId.TryParse(raw, out id);

	protected override Guid GetValue(LegId id) => id.Value;

	protected override bool IsEmpty(LegId id) => id.IsEmpty;
}

