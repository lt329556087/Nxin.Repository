using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace FinanceManagement.ApiHost
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                 .AddEnvironmentVariables()
                 .Build();


        public static void Main(string[] args)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(Configuration)
               .Enrich.FromLogContext()
               .CreateLogger();
                var host = CreateHostBuilder(args).Build();
                Log.Information("host start run");
                host.Run();
            }
            catch (System.Exception exp)
            {
                Log.Fatal(exp, "host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseStartup<Startup>().UseKestrel().
                    ConfigureKestrel(options =>
                    {
                        //设置文件上传大小为int的最大值
                        options.Limits.MaxRequestBodySize = int.MaxValue;
                    });
              })
             .UseSerilog((hostingContext, loggerConfiguration) =>
             {
                 loggerConfiguration
                 .ReadFrom.Configuration(hostingContext.Configuration)
                 .Enrich.FromLogContext();
             }); //.UseSerilog();
    }
}
