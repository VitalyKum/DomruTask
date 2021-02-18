using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using TaskService.API.Infrastructure;
using TaskService.API.Infrastructure.Services;

namespace TaskService.API
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
            services.AddOptions();
            services.Configure<AppSettings>(Configuration);

            services.AddDbContext<TaskServiceContext>(options =>
                    options.UseNpgsql(Configuration["PostgresConnection"])
                );

            services.AddCors(options =>
            {
                options.AddPolicy("corsPolicy",
                    builder => builder
                    .SetIsOriginAllowed(host => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });

            services.AddHttpClient<IErthsobesService, ErthsobesService>();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TaskService API",
                    Description = "Exclusive for DomRu company",
                    TermsOfService = new Uri("https://github.com/ERTHSobes/TaskDescription"),
                    Contact = new OpenApiContact
                    {
                        Name = "Kumratov Vitaly",
                        Email = "vkumratov@gmail.com",
                        Url = new Uri("https://twitter.com/spboyer"),
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("corsPolicy");

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            { 
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My TaskServiceAPI V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
