using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace barbican
{
    public class ProxyMiddleware
    {
        public ProxyMiddleware(RequestDelegate next)
        {
            // Required to be an aspnetcore middleware
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
            Uri destUri = mapUri(context.Request.PathBase, context.Request.Path, context.Request.QueryString);
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(context.Request.Method), destUri);

            if ( context.Request.Body != null )
            {
                request.Content = new StreamContent(context.Request.Body);
            }

            foreach ( var header in context.Request.Headers ) 
            {
                if ( !request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) ) 
                {
                    request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            request.Headers.Host = destUri.Host;

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            context.Response.StatusCode = (int) response.StatusCode;

            foreach ( var header in response.Headers ) 
            {
                context.Response.Headers.Add(header.Key, header.Value.ToArray());
            }

            foreach ( var header in response.Content.Headers ) 
            {
                context.Response.Headers.Add(header.Key, header.Value.ToArray());
            }

            context.Response.Headers.Remove("Transfer-Encoding");

            await response.Content.CopyToAsync(context.Response.Body);
            
            }
            catch ( Exception ex ) 
            {
                throw;
            }
        }

        private Uri mapUri(PathString pathBase, PathString path, QueryString queryString)
        {
            string[] pathParts = path.Value.Split('/');

            return new Uri($"{pathParts[1]}://{pathParts[2]}/{string.Join('/', pathParts.Skip(3))}");
        }
    }
}
