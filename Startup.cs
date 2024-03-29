using System.Diagnostics;
using ciklonalozi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ciklonalozi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection().PersistKeysToDbContext<AppDbContext>();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            services.AddControllers();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.ConfigureAuth(Configuration);
            services.AddDbContextFactory<AppDbContext>(builder =>
            {
                builder.UseSqlite(C.Settings.AppDbConnectionString);
                builder.EnableSensitiveDataLogging(Debugger.IsAttached);
                builder.LogTo(message => Debug.WriteLine(message), new[] { RelationalEventId.CommandExecuted });
            });
            services.AddCors(options =>
                options.AddPolicy(name: C.CorsPolicy, policy =>
                    policy.WithOrigins("http://localhost:7347", "https://ciklo-sport.hr", "https://www.ciklo-sport.hr")
                        .AllowAnyHeader()
                )
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                app.UseCookiePolicy(new() { MinimumSameSitePolicy = SameSiteMode.Lax });
            }
            else
            {
                //app.UseExceptionHandler("/Error");
                app.UseForwardedHeaders();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors();
            app.UseAuth();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors(C.CorsPolicy);
                endpoints.MapRazorPages().RequireAuthorization(Auth.AuthorizedUsers);
                endpoints.MapBlazorHub().RequireAuthorization(Auth.AuthorizedUsers);
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
