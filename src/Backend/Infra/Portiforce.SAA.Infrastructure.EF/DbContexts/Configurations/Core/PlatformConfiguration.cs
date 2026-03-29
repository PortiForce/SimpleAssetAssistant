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
		_ = builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.PlatformTableName,
			DbConstants.Domain.Entities.CoreSchema.SchemaName);

		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<PlatformId>(x => x.Value, PlatformId.From));

		_ = builder.Property(x => x.Code)
			.HasMaxLength(EntityConstraints.CommonSettings.CodeMaxLength)
			.IsRequired();

		_ = builder.Property(x => x.Name)
			.HasMaxLength(EntityConstraints.Domain.Platform.NameMaxLength)
			.IsRequired();

		_ = builder.Property(x => x.Kind)
			.IsRequired()
			.HasConversion<byte>();

		_ = builder.Property(x => x.State)
			.IsRequired()
			.HasConversion<byte>();

		_ = builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		_ = builder.HasIndex(x => x.Code)
			.IsUnique()
			.HasDatabaseName("UX_Platform_Code");

		_ = builder.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UX_Platform_Name");

		_ = builder.HasIndex(x => x.State)
			.HasDatabaseName("IX_Platform_State");
	}
}