using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/proxy") )
                {
                    string[] pathParts = context.Request.Path.Value.Split('/');

                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Proxy(pathParts[2], Int32.Parse(pathParts[3]), webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
        }

        private async Task Proxy(string hostname, int port, WebSocket webSocket)
        {
            using (TcpClient tcpClient = new TcpClient()) 
            {
                tcpClient.Connect(hostname, port);

                Socket socket = tcpClient.Client;

                CancellationTokenSource cancellationSource = new CancellationTokenSource();
                Task sendTask = Task.Run(async () => {
                    var buffer = new byte[1024 * 4];
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    while (!result.CloseStatus.HasValue)
                    {
                        socket.Send(buffer);
                        Console.WriteLine($"< {Encoding.UTF8.GetString(buffer, 0, result.Count)} |");
                        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationSource.Token);
                    } 
                    cancellationSource.Cancel();
                });

                Task receiveTask = Task.Run(async () => {
                    var buffer = new byte[1024 * 4];
                    while (socket.Connected) 
                    {
                        int i = socket.Receive(buffer); 

                        if ( i == 0 ) 
                        {
                            break;
                        }
                        string text = Encoding.UTF8.GetString(buffer, 0, i);
                        Console.WriteLine($"> {text} |");
                        // todo change this to binary when we are happy what it means, and if we can derive end of message
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, i), WebSocketMessageType.Text, i != buffer.Length, cancellationSource.Token);
                    }
                    cancellationSource.Cancel();
                });

                
                await sendTask;
                await receiveTask;
                socket.Close();
            }
        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        
        private async Task Say(string message, HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
