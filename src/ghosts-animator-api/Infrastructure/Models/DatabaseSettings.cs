// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

namespace Ghosts.Animator.Api.Infrastructure.Models;

public class DatabaseSettings
{
    public class ApplicationDatabaseSettings : IApplicationDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public string CollectionNameNPCs => "NPCs";

        public string CollectionNameIPAddresses => "IPAddresses";
    }

    public interface IApplicationDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string CollectionNameNPCs { get; }
        string CollectionNameIPAddresses { get; }
    }
}