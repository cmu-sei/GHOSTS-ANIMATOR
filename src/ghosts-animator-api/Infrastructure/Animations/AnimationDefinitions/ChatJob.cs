// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using Ghosts.Animator.Api.Hubs;
using Ghosts.Animator.Api.Infrastructure.Animations.AnimationDefinitions.Chat;
using Ghosts.Animator.Api.Infrastructure.ContentServices.Ollama;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Extensions;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using NLog;
using System.Text.Json;

namespace Ghosts.Animator.Api.Infrastructure.Animations.AnimationDefinitions;

public class ChatJob
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly ApplicationConfiguration _configuration;
    private readonly IMongoCollection<NPC> _mongo;
    private readonly Random _random;
    private readonly ChatClient _chatClient;
    private readonly int _currentStep;
    
    public ChatJob(ApplicationConfiguration configuration, IMongoCollection<NPC> mongo, Random random,
        IHubContext<ActivityHub> activityHubContext)
    {
        //todo: post results to activityHubContext for "top" reporting
        
        this._configuration = configuration;
        this._random = random;
        this._mongo = mongo;

        var chatConfiguration = JsonSerializer.Deserialize<ChatJobConfiguration>(File.ReadAllText("config/chat.json"),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidOperationException();

        var llm = new OllamaConnectorService(_configuration.Animations.Chat.ContentEngine);
        this._chatClient = new ChatClient(chatConfiguration);
        
        while (true)
        {
            if (this._currentStep > _configuration.Animations.Chat.MaximumSteps)
            {
                _log.Trace($"Maximum steps met: {this._currentStep - 1}. Chat Job is exiting...");
                return;
            }

            this.Step(llm, random, chatConfiguration);
            Thread.Sleep(this._configuration.Animations.SocialSharing.TurnLength);

            this._currentStep++;
        }
    }

    private async void Step(OllamaConnectorService llm, Random random, ChatJobConfiguration chatConfiguration)
    {
        _log.Trace("Executing a chat step...");
        var agents = this._mongo.Find(x => true).ToList().Shuffle(_random).Take(chatConfiguration.Chat.AgentsPerBatch);
        await this._chatClient.Step(llm, random, agents);
    }
}