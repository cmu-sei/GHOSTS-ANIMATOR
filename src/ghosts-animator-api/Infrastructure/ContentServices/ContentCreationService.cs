// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.ContentServices.Native;
using Ghosts.Animator.Api.Infrastructure.ContentServices.Ollama;
using Ghosts.Animator.Api.Infrastructure.ContentServices.OpenAi;
using Ghosts.Animator.Api.Infrastructure.Extensions;
using Ghosts.Animator.Api.Infrastructure.Models;
using NLog;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices;

public class ContentCreationService
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly ApplicationConfiguration.ContentEngineSettings _configuration;
    private OpenAiFormatterService _openAiFormatterService;
    private OllamaFormatterService _ollamaFormatterService;

    public ContentCreationService(ApplicationConfiguration.ContentEngineSettings configuration)
    {
        _configuration = configuration;
        _configuration.Host = Environment.GetEnvironmentVariable("OLLAMA_HOST") ??
                              configuration.Host;
        _configuration.Model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ??
                               configuration.Model;

        if (_configuration.Source.ToLower() == "openai" && this._openAiFormatterService.IsReady)
            _openAiFormatterService = new OpenAiFormatterService();
        else if (_configuration.Source.ToLower() == "ollama")
            _ollamaFormatterService = new OllamaFormatterService(_configuration);
    }

    public async Task<string> GenerateNextAction(NPC agent, string history)
    {
        var nextAction = string.Empty;
        try
        {
            if (_configuration.Source.ToLower() == "openai" && this._openAiFormatterService.IsReady)
            {
                nextAction = await this._openAiFormatterService.GenerateNextAction(agent, history).ConfigureAwait(false);
            }
            else if (_configuration.Source.ToLower() == "ollama")
            {
                nextAction = await this._ollamaFormatterService.GenerateNextAction(agent, history);
            }

            _log.Info($"{agent.Name}'s next action is: {nextAction}");
        }
        catch (Exception e)
        {
            _log.Error(e);
        }
        return nextAction;
    }
    
    public async Task<string> GenerateTweet(NPC agent)
    {
        string tweetText = null;

        try
        {
            if (_configuration.Source.ToLower() == "openai" && this._openAiFormatterService.IsReady)
            {
                tweetText = await this._openAiFormatterService.GenerateTweet(agent).ConfigureAwait(false);
            }
            else if (_configuration.Source.ToLower() == "ollama")
            {
                tweetText = await this._ollamaFormatterService.GenerateTweet(agent);
                
                var regArray = new [] {"\"activities\": \\[\"([^\"]+)\"", "\"activity\": \"([^\"]+)\"", "'activities': \\['([^\\']+)'\\]", "\"activities\": \\[\"([^\\']+)'\\]"} ;

                foreach (var reg in regArray)
                {
                    var match = Regex.Match(tweetText,reg);
                    if (match.Success)
                    {
                        // Extract the activity
                        tweetText = match.Groups[1].Value;
                        break;
                    }
                }
            }
            
            while (string.IsNullOrEmpty(tweetText))
            {
                tweetText = NativeContentFormatterService.GenerateTweet(agent);
            }

            tweetText = tweetText.ReplaceDoubleQuotesWithSingleQuotes(); // else breaks csv file, //TODO should replace this with a proper csv library

            _log.Info($"{agent.Name} said: {tweetText}");
        }
        catch (Exception e)
        {
            _log.Info(e);
        }
        return tweetText;
    }
}