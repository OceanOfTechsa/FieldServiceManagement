using System.Security.Claims;

namespace FieldServiceManagement.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetOrganisationId(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue("OrganisationId");
            return Guid.TryParse(value, out var id) ? id : null;
        }

        public static Guid GetOrganisationIdOrThrow(this ClaimsPrincipal principal)
        {
            return principal.GetOrganisationId()
                ?? throw new InvalidOperationException("OrganisationId claim missing.");
        }
    }
}
