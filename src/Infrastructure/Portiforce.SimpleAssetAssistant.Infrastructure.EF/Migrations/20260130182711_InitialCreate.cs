using System;
using Microsoft.EntityFrameworkCore.Migrations;

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
                name: "core");

            migrationBuilder.EnsureSchema(
                name: "ledger");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "Assets",
                schema: "core",
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
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                schema: "core",
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
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                schema: "core",
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
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                schema: "core",
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
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "core",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantRestrictedAssets",
                schema: "core",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRestrictedAssets", x => new { x.TenantId, x.AssetId });
                    table.ForeignKey(
                        name: "FK_TenantRestrictedAssets_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "core",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantRestrictedPlatforms",
                schema: "core",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlatformId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRestrictedPlatforms", x => new { x.TenantId, x.PlatformId });
                    table.ForeignKey(
                        name: "FK_TenantRestrictedPlatforms_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "core",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalIdentities",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    ProviderSubject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalIdentities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalIdentities_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "core",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PassKeyCredentials",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CredentialId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PublicKey = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UserHandle = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SignatureCounter = table.Column<long>(type: "bigint", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassKeyCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassKeyCredentials_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "core",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlatformAccounts",
                schema: "core",
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
                    table.PrimaryKey("PK_PlatformAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformAccounts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "core",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlatformAccounts_Platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalSchema: "core",
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlatformAccounts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "core",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                schema: "ledger",
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
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_PlatformAccounts_PlatformAccountId",
                        column: x => x.PlatformAccountId,
                        principalSchema: "core",
                        principalTable: "PlatformAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLegs",
                schema: "ledger",
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
                    table.PrimaryKey("PK_ActivityLegs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLegs_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "ledger",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityLegs_Assets_AssetId",
                        column: x => x.AssetId,
                        principalSchema: "core",
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_State",
                schema: "core",
                table: "Accounts",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_Account_TenantId",
                schema: "core",
                table: "Accounts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_PlatformAccount_OccurredAt_Id",
                schema: "ledger",
                table: "Activities",
                columns: new[] { "PlatformAccountId", "OccurredAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Leg_ActivityId",
                schema: "ledger",
                table: "ActivityLegs",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Leg_AssetId",
                schema: "ledger",
                table: "ActivityLegs",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_State",
                schema: "core",
                table: "Assets",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "UX_Asset_Code_Kind",
                schema: "core",
                table: "Assets",
                columns: new[] { "Code", "Kind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIdentities_AccountId",
                schema: "auth",
                table: "ExternalIdentities",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "UX_ExternalIdentity_Provider_ExternalId",
                schema: "auth",
                table: "ExternalIdentities",
                columns: new[] { "Provider", "ProviderSubject" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PassKeyCredentials_AccountId",
                schema: "auth",
                table: "PassKeyCredentials",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "UX_PasskeyCredential_CredentialId",
                schema: "auth",
                table: "PassKeyCredentials",
                column: "CredentialId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformAccount_Tenant_Account",
                schema: "core",
                table: "PlatformAccounts",
                columns: new[] { "TenantId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlatformAccount_Tenant_Platform",
                schema: "core",
                table: "PlatformAccounts",
                columns: new[] { "TenantId", "PlatformId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlatformAccounts_AccountId",
                schema: "core",
                table: "PlatformAccounts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformAccounts_PlatformId",
                schema: "core",
                table: "PlatformAccounts",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "UX_PlatformAccount_Tenant_Account_Platform",
                schema: "core",
                table: "PlatformAccounts",
                columns: new[] { "TenantId", "AccountId", "PlatformId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Platform_State",
                schema: "core",
                table: "Platforms",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "UX_Platform_Code",
                schema: "core",
                table: "Platforms",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Platform_Name",
                schema: "core",
                table: "Platforms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantRestrictedAsset_TenantId",
                schema: "core",
                table: "TenantRestrictedAssets",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRestrictedPlatform_TenantId",
                schema: "core",
                table: "TenantRestrictedPlatforms",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_State",
                schema: "core",
                table: "Tenants",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "UX_Tenant_Code",
                schema: "core",
                table: "Tenants",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Tenant_DomainPrefix",
                schema: "core",
                table: "Tenants",
                column: "DomainPrefix",
                unique: true,
                filter: "[DomainPrefix] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLegs",
                schema: "ledger");

            migrationBuilder.DropTable(
                name: "ExternalIdentities",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "PassKeyCredentials",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "TenantRestrictedAssets",
                schema: "core");

            migrationBuilder.DropTable(
                name: "TenantRestrictedPlatforms",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Activities",
                schema: "ledger");

            migrationBuilder.DropTable(
                name: "Assets",
                schema: "core");

            migrationBuilder.DropTable(
                name: "PlatformAccounts",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Accounts",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Platforms",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Tenants",
                schema: "core");
        }
    }
}
