using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.StartupModules.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Globalization;

namespace FieldServiceManagement.Web.StartupModules.PipelineModules;

public static class PipelineExtensions
{
    public static IApplicationBuilder UseFSMPipeline(
        this IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        // 🔥 Exception handling (MUST be first)
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error/500");
            app.UseHsts();
        }

        app.UseMiddleware<SecurityHeadersMiddleware>();

        // app.UseMiddleware<AuthGateMiddleware>();

        app.UseHttpsRedirection();

        // 🔥 Static files with no-cache headers
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                    "no-cache, no-store, must-revalidate";
                ctx.Context.Response.Headers[HeaderNames.Pragma] = "no-cache";
                ctx.Context.Response.Headers[HeaderNames.Expires] = "0";
            }
        });

        // 🔥 API status code handling (JSON only)
        app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"),
            appBuilder =>
            {
                appBuilder.UseStatusCodePages(async context =>
                {
                    var response = context.HttpContext.Response;
                    response.ContentType = "application/json";

                    await response.WriteAsync(
                        $"{{\"status\":{response.StatusCode}," +
                        $"\"message\":\"{ReasonPhrases.GetReasonPhrase(response.StatusCode)}\"}}");
                });
            });

        // 🔥 MVC status code handling (HTML pages)
        app.UseStatusCodePagesWithReExecute("/Error/{0}");

        // 🔥 Localization
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            SupportedCultures = new List<CultureInfo> { new("en-GB") },
            SupportedUICultures = new List<CultureInfo> { new("en-GB") },
            DefaultRequestCulture = new RequestCulture("en-GB")
        });

        app.UseCookiePolicy();

        // 🔥 Routing MUST come before auth
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            // 🔹 Dev-only auth endpoints
            if (env.IsDevelopment())
            {
                endpoints.MapPost("/api/auth/login", async (
                    HttpContext context,
                    SignInManager<ApplicationUser> signInManager,
                    UserManager<ApplicationUser> userManager) =>
                {
                    var form = await context.Request.ReadFromJsonAsync<LoginRequest>();
                    if (form is null)
                        return Results.BadRequest("Invalid request body.");

                    var user = await userManager.FindByEmailAsync(form.Email);
                    if (user is null)
                        return Results.Unauthorized();

                    var result = await signInManager.PasswordSignInAsync(
                        user,
                        form.Password,
                        isPersistent: false,
                        lockoutOnFailure: true);

                    return result.Succeeded
                        ? Results.Ok(new { message = "Authenticated. Cookie set." })
                        : Results.Unauthorized();
                });

                endpoints.MapPost("/api/auth/logout", async (
                    HttpContext context,
                    SignInManager<ApplicationUser> signInManager) =>
                {
                    await signInManager.SignOutAsync();
                    return Results.Ok(new { message = "Signed out." });
                });
            }

            // 🔹 Areas route (MUST be before default)
            endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Account}/{action=Login}/{id?}");

            // 🔹 Default route
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        return app;
    }
}

internal record LoginRequest(string Email, string Password);