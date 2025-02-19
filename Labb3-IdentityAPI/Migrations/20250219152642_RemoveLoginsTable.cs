﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Labb3_IdentityAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLoginsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Logins");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
        name: "Logins",
        columns: table => new
        {
            Id = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Logins", x => x.Id);
        });
        }
    }
}
