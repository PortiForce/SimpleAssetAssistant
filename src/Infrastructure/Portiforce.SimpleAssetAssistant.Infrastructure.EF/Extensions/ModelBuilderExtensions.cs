using Microsoft.EntityFrameworkCore;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Extensions;

public static class ModelBuilderExtensions
{
	public static void ConfigureRowVersion(this ModelBuilder modelBuilder)
	{
		//foreach (var entityType in modelBuilder.Model.GetEntityTypes())
		//{
		//	// BaseMonitoredEntity configurations
		//	if (typeof(BaseMonitoredEntity).IsAssignableFrom(entityType.ClrType))
		//	{
		//		modelBuilder.Entity(entityType.ClrType)
		//			.Property<byte[]>("RowVersion")
		//			.IsRowVersion()
		//			.IsConcurrencyToken()
		//			.ValueGeneratedOnAddOrUpdate()
		//			.HasColumnType("rowversion");
		//	}

		//	// BaseEntity configurations
		//	if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
		//	{
		//		// Apply configuration to the UpdatedOn property
		//		modelBuilder.Entity(entityType.ClrType)
		//			.Property<DateTime>("UpdatedOn")
		//			.HasColumnType(DbConstants.CommonSettings.DateTimeSecOnlyAccuracyFormat)
		//			.IsRequired();
		//	}
		//}
	}
}
