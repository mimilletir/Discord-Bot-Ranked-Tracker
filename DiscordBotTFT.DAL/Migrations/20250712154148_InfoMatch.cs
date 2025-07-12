using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordBotTFT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InfoMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "currentlyInGame",
                table: "Profiles",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "region",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "server",
                table: "Profiles",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "MatchInfoDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchInfoDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    queueType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rank = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    leaguePoints = table.Column<int>(type: "int", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rank_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MatchInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    server = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    puuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gameId = table.Column<long>(type: "bigint", nullable: false),
                    gameMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    championId = table.Column<long>(type: "bigint", nullable: false),
                    infoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchInfo_MatchInfoDetail_infoId",
                        column: x => x.infoId,
                        principalTable: "MatchInfoDetail",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Participant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    puuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    championId = table.Column<long>(type: "bigint", nullable: false),
                    kills = table.Column<int>(type: "int", nullable: true),
                    deaths = table.Column<int>(type: "int", nullable: true),
                    assists = table.Column<int>(type: "int", nullable: true),
                    win = table.Column<bool>(type: "bit", nullable: true),
                    MatchInfoDetailId = table.Column<int>(type: "int", nullable: true),
                    MatchInfoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participant_MatchInfoDetail_MatchInfoDetailId",
                        column: x => x.MatchInfoDetailId,
                        principalTable: "MatchInfoDetail",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Participant_MatchInfo_MatchInfoId",
                        column: x => x.MatchInfoId,
                        principalTable: "MatchInfo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchInfo_infoId",
                table: "MatchInfo",
                column: "infoId");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_MatchInfoDetailId",
                table: "Participant",
                column: "MatchInfoDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_MatchInfoId",
                table: "Participant",
                column: "MatchInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Rank_ProfileId",
                table: "Rank",
                column: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Participant");

            migrationBuilder.DropTable(
                name: "Rank");

            migrationBuilder.DropTable(
                name: "MatchInfo");

            migrationBuilder.DropTable(
                name: "MatchInfoDetail");

            migrationBuilder.DropColumn(
                name: "currentlyInGame",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "region",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "server",
                table: "Profiles");
        }
    }
}
