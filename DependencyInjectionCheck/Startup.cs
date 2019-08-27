using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionCheck.Modals;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace DependencyInjectionCheck
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public enum LoggerType
        {
            text = 0,
            sql = 1,
            csv = 2
        }
        public string logtype { get; set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<Func<string, ILogs>>((provider) =>
            {
            return new Func<string, ILogs>(
                (logtype) => new FactoryLogger().GetLogger(logtype)                                            
                );
            });


            //services.AddTransient<ILogs, LogWriterSql>();
            //services.AddTransient<ILogs, LogWriterCSV>();
            //services.AddTransient<ILogs, LogWriterText>();
            //services.AddTransient<Func<LoggerType, ILogs>>(serviceProvider => key =>
            //{
            //    switch (key)
            //    {
            //        case LoggerType.csv:
            //            return serviceProvider.GetService<LogWriterCSV>();
            //        case LoggerType.sql:
            //            return serviceProvider.GetService<LogWriterSql>();
            //        case LoggerType.text:
            //            return serviceProvider.GetService<LogWriterText>();
            //        default:
            //            throw new KeyNotFoundException();
            //    }
            //});
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "My API",
                    Version = "v1"
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
