using System;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.ContentServices.Native;
using Ghosts.Animator.Api.Infrastructure.ContentServices.OpenAi;
using Ghosts.Animator.Api.Infrastructure.Extensions;
using Ghosts.Animator.Api.Infrastructure.Models;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices;

public class ContentCreationService
{
    private readonly OpenAIConnectorService _llmConnectorService;

    public ContentCreationService()
    {
        _llmConnectorService = new OpenAIConnectorService();
    }

    public async Task<string> GenerateTweet(NPC agent)
    {
        string tweetText = null;

        if (Program.Configuration.Animations.SocialSharing.IsChatGptEnabled)
        {
            tweetText = await this._llmConnectorService.GenerateTweet(agent).ConfigureAwait(false);
        }

        while (string.IsNullOrEmpty(tweetText))
        {
            tweetText = NativeContentConnectorService.GenerateTweet(agent);
        }

        tweetText = tweetText.ReplaceDoubleQuotesWithSingleQuotes(); // else breaks csv file

        Console.WriteLine($"{agent.Name} said: {tweetText}");
        return tweetText;
    }
}