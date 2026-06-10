using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FieldServiceManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailInfrastructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailErrorLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProtocolLog = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResentProtocolLog = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SendTo = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SendCC = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SendBCC = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AttachmentFiles = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Attachment = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    SuccessfullySent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailErrorLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormsSubmissionAuditId = table.Column<int>(type: "int", nullable: true),
                    MessageId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProtocolLog = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadReceipt = table.Column<bool>(type: "bit", nullable: false),
                    DateAccessed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailQueue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipientCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Attachment = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Cc = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Bcc = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AttachmentFiles = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailQueue", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailErrorLog");

            migrationBuilder.DropTable(
                name: "EmailLog");

            migrationBuilder.DropTable(
                name: "EmailQueue");
        }
    }
}
