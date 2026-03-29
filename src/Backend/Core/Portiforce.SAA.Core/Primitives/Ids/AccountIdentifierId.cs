using Portiforce.SAA.Core.Extensions;
using Portiforce.SAA.Core.Interfaces;
using Portiforce.SAA.Core.Parsers;

namespace Portiforce.SAA.Core.Primitives.Ids;

public readonly record struct AccountIdentifierId(Guid Value) : IGuidId<AccountIdentifierId>
{
	public static AccountIdentifierId New() => new(GuidExtensions.New());

	public static AccountIdentifierId Empty => new(Guid.Empty);

	public static AccountIdentifierId From(Guid value) => new(value);

	public bool IsEmpty => this.Value == Guid.Empty;

	public static AccountIdentifierId Parse(string raw)
		=> GuidIdParser.Parse(raw, From, nameof(AccountIdentifierId));

	public static bool TryParse(string? raw, out AccountIdentifierId id)
		=> GuidIdParser.TryParse(raw, From, Empty, out id);

	public override string ToString() => GuidExtensions.ToString(this.Value);
}