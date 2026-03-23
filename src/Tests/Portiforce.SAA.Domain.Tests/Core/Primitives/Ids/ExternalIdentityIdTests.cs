using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Domain.Tests.Core.Primitives.Ids.Common;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives.Ids;

public sealed class ExternalIdentityIdTests : StronglyTypedGuidIdTests<ExternalIdentityId>
{
	protected override ExternalIdentityId Empty => ExternalIdentityId.Empty;

	protected override ExternalIdentityId New() => ExternalIdentityId.New();

	protected override ExternalIdentityId From(Guid value) => ExternalIdentityId.From(value);

	protected override ExternalIdentityId Parse(string raw) => ExternalIdentityId.Parse(raw);

	protected override bool TryParse(string? raw, out ExternalIdentityId id) => ExternalIdentityId.TryParse(raw, out id);

	protected override Guid GetValue(ExternalIdentityId id) => id.Value;

	protected override bool IsEmpty(ExternalIdentityId id) => id.IsEmpty;
}

