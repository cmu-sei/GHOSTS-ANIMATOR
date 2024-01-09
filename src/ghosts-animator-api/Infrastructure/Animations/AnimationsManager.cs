// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Threading;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Hubs;
using Ghosts.Animator.Api.Infrastructure.Animations.AnimationDefinitions;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using NLog;

namespace Ghosts.Animator.Api.Infrastructure.Animations;

public class AnimationsManager : IHostedService
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly DatabaseSettings.IApplicationDatabaseSettings _databaseSettings;
    private readonly ApplicationConfiguration _configuration;
    private IMongoCollection<NPC> _mongo;
    private readonly Random _random;
    private Thread _socialSharingJobThread;
    private Thread _socialGraphJobThread;
    private Thread _socialBeliefsJobThread;
    private Thread _chatJobThread;
    
    private readonly IHubContext<ActivityHub> _activityHubContext;

    public AnimationsManager(IHubContext<ActivityHub> activityHubContext, DatabaseSettings.IApplicationDatabaseSettings settings)
    {
        // ReSharper disable once ConvertToPrimaryConstructor
        this._databaseSettings = settings;
        this._configuration = Program.Configuration;
        this._random = Random.Shared;
        this._activityHubContext = activityHubContext;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _log.Info("Animations Manager initializing...");
        Run();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _log.Info("Stopping Animations...");
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
        
        try
        {
            _chatJobThread?.Interrupt();
        }
        catch
        {
            // ignore
        }

        _log.Info("Animations stopped.");
        
        return Task.CompletedTask;
    }

    private void Run()
    {
        if (!this._configuration.Animations.IsEnabled)
        {
            _log.Info($"Animations are not enabled, exiting...");
            return;
        }

        _log.Info($"Animations are enabled, starting up...");

        var client = new MongoClient(this._databaseSettings.ConnectionString);
        var database = client.GetDatabase(this._databaseSettings.DatabaseName);
        this._mongo = database.GetCollection<NPC>(this._databaseSettings.CollectionNameNPCs);
        
        try
        {
            if (this._configuration.Animations.SocialGraph.IsEnabled && this._configuration.Animations.SocialGraph.IsInteracting)
            {
                _log.Info($"Starting SocialGraph...");
                if (this._configuration.Animations.SocialGraph.IsMultiThreaded)
                {
                    _socialGraphJobThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        _ = new SocialGraphJob(this._configuration, this._mongo, this._random, this._activityHubContext);
                    });
                    _socialGraphJobThread.Start();
                }
                else
                {
                    _ = new SocialGraphJob(this._configuration, this._mongo, this._random, this._activityHubContext);
                }
            }
            else
            {
                _log.Info($"SocialGraph is not enabled.");
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
                if (this._configuration.Animations.SocialBelief.IsMultiThreaded)
                {
                    _socialBeliefsJobThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        _ = new SocialBeliefJob(this._configuration, this._mongo, this._random, this._activityHubContext);
                    });
                    _socialBeliefsJobThread.Start();
                }
                else
                {
                    _ = new SocialBeliefJob(this._configuration, this._mongo, this._random, this._activityHubContext);
                }
            }
            else
            {
                _log.Info($"SocialBelief is not enabled.");
            }
        }
        catch (Exception e)
        {
            _log.Trace(e);
        }
        
        try
        {
            if (this._configuration.Animations.Chat.IsEnabled && this._configuration.Animations.Chat.IsInteracting)
            {
                _log.Info($"Starting chat job...");
                if (this._configuration.Animations.Chat.IsMultiThreaded)
                {
                    _chatJobThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        _ = new ChatJob(this._configuration, this._mongo, this._random, this._activityHubContext);
                    });
                    _chatJobThread.Start();
                }
                else
                {
                    _ = new ChatJob(this._configuration, this._mongo, this._random, this._activityHubContext);
                }
            }
            else
            {
                _log.Info($"Chat job is not enabled.");
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
                if (this._configuration.Animations.SocialSharing.IsMultiThreaded)
                {
                    _socialSharingJobThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        _ = new SocialSharingJob(this._configuration, this._mongo, this._random, this._activityHubContext);
                    });
                    _socialSharingJobThread.Start();
                }
                else
                {
                    _ = new SocialSharingJob(this._configuration, this._mongo, this._random, this._activityHubContext);
                }
            }
            else
            {
                _log.Info($"SocialSharing is not enabled.");
            }
        }
        catch (Exception e)
        {
            _log.Trace(e);
        }
        
        try
        {
            if (this._configuration.Animations.FullAutonomy.IsEnabled && this._configuration.Animations.FullAutonomy.IsInteracting)
            {
                _log.Info($"Starting FullAutonomy...");
                if (this._configuration.Animations.FullAutonomy.IsMultiThreaded)
                {
                    _socialSharingJobThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        _ = new FullAutonomyJob(this._configuration, this._mongo, this._random, this._activityHubContext);
                    });
                    _socialSharingJobThread.Start();
                }
                else
                {
                    _ = new FullAutonomyJob(this._configuration, this._mongo, this._random, this._activityHubContext);
                }
            }
            else
            {
                _log.Info($"FullAutonomy is not enabled.");
            }
        }
        catch (Exception e)
        {
            _log.Trace(e);
        }
    }
}