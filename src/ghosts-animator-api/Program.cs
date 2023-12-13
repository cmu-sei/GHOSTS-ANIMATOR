// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure;
using Ghosts.Animator.Api.Infrastructure.Extensions;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog;

namespace Ghosts.Animator.Api;

public class Program
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();
    public static ApplicationConfiguration Configuration { get; set; }
    
    public static async Task Main(string[] args)
    {
        log.Warn(ApplicationDetails.Header);
        log.Warn($"GHOSTS ANIMATOR API {ApplicationDetails.Version} ({ApplicationDetails.VersionFile}) coming online...");

        await AppConfigurationBuilder.Build();
        
        BuildWebHost(args).Run();
    }

    private static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
}