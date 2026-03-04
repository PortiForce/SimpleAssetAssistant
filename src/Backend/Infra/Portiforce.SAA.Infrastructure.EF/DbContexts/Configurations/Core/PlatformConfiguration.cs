using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portiforce.SAA.Core.Assets.Models;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Core;

public sealed class PlatformConfiguration : IEntityTypeConfiguration<Platform>
{
	public void Configure(EntityTypeBuilder<Platform> builder)
	{
		builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.PlatformTableName,
			schema: DbConstants.Domain.Entities.CoreSchema.SchemaName);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<PlatformId>(x => x.Value, PlatformId.From));

		builder.Property(x => x.Code)
			.HasMaxLength(EntityConstraints.CommonSettings.CodeMaxLength)
			.IsRequired();

		builder.Property(x => x.Name)
			.HasMaxLength(EntityConstraints.Domain.Platform.NameMaxLength)
			.IsRequired();

		builder.Property(x => x.Kind)
			.IsRequired()
			.HasConversion<byte>();

		builder.Property(x => x.State)
			.IsRequired()
			.HasConversion<byte>();

		builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		builder.HasIndex(x => x.Code)
			.IsUnique()
			.HasDatabaseName("UX_Platform_Code");

		builder.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UX_Platform_Name");

		builder.HasIndex(x => x.State)
			.HasDatabaseName("IX_Platform_State");
	}
}

