// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Threading;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Api.Infrastructure.Social.SocialJobs;
using MongoDB.Driver;
using NLog;
using NLog.Targets;

namespace Ghosts.Animator.Api.Infrastructure.Social;

public class SocialJobManager
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly ApplicationConfiguration _configuration;
    private IMongoCollection<NPC> _mongo;
    private readonly Random _random;
    private Thread _socialSharingJobThread;
    private Thread _socialGraphJobThread;
        
    public SocialJobManager()
    {
        this._configuration = Program.Configuration;
        this._random = Random.Shared;
    }

    public void Run()
    {
        if (!this._configuration.SocialJobs.IsEnabled)
        {
            _log.Info($"Social Jobs are not enabled, skipping.");
            return;
        }

        _log.Info($"Social Jobs are enabled, starting up...");

        var client = new MongoClient(this._configuration.DatabaseSettings.ConnectionString);
        var database = client.GetDatabase(this._configuration.DatabaseSettings.DatabaseName);
        this._mongo = database.GetCollection<NPC>(this._configuration.DatabaseSettings.CollectionNameNPCs);
            
        try
        {
            if (!this._configuration.SocialJobs.SocialGraph.IsEnabled)
            {
                _log.Info($"Social Graph is not enabled, skipping.");
            }
            else
            {
                _socialGraphJobThread = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    var _ = new SocialGraphJob(this._configuration, this._mongo, this._random);
                });
                _socialGraphJobThread.Start();
            }
        }
        catch (Exception e)
        {
            _log.Trace(e);
        }
            
        try
        {
            if (!this._configuration.SocialJobs.SocialSharing.IsEnabled)
            {
                _log.Info($"Social Sharing is not enabled, skipping.");
            }
            else
            {
                _socialSharingJobThread = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    var _ = new SocialSharingJob(this._configuration, this._mongo, this._random);
                });
                _socialSharingJobThread.Start();
                
            }
        }
        catch (Exception e)
        {
            _log.Trace(e);
        }
    }
    
    public void Stop()
    {
        Console.WriteLine("Stopping social jobs...");
        try
        {
            _socialGraphJobThread?.Interrupt();
        }
        catch
        {
            
        }

        try
        {
            _socialSharingJobThread?.Interrupt();
        }
        catch
        {
            
        }
        Console.WriteLine("Social jobs stopped");
    }
}
    