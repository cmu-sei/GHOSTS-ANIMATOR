using Ghosts.Animator.Api.Infrastructure.Models;
using Newtonsoft.Json;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices;

public class GenericContentHelpers
{
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