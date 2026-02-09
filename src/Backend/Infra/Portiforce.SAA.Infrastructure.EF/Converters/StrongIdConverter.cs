using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Portiforce.SAA.Infrastructure.EF.Converters;

public sealed class StrongIdConverter<TId> : ValueConverter<TId, Guid>
	where TId : struct
{
	public StrongIdConverter(Func<TId, Guid> toGuid, Func<Guid, TId> fromGuid)
		: base(id => toGuid(id), value => fromGuid(value))
	{
	}
}
