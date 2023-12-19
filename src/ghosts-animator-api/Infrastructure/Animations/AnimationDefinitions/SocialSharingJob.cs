// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Ghosts.Animator.Api.Hubs;
using Ghosts.Animator.Api.Infrastructure.ContentServices;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Extensions;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using NLog;
using RestSharp;

namespace Ghosts.Animator.Api.Infrastructure.Animations.AnimationDefinitions;

public class SocialSharingJob
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly ApplicationConfiguration _configuration;
    private readonly IMongoCollection<NPC> _mongo;
    private readonly Random _random;
    private const string SavePath = "output/socialsharing/";
    private readonly int CurrentStep;
    private readonly IHubContext<ActivityHub> _activityHubContext;

    public SocialSharingJob(ApplicationConfiguration configuration, IMongoCollection<NPC> mongo, Random random, IHubContext<ActivityHub> activityHubContext)
    {
        try
        {
            this._activityHubContext = activityHubContext;
            this._configuration = configuration;
            this._random = random;
            this._mongo = mongo;
            

            if (_configuration.Animations.SocialSharing.IsInteracting)
            {
                if (!Directory.Exists(SavePath))
                {
                    Directory.CreateDirectory(SavePath);
                }

                while (true)
                {
                    if (this.CurrentStep > _configuration.Animations.SocialSharing.MaximumSteps)
                    {
                        _log.Trace($"Maximum steps met: {this.CurrentStep - 1}. Social sharing is exiting...");
                        return;
                    }

                    this.Step();
                    Thread.Sleep(this._configuration.Animations.SocialSharing.TurnLength);

                    this.CurrentStep++;
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
        //take some random NPCs
        var lines = new StringBuilder();
        var agents = this._mongo.Find(x => true).ToList().Shuffle(_random).Take(_random.Next(5, 20));
        foreach (var agent in agents)
        {
            var tweetText = await contentService.GenerateTweet(agent);
            if (string.IsNullOrEmpty(tweetText))
                return;
            
            lines.AppendFormat($"{DateTime.Now},{agent.Id},\"{tweetText}\"{Environment.NewLine}");

            var userFormValue = new[] { "user", "usr", "u", "uid", "user_id", "u_id" }.RandomFromStringArray();
            var messageFormValue = new[] { "message", "msg", "m", "message_id", "msg_id", "msg_text", "text", "payload" }.RandomFromStringArray();
            
            if (_configuration.Animations.SocialSharing.IsSendingTimelinesDirectToSocializer &&
                !string.IsNullOrEmpty(_configuration.GhostsApiUrl))
            {
                var client = new RestClient(_configuration.Animations.SocialSharing.SocializerUrl);
                var request = new RestRequest("/", Method.Post)
                {
                    RequestFormat = DataFormat.Json
                };
                request.AddParameter(userFormValue, agent.Name.ToString());
                request.AddParameter(messageFormValue, tweetText);
                
                try
                {
                    var response = client.Execute(request);
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw (new Exception($"Socializer responded with {response.StatusCode} to the request agent: {agent.Name} text: {tweetText}"));
                    }
                }
                catch (Exception e)
                {
                    _log.Error($"Could not post timeline command to Socializer {_configuration.Animations.SocialSharing.SocializerUrl}: {e}");
                }
            }
            
            if (_configuration.Animations.SocialSharing.IsSendingTimelinesToGhostsApi && !string.IsNullOrEmpty(_configuration.GhostsApiUrl))
            {
                //{\"user\":\"{user}\",\"message\":\"{message}}
                var formValues = new StringBuilder();
                formValues.Append('{')
                    .Append("\\\"").Append(userFormValue).Append("\\\":\\\"").Append(agent.Email).Append("\\\"")
                    .Append(",\\\"").Append(messageFormValue).Append("\\\":\\\"").Append(tweetText).Append("\\\"");
                for (var i = 0; i < AnimatorRandom.Rand.Next(0, 6); i++)
                {
                    formValues
                        .Append(",\\\"").Append(Lorem.GetWord().ToLower()).Append("\\\":\\\"").Append(AnimatorRandom.Rand.NextDouble()).Append("\\\"");
                }

                formValues.Append('}');
                
                var postPayload = File.ReadAllText("config/socializer_post.json");
                postPayload = postPayload.Replace("{id}", Guid.NewGuid().ToString());
                postPayload = postPayload.Replace("{user}", agent.Email);
                postPayload = postPayload.Replace("{payload}", formValues.ToString());
                postPayload = postPayload.Replace("{url}", _configuration.Animations.SocialSharing.SocializerUrl);
                postPayload = postPayload.Replace("{now}", DateTime.Now.ToString(CultureInfo.InvariantCulture));

                _log.Trace(postPayload);

                var client = new RestClient(_configuration.GhostsApiUrl);
                var request = new RestRequest("api/machineupdates", Method.Post)
                {
                    RequestFormat = DataFormat.Json
                };
                request.AddBody(postPayload);

                try
                {
                    var response = client.Execute(request);
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw (new Exception($"ghosts api responded with {response.StatusCode}"));
                }
                catch (Exception e)
                {
                    _log.Error($"Could not post timeline command to ghosts api {_configuration.GhostsApiUrl}: {e}");
                }
            }
            
            //post to hub
            await this._activityHubContext.Clients.All.SendAsync("show",
                "1",
                agent.Id.ToString(),
                "social",
                tweetText,
                DateTime.Now.ToString(CultureInfo.InvariantCulture)
            );
        }

        await File.AppendAllTextAsync($"{SavePath}tweets.csv", lines.ToString());
    }
}