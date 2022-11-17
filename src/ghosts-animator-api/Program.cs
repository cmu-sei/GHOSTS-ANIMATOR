/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

using System.IO;
using System.Reflection;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Api.Infrastructure.Social;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NLog;

namespace Ghosts.Animator.Api
{
    public class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        public static ApplicationConfiguration Configuration { get; private set; } 
        public static void Main(string[] args)
        {
            log.Warn($"GHOSTS ANIMATOR API {Assembly.GetExecutingAssembly().GetName().Version} coming online...");
            
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

            var s = new SocialJobManager();
            s.Run();
            
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}