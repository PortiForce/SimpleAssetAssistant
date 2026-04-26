using Microsoft.EntityFrameworkCore.Migrations;

using Portiforce.SAA.Infrastructure.EF.Configuration;

#nullable disable

namespace Portiforce.SAA.Infrastructure.EF.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.EnsureSchema(
				name: "auth");

			_ = migrationBuilder.EnsureSchema(
				name: "core");

			_ = migrationBuilder.EnsureSchema(
				name: "ledger");

			_ = migrationBuilder.EnsureSchema(
				name: "pf");

			_ = migrationBuilder.EnsureSchema(
				name: "inf");

			_ = migrationBuilder.CreateTable(
				name: "Assets",
				schema: "core",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
					Kind = table.Column<byte>(type: "tinyint", nullable: false),
					Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					NativeDecimals = table.Column<byte>(type: "tinyint", nullable: false),
					State = table.Column<byte>(type: "tinyint", nullable: false),
					RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_Assets", x => x.Id);
				});

			_ = migrationBuilder.CreateTable(
				name: "Platforms",
				schema: "core",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
					Kind = table.Column<byte>(type: "tinyint", nullable: false),
					State = table.Column<byte>(type: "tinyint", nullable: false),
					RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_Platforms", x => x.Id);
				});

			_ = migrationBuilder.CreateTable(
				name: "Tenants",
				schema: "core",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
					BrandName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					DomainPrefix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					State = table.Column<byte>(type: "tinyint", nullable: false),
					Plan = table.Column<byte>(type: "tinyint", nullable: false),
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
					_ = table.PrimaryKey("PK_Tenants", x => x.Id);
				});

			_ = migrationBuilder.CreateTable(
				name: "Accounts",
				schema: "core",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Alias = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
					Role = table.Column<byte>(type: "tinyint", nullable: false),
					State = table.Column<byte>(type: "tinyint", nullable: false),
					Tier = table.Column<byte>(type: "tinyint", nullable: false),
					RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
					ContactEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					ContactPhone = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
					Settings_DefaultFiatCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
					Settings_Locale = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
					Settings_TimeZone = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
					TwoFactorPreferred = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_Accounts", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_Accounts_Tenants_TenantId",
						column: x => x.TenantId,
						principalSchema: "core",
						principalTable: "Tenants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			_ = migrationBuilder.CreateTable(
				name: "OutboxMessages",
				schema: "inf",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Type = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
					PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
					State = table.Column<byte>(type: "tinyint", nullable: false),
					CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					PublishedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					ProcessedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					AttemptCount = table.Column<int>(type: "int", nullable: false),
					NextAttemptAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					LastError = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
					IdempotencyKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
					RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_OutboxMessages", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_OutboxMessages_Tenants_TenantId",
						column: x => x.TenantId,
						principalSchema: "core",
						principalTable: "Tenants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			_ = migrationBuilder.CreateTable(
				name: "TenantRestrictedAssets",
				schema: "core",
				columns: table => new
				{
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_TenantRestrictedAssets", x => new { x.TenantId, x.AssetId });
					_ = table.ForeignKey(
						name: "FK_TenantRestrictedAssets_Tenants_TenantId",
						column: x => x.TenantId,
						principalSchema: "core",
						principalTable: "Tenants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "TenantRestrictedPlatforms",
				schema: "core",
				columns: table => new
				{
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					PlatformId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_TenantRestrictedPlatforms", x => new { x.TenantId, x.PlatformId });
					_ = table.ForeignKey(
						name: "FK_TenantRestrictedPlatforms_Tenants_TenantId",
						column: x => x.TenantId,
						principalSchema: "core",
						principalTable: "Tenants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "AccountIdentifiers",
				schema: "auth",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Kind = table.Column<byte>(type: "tinyint", nullable: false),
					Value = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					IsVerified = table.Column<bool>(type: "bit", nullable: false),
					IsPrimary = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_AccountIdentifiers", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_AccountIdentifiers_Accounts_AccountId",
						column: x => x.AccountId,
						principalSchema: "core",
						principalTable: "Accounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_AccountIdentifiers_Tenants_TenantId",
						column: x => x.TenantId,
						principalSchema: "core",
						principalTable: "Tenants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "ExternalIdentities",
				schema: "auth",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Provider = table.Column<byte>(type: "tinyint", nullable: false),
					ProviderSubject = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					IsPrimary = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ExternalIdentities", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ExternalIdentities_Accounts_AccountId",
						column: x => x.AccountId,
						principalSchema: "core",
						principalTable: "Accounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_ExternalIdentities_Tenants_TenantId",
						column: x => x.TenantId,
						principalSchema: "core",
						principalTable: "Tenants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "Invites",
				schema: "pf",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					InvitedByAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					IntendedRole = table.Column<int>(type: "int", nullable: false),
					IntendedTier = table.Column<int>(type: "int", nullable: false),
					TokenHash = table.Column<byte[]>(type: "varbinary(32)", fixedLength: true, maxLength: 32, nullable: false),
					State = table.Column<int>(type: "int", nullable: false),
					CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					ExpiresAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					SentAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					SendCount = table.Column<int>(type: "int", nullable: false),
					UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					AcceptedAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
					RevokedByAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
					Alias = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					BlockFutureInvites = table.Column<bool>(type: "bit", nullable: true),
					RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
					InviteTargetChannel = table.Column<byte>(type: "tinyint", nullable: false),
					InviteTargetKind = table.Column<byte>(type: "tinyint", nullable: false),
					InviteTargetLocale = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
					InviteTargetValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_Invites", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_Invites_Accounts_AcceptedAccountId",
						column: x => x.AcceptedAccountId,
						principalSchema: "core",
						principalTable: "Accounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					_ = table.ForeignKey(
						name: "FK_Invites_Accounts_InvitedByAccountId",
						column: x => x.InvitedByAccountId,
						principalSchema: "core",
						principalTable: "Accounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					_ = table.ForeignKey(
						name: "FK_Invites_Tenants_TenantId",
						column: x => x.TenantId,
						principalSchema: "core",
						principalTable: "Tenants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			_ = migrationBuilder.CreateTable(
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
					_ = table.PrimaryKey("PK_PassKeyCredentials", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_PassKeyCredentials_Accounts_AccountId",
						column: x => x.AccountId,
						principalSchema: "core",
						principalTable: "Accounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "PlatformAccounts",
				schema: "core",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					PlatformId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					State = table.Column<byte>(type: "tinyint", nullable: false),
					ExternalAccountId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					ExternalUserId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_PlatformAccounts", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_PlatformAccounts_Accounts_AccountId",
						column: x => x.AccountId,
						principalSchema: "core",
						principalTable: "Accounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					_ = table.ForeignKey(
						name: "FK_PlatformAccounts_Platforms_PlatformId",
						column: x => x.PlatformId,
						principalSchema: "core",
						principalTable: "Platforms",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					_ = table.ForeignKey(
						name: "FK_PlatformAccounts_Tenants_TenantId",
						column: x => x.TenantId,
						principalSchema: "core",
						principalTable: "Tenants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			_ = migrationBuilder.CreateTable(
				name: "SessionTokens",
				schema: "auth",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TokenHash = table.Column<byte[]>(type: "varbinary(32)", fixedLength: true, maxLength: 32, nullable: false),
					CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					CreatedByIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
					CreatedUserAgent = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					UserAgentFingerprint = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
					RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					RevokedByIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
					ReplacedByTokenHash = table.Column<byte[]>(type: "varbinary(32)", nullable: true),
					RevokedReason = table.Column<byte>(type: "tinyint", nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_SessionTokens", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_SessionTokens_Accounts_AccountId",
						column: x => x.AccountId,
						principalSchema: "core",
						principalTable: "Accounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_SessionTokens_Tenants_TenantId",
						column: x => x.TenantId,
						principalSchema: "core",
						principalTable: "Tenants",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "Activities",
				schema: "ledger",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					PlatformAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Kind = table.Column<byte>(type: "tinyint", nullable: false),
					OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					ExternalId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					Fingerprint = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					ExternalNotes = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					ExternalSource = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					Reason = table.Column<byte>(type: "tinyint", nullable: true),
					ExchangeType = table.Column<byte>(type: "tinyint", nullable: true),
					CompletionType = table.Column<byte>(type: "tinyint", nullable: true),
					ServiceType = table.Column<byte>(type: "tinyint", nullable: true),
					ExecutionType = table.Column<byte>(type: "tinyint", nullable: true),
					MarketKind = table.Column<byte>(type: "tinyint", nullable: true),
					Futures_InstrumentKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					Futures_ContractKind = table.Column<byte>(type: "tinyint", nullable: true),
					Futures_BaseAssetCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
					Futures_QuoteAssetCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
					Futures_PositionEffect = table.Column<byte>(type: "tinyint", nullable: true),
					TransferKind = table.Column<byte>(type: "tinyint", nullable: true),
					Direction = table.Column<byte>(type: "tinyint", nullable: true),
					Reference = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					Counterparty = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_Activities", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_Activities_PlatformAccounts_PlatformAccountId",
						column: x => x.PlatformAccountId,
						principalSchema: "core",
						principalTable: "PlatformAccounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			_ = migrationBuilder.CreateTable(
				name: "ActivityLegs",
				schema: "ledger",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Amount = table.Column<decimal>(type: "decimal(38,18)", nullable: false),
					Direction = table.Column<byte>(type: "tinyint", nullable: false),
					Role = table.Column<byte>(type: "tinyint", nullable: false),
					Allocation = table.Column<byte>(type: "tinyint", nullable: false),
					InstrumentKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ActivityLegs", x => x.Id);
					_ = table.ForeignKey(
						name: "FK_ActivityLegs_Activities_ActivityId",
						column: x => x.ActivityId,
						principalSchema: "ledger",
						principalTable: "Activities",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_ActivityLegs_Assets_AssetId",
						column: x => x.AssetId,
						principalSchema: "core",
						principalTable: "Assets",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			_ = migrationBuilder.CreateIndex(
				name: "IX_AccountIdentifiers_AccountId",
				schema: "auth",
				table: "AccountIdentifiers",
				column: "AccountId");

			_ = migrationBuilder.CreateIndex(
				name: "UX_AccountIdentifier_Tenant_Kind_Value",
				schema: "auth",
				table: "AccountIdentifiers",
				columns: new[] { "TenantId", "Kind", "Value" },
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_Account_State",
				schema: "core",
				table: "Accounts",
				column: "State");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Account_TenantId",
				schema: "core",
				table: "Accounts",
				column: "TenantId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Activity_PlatformAccount_OccurredAt_Id",
				schema: "ledger",
				table: "Activities",
				columns: new[] { "PlatformAccountId", "OccurredAt", "Id" });

			_ = migrationBuilder.CreateIndex(
				name: "IX_Leg_ActivityId",
				schema: "ledger",
				table: "ActivityLegs",
				column: "ActivityId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Leg_AssetId",
				schema: "ledger",
				table: "ActivityLegs",
				column: "AssetId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Asset_State",
				schema: "core",
				table: "Assets",
				column: "State");

			_ = migrationBuilder.CreateIndex(
				name: "UX_Asset_Code_Kind",
				schema: "core",
				table: "Assets",
				columns: new[] { "Code", "Kind" },
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_ExternalIdentities_AccountId",
				schema: "auth",
				table: "ExternalIdentities",
				column: "AccountId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_ExternalIdentities_TenantId",
				schema: "auth",
				table: "ExternalIdentities",
				column: "TenantId");

			_ = migrationBuilder.CreateIndex(
				name: "UX_ExternalIdentity_Provider_ExternalId",
				schema: "auth",
				table: "ExternalIdentities",
				columns: new[] { "Provider", "ProviderSubject" },
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_Invite_State",
				schema: "pf",
				table: "Invites",
				column: "State");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Invite_TenantId",
				schema: "pf",
				table: "Invites",
				column: "TenantId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Invite_TenantId_State_ExpiresAt",
				schema: "pf",
				table: "Invites",
				columns: new[] { "TenantId", "State", "ExpiresAtUtc" });

			_ = migrationBuilder.CreateIndex(
				name: "IX_Invites_AcceptedAccountId",
				schema: "pf",
				table: "Invites",
				column: "AcceptedAccountId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Invites_InvitedByAccountId",
				schema: "pf",
				table: "Invites",
				column: "InvitedByAccountId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Outbox_State_NextAttempt_Type",
				schema: "inf",
				table: "OutboxMessages",
				columns: new[] { "State", "NextAttemptAtUtc", "Type" });

			_ = migrationBuilder.CreateIndex(
				name: "IX_Outbox_TenantId",
				schema: "inf",
				table: "OutboxMessages",
				column: "TenantId");

			_ = migrationBuilder.CreateIndex(
				name: "UX_Outbox_IdempotencyKey",
				schema: "inf",
				table: "OutboxMessages",
				column: "IdempotencyKey",
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_PassKeyCredentials_AccountId",
				schema: "auth",
				table: "PassKeyCredentials",
				column: "AccountId");

			_ = migrationBuilder.CreateIndex(
				name: "UX_PasskeyCredential_CredentialId",
				schema: "auth",
				table: "PassKeyCredentials",
				column: "CredentialId",
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_PlatformAccount_Tenant_Account",
				schema: "core",
				table: "PlatformAccounts",
				columns: new[] { "TenantId", "AccountId" });

			_ = migrationBuilder.CreateIndex(
				name: "IX_PlatformAccount_Tenant_Platform",
				schema: "core",
				table: "PlatformAccounts",
				columns: new[] { "TenantId", "PlatformId" });

			_ = migrationBuilder.CreateIndex(
				name: "IX_PlatformAccounts_AccountId",
				schema: "core",
				table: "PlatformAccounts",
				column: "AccountId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_PlatformAccounts_PlatformId",
				schema: "core",
				table: "PlatformAccounts",
				column: "PlatformId");

			_ = migrationBuilder.CreateIndex(
				name: "UX_PlatformAccount_Tenant_Account_Platform",
				schema: "core",
				table: "PlatformAccounts",
				columns: new[] { "TenantId", "AccountId", "PlatformId" },
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_Platform_State",
				schema: "core",
				table: "Platforms",
				column: "State");

			_ = migrationBuilder.CreateIndex(
				name: "UX_Platform_Code",
				schema: "core",
				table: "Platforms",
				column: "Code",
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "UX_Platform_Name",
				schema: "core",
				table: "Platforms",
				column: "Name",
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_SessionTokens_AccountId",
				schema: "auth",
				table: "SessionTokens",
				column: "AccountId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_SessionTokens_SessionId",
				schema: "auth",
				table: "SessionTokens",
				column: "SessionId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_SessionTokens_TenantId",
				schema: "auth",
				table: "SessionTokens",
				column: "TenantId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_SessionTokens_TokenHash",
				schema: "auth",
				table: "SessionTokens",
				column: "TokenHash",
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_TenantRestrictedAsset_TenantId",
				schema: "core",
				table: "TenantRestrictedAssets",
				column: "TenantId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_TenantRestrictedPlatform_TenantId",
				schema: "core",
				table: "TenantRestrictedPlatforms",
				column: "TenantId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Tenant_State",
				schema: "core",
				table: "Tenants",
				column: "State");

			_ = migrationBuilder.CreateIndex(
				name: "UX_Tenant_Code",
				schema: "core",
				table: "Tenants",
				column: "Code",
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "UX_Tenant_DomainPrefix",
				schema: "core",
				table: "Tenants",
				column: "DomainPrefix",
				unique: true,
				filter: "[DomainPrefix] IS NOT NULL");

			_ = migrationBuilder.CreateIndex(
				name: "UX_Account_Tenant_ContactEmail",
				schema: DbConstants.Domain.Entities.CoreSchema.SchemaName,
				table: DbConstants.Domain.Entities.CoreSchema.AccountTableName,
				columns: new[] { "TenantId", "ContactEmail" },
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "UX_Invite_Tenant_InviteTarger",
				schema: DbConstants.Domain.Entities.DefaultSchema.SchemaName,
				table: DbConstants.Domain.Entities.DefaultSchema.InviteTableName,
				columns: new[] { "TenantId", "InviteTargetValue" },
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "UX_Activity_ExternalId",
				schema: DbConstants.Domain.Entities.LedgerSchema.SchemaName,
				table: DbConstants.Domain.Entities.LedgerSchema.ActivityTableName,
				columns: new[] { "TenantId", "PlatformAccountId", "Kind", "ExternalId" },
				unique: true,
				filter: "[ExternalId] IS NOT NULL");

			_ = migrationBuilder.CreateIndex(
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
			_ = migrationBuilder.DropTable(
				name: "AccountIdentifiers",
				schema: "auth");

			_ = migrationBuilder.DropTable(
				name: "ActivityLegs",
				schema: "ledger");

			_ = migrationBuilder.DropTable(
				name: "ExternalIdentities",
				schema: "auth");

			_ = migrationBuilder.DropTable(
				name: "Invites",
				schema: "pf");

			_ = migrationBuilder.DropTable(
				name: "OutboxMessages",
				schema: "inf");

			_ = migrationBuilder.DropTable(
				name: "PassKeyCredentials",
				schema: "auth");

			_ = migrationBuilder.DropTable(
				name: "SessionTokens",
				schema: "auth");

			_ = migrationBuilder.DropTable(
				name: "TenantRestrictedAssets",
				schema: "core");

			_ = migrationBuilder.DropTable(
				name: "TenantRestrictedPlatforms",
				schema: "core");

			_ = migrationBuilder.DropTable(
				name: "Activities",
				schema: "ledger");

			_ = migrationBuilder.DropTable(
				name: "Assets",
				schema: "core");

			_ = migrationBuilder.DropTable(
				name: "PlatformAccounts",
				schema: "core");

			_ = migrationBuilder.DropTable(
				name: "Accounts",
				schema: "core");

			_ = migrationBuilder.DropTable(
				name: "Platforms",
				schema: "core");

			_ = migrationBuilder.DropTable(
				name: "Tenants",
				schema: "core");
		}
	}
}
