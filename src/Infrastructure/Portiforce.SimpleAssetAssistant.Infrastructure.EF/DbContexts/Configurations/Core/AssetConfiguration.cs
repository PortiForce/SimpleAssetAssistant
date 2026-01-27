
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SimpleAssetAssistant.Core.Assets.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Core.StaticResources;
using Portiforce.SimpleAssetAssistant.Infrastructure.EF.Converters;

using Povrtiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DbContexts.Configurations.Core;

public sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
	public void Configure(EntityTypeBuilder<Asset> builder)
	{
		builder.ToTable(DbConstants.Domain.Entities.CoreSchema.AssetTableName,
			schema: DbConstants.Domain.Entities.CoreSchema.SchemaName);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<AssetId>(x => x.Value, AssetId.From));

		// AssetCode is a VO: ensure you have converter
		builder.Property(x => x.Code)
			.HasConversion(
				v => v.Value,
				v => AssetCode.Create(v)
			)
			.HasMaxLength(EntityConstraints.Domain.Asset.CodeMaxLength)
			.IsRequired();

		builder.Property(x => x.Name)
			.HasMaxLength(EntityConstraints.Domain.Asset.NameMaxLength)
			.IsRequired();

		builder.Property(x => x.NativeDecimals)
			.IsRequired();

		builder.Property(x => x.Kind)
			.IsRequired()
			.HasConversion<int>();

		builder.Property(x => x.State)
			.IsRequired()
			.HasConversion<int>();

		// RowVersion: optional but safe for catalog edits
		builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		builder.HasIndex(x => new { x.Code, x.Kind })
			.IsUnique()
			.HasDatabaseName("UX_Asset_Code_Kind");

		builder.HasIndex(x => x.State)
			.HasDatabaseName("IX_Asset_State");
	}
}
