// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Threading;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Api.Infrastructure.Social.Animations;
using MongoDB.Driver;
using NLog;

namespace Ghosts.Animator.Api.Infrastructure.Social;

public class AnimationsManager
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly ApplicationConfiguration _configuration;
    private IMongoCollection<NPC> _mongo;
    private readonly Random _random;
    private Thread _socialSharingJobThread;
    private Thread _socialGraphJobThread;
    private Thread _socialBeliefsJobThread;

    public AnimationsManager()
    {
        this._configuration = Program.Configuration;
        this._random = Random.Shared;
    }

    public void Run()
    {
        if (!this._configuration.Animations.IsEnabled)
        {
            _log.Info($"Animations are not enabled, skipping.");
            return;
        }

        _log.Info($"Animations are enabled, starting up...");

        var client = new MongoClient(this._configuration.DatabaseSettings.ConnectionString);
        var database = client.GetDatabase(this._configuration.DatabaseSettings.DatabaseName);
        this._mongo = database.GetCollection<NPC>(this._configuration.DatabaseSettings.CollectionNameNPCs);

        try
        {
            if (this._configuration.Animations.SocialGraph.IsEnabled && this._configuration.Animations.SocialGraph.IsInteracting)
            {
                _log.Info($"Starting SocialGraph...");
                _socialGraphJobThread = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    var _ = new SocialGraphJob(this._configuration, this._mongo, this._random);
                });
                _socialGraphJobThread.Start();
            }
            else
            {
                _log.Info($"SocialGraph is not enabled, skipping.");
            }
        }
        catch (Exception e)
        {
            _log.Trace(e);
        }

        try
        {
            if (this._configuration.Animations.SocialBelief.IsEnabled && this._configuration.Animations.SocialBelief.IsInteracting)
            {
                _log.Info($"Starting SocialBelief...");
                _socialBeliefsJobThread = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    var _ = new SocialBeliefJob(this._configuration, this._mongo, this._random);
                });
                _socialBeliefsJobThread.Start();
            }
            else
            {
                _log.Info($"SocialBelief is not enabled, skipping.");
            }
        }
        catch (Exception e)
        {
            _log.Trace(e);
        }

        try
        {
            if (this._configuration.Animations.SocialSharing.IsEnabled && this._configuration.Animations.SocialSharing.IsInteracting)
            {
                _log.Info($"Starting SocialSharing...");
                _socialSharingJobThread = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    var _ = new SocialSharingJob(this._configuration, this._mongo, this._random);
                });
                _socialSharingJobThread.Start();
            }
            else
            {
                _log.Info($"SocialSharing is not enabled, skipping.");
            }
        }
        catch (Exception e)
        {
            _log.Trace(e);
        }
    }

    public void Stop()
    {
        Console.WriteLine("Stopping Animations...");
        try
        {
            _socialGraphJobThread?.Interrupt();
        }
        catch
        {
            // ignore
        }

        try
        {
            _socialSharingJobThread?.Interrupt();
        }
        catch
        {
            // ignore
        }

        try
        {
            _socialBeliefsJobThread?.Interrupt();
        }
        catch
        {
            // ignore
        }

        Console.WriteLine("Animations stopped");
    }
}