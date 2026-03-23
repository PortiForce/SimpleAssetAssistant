using Portiforce.SAA.Core.Extensions;

namespace Portiforce.SAA.Core.Primitives.Ids;

public readonly record struct ExternalIdentityId(Guid Value) : IEquatable<ExternalIdentityId>
{
	public static ExternalIdentityId New() => new(GuidExtensions.New());
	public static ExternalIdentityId Empty => new(Guid.Empty);
	public static ExternalIdentityId From(Guid value) => new(value);
	public bool IsEmpty => Value == Guid.Empty;
	public override string ToString() => GuidExtensions.ToString(Value);

	public static ExternalIdentityId Parse(string raw)
		=> GuidIdParser.Parse(raw, From, nameof(ExternalIdentityId));

	public static bool TryParse(string? raw, out ExternalIdentityId id)
		=> GuidIdParser.TryParse(raw, From, Empty, out id);
}
