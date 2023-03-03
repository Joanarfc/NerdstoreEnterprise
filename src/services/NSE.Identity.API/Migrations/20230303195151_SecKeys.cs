using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NSE.Identity.API.Migrations
{
    public partial class SecKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecurityKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    KeyId = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Parameters = table.Column<string>(nullable: true),
                    IsRevoked = table.Column<bool>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ExpiredAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityKeys", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecurityKeys");
        }
    }
}
