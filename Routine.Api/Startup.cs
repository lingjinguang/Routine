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
                validation.MustRevalidate = true; //�����Ƿ�����֤
            });
            services.AddResponseCaching(); //��ӻ������
            //configure: setup => setup.ReturnHttpNotAcceptable = true  ֵĬ��Ϊfalse�������󣬵������������Ͳ������ܣ�����406
            services.AddCors(option => {    // ��ӿ���
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
                //setup.OutputFormatters.Insert(0,new XmlDataContractSerializerOutputFormatter());   ��������ʽxml ����2�����Ը��Ĳ���˳��
                setup.CacheProfiles.Add("120sCacheProfile",new CacheProfile 
                {
                    Duration = 120
                });

            }).AddNewtonsoftJson(setup=> 
                {
                    setup.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters(); //core3 ֮����÷�

            services.Configure<MvcOptions>(config => 
            {
                var newtonSoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                newtonSoftJsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.company.hateoas+json");
            });
            //���AutoMapper 
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //���ݿ����ÿһ��Http����
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
            //app.UseResponseCaching();   û��ʵ����֤ģ��

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
