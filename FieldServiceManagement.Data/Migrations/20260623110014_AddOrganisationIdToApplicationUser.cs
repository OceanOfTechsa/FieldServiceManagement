using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FieldServiceManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganisationIdToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "OrganisationSubscriptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CancellationNote",
                table: "OrganisationSubscriptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsTrial",
            //    table: "OrganisationSubscriptions",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "ShowComment",
            //    table: "AuditLogs",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.AddColumn<string>(
            //    name: "VisibleTo",
            //    table: "AuditLogs",
            //    type: "nvarchar(max)",
            //    nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            //migrationBuilder.CreateTable(
            //    name: "Contacts",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        Salutation = table.Column<int>(type: "int", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        ServiceAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //        BillingAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //        OrganisationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Contacts", x => x.Id);
            //    });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Contacts");

            //migrationBuilder.DropColumn(
            //    name: "IsTrial",
            //    table: "OrganisationSubscriptions");

            //migrationBuilder.DropColumn(
            //    name: "ShowComment",
            //    table: "AuditLogs");

            //migrationBuilder.DropColumn(
            //    name: "VisibleTo",
            //    table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "OrganisationSubscriptions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CancellationNote",
                table: "OrganisationSubscriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
