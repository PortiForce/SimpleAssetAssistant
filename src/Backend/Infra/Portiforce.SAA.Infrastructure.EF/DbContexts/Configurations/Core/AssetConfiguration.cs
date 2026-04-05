using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Portiforce.SAA.Core.Assets.Models;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Core.StaticResources;
using Portiforce.SAA.Infrastructure.EF.Configuration;
using Portiforce.SAA.Infrastructure.EF.Converters;

namespace Portiforce.SAA.Infrastructure.EF.DbContexts.Configurations.Core;

public sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
	public void Configure(EntityTypeBuilder<Asset> builder)
	{
		_ = builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.AssetTableName,
			DbConstants.Domain.Entities.CoreSchema.SchemaName);

		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.ValueGeneratedNever()
			.HasConversion(new StrongIdConverter<AssetId>(x => x.Value, AssetId.From));

		// AssetCode is a VO: ensure you have converter
		_ = builder.Property(x => x.Code)
			.HasConversion(
				v => v.Value,
				v => AssetCode.Create(v))
			.HasMaxLength(EntityConstraints.Domain.Asset.CodeMaxLength)
			.IsRequired();

		_ = builder.Property(x => x.Name)
			.HasMaxLength(EntityConstraints.Domain.Asset.NameMaxLength)
			.IsRequired();

		_ = builder.Property(x => x.NativeDecimals)
			.IsRequired();

		_ = builder.Property(x => x.Kind)
			.IsRequired()
			.HasConversion<byte>();

		_ = builder.Property(x => x.State)
			.IsRequired()
			.HasConversion<byte>();

		// RowVersion: optional but safe for catalog edits
		_ = builder.Property<byte[]>("RowVersion")
			.IsRowVersion()
			.IsConcurrencyToken();

		_ = builder.HasIndex(x => new { x.Code, x.Kind })
			.IsUnique()
			.HasDatabaseName("UX_Asset_Code_Kind");

		_ = builder.HasIndex(x => x.State)
			.HasDatabaseName("IX_Asset_State");
	}
}