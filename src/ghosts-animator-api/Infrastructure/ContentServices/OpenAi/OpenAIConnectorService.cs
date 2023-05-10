using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.Models;
using NLog;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices.OpenAi;

public class OpenAIConnectorService
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly OpenAIService _service;
    public bool IsReady { get; set; }

    public OpenAIConnectorService()
    {
        var apiKey = OpenAIHelpers.GetApiKey();
        if (string.IsNullOrEmpty(apiKey))
        {
            _log.Warn("OpenAI enabled, but no API key supplied");
            return;
        }
        
        HttpClientHandler handler;
        if (!string.IsNullOrEmpty(Program.Configuration.Proxy))
        {
            handler = new HttpClientHandler()
            {
                Proxy = new WebProxy(Program.Configuration.Proxy),
                UseProxy = true
            };
        }
        else
        {
            handler = new HttpClientHandler()
            {
                Proxy = new WebProxy(),
                UseProxy = false
            };
        }

        var client = new HttpClient(handler);

        this._service = new OpenAIService(new OpenAiOptions
        {
            ApiKey = apiKey
        }, client);

        this.IsReady = true;
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
            _log.Trace(resp);
            return resp;
        }

        if (completionResult.Error != null)
        {
            _log.Warn(completionResult.Error.Message);
        }

        return string.Empty;
    }
}