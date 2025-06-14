using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordBotTFT.DAL.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddedInventoryAndGold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "XP",
                table: "Profiles",
                newName: "Xp");

            migrationBuilder.RenameColumn(
                name: "DiscordID",
                table: "Profiles",
                newName: "DiscordId");

            migrationBuilder.AddColumn<int>(
                name: "Gold",
                table: "Profiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProfileItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileItems_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileItems_ItemId",
                table: "ProfileItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileItems_ProfileId",
                table: "ProfileItems",
                column: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfileItems");

            migrationBuilder.DropColumn(
                name: "Gold",
                table: "Profiles");

            migrationBuilder.RenameColumn(
                name: "Xp",
                table: "Profiles",
                newName: "XP");

            migrationBuilder.RenameColumn(
                name: "DiscordId",
                table: "Profiles",
                newName: "DiscordID");
        }
    }
}
