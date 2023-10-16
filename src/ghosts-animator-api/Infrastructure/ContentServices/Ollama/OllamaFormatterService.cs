using System;
using System.Text;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.Models;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices.Ollama;

public class OllamaFormatterService
{
    private OllamaConnectorService _ollamaConnectorService;

    public OllamaFormatterService()
    {
        _ollamaConnectorService = new OllamaConnectorService();
    }

    public async Task<string> GenerateTweet(NPC npc)
    {
        var flattenedAgent = GenericContentHelpers.GetFlattenedNPC(npc);

        var messages = new StringBuilder();
        messages.Append($"Given this json information about a person ```{flattenedAgent[..3050]}```");
        messages.Append("Provide one or two relevant hashtags, if they add value to the tweet");
        messages.Append(
            "Avoid always starting the tweet with the word \"just\" or inferring that the person just did something");
        messages.Append("Consider the person's interests, activities, or general thoughts when crafting the tweet");
        messages.Append("Write something the provided person might tweet");

        return await _ollamaConnectorService.ExecuteQuery(messages.ToString());
    }

    public async Task<string> GenerateNextAction(NPC npc, string history)
    {
        var flattenedAgent = GenericContentHelpers.GetFlattenedNPC(npc);
        if (flattenedAgent.Length > 3050)
        {
            flattenedAgent = flattenedAgent[..3050];
        }

        Console.WriteLine($"{npc.Name} with {history.Length} history records");

        var messages = new StringBuilder();
        messages.Append($"Given this json information about a person ```{flattenedAgent}```");
        messages.Append($"And that they've recently done the following: ```{history}```");
        messages.Append("And that they can use any program on a computer.");
        messages.Append("What might they do—given who they are, what role they play within the organization, and what a realistic representation of this person might actually do?");
        messages.Append("People do a wide array of activities in a day, and we want to mimic that.");
        messages.Append("People sometimes do irrational things, and we need to account for this.");
        messages.Append("Consider that 14 times a year there is a full moon, and we need to account for this.");
        messages.Append("Consider that periodically someone will do something on their computer that is not allowed by company policy—they will do this by mistake or intentionally.");
        messages.Append("Consider that people often do things off-line that influence their online actions.");
        messages.Append("Just tell me what they do, there is no need to tell me about what data I sent you, just reply with the action and why you chose that action IN ONE SENTENCE.");
        messages.Append("Do not use word, or excel. what would this person do?");

        return await _ollamaConnectorService.ExecuteQuery(messages.ToString());
    }
}