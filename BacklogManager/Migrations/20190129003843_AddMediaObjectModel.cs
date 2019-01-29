﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BacklogManager.Migrations
{
    public partial class AddMediaObjectModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaObjects",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    MediaType = table.Column<int>(nullable: false),
                    DatabaseSource = table.Column<int>(nullable: false),
                    Completed = table.Column<bool>(nullable: false),
                    Started = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    UpdateCount = table.Column<int>(nullable: false),
                    DateAdded = table.Column<DateTime>(type: "date", nullable: false),
                    RecommendSource = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaObjects", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaObjects");
        }
    }
}
