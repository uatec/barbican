using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace barbican.Authentication.AzureAd
{
    public class ChallengeMiddleware
    {
        private readonly RequestDelegate _next;

        public ChallengeMiddleware(RequestDelegate next) 
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // TODO Improve mising-identity detection
            if ( context.User.Identity.Name == null ) 
            {
                await context.ChallengeAsync();
                return;
            }

            await _next(context);
        }
    }
}
