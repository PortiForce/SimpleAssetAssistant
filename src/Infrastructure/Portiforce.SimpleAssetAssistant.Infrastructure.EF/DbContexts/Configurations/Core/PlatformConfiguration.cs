using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SimpleAssetAssistant.Core.Assets.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Converters;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts.Configurations.Core;

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
			.HasConversion<int>();

		builder.Property(x => x.State)
			.IsRequired()
			.HasConversion<int>();

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

