using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.Models;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices.OpenAi;

public class OpenAIFormatterService
{
    private OpenAIConnectorService _openAIConnectorService;
    
    public bool IsReady { get; set; }
    
    public OpenAIFormatterService()
    {
        _openAIConnectorService = new OpenAIConnectorService();
        this.IsReady = _openAIConnectorService.IsReady;
    }
    
    public async Task<string> GenerateTweet(NPC npc)
    {
        var flattenedAgent = GenericContentHelpers.GetFlattenedNPC(npc);

        var messages = new List<ChatMessage>
        {
            ChatMessage.FromSystem($"Given this json information about a person ```{flattenedAgent[..3050]}```"),
            ChatMessage.FromSystem("Provide one or two relevant hashtags, if they add value to the tweet"),
            ChatMessage.FromSystem("Avoid always starting the tweet with the word \"just\" or inferring that the person just did something"),
            ChatMessage.FromSystem("Consider the person's interests, activities, or general thoughts when crafting the tweet"),
            ChatMessage.FromSystem("Write something the provided person might tweet")
        };

        return await _openAIConnectorService.ExecuteQuery(messages);
    }
    
    public async Task<string> GenerateNextAction(NPC npc, string history)
    {
        var flattenedAgent = GenericContentHelpers.GetFlattenedNPC(npc);

        Console.WriteLine($"{npc.Name} with {history.Length} history records");

        var messages = new List<ChatMessage>
        {
            ChatMessage.FromSystem($"Given this json information about a person ```{flattenedAgent[..3050]}```"),
            ChatMessage.FromSystem($"And that they've recently done the following: ```{history}```"),
            ChatMessage.FromSystem($"And that they can use any program on a computer."),
            ChatMessage.FromSystem(
                "What might they do—given who they are, what role they play within the organization, and what a realistic representation of this person might actually do?"),
            ChatMessage.FromSystem("People do a wide array of activities in a day, and we want to mimic that."),
            ChatMessage.FromSystem("People sometimes do irrational things, and we need to account for this."),
            ChatMessage.FromSystem("Consider that 14 times a year there is a full moon, and we need to account for this."),
            ChatMessage.FromSystem(
                "Consider that periodically someone will do something on their computer that is not allowed by company policy—they will do this by mistake or intentionally."),
            ChatMessage.FromSystem("Consider that people often do things off-line that influence their online actions."),
            ChatMessage.FromSystem(
                "Just tell me what they do, there is no need to tell me about what data I sent you, just reply with the action and why you chose that action IN ONE SENTENCE."),
            ChatMessage.FromSystem("Do not use word, or excel. what would this person do?"),
        };

        return await _openAIConnectorService.ExecuteQuery(messages);
    }
    
    public async Task<string> GeneratePowershellScript(NPC npc)
    {
        var flattenedAgent = GenericContentHelpers.GetFlattenedNPC(npc);

        var messages = new List<ChatMessage>
        {
            ChatMessage.FromSystem($"Given this json information about a person: ```{flattenedAgent[..3050]}```"),
            ChatMessage.FromSystem("Generate a relevant powershell script for this person to execute on their windows computer")
        };

        return await _openAIConnectorService.ExecuteQuery(messages);
    }

    public async Task<string> GenerateCommand(NPC npc)
    {
        var flattenedAgent = GenericContentHelpers.GetFlattenedNPC(npc);

        var messages = new List<ChatMessage>
        {
            ChatMessage.FromSystem($"Given this json information about a person: ```{flattenedAgent[..3050]}```"),
            ChatMessage.FromSystem("Generate a relevant command-line command for this person to execute on their windows computer")
        };

        return await _openAIConnectorService.ExecuteQuery(messages);
    }
    
    //public async Task<string> GenerateDocumentContent(NPC npc)
    //public async Task<string> GenerateExcelContent(NPC npc)
    //public async Task<string> GeneratePowerPointContent(NPC npc)
    //public async Task<string> GenerateEmail(NPC npc)
}