using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DawnQuant.AShare.Api.Migrations.UserProfileDb
{
    public partial class InitUserProfileDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExclusionStocks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TSCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Industry = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortNum = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExclusionStocks", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SelfSelectStockCategorys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SortNum = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Desc = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsGroupByIndustry = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfSelectStockCategorys", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockStrategyCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SortNum = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Desc = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockStrategyCategories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StrategyScheduledTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    StrategyIds = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OutputStockCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Desc = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortNum = table.Column<int>(type: "int", nullable: false),
                    LatestExecuteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsJoinServerScheduleTask = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategyScheduledTasks", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SelfSelectStocks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    TSCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Industry = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortNum = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfSelectStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelfSelectStocks_SelfSelectStockCategorys_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "SelfSelectStockCategorys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockStrategies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Desc = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortNum = table.Column<int>(type: "int", nullable: false),
                    StockStragyContent = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockStrategies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockStrategies_StockStrategyCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "StockStrategyCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ExclusionStocks_Id",
                table: "ExclusionStocks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExclusionStocks_UserId",
                table: "ExclusionStocks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SelfSelectStockCategorys_Id",
                table: "SelfSelectStockCategorys",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SelfSelectStockCategorys_UserId",
                table: "SelfSelectStockCategorys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SelfSelectStocks_CategoryId",
                table: "SelfSelectStocks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SelfSelectStocks_Id",
                table: "SelfSelectStocks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SelfSelectStocks_UserId",
                table: "SelfSelectStocks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockStrategies_CategoryId",
                table: "StockStrategies",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockStrategies_Id",
                table: "StockStrategies",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StockStrategies_UserId",
                table: "StockStrategies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockStrategyCategories_Id",
                table: "StockStrategyCategories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StockStrategyCategories_UserId",
                table: "StockStrategyCategories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StrategyScheduledTasks_Id",
                table: "StrategyScheduledTasks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StrategyScheduledTasks_UserId",
                table: "StrategyScheduledTasks",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExclusionStocks");

            migrationBuilder.DropTable(
                name: "SelfSelectStocks");

            migrationBuilder.DropTable(
                name: "StockStrategies");

            migrationBuilder.DropTable(
                name: "StrategyScheduledTasks");

            migrationBuilder.DropTable(
                name: "SelfSelectStockCategorys");

            migrationBuilder.DropTable(
                name: "StockStrategyCategories");
        }
    }
}
