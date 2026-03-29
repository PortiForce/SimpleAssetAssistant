using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portiforce.SAA.Infrastructure.EF.Migrations
{
	/// <inheritdoc />
	public partial class AddBlockFutureInvitesToTenantInvite : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.AddColumn<bool>(
				name: "BlockFutureInvites",
				schema: "pf",
				table: "Invites",
				type: "bit",
				nullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropColumn(
				name: "BlockFutureInvites",
				schema: "pf",
				table: "Invites");
		}
	}
}
