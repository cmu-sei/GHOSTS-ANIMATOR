// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Ghosts.Animator.Api.Hubs;
using Ghosts.Animator.Api.Infrastructure.ContentServices;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Extensions;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using NLog;

namespace Ghosts.Animator.Api.Infrastructure.Animations.AnimationDefinitions;

public class FullAutonomyJob
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly ApplicationConfiguration _configuration;
    private readonly IMongoCollection<NPC> _mongo;
    private readonly Random _random;
    private const string SavePath = "output/fullautonomy/";
    private readonly string _historyFile = $"{SavePath}/history.txt";
    private readonly List<string> _history;
    private readonly int _currentStep;
    private readonly IHubContext<ActivityHub> _activityHubContext;

    public FullAutonomyJob(ApplicationConfiguration configuration, IMongoCollection<NPC> mongo, Random random,
        IHubContext<ActivityHub> activityHubContext)
    {
        try
        {
            this._activityHubContext = activityHubContext;
            this._configuration = configuration;
            this._random = random;
            this._mongo = mongo;

            this._history = File.Exists(this._historyFile)
                ? File.ReadAllLinesAsync(this._historyFile).Result.ToList()
                : new List<string>();

            if (_configuration.Animations.SocialSharing.IsInteracting)
            {
                if (!Directory.Exists(SavePath))
                {
                    Directory.CreateDirectory(SavePath);
                }

                while (true)
                {
                    if (this._currentStep > _configuration.Animations.SocialSharing.MaximumSteps)
                    {
                        _log.Trace($"Maximum steps met: {this._currentStep - 1}. Full Autonomy is exiting...");
                        return;
                    }

                    this.Step();
                    Thread.Sleep(this._configuration.Animations.SocialSharing.TurnLength);

                    this._currentStep++;
                }
            }
        }
        catch (ThreadInterruptedException)
        {
            // continue
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async void Step()
    {
        var contentService = new ContentCreationService();

        var agents = this._mongo.Find(x => true).ToList().Shuffle(_random).Take(_random.Next(5, 20));
        foreach (var agent in agents)
        {
            var history = this._history.Where(x => x.StartsWith(agent.Id.ToString()));
            var nextAction = await contentService.GenerateNextAction(agent, string.Join('\n', history));

            var line = $"{agent.Id}|{nextAction}|{DateTime.UtcNow}";
            line = $"{line.Replace(Environment.NewLine, "")}\n";

            await File.AppendAllTextAsync(_historyFile, line);
            this._history.Add(line);

            Thread.Sleep(500);

            // post to hub
            await this._activityHubContext.Clients.All.SendAsync("show",
                "1",
                agent.Id.ToString(),
                "social",
                nextAction,
                DateTime.Now.ToString(CultureInfo.InvariantCulture)
            );
        }

        await File.AppendAllTextAsync($"{SavePath}tweets.csv", this._history.ToString());
    }
}