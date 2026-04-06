using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Portiforce.SAA.Infrastructure.EF.Converters;

public sealed class NullableStrongIdConverter<TStrongId> : ValueConverter<TStrongId?, Guid?>
	where TStrongId : struct
{
	public NullableStrongIdConverter(Func<TStrongId, Guid> toGuid, Func<Guid, TStrongId> fromGuid)
		: base(
			id => id.HasValue ? toGuid(id.Value) : null,
			guid => guid.HasValue ? fromGuid(guid.Value) : null)
	{
	}
}