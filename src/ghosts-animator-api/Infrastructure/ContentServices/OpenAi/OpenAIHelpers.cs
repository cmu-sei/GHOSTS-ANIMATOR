using System.IO;
using System.Linq;
using Ghosts.Animator.Api.Infrastructure.Models;
using Newtonsoft.Json;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices.OpenAi;

public static class OpenAIHelpers
{
    /// <summary>
    /// You'll need a .env file at config/.env with this line:
    /// chatgtp_api_key:your_key_here
    /// </summary>
    /// <returns></returns>
    public static string GetApiKey()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "config", ".env");
        if (!File.Exists(path)) return null;
        var lines = File.ReadAllLines(path);
        return (from line in lines where line.StartsWith("open_ai_api_key") select line.Replace("open_ai_api_key:", "")).FirstOrDefault();
    }
    
    public static string GetFlattenedNPC(NPC agent)
    {
        // squish parts of the json that are irrelevant for LLM & to keep tokens/costs down
        agent.Campaign = null;
        agent.Enclave = null;
        agent.Team = null;
        agent.Rank = null;
        agent.CAC = null;
        agent.PGP = null;
        agent.Unit = null;

        var flattenedAgent = JsonConvert.SerializeObject(agent);
        flattenedAgent = flattenedAgent.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(" \"", "").Replace("\"", "");

        return flattenedAgent;
    }
}