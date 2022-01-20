using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Adorable.Hubs;
//using VueCliMiddleware;

namespace Adorable
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

            services.AddCors(c => {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Adorable", Version = "v1" });
            });

             services.AddSpaStaticFiles(configuration =>
             {
                 configuration.RootPath = "ClientApp";
            });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(options => 
                options.WithOrigins("https://shop-swiftly.vercel.app")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
            );
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Adorable v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //  app.UseSpaStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignalrHub>("/signalr");
            });

            /*app.UseSpa(spa => 
            {
                if (env.IsDevelopment())
                {
                    spa.Options.SourcePath = "ClientApp/";
                }
                else{
                    spa.Options.SourcePath = "dist";
                }

                if (env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve");
                }
            });*/

            app.UseStaticFiles(new StaticFileOptions
             {
                 FileProvider = new PhysicalFileProvider(
                     Path.Combine(Directory.GetCurrentDirectory(),"Photos")),
                        RequestPath = "/Photos"
            });

	        AppDomain.CurrentDomain.SetData("ContentRootPath", env.ContentRootPath);
            AppDomain.CurrentDomain.SetData("WebRootPath", env.WebRootPath);
        }
    }
}
