using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FieldServiceManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class BackfillOrganisationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE u
                SET u.OrganisationId = o.Id
                FROM AspNetUsers u
                INNER JOIN Users up ON up.UserId = u.Id
                INNER JOIN Organisations o ON o.Id = up.OrganisationId
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
