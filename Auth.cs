using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ciklonalozi
{
    public class AuthSettings
    {
        public string[] AdminEmails { get; set; } = Array.Empty<string>();
        public string CookieName { get; set; } = nameof(ciklonalozi);
        public string CorrelationCookieName { get; set; } = $"{nameof(ciklonalozi)}.correlation";
        public string RedirectParam = "redirect";
        public string GitHubId { get; set; } = string.Empty;
        public string GitHubSecret { get; set; } = string.Empty;
        public string GitHubCallback = "/signin";
        public int ExpirationHour { get; set; } = 5;
        public int ExpirationMinute { get; set; } = 55;

        public DateTimeOffset GetNextExpirationUtc()
        {
            var now = DateTime.Now;
            var tomorrow = now.AddDays(1);
            var expirationTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, ExpirationHour, ExpirationMinute, default, DateTimeKind.Local);
            var expiration = expirationTime - DateTime.Now;

            if (expiration.TotalDays > 1)
                return new DateTimeOffset(expirationTime.AddDays(-1));
            else
                return new DateTimeOffset(expirationTime);
        }
    }
    public static partial class Auth
    {
        public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var auth = configuration.GetSection("Auth").Get<AuthSettings>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(opt =>
               {
                   opt.Events.OnRedirectToLogin = RedirectToLogin;
                   opt.ReturnUrlParameter = auth.RedirectParam;
                   opt.Cookie.Name = auth.CookieName;
                   opt.Cookie.SameSite = SameSiteMode.Strict;
               })
               .AddGitHub(opt =>
               {
                   opt.ClientId = auth.GitHubId;
                   opt.ClientSecret = auth.GitHubSecret;
                   opt.CallbackPath = auth.GitHubCallback;
                   opt.ReturnUrlParameter = auth.RedirectParam;
                   opt.Events.OnCreatingTicket = CreatingTicket;

                   opt.CorrelationCookie.Name = auth.CorrelationCookieName;
                   opt.CorrelationCookie.SameSite = SameSiteMode.Lax;
               });

            return services;
        }
        public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
        public static async Task RedirectToLogin(Microsoft.AspNetCore.Authentication.RedirectContext<CookieAuthenticationOptions> ctx)
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await ctx.Response.WriteAsync("{}");
        }
        public static async Task CreatingTicket(OAuthCreatingTicketContext ctx)
        {
            var auth = ctx.HttpContext.RequestServices.GetRequiredService<IOptions<AuthSettings>>();
            ctx.Properties.ExpiresUtc = auth.Value.GetNextExpirationUtc();
            await Task.CompletedTask;

            // var username = ctx.Principal.Claims.Any(c=>c.ClaimTypes.PrimarySid);

            ctx.Fail("Unauthorized");

            // Todo is Admin user

            // var mediator = ctx.HttpContext.RequestServices.GetRequiredService<IMediator>();
            // var command = new EnsureGitHubUserCommand(ctx.Identity);
            // var user = await mediator.Send(command);

            // var sid = new Claim(ClaimTypes.Sid, user.Id.ToString());
            // ctx.Identity.AddClaim(sid);
        }
    }
}