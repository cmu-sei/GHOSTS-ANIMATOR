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
using Newtonsoft.Json;
using NLog;

namespace Ghosts.Animator.Api.Infrastructure.Animations;

public interface IManageableHostedService : IHostedService
{
    new Task StartAsync(CancellationToken cancellationToken);
    new Task StopAsync(CancellationToken cancellationToken);

    Task StartJob(AnimationConfiguration config, CancellationToken cancellationToken);
    Task StopJob(string jobId);
}

public class AnimationConfiguration
{
    public string JobId { get; set; }
    public string JobConfiguration { get; set; }
}

public class AnimationsManager : IManageableHostedService
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
    private Thread _fullAutonomyJobThread;
    
    private CancellationTokenSource _socialSharingJobCancellationTokenSource = new CancellationTokenSource();
    private CancellationTokenSource _socialGraphJobCancellationTokenSource = new CancellationTokenSource();
    private CancellationTokenSource _socialBeliefsJobCancellationTokenSource = new CancellationTokenSource();
    private CancellationTokenSource _chatJobJobCancellationTokenSource = new CancellationTokenSource();
    private CancellationTokenSource _fullAutonomyCancellationTokenSource = new CancellationTokenSource();
    
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
            this._socialGraphJobCancellationTokenSource.Cancel();
            this._socialGraphJobThread?.Join();
        }
        catch
        {
            // ignore
        }

        try
        {
            this._socialSharingJobCancellationTokenSource.Cancel();
            this._socialSharingJobThread?.Join();
        }
        catch
        {
            // ignore
        }

        try
        {
            this._socialSharingJobCancellationTokenSource.Cancel();
            this._socialBeliefsJobThread?.Join();
        }
        catch
        {
            // ignore
        }
        
        try
        {
            this._chatJobJobCancellationTokenSource.Cancel();
            this._chatJobThread?.Join();
        }
        catch
        {
            // ignore
        }
        
        try
        {
            this._fullAutonomyCancellationTokenSource.Cancel();
            this._fullAutonomyJobThread?.Join();
        }
        catch
        {
            // ignore
        }

        _log.Info("Animations stopped.");
        
        return Task.CompletedTask;
    }

    public Task StartJob(AnimationConfiguration config, CancellationToken cancellationToken)
    {
        var jobId = Guid.NewGuid().ToString();
        _log.Info("Animations Manager initializing...");
        Run(config);
        return Task.CompletedTask;
    }

    public Task StopJob(string jobId)
    {
        _log.Info($"Stopping Animation {jobId}...");
        try
        {
            var ct = new CancellationToken();
            switch (jobId.ToUpper())
            {
                case "SOCIALGRAPH":
                    this._socialGraphJobCancellationTokenSource.Cancel();
                    this._socialGraphJobThread?.Join();
                    break;
                case "SOCIALSHARING":
                    this._socialSharingJobCancellationTokenSource.Cancel();
                    this._socialSharingJobThread?.Join();
                    break;
                case "SOCIALBELIEFS":
                    this._socialSharingJobCancellationTokenSource.Cancel();
                    this._socialBeliefsJobThread?.Join();
                    break;
                case "CHAT":
                    this._chatJobJobCancellationTokenSource.Cancel();
                    this._chatJobThread?.Join();
                    break;
                case "FULLAUTONOMY":
                    this._fullAutonomyCancellationTokenSource.Cancel();
                    this._fullAutonomyJobThread?.Join();
                    break;
            }
        }
        catch
        {
            // ignore
        }
        
        _log.Info($"Animation {jobId} stopped.");
        return Task.CompletedTask;
    }

    private void Run(AnimationConfiguration animationConfiguration)
    {
        _log.Info($"Attempting to start {animationConfiguration.JobId}...");
        var settings = _configuration;
        
        var client = new MongoClient(this._databaseSettings.ConnectionString);
        var database = client.GetDatabase(this._databaseSettings.DatabaseName);
        this._mongo = database.GetCollection<NPC>(this._databaseSettings.CollectionNameNPCs);
        
        switch (animationConfiguration.JobId.ToUpper())
        {
            case "SOCIALGRAPH":
                var graphSettings = JsonConvert.DeserializeObject<ApplicationConfiguration.AnimationsSettings.SocialGraphSettings>(animationConfiguration
                    .JobConfiguration);
                settings.Animations.SocialGraph = graphSettings;
                if (graphSettings.IsMultiThreaded)
                {
                    _socialGraphJobThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        _ = new SocialGraphJob(settings, this._mongo, this._random, this._activityHubContext, this._socialGraphJobCancellationTokenSource.Token);
                    });
                    _socialGraphJobThread.Start();
                }
                else
                {
                    _ = new SocialGraphJob(settings, this._mongo, this._random, this._activityHubContext, this._socialGraphJobCancellationTokenSource.Token);
                }
                
                break;
            case "SOCIALSHARING":
                var socialSettings = JsonConvert.DeserializeObject<ApplicationConfiguration.AnimationsSettings.SocialSharingSettings>(animationConfiguration
                    .JobConfiguration);
                settings.Animations.SocialSharing = socialSettings;
                if (socialSettings.IsMultiThreaded)
                {
                    _socialSharingJobThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        _ = new SocialSharingJob(settings, this._mongo, this._random, this._activityHubContext, this._socialSharingJobCancellationTokenSource.Token);
                    });
                    _socialSharingJobThread.Start();
                }
                else
                {
                    _ = new SocialSharingJob(settings, this._mongo, this._random, this._activityHubContext, this._socialSharingJobCancellationTokenSource.Token);
                }
                
                break;
            case "SOCIALBELIEFS":
                var beliefSettings = JsonConvert.DeserializeObject<ApplicationConfiguration.AnimationsSettings.SocialBeliefSettings>(animationConfiguration
                    .JobConfiguration);
                settings.Animations.SocialBelief = beliefSettings;
                if (beliefSettings.IsMultiThreaded)
                {
                    _socialBeliefsJobThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        _ = new SocialBeliefJob(settings, this._mongo, this._random, this._activityHubContext, this._socialBeliefsJobCancellationTokenSource.Token);
                    });
                    _socialBeliefsJobThread.Start();
                }
                else
                {
                    _ = new SocialBeliefJob(settings, this._mongo, this._random, this._activityHubContext, this._socialBeliefsJobCancellationTokenSource.Token);
                }
                
                break;
            case "CHAT":
                var chatSettings = JsonConvert.DeserializeObject<ApplicationConfiguration.AnimationsSettings.ChatSettings>(animationConfiguration
                    .JobConfiguration);
                settings.Animations.Chat = chatSettings;
                if (chatSettings.IsMultiThreaded)
                {
                    _chatJobThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        _ = new ChatJob(settings, this._mongo, this._random, this._activityHubContext, this._chatJobJobCancellationTokenSource.Token);
                    });
                    _chatJobThread.Start();
                }
                else
                {
                    _ = new ChatJob(settings, this._mongo, this._random, this._activityHubContext, this._chatJobJobCancellationTokenSource.Token);
                }
                
                break;
            case "FULLATONOMY":
                var autonomySettings = JsonConvert.DeserializeObject<ApplicationConfiguration.AnimationsSettings.FullAutonomySettings>(animationConfiguration
                    .JobConfiguration);
                settings.Animations.FullAutonomy = autonomySettings;
                if (autonomySettings.IsMultiThreaded)
                {
                    _fullAutonomyJobThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        _ = new FullAutonomyJob(settings, this._mongo, this._random, this._activityHubContext, this._fullAutonomyCancellationTokenSource.Token);
                    });
                    _fullAutonomyJobThread.Start();
                }
                else
                {
                    _ = new FullAutonomyJob(settings, this._mongo, this._random, this._activityHubContext, this._fullAutonomyCancellationTokenSource.Token);
                }
                
                break;
        }
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
                        _ = new SocialGraphJob(this._configuration, this._mongo, this._random, this._activityHubContext, this._socialGraphJobCancellationTokenSource.Token);
                    });
                    _socialGraphJobThread.Start();
                }
                else
                {
                    _ = new SocialGraphJob(this._configuration, this._mongo, this._random, this._activityHubContext, this._socialGraphJobCancellationTokenSource.Token);
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
                        _ = new SocialBeliefJob(this._configuration, this._mongo, this._random, this._activityHubContext, this._socialBeliefsJobCancellationTokenSource.Token);
                    });
                    _socialBeliefsJobThread.Start();
                }
                else
                {
                    _ = new SocialBeliefJob(this._configuration, this._mongo, this._random, this._activityHubContext, this._socialBeliefsJobCancellationTokenSource.Token);
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
                        _ = new ChatJob(this._configuration, this._mongo, this._random, this._activityHubContext, this._chatJobJobCancellationTokenSource.Token);
                    });
                    _chatJobThread.Start();
                }
                else
                {
                    _ = new ChatJob(this._configuration, this._mongo, this._random, this._activityHubContext, this._chatJobJobCancellationTokenSource.Token);
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
                        _ = new SocialSharingJob(this._configuration, this._mongo, this._random, this._activityHubContext, this._socialSharingJobCancellationTokenSource.Token);
                    });
                    _socialSharingJobThread.Start();
                }
                else
                {
                    _ = new SocialSharingJob(this._configuration, this._mongo, this._random, this._activityHubContext, this._socialSharingJobCancellationTokenSource.Token);
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
                    _fullAutonomyJobThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        _ = new FullAutonomyJob(this._configuration, this._mongo, this._random, this._activityHubContext, this._fullAutonomyCancellationTokenSource.Token);
                    });
                    _fullAutonomyJobThread.Start();
                }
                else
                {
                    _ = new FullAutonomyJob(this._configuration, this._mongo, this._random, this._activityHubContext, this._fullAutonomyCancellationTokenSource.Token);
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