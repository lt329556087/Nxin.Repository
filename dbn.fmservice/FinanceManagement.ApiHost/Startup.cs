using Architecture.Common.ServiceRegister;
using Architecture.Seedwork.Logging;
using Architecture.Seedwork.Security;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FinanceManagement.ApiHost.Extensions;
using FinanceManagement.Infrastructure;
using System.IO;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Nxin.Qlw.Common.MQ.RabbitMQ;
using FinanceManagement.ApiHost.Controllers.FD_PaymentReceivables;
using Architecture.Common.HttpClientUtil;
using FinanceManagement.Common;
using Microsoft.AspNetCore.Http.Features;
using Serilog;

namespace FinanceManagement.ApiHost
{
    public class Startup
    {
        private const string VERSION = "v1";
        private const string TITLE = "FinanceManagement.ApiHost";
        private IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.EnableEndpointRouting = false;
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            }).AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                option.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            var jwtSetting = new JwtSetting();
            Configuration.Bind("JwtSetting", jwtSetting);
            services.Configure<JwtSetting>(Configuration.GetSection("JwtSetting"));
            services.AddAuthenticationWithPermission(jwtSetting, Configuration.GetSection("ConnectionStrings:redis").Value);

            //流水号
            services.ConfigureNumericalOrderCreator(op => op.ServiceAddress = Configuration.GetValue<string>("AppSettings:NumericalOrderService"));
            services.ConfigureNumberCreator<Nxin_Qlw_BusinessContext>();
            services.AddDbContextService<Qlw_Nxin_ComContext>(Configuration.GetConnectionString("qlw_nxin_com"), _env.IsDevelopment());
            services.AddDbContextService<Nxin_Qlw_BusinessContext>(Configuration.GetConnectionString("nxin_qlw_business"), _env.IsDevelopment());
            services.AddDbContextPoolService<QlwCrossDbContext>(Configuration.GetConnectionString("qlw_cross"), _env.IsDevelopment());

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddHttpContextAccessor();
            services.AddMediatRServices();
            services.AddAppSettings(Configuration);
            services.AddRepositories();
            services.AddAppServices();
            services.AddOData();
            services.AddODataQueryFilter();
            services.AddHttpClientUtil();
            services.AddHttpContextAccessor();
            //通过设置即重置文件上传的大小限制
            services.Configure<FormOptions>(o =>
            {
                o.BufferBodyLengthLimit = long.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = long.MaxValue;
                o.MultipartBoundaryLengthLimit = int.MaxValue;
                o.MultipartHeadersCountLimit = int.MaxValue;
                o.MultipartHeadersLengthLimit = int.MaxValue;
            });
            //单据号
            services.ConfigureNumberCreator<Nxin_Qlw_BusinessContext>();
            services.ConfigureNumberCreator<Qlw_Nxin_ComContext>();

            services.ConfigureWebApp(op =>
            {
                op.AppCode = Configuration.GetValue<int>("AppSettings:AppCode");//AppCode即为本服务配置的2、3位应用码
            });


            services.AddCors(options =>
            {
                options.AddPolicy("allowSpecificOrigins",
                builder =>
                {
                    builder
                    .WithOrigins("*")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddResponseCompression(option =>
            {
                option.EnableForHttps = true;
            });

            services.AddSwaggerGen(options =>
            {
                options.ResolveConflictingActions(x => x.FirstOrDefault());
                options.SwaggerDoc(VERSION, new OpenApiInfo() { Title = TITLE, Version = VERSION });
                options.DocInclusionPredicate((docName, description) => true);
                options.OrderActionsBy(x => x.RelativePath);
                var xmlPath = Path.Combine(_env.ContentRootPath, $"{TITLE}.xml");
                options.IncludeXmlComments(xmlPath, true);
                #region 启用swagger验证功能
                OpenApiSecurityRequirement security = new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new List<string>()
                }};
                options.AddSecurityRequirement(security);
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "input a token value",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                #endregion
            });
            SetOutputFormatters(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint($"/swagger/{VERSION}/swagger.json", "客户服务");
                });
            }
            //app.UseHsts();
            //app.UseHttpsRedirection();
            app.UseRouting();
            //放置在app.UseRouting()之后
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = SerilogHelper.EnrichFromHttpRequest;
            });
            app.UseMiddleware<LoggingMiddleware>();
            app.UseCors("allowSpecificOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<LoggingMiddleware>();
            app.RegisterToConsul(Configuration, applicationLifetime);
            app.UseResponseCompression();
            app.UseODataService();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private static void SetOutputFormatters(IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
                foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });
        }
    }
}
