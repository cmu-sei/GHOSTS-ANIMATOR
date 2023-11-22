using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.Models;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices.Ollama;

public class OllamaFormatterService
{
    private OllamaConnectorService _ollamaConnectorService = new();

    public async Task<string> GenerateTweet(NPC npc)
    {
        var flattenedAgent = GenericContentHelpers.GetFlattenedNPC(npc);

        var prompt = await File.ReadAllTextAsync("config/ContentServices/Ollama/GenerateTweet.txt");
        var messages = new StringBuilder();
        foreach (var p in prompt.Split(System.Environment.NewLine))
        {
            var s = p.Replace("[[flattenedAgent]]", flattenedAgent[..3050]);
            messages.Append(s);    
        }
        
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

        var prompt = await File.ReadAllTextAsync("config/ContentServices/Ollama/GenerateNextAction.txt");
        var messages = new StringBuilder();
        foreach (var p in prompt.Split(System.Environment.NewLine))
        {
            var s = p.Replace("[[flattenedAgent]]", flattenedAgent[..3050]);
            s = s.Replace("[[history]]", history);
            messages.Append(s);    
        }

        return await _ollamaConnectorService.ExecuteQuery(messages.ToString());
    }
}