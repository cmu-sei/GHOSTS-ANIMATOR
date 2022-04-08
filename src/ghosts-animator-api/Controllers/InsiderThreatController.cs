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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FileHelpers;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Ghosts.Animator.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class InsiderThreatController : ControllerBase
    {
        private readonly IMongoCollection<NPC> _mongo;
        
        public InsiderThreatController(DatabaseSettings.IApplicationDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _mongo = database.GetCollection<NPC>(settings.CollectionNameNPCs);
        }
        
        /// <summary>
        /// Create an insider threat specific NPC build
        /// </summary>
        /// <param name="config"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(IEnumerable<NPC>), (int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IEnumerable<NPC>))]
        [SwaggerRequestExample(typeof(InsiderThreatGenerationConfiguration), typeof(InsiderThreatGenerationConfigurationExample))]
        [SwaggerOperation("createInsiderThreatBuild")]
        [HttpPost]
        public IEnumerable<NPC> Create(InsiderThreatGenerationConfiguration config, CancellationToken ct)
        {
            var createdNPCs = new List<NPC>();
            foreach (var enclave in config.Enclaves)
            {
                foreach (var team in enclave.Teams)
                {
                    for (var i = 0; i < team.Npcs.Number; i++)
                    {
                        var branch = team.Npcs.Configuration.Branch ?? MilitaryUnits.GetServiceBranch();
                        var npc = NPC.TransformTo(Npc.Generate(branch));
                        npc.Team = team.Name;
                        npc.Campaign = config.Campaign;
                        npc.Enclave = enclave.Name;
                        createdNPCs.Add(npc);
                    }
                }
            }
            
            foreach (var npc in createdNPCs)
            {
                foreach (var job in npc.Employment.EmploymentRecords)
                {
                    //get same company departments and highest ranked in that department

                    var managerList = createdNPCs.Where(x => x.Id != npc.Id 
                                                             && x.Employment.EmploymentRecords.Any(
                                                                 o => o.Company == job.Company 
                                                                            && o.Department == job.Department 
                                                                            && o.Level >= job.Level)).ToList();

                    if (managerList.Any())
                    {
                        var manager = managerList.RandomElement();
                        job.Manager = manager.Id;
                    }
                }
            }

            _mongo.InsertMany(createdNPCs, new InsertManyOptions {IsOrdered = false}, ct);
            return createdNPCs;
        }

        /// <summary>
        /// Export insider threat specific csv file
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(IActionResult), (int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IActionResult))]
        [SwaggerOperation("getInsiderThreatCsv")]
        [HttpGet("csv")]
        public async Task<IActionResult> GetAsCsv()
        {
            var engine = new FileHelperEngine<NPCToInsiderThreatCsv>();
            engine.HeaderText = engine.GetFileHeader();

            var list = await _mongo.FindAsync(x => true);
            var finalList = NPCToInsiderThreatCsv.ConvertToCsv(list.ToList());
            
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            engine.WriteStream(streamWriter, finalList);
            await streamWriter.FlushAsync();
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "text/csv", $"insider_threat_{Guid.NewGuid()}.csv");
        }
    }
}