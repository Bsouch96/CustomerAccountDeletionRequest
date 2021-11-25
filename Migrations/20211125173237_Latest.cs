using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerAccountDeletionRequest.Migrations
{
    public partial class Latest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "_deletionRequestContext",
                columns: table => new
                {
                    DeletionRequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    DeletionReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateRequested = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateApproved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: false),
                    DeletionRequestStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__deletionRequestContext", x => x.DeletionRequestID);
                });

            migrationBuilder.InsertData(
                table: "_deletionRequestContext",
                columns: new[] { "DeletionRequestID", "CustomerID", "DateApproved", "DateRequested", "DeletionReason", "DeletionRequestStatus", "StaffID" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2010, 10, 1, 8, 5, 3, 0, DateTimeKind.Unspecified), "Terrible Site.", 1, 1 },
                    { 2, 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2012, 1, 2, 10, 3, 45, 0, DateTimeKind.Unspecified), "Prefer Amazon.", 1, 1 },
                    { 3, 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2013, 2, 3, 12, 2, 40, 0, DateTimeKind.Unspecified), "Too many clicks.", 1, 2 },
                    { 4, 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2014, 3, 4, 14, 1, 35, 0, DateTimeKind.Unspecified), "Scammed into signing up.", 1, 3 },
                    { 5, 5, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2007, 4, 5, 16, 50, 30, 0, DateTimeKind.Unspecified), "If Wish was built by students...", 1, 4 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "_deletionRequestContext");
        }
    }
}
