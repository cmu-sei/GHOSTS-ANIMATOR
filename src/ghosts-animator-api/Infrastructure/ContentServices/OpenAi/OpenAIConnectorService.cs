using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.Models;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices.OpenAi;

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

    //public async Task<string> GenerateDocumentContent(NPC npc)
    //public async Task<string> GenerateExcelContent(NPC npc)
    //public async Task<string> GeneratePowerPointContent(NPC npc)

    //public async Task<string> GenerateEmail(NPC npc)
    
    public async Task<string> GeneratePowershellScript(NPC npc)
    {
        var flattenedAgent = OpenAIHelpers.GetFlattenedNPC(npc);

        var messages = new List<ChatMessage>
        {
            ChatMessage.FromSystem($"Given this json information about a person: ```{flattenedAgent[..3050]}```"),
            ChatMessage.FromSystem("Generate a relevant powershell script for this person to execute on their windows computer")
        };

        return await ExecuteQuery(messages);
    }
    
    public async Task<string> GenerateCommand(NPC npc)
    {
        var flattenedAgent = OpenAIHelpers.GetFlattenedNPC(npc);

        var messages = new List<ChatMessage>
        {
            ChatMessage.FromSystem($"Given this json information about a person: ```{flattenedAgent[..3050]}```"),
            ChatMessage.FromSystem("Generate a relevant command-line command for this person to execute on their windows computer")
        };

        return await ExecuteQuery(messages);
    }

    public async Task<string> GenerateTweet(NPC npc)
    {
        var flattenedAgent = OpenAIHelpers.GetFlattenedNPC(npc);

        var messages = new List<ChatMessage>
        {
            ChatMessage.FromSystem($"Given this json information about a person ```{flattenedAgent[..3050]}```"),
            ChatMessage.FromSystem("Provide one or two relevant hashtags, if they add value to the tweet"),
            ChatMessage.FromSystem("Avoid always starting the tweet with the word \"just\" or inferring that the person just did something"),
            ChatMessage.FromSystem("Consider the person's interests, activities, or general thoughts when crafting the tweet"),
            ChatMessage.FromSystem("Write something the provided person might tweet")
        };

        return await ExecuteQuery(messages);
    }

    //TODO: shouldn't this method save off every request and response somewhere?
    private async Task<string> ExecuteQuery(IList<ChatMessage> messages)
    {
        var completionResult = await this._service.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = OpenAI.GPT3.ObjectModels.Models.Gpt_4,
            MaxTokens = 50 //optional
            //Temperature = 0.7 //optional
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