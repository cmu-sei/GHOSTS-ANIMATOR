/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using Ghosts.Animator.Api.Infrastructure.Extensions;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Ghosts.Animator.Api
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
            services.Configure<DatabaseSettings.ApplicationDatabaseSettings>(
                Configuration.GetSection(nameof(DatabaseSettings.ApplicationDatabaseSettings)));

            services.AddSingleton<DatabaseSettings.IApplicationDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<DatabaseSettings.ApplicationDatabaseSettings>>().Value);
            
            services.AddCors(options => options.UseConfiguredCors(Configuration.GetSection("CorsPolicy")));

            services.AddSwaggerGen(c =>
            {
                //c.CustomSchemaIds(x => x.FullName);
                c.SwaggerDoc($"v1", new OpenApiInfo
                {
                    Version = $"v1",
                    Title = "GHOSTS Animator API",
                    Description = $"Animator API v1 - Assembly: {Assembly.GetExecutingAssembly().GetName().Version}",
                    Contact = new OpenApiContact()
                    {
                        Name = "THE GHOSTS DEVELOPMENT TEAM",
                        Email = "ddupdyke@sei.cmu.edu",
                    },
                    License = new OpenApiLicense()
                    {
                        Name = $"Copyright 2020 - {DateTime.Now.Year} Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms"
                    }
                });
                var filePath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                c.IncludeXmlComments(filePath);
                c.ExampleFilters();
            });
            services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
            
            services.AddControllers().AddJsonOptions(options => 
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddMvc();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRouting(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); } );

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Animator API V1"); });
        }
    }
}