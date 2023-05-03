using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.Models;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace Ghosts.Animator.Api.Infrastructure.OpenAi;

public class OpenAIConnectorService
{
    private readonly OpenAIService _service;

    public OpenAIConnectorService()
    {
        this._service = new OpenAIService(new OpenAiOptions
        {
            ApiKey = OpenAIHelpers.GetApiKey()
        });
    }

    public async Task<string> GenerateTweet(NPC npc)
    {
        var flattenedAgent = OpenAIHelpers.GetFlattenedNPC(npc);
        
        var completionResult = await this._service.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem($"Given this json file about a person ```{flattenedAgent[..3050]}```"),
                ChatMessage.FromSystem("Provide few, if any hashtags"),
                ChatMessage.FromSystem("Don't always start the tweet with the word \"just\", don't infer that the person just did something"),
                ChatMessage.FromUser("Write something the provided person might tweet")
            },
            Model = OpenAI.GPT3.ObjectModels.Models.Gpt_4,
            MaxTokens = 50 //optional
        });
        if (completionResult.Successful)
        {
            var resp = completionResult.Choices.First().Message.Content;
            Console.WriteLine(resp);
            return resp;
        }
        
        if (completionResult.Error != null)
        {
            Console.WriteLine(completionResult.Error.Message);
        }
    
        return string.Empty;
    }
}