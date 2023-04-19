// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.IO;
using Ghosts.Animator.Api.Infrastructure;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NLog;

namespace Ghosts.Animator.Api;

public class Program
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();
    public static ApplicationConfiguration Configuration { get; private set; }
    
    public static void Main(string[] args)
    {
        log.Warn(ApplicationDetails.Header);
        log.Warn($"GHOSTS ANIMATOR API {ApplicationDetails.Version} ({ApplicationDetails.VersionFile}) coming online...");
            
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        var config = builder.Build();
        var appConfig = new ApplicationConfiguration();
        config.GetSection("ApplicationSettings").Bind(appConfig);

        var dbConfig = new DatabaseSettings.ApplicationDatabaseSettings();
        config.GetSection("ApplicationDatabaseSettings").Bind(dbConfig);

        Configuration = appConfig;
        Configuration.DatabaseSettings = dbConfig;
    
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
}