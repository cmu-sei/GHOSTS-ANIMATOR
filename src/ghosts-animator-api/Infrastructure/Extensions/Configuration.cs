using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.Extensions.Configuration;

namespace Ghosts.Animator.Api.Infrastructure.Extensions;

public static class Configuration
{
    /// <summary>
    /// The typical appsettings.json only contains an entry for the actual app config
    /// which can easily be overridden with an env var. Easy to default,
    /// and easy to override for docker. That also allows docker users
    /// to put the actual app config wherever they want.
    /// ...and if appsettings.json Configuration is a url, we fetch it with an http client.
    /// Then it be easy to update/collaborate/share various app
    /// configs (and docker folks wouldn't even have to mount it into the container).
    /// So, this loads the location of the actual config file — could be a local path or a URL
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>the location of the file</returns>
    public static async Task BuildApp(this IConfigurationBuilder builder)
    {
        // get the location of app configuration
        var config = builder.Build();
        await BuildConfig(builder, config.GetSection("Configuration").Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="fileLocation"></param>
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
        Program.Configuration.DatabaseSettings = dbConfig;
        Program.Configuration.RawConfiguration = config;
    }

    private static async Task<string> HandleUrlConfig(string fileLocation)
    {
        if (!fileLocation.StartsWith("http"))
            return fileLocation;

        var client = new HttpClient();
        var response = await client.GetAsync(fileLocation);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsByteArrayAsync();
        var f = $"config/config_{Guid.NewGuid()}.json";
        await File.WriteAllBytesAsync(f, content);
        return f;
    }
}