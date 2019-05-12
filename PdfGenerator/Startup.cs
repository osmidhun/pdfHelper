using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PdfGenerator.Formatters;
using Swashbuckle.AspNetCore.Swagger;

namespace PdfGenerator
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            HostingEnvironment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("api-v1", new Info { Title = "pdf generator  API", Version = "v1" });
            });
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            Dictionary<string, string> templates = new Dictionary<string, string>();
            foreach (var template in Directory.EnumerateFiles(Path.Combine(HostingEnvironment.ContentRootPath, "templates"), "*.html"))
            {
                var templateContent = File.ReadAllText(template);
                templates.Add(Path.GetFileNameWithoutExtension(template).ToUpper(), templateContent);
            }
           // _ = File.ReadAllText();
            services.AddMvc(options =>
            {
                options.OutputFormatters.Insert(1, new PdfFormatter( serviceProvider.GetService<IConverter>(), templates));
               
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/api-v1/swagger.json", "flydubai Document Viewer API v1");
            });
          //  app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
