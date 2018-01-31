using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace barbican.Authentication.BasicAuth
{
    public class BasicAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BasicAuthOptions options;

        public BasicAuthenticationMiddleware(RequestDelegate next, BasicAuthOptions options) 
        {
            _next = next;
            this.options = options;
        }

        // TODO Log error types
        // TODO Set up User Principle
        private bool ValidateRequest(HttpRequest request)
        {
            if ( !request.Headers.ContainsKey("Authorization") ) return false;

            string authHeader = request.Headers["Authorization"];

            if ( string.IsNullOrEmpty(authHeader) ) return false;

            if ( !authHeader.StartsWith("Basic ") ) return false;
            authHeader = authHeader.Substring(6);

            string[] authHeaderParts = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader)).Split(':');

            if ( authHeaderParts.Length != 2 ) return false;

            if ( !options.Username.Equals(authHeaderParts[0]) ) return false;
            if ( !options.Password.Equals(authHeaderParts[1]) ) return false;

            return true;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if ( !ValidateRequest(context.Request) )
            {
                context.Response.StatusCode = 401;
                context.Response.Headers.Add("WWW-Authenticate", $"Basic realm=\"{options.Realm}\"");
                return;
            }
            
            await _next(context);
        }
    }
}
