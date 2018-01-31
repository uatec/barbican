using System;
using System.Security.Cryptography;
using System.Text;
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

            if ( string.IsNullOrEmpty(basicAuthOptions.Username) && string.IsNullOrEmpty(basicAuthOptions.Password) )
            {
                byte[] data = new byte[20];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                rng.GetBytes(data);
                basicAuthOptions.Username = Convert.ToBase64String(data);

                rng.GetBytes(data);
                basicAuthOptions.Password = Convert.ToBase64String(data);

                Console.WriteLine($"Using Basic Auth - Username: \"{basicAuthOptions.Username}\" Password: \"{basicAuthOptions.Password}\"");
            }

            services.AddSingleton(basicAuthOptions);
        }

        public static void UseBasicAuthentication(this IApplicationBuilder app)
        {
            app.UseMiddleware<BasicAuthenticationMiddleware>();
        }
    }
}
