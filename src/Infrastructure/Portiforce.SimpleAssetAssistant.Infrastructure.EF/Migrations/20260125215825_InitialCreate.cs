using System;

using Microsoft.EntityFrameworkCore.Migrations;
using Povrtiforce.SimpleAssetAssistant.Infrastructure.EF.Configuration;

#nullable disable

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.EnsureSchema(
				name: DbConstants.Domain.Entities.DefaultSchemaName);

			migrationBuilder.CreateTable(
				name: "Asset",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
					Kind = table.Column<int>(type: "int", nullable: false),
					Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					NativeDecimals = table.Column<byte>(type: "tinyint", nullable: false),
					State = table.Column<int>(type: "int", nullable: false),
					RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Asset", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Platform",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
					Kind = table.Column<int>(type: "int", nullable: false),
					State = table.Column<int>(type: "int", nullable: false),
					RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Platform", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Tenant",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
					BrandName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					DomainPrefix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					State = table.Column<int>(type: "int", nullable: false),
					Plan = table.Column<int>(type: "int", nullable: false),
					RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
					Defaults_DefaultCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
					Defaults_DefaultLocale = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
					Defaults_DefaultTimeZone = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
					Import_MaxFileSizeMb = table.Column<int>(type: "int", nullable: false),
					Import_MaxRows = table.Column<int>(type: "int", nullable: false),
					Import_RequireReview = table.Column<bool>(type: "bit", nullable: false),
					Retention_DeletedDays = table.Column<int>(type: "int", nullable: false),
					Security_EnforceTwoFactor = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Tenant", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Account",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Alias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					Role = table.Column<byte>(type: "tinyint", nullable: false),
					State = table.Column<byte>(type: "tinyint", nullable: false),
					Tier = table.Column<byte>(type: "tinyint", nullable: false),
					RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
					ContactBackupEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					ContactEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					ContactPhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
					Settings_DefaultFiatCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
					Settings_Locale = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
					Settings_TimeZone = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
					TwoFactorPreferred = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Account", x => x.Id);
					table.ForeignKey(
						name: "FK_Account_Tenant_TenantId",
						column: x => x.TenantId,
						principalSchema: DbConstants.Domain.Entities.DefaultSchemaName,
						principalTable: "Tenant",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "TenantRestrictedAsset",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				columns: table => new
				{
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_TenantRestrictedAsset", x => new { x.TenantId, x.AssetId });
					table.ForeignKey(
						name: "FK_TenantRestrictedAsset_Tenant_TenantId",
						column: x => x.TenantId,
						principalSchema: DbConstants.Domain.Entities.DefaultSchemaName,
						principalTable: "Tenant",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "TenantRestrictedPlatform",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				columns: table => new
				{
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					PlatformId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_TenantRestrictedPlatform", x => new { x.TenantId, x.PlatformId });
					table.ForeignKey(
						name: "FK_TenantRestrictedPlatform_Tenant_TenantId",
						column: x => x.TenantId,
						principalSchema: DbConstants.Domain.Entities.DefaultSchemaName,
						principalTable: "Tenant",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "PlatformAccount",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					PlatformId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					State = table.Column<int>(type: "int", nullable: false),
					ExternalAccountId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					ExternalUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_PlatformAccount", x => x.Id);
					table.ForeignKey(
						name: "FK_PlatformAccount_Account_AccountId",
						column: x => x.AccountId,
						principalSchema: DbConstants.Domain.Entities.DefaultSchemaName,
						principalTable: "Account",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_PlatformAccount_Platform_PlatformId",
						column: x => x.PlatformId,
						principalSchema: DbConstants.Domain.Entities.DefaultSchemaName,
						principalTable: "Platform",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_PlatformAccount_Tenant_TenantId",
						column: x => x.TenantId,
						principalSchema: DbConstants.Domain.Entities.DefaultSchemaName,
						principalTable: "Tenant",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "Activity",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					PlatformAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Kind = table.Column<int>(type: "int", nullable: false),
					OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					ExternalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					Fingerprint = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					ExternalNotes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
					ExternalSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Reason = table.Column<byte>(type: "tinyint", nullable: true),
					ExchangeType = table.Column<byte>(type: "tinyint", nullable: true),
					CompletionType = table.Column<byte>(type: "tinyint", nullable: true),
					ServiceType = table.Column<byte>(type: "tinyint", nullable: true),
					ExecutionType = table.Column<byte>(type: "tinyint", nullable: true),
					MarketKind = table.Column<byte>(type: "tinyint", nullable: true),
					Futures_InstrumentKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					Futures_ContractKind = table.Column<int>(type: "int", nullable: true),
					Futures_BaseAssetCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
					Futures_QuoteAssetCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
					Futures_PositionEffect = table.Column<int>(type: "int", nullable: true),
					TransferKind = table.Column<byte>(type: "tinyint", nullable: true),
					Direction = table.Column<byte>(type: "tinyint", nullable: true),
					Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					Counterparty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Activity", x => x.Id);
					table.ForeignKey(
						name: "FK_Activity_PlatformAccount_PlatformAccountId",
						column: x => x.PlatformAccountId,
						principalSchema: DbConstants.Domain.Entities.DefaultSchemaName,
						principalTable: "PlatformAccount",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "ActivityLeg",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Amount = table.Column<decimal>(type: "decimal(38,18)", nullable: false),
					Direction = table.Column<int>(type: "int", nullable: false),
					Role = table.Column<int>(type: "int", nullable: false),
					Allocation = table.Column<int>(type: "int", nullable: false),
					InstrumentKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ActivityLeg", x => x.Id);
					table.ForeignKey(
						name: "FK_ActivityLeg_Activity_ActivityId",
						column: x => x.ActivityId,
						principalSchema: DbConstants.Domain.Entities.DefaultSchemaName,
						principalTable: "Activity",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ActivityLeg_Asset_AssetId",
						column: x => x.AssetId,
						principalSchema: DbConstants.Domain.Entities.DefaultSchemaName,
						principalTable: "Asset",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Account_State",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "Account",
				column: "State");

			migrationBuilder.CreateIndex(
				name: "IX_Account_TenantId",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "Account",
				column: "TenantId");

			migrationBuilder.CreateIndex(
				name: "IX_Activity_PlatformAccount_OccurredAt_Id",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "Activity",
				columns: new[] { "PlatformAccountId", "OccurredAt", "Id" });

			migrationBuilder.CreateIndex(
				name: "IX_Leg_ActivityId",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "ActivityLeg",
				column: "ActivityId");

			migrationBuilder.CreateIndex(
				name: "IX_Leg_AssetId",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "ActivityLeg",
				column: "AssetId");

			migrationBuilder.CreateIndex(
				name: "IX_Asset_State",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "Asset",
				column: "State");

			migrationBuilder.CreateIndex(
				name: "UX_Asset_Code_Kind",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "Asset",
				columns: new[] { "Code", "Kind" },
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Platform_State",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "Platform",
				column: "State");

			migrationBuilder.CreateIndex(
				name: "UX_Platform_Code",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "Platform",
				column: "Code",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "UX_Platform_Name",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "Platform",
				column: "Name",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_PlatformAccount_AccountId",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "PlatformAccount",
				column: "AccountId");

			migrationBuilder.CreateIndex(
				name: "IX_PlatformAccount_PlatformId",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "PlatformAccount",
				column: "PlatformId");

			migrationBuilder.CreateIndex(
				name: "IX_PlatformAccount_Tenant_Account",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "PlatformAccount",
				columns: new[] { "TenantId", "AccountId" });

			migrationBuilder.CreateIndex(
				name: "IX_PlatformAccount_Tenant_Platform",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "PlatformAccount",
				columns: new[] { "TenantId", "PlatformId" });

			migrationBuilder.CreateIndex(
				name: "UX_PlatformAccount_Tenant_Account_Platform",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "PlatformAccount",
				columns: new[] { "TenantId", "AccountId", "PlatformId" },
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Tenant_State",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "Tenant",
				column: "State");

			migrationBuilder.CreateIndex(
				name: "UX_Tenant_Code",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "Tenant",
				column: "Code",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "UX_Tenant_DomainPrefix",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "Tenant",
				column: "DomainPrefix",
				unique: true,
				filter: "[DomainPrefix] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_TenantRestrictedAsset_TenantId",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "TenantRestrictedAsset",
				column: "TenantId");

			migrationBuilder.CreateIndex(
				name: "IX_TenantRestrictedPlatform_TenantId",
				schema: DbConstants.Domain.Entities.DefaultSchemaName,
				table: "TenantRestrictedPlatform",
				column: "TenantId");

			// manually added custom indexes
			migrationBuilder.CreateIndex(
				name: "UX_Account_Tenant_ContactEmail",
				schema: DbConstants.Domain.Entities.CoreSchema.SchemaName,
				table: DbConstants.Domain.Entities.CoreSchema.AccountTableName,
				columns: new[] { "TenantId", "ContactEmail" },
				unique: true);

			migrationBuilder.CreateIndex(
				name: "UX_Activity_ExternalId",
				schema: DbConstants.Domain.Entities.LedgerSchema.SchemaName,
				table: DbConstants.Domain.Entities.LedgerSchema.ActivityTableName,
				columns: new[] { "TenantId", "PlatformAccountId", "Kind", "ExternalId" },
				unique: true,
				filter: "[ExternalId] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "UX_Activity_Fingerprint",
				schema: DbConstants.Domain.Entities.LedgerSchema.SchemaName,
				table: DbConstants.Domain.Entities.LedgerSchema.ActivityTableName,
				columns: new[] { "TenantId", "PlatformAccountId", "Kind", "Fingerprint" },
				unique: true,
				filter: "[Fingerprint] IS NOT NULL");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "ActivityLeg",
				schema: DbConstants.Domain.Entities.DefaultSchemaName);

			migrationBuilder.DropTable(
				name: "TenantRestrictedAsset",
				schema: DbConstants.Domain.Entities.DefaultSchemaName);

			migrationBuilder.DropTable(
				name: "TenantRestrictedPlatform",
				schema: DbConstants.Domain.Entities.DefaultSchemaName);

			migrationBuilder.DropTable(
				name: "Activity",
				schema: DbConstants.Domain.Entities.DefaultSchemaName);

			migrationBuilder.DropTable(
				name: "Asset",
				schema: DbConstants.Domain.Entities.DefaultSchemaName);

			migrationBuilder.DropTable(
				name: "PlatformAccount",
				schema: DbConstants.Domain.Entities.DefaultSchemaName);

			migrationBuilder.DropTable(
				name: "Account",
				schema: DbConstants.Domain.Entities.DefaultSchemaName);

			migrationBuilder.DropTable(
				name: "Platform",
				schema: DbConstants.Domain.Entities.DefaultSchemaName);

			migrationBuilder.DropTable(
				name: "Tenant",
				schema: DbConstants.Domain.Entities.DefaultSchemaName);
		}
	}
}
