using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace RithV.Services.CORE.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);

            //CreateHostBuilder(args).Build().Run();

            try
            {
                    Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var host = BuildWebHost(configuration, args);

                Log.Information("Applying migrations ({ApplicationContext})...", AppName);
                //host.MigrateDbContext<OrderingContext>((context, services) =>
                //{
                //    var env = services.GetService<IWebHostEnvironment>();
                //    var settings = services.GetService<IOptions<OrderingSettings>>();
                //    var logger = services.GetService<ILogger<OrderingContextSeed>>();

                //    new OrderingContextSeed()
                //        .SeedAsync(context, env, settings, logger)
                //        .Wait();
                //})
                //.MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

                Log.Information("Starting web host ({ApplicationContext})...", AppName);

                //Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
                //Serilog.Debugging.SelfLog.Enable(Console.Error);

                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .CaptureStartupErrors(true)
                .ConfigureKestrel(options =>
                {
                    var ports = GetDefinedPorts(configuration);
                    options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    });

                    //options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
                    //{
                    //    listenOptions.Protocols = HttpProtocols.Http2;
                    //});

                })
                //.ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .ConfigureAppConfiguration((config) => {
                    config.AddConfiguration(configuration);
                    config.AddEnvironmentVariables();
                })
               .UseStartup<Startup>()
               .UseContentRoot(Directory.GetCurrentDirectory())
               .UseSerilog()
               .UseKestrel()

               .Build();

        private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
        {
            var grpcPort = config.GetValue("GRPC_PORT", 5001);
            var port = config.GetValue("PORT", 8080);
            return (port, grpcPort);
        }

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog-SeqServerUrl"];
            var logstashUrl = configuration["Serilog-LogstashgUrl"];
            string loggerTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}]<{ThreadId}> [{SourceContext:l}] {Message:lj}{NewLine}{Exception}";
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var logfile = Path.Combine(baseDir, "App_Data", "logs", "log.txt");

            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(logfile, LogEventLevel.Information, loggerTemplate, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 90)
                .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
