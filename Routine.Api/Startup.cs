using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Routine.Api.Data;
using Routine.Api.Services;

namespace Routine.Api
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
            services.AddHttpCacheHeaders(expires=> 
            {
                expires.MaxAge = 60;
                expires.CacheLocation = CacheLocation.Private;
            },validation=> 
            {
                validation.MustRevalidate = true; //过期是否开启验证
            });
            services.AddResponseCaching(); //添加缓存服务
            //configure: setup => setup.ReturnHttpNotAcceptable = true  值默认为false，开启后，当链接请求类型不被接受，返回406
            services.AddCors(option => {    // 添加跨域
                option.AddPolicy("myAllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:8080");
                    });
            });
            services.AddControllers(configure:setup=>
            {
                setup.ReturnHttpNotAcceptable = true;
                //setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                //setup.OutputFormatters.Insert(0,new XmlDataContractSerializerOutputFormatter());   添加输出格式xml 方法2，可以更改插入顺序
                setup.CacheProfiles.Add("120sCacheProfile",new CacheProfile 
                {
                    Duration = 120
                });

            }).AddNewtonsoftJson(setup=> 
                {
                    setup.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters(); //core3 之后的用法

            services.Configure<MvcOptions>(config => 
            {
                var newtonSoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                newtonSoftJsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.company.hateoas+json");
            });
            //添加AutoMapper 
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //数据库服务，每一次Http请求
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddDbContext<RoutineDbContext>(optionsAction:option=> 
            {
                //option.UseSqlite(connectionString:"Data Source=routine.db");
                option.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder=>
                {
                    appBuilder.Run(handler: async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync(text: "Unexpected Error!");
                    });
                });
            }
            //app.UseResponseCaching();   没有实现验证模型

            app.UseHttpCacheHeaders();

            app.UseRouting();

            app.UseCors("myAllowSpecificOrigins");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
