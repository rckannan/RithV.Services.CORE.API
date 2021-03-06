using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RithV.Services.CORE.API.Infra;
using Serilog;

namespace RithV.Services.CORE.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
              //.AddGrpc(options =>
              //{
              //    options.EnableDetailedErrors = true;
              //})
             // .Services
              //  .AddApplicationInsights(Configuration)
              .AddCustomMvc()
              .AddHealthChecks(Configuration)
              .AddCustomDbContext(Configuration)
              .AddCustomSwagger(Configuration);//.AddControllers();
                                               //.AddCustomIntegrations(Configuration)
                                               //.AddCustomConfiguration(Configuration)
                                               //.AddEventBus(Configuration)
                                               //.AddCustomAuthentication(Configuration);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });

            //services.Configure<approute>(Configuration.GetSection("approute"));

            var container = new ContainerBuilder();
            container.Populate(services);

            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new ApplicationModule(Configuration["ConnectionString"]));

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                //loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'", pathBase);
                Log.Information($"pathBase :: {pathBase}");
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");
             
            app.UseSwagger()
             .UseSwaggerUI(c =>
             {
                 c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "CORE.API V1");
                 c.OAuthClientId("COREswaggerui");
                 c.OAuthAppName("CORE Swagger UI");
                 
             });

            app.UseRouting();
            //app.UseAuthorization();

            app.UseSerilogRequestLogging();
            app.UseStaticFiles();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            app.Use(async (context, next) =>
            {
                // Do loging
                // Do work that doesn't write to the Response.
                var name = Dns.GetHostName(); // get container id

                Log.Information("------------Request logging --------------");
               var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                Log.Information($"Host Name: { Environment.MachineName} \t {name}\t {ip} \t {Environment.MachineName} \t {context.Request.GetDisplayUrl()}");
                Log.Information($"{context.Request.Method} \t {context.Request.Scheme} \t {context.Request.Path} \t {context.Connection.RemoteIpAddress}");
                foreach (var (key, value) in context.Request.Headers)
                { 
                    Log.Information($"\t {key}: {value} \t ");
                }
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            });

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGrpcService<OrderingService>();
                //endpoints.MapControllerRoute("default", "{controller=Customer}/{action=Index}/{id?}");
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                //endpoints.MapGet("/_proto/", async ctx =>
                //{
                //    using var fs = new FileStream(Path.Combine(env.ContentR
                //    ctx.Response.ContentType = "text/plain";ootPath, "Proto", "basket.proto"), FileMode.Open, FileAccess.Read);
                //    using var sr = new StreamReader(fs);
                //    while (!sr.EndOfStream)
                //    {
                //        var line = await sr.ReadLineAsync();
                //        if (line != "/* >>" || line != "<< */")
                //        {
                //            await ctx.Response.WriteAsync(line);
                //        }
                //    }
                //});
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });

                endpoints.MapGet("/", async context =>
                {
                    context.Response.ContentType = "text/plain";

                    // Host info
                    var name = Dns.GetHostName(); // get container id
                    var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    Console.WriteLine($"Host Name: { Environment.MachineName} \t {name}\t {ip}");
                    Log.Information($"Host Name: { Environment.MachineName} \t {name}\t {ip}");
                    await context.Response.WriteAsync($"Host Name: {Environment.MachineName}{Environment.NewLine}");
                    await context.Response.WriteAsync(Environment.NewLine);
                    Log.Information($"Host Name: {Environment.MachineName}{Environment.NewLine}");
                    // Request method, scheme, and path
                    await context.Response.WriteAsync($"Request Method: {context.Request.Method}{Environment.NewLine}");
                    await context.Response.WriteAsync($"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
                    await context.Response.WriteAsync($"Request URL: {context.Request.GetDisplayUrl()}{Environment.NewLine}");
                    await context.Response.WriteAsync($"Request Path: {context.Request.Path}{Environment.NewLine}");

                    Log.Information($"Request Method: {context.Request.Method}{Environment.NewLine}");
                    Log.Information($"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
                    Log.Information($"Request URL: {context.Request.GetDisplayUrl()}{Environment.NewLine}");
                    Log.Information($"Request Path: {context.Request.Path}{Environment.NewLine}");
                    // Headers
                    await context.Response.WriteAsync($"Request Headers:{Environment.NewLine}");
                    Log.Information($"Request Path: {context.Request.Path}{Environment.NewLine}");
                    foreach (var (key, value) in context.Request.Headers)
                    {
                        await context.Response.WriteAsync($"\t {key}: {value}{Environment.NewLine}");
                        Log.Information($"\t {key}: {value}{Environment.NewLine}");
                    }
                    await context.Response.WriteAsync(Environment.NewLine);

                    // Connection: RemoteIp
                    await context.Response.WriteAsync($"Request Remote IP: {context.Connection.RemoteIpAddress}");
                    Log.Information($"Request Remote IP: {context.Connection.RemoteIpAddress}");
                });
            });
        }
    }
}
