using System;
using System.Linq;
using System.Net;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Api.Infrastructure.Social;
using Ghosts.Animator.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace Ghosts.Animator.Api.Controllers;

[Controller]
[Produces("application/json")]
[Route("/[controller]")]
public class SocialGraphController : Controller
{
    private readonly IMongoCollection<NPC> _mongo;
        
    public SocialGraphController(DatabaseSettings.IApplicationDatabaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _mongo = database.GetCollection<NPC>(settings.CollectionNameNPCs);
    }

    
    /// <summary>
    /// Get NPC's social graph by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(SocialGraph), (int) HttpStatusCode.OK)]
    [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(NpcProfile))]
    [SwaggerOperation("getSocialGraphById")]
    [HttpGet("{id:guid}")]
    public SocialGraph GetById(Guid id)
    {
        var graph = new SocialGraph();
        graph.Id = id;
        var list = _mongo.Find(x => true).ToList().OrderBy(o => o.Enclave).ThenBy(o=>o.Team);
        
        NPC previousNpc = null;
        var enclave = string.Empty;
        var team = string.Empty;
            
        foreach (var npc in list)
        {
            var connection = new SocialGraph.SocialConnection();
            connection.Id = npc.Id;
            connection.Name = npc.Name.ToString();
            
            if (previousNpc == null)
            {
                connection.Distance = npc.Campaign;
            }
            else if (previousNpc?.Enclave != npc.Enclave)
            {
                enclave = npc.Enclave;
                connection.Distance = npc.Enclave;
                continue;
            }
            else if (string.IsNullOrEmpty(team) || previousNpc?.Team != npc.Team)
            {
                team = $"{enclave}/{npc.Team}";
                connection.Distance = team;
                continue;
            }

            connection.Distance = team;
            graph.Connections.Add(connection);
            previousNpc = npc;
        }

        return graph;
    }
}