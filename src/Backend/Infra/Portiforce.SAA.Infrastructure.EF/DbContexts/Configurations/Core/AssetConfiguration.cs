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
		builder.ToTable(
			DbConstants.Domain.Entities.CoreSchema.AssetTableName,
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
			.HasConversion<byte>();

		builder.Property(x => x.State)
			.IsRequired()
			.HasConversion<byte>();

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
