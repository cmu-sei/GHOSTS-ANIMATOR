// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.Extensions.Configuration;

namespace Ghosts.Animator.Api.Infrastructure.Extensions;

public static class AppConfigurationBuilder
{
    public static async Task Build()
    {
        await new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").BuildApp();
    }
    
    /// <summary>
    /// Retrieves configuration location from appsettings.json (local paths or URLs).
    /// Enables overriding via environment variables for Docker compatibility.
    /// For URL-based configurations, fetches content and saves locally.
    /// </summary>
    private static async Task BuildApp(this IConfigurationBuilder builder)
    {
        var config = builder.Build();
        await BuildConfig(builder, config.GetSection("Configuration").Value);
    }

    /// <summary>
    /// Constructs the app configuration from a file location or URL.
    /// Creates and binds configuration sections to specific settings,
    /// and updates the Program's static Configuration property.
    /// </summary>
    private static async Task BuildConfig(this IConfigurationBuilder builder, string fileLocation)
    {
        fileLocation = await HandleUrlConfig(fileLocation);        
        
        // now build the actual configuration
        builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(fileLocation);
        var config = builder.Build();
        
        var appConfig = new ApplicationConfiguration();
        config.GetSection("ApplicationSettings").Bind(appConfig);

        var dbConfig = new DatabaseSettings.ApplicationDatabaseSettings();
        config.GetSection("ApplicationDatabaseSettings").Bind(dbConfig);
        
        Program.Configuration = appConfig;
    }

    /// <summary>
    /// Checks the file location for a URL and downloads the file if so, saving it w/ a unique filename. 
    /// Returns the local file path or the new path of the downloaded file.
    /// </summary>
    private static async Task<string> HandleUrlConfig(string fileLocation)
    {
        if (!fileLocation.StartsWith("http"))
            return fileLocation;

        var client = new HttpClient();
        var response = await client.GetAsync(fileLocation);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(content))
            throw new Exception("Http configuration file could not be found");
        
        var f = $"config/config_{Guid.NewGuid()}.json";
        await File.WriteAllTextAsync(f, content);
        return f;
    }
}