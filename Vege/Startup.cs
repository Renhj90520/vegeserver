using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vege.Models;
using Vege.Repositories;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Vege.WeChatOauth;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using log4net.Config;
using Vege.Log;

namespace Vege
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                                        typeof(log4net.Repository.Hierarchy.Hierarchy));

            XmlConfigurator.Configure(repo, new FileInfo("log4net.config"));
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddSingleton(this.Configuration);
            services.AddDbContext<VegeContext>();
            services.AddScoped<IVegeRepository, VegeRepository>();
            services.AddCors();
            services.AddMemoryCache();
            //services.AddLogging();
            //services.AddIdentity<ApplicationUser, IdentityRole>();
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors(builder => builder.AllowAnyOrigin());
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            loggerFactory.AddLog4Net();
            app.UseDeveloperExceptionPage();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseIdentity();
            //app.UseWeChatAuthentication(new WeChatOptions()
            //{
            //    AppId = "wxcdb956aaa555d76f",
            //    AppSecret = "58a6fb9aadf9747a6ed356af2f327e1e"
            //});
            app.UseJwtBearerAuthentication(new JwtBearerOptions()
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = this.Configuration["Server"],
                    ValidAudience = this.Configuration["Server"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["Tokens:Key"])),
                    //ValidateLifetime = true
                }
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Authorization}/{action=Redirect}");
            });
        }
    }
}
