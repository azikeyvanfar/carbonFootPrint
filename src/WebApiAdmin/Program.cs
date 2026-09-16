using System;
using System.Data;
using ContractorBackend.Application.Common.Extensions;
using ContractorBackend.Persistence.DependencyInjectionExtensions;
using ContractorBackend.WebApiAdmin.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace ContractorBackend.WebApiAdmin
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
                try
                {
                    host.Run();
                }
                catch (Exception runEx)
                {
                    System.IO.File.WriteAllText(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ghg-host-crash.log"), runEx.ToString());
                    throw;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
                try { System.IO.File.WriteAllText(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ghg-startup-crash.log"), ex.ToString()); } catch { }
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
                new SqlColumn("CreatedByIp", SqlDbType.NVarChar, dataLength: 100),

                new SqlColumn("Path", SqlDbType.NVarChar, dataLength: 128),
                new SqlColumn("Data", SqlDbType.NVarChar, dataLength: -1) // -1  nvarchar(max)
            };

            var sinkOptions = new MSSqlServerSinkOptions
            {
                TableName = tableName,
                SchemaName = "dbo",
                AutoCreateSqlTable = true
            };

            // Serilog Logger
            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(builder.Configuration)
            //    .WriteTo.MSSqlServer(
            //        connectionString: connectionString,
            //        sinkOptions: sinkOptions,
            //        restrictedToMinimumLevel: LogEventLevel.Warning,
            //        columnOptions: columnOptions
            //    )
            //    .CreateLogger();


            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        }
    }
}
