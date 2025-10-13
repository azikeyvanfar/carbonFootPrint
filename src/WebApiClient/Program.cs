using System;
using System.Data;
using ContractorBackend.Application.Common.Extensions;
using ContractorBackend.Persistence.DependencyInjectionExtensions;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace ContractorBackend.WebApiClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json")
                 .Build();

            Log.Logger = new LoggerConfiguration()
             .ReadFrom.Configuration(configuration)
             .Enrich.With(new ShadowPropertyEnricher(new HttpContextAccessor(), configuration)).Enrich.FromLogContext().CreateLogger();
            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                Console.WriteLine(msg);
            });

            try
            {
                Log.Information("Starting up");
                var host = CreateHostBuilder(args).Build();
                host.Services.InitializeDb();
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string connectionString = builder.Configuration["connectionStrings:LogConnection"].Decrypt();
            string tableName = builder.Configuration["loggingSerilog:tableName"];
            //var columnOptionsSection = builder.Configuration["loggingSerilog:additionalColumns"];
            var columnOptions = new ColumnOptions();
            columnOptions.Id.DataType = SqlDbType.BigInt;
            columnOptions.Store.Remove(StandardColumn.Properties);
            columnOptions.AdditionalColumns = new[]
            {
                new SqlColumn("UserName", SqlDbType.NVarChar, dataLength: 450),
                new SqlColumn("UserId", SqlDbType.NVarChar, dataLength: 50),
                new SqlColumn("UserDisplayName", SqlDbType.NVarChar, dataLength: 100), // -1  nvarchar(max)
                new SqlColumn("CreatedByIp", SqlDbType.NVarChar, dataLength: 50)
            };

            return
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.ClearProviders();
                        logging.AddSerilog();

                        if (hostingContext.HostingEnvironment.IsDevelopment())
                        {
                            logging.AddConsole();
                            logging.AddDebug();
                        }

                        //logging.AddDbLogger(); // You can change its Log Level using the `appsettings.json` file -> Logging -> LogLevel -> Default
                        //logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    })
                        //.UseSerilog()
                        .UseSerilog((ctx, lc) =>
                            lc.ReadFrom.Configuration(ctx.Configuration)
                            .WriteTo.MSSqlServer(
                                connectionString: connectionString,
                                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning,
                                columnOptions: columnOptions,

                                sinkOptions: new MSSqlServerSinkOptions
                                {
                                    TableName = tableName,
                                    SchemaName = "dbo",
                                    AutoCreateSqlTable = true
                                }
                            )
                        )

                        .UseStartup<Startup>();
                });
        }
    }
}
