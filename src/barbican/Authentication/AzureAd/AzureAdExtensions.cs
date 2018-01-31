using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace barbican.Authentication.AzureAd
{
    public static class AzureAdExtensions
    {
        public static void AddAzureAd(this IServiceCollection services, IConfiguration configuration)
        {
            var authBuilder = services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddAzureAd(options => configuration.Bind(options))
                .AddCookie() ;
        }

        public static void UseAzureAd(this IApplicationBuilder app)
        {
            app.UseAuthentication();            
            app.UseMiddleware<ChallengeMiddleware>();               
        }
    }
}
