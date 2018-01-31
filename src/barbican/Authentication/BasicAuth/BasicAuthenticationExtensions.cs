using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace barbican.Authentication.BasicAuth
{
    public static class BasicAuthenticationExtensions
    {
        public static void AddBasicAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var basicAuthOptions = new BasicAuthOptions();
            configuration.Bind(basicAuthOptions);
            services.AddSingleton(basicAuthOptions);
        }

        public static void UseBasicAuthentication(this IApplicationBuilder app)
        {
            app.UseMiddleware<BasicAuthenticationMiddleware>();
        }
    }
}
