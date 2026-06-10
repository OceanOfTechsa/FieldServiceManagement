using Microsoft.AspNetCore.Identity;

namespace FieldServiceManagement.StartupModules.ServiceModules;

public static class AuthenticationModule
{
    public static IServiceCollection AddAuthenticationModule(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        });

        services.AddAuthorization();

        return services;
    }
}