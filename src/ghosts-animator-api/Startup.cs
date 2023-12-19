// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using Ghosts.Animator.Api.Hubs;
using Ghosts.Animator.Api.Infrastructure;
using Ghosts.Animator.Api.Infrastructure.Animations;
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

namespace Ghosts.Animator.Api;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<DatabaseSettings.ApplicationDatabaseSettings>(
            Configuration.GetSection(nameof(DatabaseSettings.ApplicationDatabaseSettings)));

        services.AddSingleton<DatabaseSettings.IApplicationDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<DatabaseSettings.ApplicationDatabaseSettings>>().Value);

        services.AddCors(options => options.UseConfiguredCors(Program.Configuration.RawConfiguration.GetSection("CorsPolicy")));

        services.AddSwaggerGen(c =>
        {
            //c.CustomSchemaIds(x => x.FullName);
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "GHOSTS Animator API",
                Description = $"Animator API v1 - Assembly: {ApplicationDetails.Version} - File: {ApplicationDetails.VersionFile}",
                Contact = new OpenApiContact
                {
                    Name = "THE GHOSTS DEVELOPMENT TEAM",
                    Email = "ddupdyke@sei.cmu.edu"
                },
                License = new OpenApiLicense
                {
                    Name = "Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms."
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
        
        services.AddSignalR()
            .AddJsonProtocol(options => { options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
        
        services.AddControllersWithViews().AddRazorRuntimeCompilation();
        services.AddRouting(options => options.LowercaseUrls = true);
        
        // start any configured animation jobs
        services.AddSingleton<IHostedService, AnimationsManager>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); 
            endpoints.MapHub<ActivityHub>("/hubs/activities");
        } );

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Animator API V1");
        });
    }
}