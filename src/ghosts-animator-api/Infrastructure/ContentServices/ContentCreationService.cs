using System;
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
    private OpenAiFormatterService _openAiFormatterService = new();
    private OllamaFormatterService _ollamaFormatterService = new();
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();

    public async Task<string> GenerateNextAction(NPC agent, string history)
    {
        var nextAction = string.Empty;
        try
        {
            if (Program.Configuration.Animations.FullAutonomy.ContentEngine.Source.ToLower() == "openai" && this._openAiFormatterService.IsReady)
            {
                nextAction = await this._openAiFormatterService.GenerateNextAction(agent, history).ConfigureAwait(false);
            }
            else if (Program.Configuration.Animations.FullAutonomy.ContentEngine.Source.ToLower() == "ollama")
            {
                nextAction = await this._ollamaFormatterService.GenerateNextAction(agent, history);
            }

            Console.WriteLine($"{agent.Name}'s next action is: {nextAction}");
        }
        catch (Exception e)
        {
            _log.Info(e);
        }
        return nextAction;
    }
    
    public async Task<string> GenerateTweet(NPC agent)
    {
        string tweetText = null;

        try
        {
            while (string.IsNullOrEmpty(tweetText))
            {
                tweetText = NativeContentFormatterService.GenerateTweet(agent);
            }

            tweetText = tweetText.ReplaceDoubleQuotesWithSingleQuotes(); // else breaks csv file, //TODO should replace this with a proper csv library

            Console.WriteLine($"{agent.Name} said: {tweetText}");
        }
        catch (Exception e)
        {
            _log.Info(e);
        }
        return tweetText;
    }
}