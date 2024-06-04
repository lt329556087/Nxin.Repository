using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Architecture.Seedwork.Security;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Architecture.Seedwork.Infrastructure.Behaviors;
using Microsoft.EntityFrameworkCore;
using FinanceManagement.Infrastructure;

namespace FinanceManagement.ApiHost.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            return services.AddMediatR(typeof(Program).Assembly);
        }

        public static IServiceCollection AddDbContextService<TDbContext>(this IServiceCollection services, string connectionString, bool enableDbLog = false) where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>(builder =>
            {

                //链接数据库
                builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b =>
                {
                    b.CommandTimeout(60);
                    b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
                //日志
                if (enableDbLog)
                {
                    builder.UseLoggerFactory(LoggerFactory.Create(configure =>
                    {
                        configure
                        .AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                        .AddConsole();
                    }));
                    builder.EnableSensitiveDataLogging();
                }
            });
            return services;
        }
        public static IServiceCollection AddDbContextPoolService<TDbContext>(this IServiceCollection services, string connectionString, bool enableDbLog = false) where TDbContext : DbContext
        {
            services.AddDbContextPool<TDbContext>(builder =>
            {
                //链接数据库
                builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b =>
                {
                    b.CommandTimeout(60);
                    b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
                //日志
                if (enableDbLog)
                {
                    builder.UseLoggerFactory(LoggerFactory.Create(configure =>
                    {
                        configure
                        .AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                        .AddConsole();
                    }));
                    builder.EnableSensitiveDataLogging();
                }
            });
            return services;
        }
    }
}
