// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure;
using Ghosts.Animator.Api.Infrastructure.Extensions;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog;

namespace Ghosts.Animator.Api;

// ReSharper disable once ClassNeverInstantiated.Global
public class Program
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    public static ApplicationConfiguration Configuration { get; set; }
    
    public static async Task Main(string[] args)
    {
        await AppConfigurationBuilder.Build();
        
        _log.Warn(ApplicationDetails.Header);
        _log.Warn($"GHOSTS ANIMATOR API {ApplicationDetails.Version} ({ApplicationDetails.VersionFile}) coming online...");

        await BuildWebHost(args).RunAsync();
    }

    private static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
}