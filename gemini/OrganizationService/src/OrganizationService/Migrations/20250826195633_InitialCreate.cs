using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganizationService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "organization_nodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Lft = table.Column<int>(type: "integer", nullable: false),
                    Rgt = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_nodes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "organization_nodes",
                columns: new[] { "Id", "Description", "Level", "Lft", "Name", "ParentId", "Rgt" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6"), "The root of the organization", 0, 1, "ROOT", null, 2 });

            migrationBuilder.CreateIndex(
                name: "IX_organization_nodes_Lft",
                table: "organization_nodes",
                column: "Lft");

            migrationBuilder.CreateIndex(
                name: "IX_organization_nodes_Lft_Rgt",
                table: "organization_nodes",
                columns: new[] { "Lft", "Rgt" });

            migrationBuilder.CreateIndex(
                name: "IX_organization_nodes_Rgt",
                table: "organization_nodes",
                column: "Rgt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "organization_nodes");
        }
    }
}
