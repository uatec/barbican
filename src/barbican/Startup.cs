using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using barbican.Authentication;
using barbican.Authentication.AzureAd;
using barbican.Authentication.BasicAuth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace barbican
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authConfig = Configuration.GetSection("Authentication");
            switch ( authConfig["Provider"] )
            {
                case "AzureAd":
                    services.AddAzureAd(authConfig);
                    break;
                case "Basic":
                default:
                    services.AddBasicAuthentication(authConfig);
                    break;
            }

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseExceptionDemystifier();
            }

            app.UseMvc();
            switch ( Configuration["Authentication:Provider"] )
            {
                case "AzureAd":
                    app.UseAzureAd();
                    break;
                case "Basic":
                default:
                    app.UseBasicAuthentication();
                    break;
            }

            app.UseMiddleware<ProxyMiddleware>();
        }
    }
}
