using System;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.ContentServices.Native;
using Ghosts.Animator.Api.Infrastructure.ContentServices.OpenAi;
using Ghosts.Animator.Api.Infrastructure.Extensions;
using Ghosts.Animator.Api.Infrastructure.Models;
using NLog;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices;

public class ContentCreationService
{
    private readonly OpenAIConnectorService _llmConnectorService;
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();

    public ContentCreationService()
    {
        _llmConnectorService = new OpenAIConnectorService();
    }

    public async Task<string> GenerateNextAction(NPC agent, string history)
    {
        var nextAction = string.Empty;
        try
        {
            if (Program.Configuration.Animations.FullAutonomy.IsChatGptEnabled && this._llmConnectorService.IsReady)
            {
                nextAction = await this._llmConnectorService.GenerateNextAction(agent, history).ConfigureAwait(false);
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
            if (Program.Configuration.Animations.SocialSharing.IsChatGptEnabled && this._llmConnectorService.IsReady)
            {
                tweetText = await this._llmConnectorService.GenerateTweet(agent).ConfigureAwait(false);
            }

            while (string.IsNullOrEmpty(tweetText))
            {
                tweetText = NativeContentConnectorService.GenerateTweet(agent);
            }

            tweetText = tweetText.ReplaceDoubleQuotesWithSingleQuotes(); // else breaks csv file

            Console.WriteLine($"{agent.Name} said: {tweetText}");
        }
        catch (Exception e)
        {
            _log.Info(e);
        }
        return tweetText;
    }
}