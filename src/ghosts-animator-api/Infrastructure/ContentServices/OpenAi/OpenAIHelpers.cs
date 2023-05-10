using System;
using System.IO;
using System.Linq;
using Ghosts.Animator.Api.Infrastructure.Models;
using Newtonsoft.Json;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices.OpenAi;

public static class OpenAIHelpers
{
    /// <summary>
    /// You'll need to supply your openAi api key via an environment variable
    /// </summary>
    public static string GetApiKey()
    {
        return Environment.GetEnvironmentVariable("OPEN_AI_API_KEY");
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