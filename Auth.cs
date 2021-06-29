using System;
using System.Collections.Generic;
using System.Security.Claims;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ciklonalozi
{
    public class AuthSettings
    {
        public string CookieName { get; set; } = nameof(ciklonalozi);
        public string CorrelationCookieName { get; set; } = $"{nameof(ciklonalozi)}.correlation";
        public string GitHubId { get; set; } = string.Empty;
        public string GitHubSecret { get; set; } = string.Empty;
        public List<string> GitHubUsers { get; set; } = new();
    }
    public static partial class Auth
    {
        public static readonly AuthorizeAttribute AuthorizedUsers = new AuthorizeAttribute { AuthenticationSchemes = GitHubAuthenticationDefaults.AuthenticationScheme, Policy = nameof(AuthorizedUsers) };
        public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var auth = configuration.GetSection("Auth").Get<AuthSettings>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(opt =>
               {
                   opt.Cookie.Name = auth.CookieName;
                   opt.AccessDeniedPath = new("/Denied");
               })
               .AddGitHub(opt =>
               {
                   opt.ClientId = auth.GitHubId;
                   opt.ClientSecret = auth.GitHubSecret;

                   opt.CorrelationCookie.Name = auth.CorrelationCookieName;
               });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(AuthorizedUsers), policy => policy.RequireAssertion(ctx =>
                {
                    var username = ctx.User.FindFirst(ClaimTypes.Name)?.Value;

                    foreach (var user in auth.GitHubUsers)
                        if (user.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                            return true;

                    return false;
                }));
            });

            return services;
        }
        public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}