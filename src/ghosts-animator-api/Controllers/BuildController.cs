/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace Ghosts.Animator.Api.Controllers
{
    /// <summary>
    /// Build entire team of NPCs for a campaign and enclave
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BuildController : ControllerBase
    {
        private readonly IMongoCollection<NPC> _mongo;
        private readonly IMongoCollection<NPCIpAddress> _mongoIps;

        public BuildController(DatabaseSettings.IApplicationDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _mongo = database.GetCollection<NPC>(settings.CollectionNameNPCs);
            _mongoIps = database.GetCollection<NPCIpAddress>(settings.CollectionNameIPAddresses);
        }

        private FilterDefinition<NPC> BuildTeamFilter(string team, string enclave, string campaign)
        {
            var filter = Builders<NPC>.Filter.And(
                Builders<NPC>.Filter.Eq("Campaign", campaign),
                Builders<NPC>.Filter.Eq("Enclave", enclave),
                Builders<NPC>.Filter.Eq("Team", team));
            return filter;
        }

        /// <summary>
        /// Returns all NPCs at the specified level - Campaign, Enclave, or Team
        /// </summary>
        /// <param name="key">campaign, enclave, team</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(IEnumerable<NPC>), (int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IEnumerable<NPC>))]
        [SwaggerOperation("getKeys")]
        [HttpGet]
        public IEnumerable<string> GetKeys(string key)
        {
            return key.ToLower() switch
            {
                "campaign" => _mongo.Distinct(x => x.Campaign, x => x.Campaign != null, new DistinctOptions()).ToList(),
                "enclave" => _mongo.Distinct(x => x.Enclave, x => x.Enclave != null, new DistinctOptions()).ToList(),
                "team" => _mongo.Distinct(x => x.Team, x => x.Team != null, new DistinctOptions()).ToList(),
                _ => throw new KeyNotFoundException("Invalid key! Key must be campaign, enclave or team")
            };
        }

        /// <summary>
        /// Create NPCs belonging to a campaign, enclave and team based on configuration
        /// </summary>
        /// <param name="config"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(IEnumerable<NPC>), (int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IEnumerable<NPC>))]
        [SwaggerOperation("createBuild")]
        [HttpPost]
        public IEnumerable<NPC> Create(GenerationConfiguration config, CancellationToken ct)
        {
            var t = new Stopwatch();
            t.Start();
            
            var createdNPCs = new List<NPC>();
            foreach (var enclave in config.Enclaves)
            {
                foreach (var team in enclave.Teams)
                {
                    for (var i = 0; i < team.Npcs.Number; i++)
                    {
                        var last = t.ElapsedMilliseconds;
                        var branch = team.Npcs.Configuration.Branch ?? MilitaryUnits.GetServiceBranch();
                        var npc = NPC.TransformTo(Npc.Generate(branch));
                        npc.Team = team.Name;
                        npc.Campaign = config.Campaign;
                        npc.Enclave = enclave.Name;
                        _mongo.InsertOne(npc, cancellationToken: ct);
                        createdNPCs.Add(npc);
                        Console.WriteLine($"{i} generated in {t.ElapsedMilliseconds - last} ms");
                    }
                }
            }
            
            t.Stop();
            Console.WriteLine($"{createdNPCs.Count} NPCs generated in {t.ElapsedMilliseconds} ms");

            return createdNPCs;
        }
    }
}