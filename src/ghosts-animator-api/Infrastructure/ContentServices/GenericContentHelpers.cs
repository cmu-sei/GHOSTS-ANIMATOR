// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using Ghosts.Animator.Api.Infrastructure.Models;
using Newtonsoft.Json;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices;

public class GenericContentHelpers
{
    public static string GetFlattenedNpc(NPC agent)
    {
        // squish parts of the json that are irrelevant for LLM & to keep tokens/costs down
        agent.Campaign = null;
        agent.Enclave = null;
        agent.Team = null;
        agent.Rank = null;
        agent.CAC = null;
        agent.PGP = null;
        agent.Unit = null;

        var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        var flattenedAgent = JsonConvert.SerializeObject(agent, settings);
        flattenedAgent = flattenedAgent.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(" \"", "").Replace("\"", "");

        return flattenedAgent;
    }
}