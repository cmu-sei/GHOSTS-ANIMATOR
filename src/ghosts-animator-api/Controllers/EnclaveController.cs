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
using FileHelpers;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace Ghosts.Animator.Api.Controllers
{
    /// <summary>
    /// Export or delete all NPCs from a specific enclave
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EnclaveController : ControllerBase
    {
        private readonly IMongoCollection<NPC> _mongo;
        private readonly IMongoCollection<NPCIpAddress> _mongoIps;

        public EnclaveController(DatabaseSettings.IApplicationDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _mongo = database.GetCollection<NPC>(settings.CollectionNameNPCs);
            _mongoIps = database.GetCollection<NPCIpAddress>(settings.CollectionNameIPAddresses);
        }

        private FilterDefinition<NPC> BuildEnclaveFilter(string campaign, string enclave)
        {
            var filter = Builders<NPC>.Filter.And(
                Builders<NPC>.Filter.Eq("Campaign", campaign),
                Builders<NPC>.Filter.Eq("Enclave", enclave));
            return filter;
        }

        /// <summary>
        /// Gets all NPCs by from a specific enclave that is part of a specific campaign
        /// </summary>
        /// <param name="enclave">The name of the enclave</param>
        /// <param name="campaign">The name of the campaign the enclave is part of</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(IEnumerable<NPC>), (int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IEnumerable<NPC>))]
        [SwaggerOperation("getEnclave")]
        [HttpGet("{campaign}/{enclave}")]
        public IEnumerable<NPC> GetEnclave(string campaign, string enclave)
        {
            var filter = BuildEnclaveFilter(campaign, enclave);
            return _mongo.Find(filter).ToList();
        }

        /// <summary>
        /// Gets the csv output of a query
        /// </summary>
        /// <param name="enclave">The name of the enclave</param>
        /// <param name="campaign">The name of the campaign the enclave is part of</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(IActionResult), (int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IActionResult))]
        [SwaggerOperation("getEnclaveCsv")]
        [HttpGet("{campaign}/{enclave}/csv")]
        public IActionResult GetAsCsv(string campaign, string enclave)
        {
            var engine = new FileHelperEngine<NPCToCsv>();
            var list = GetEnclave(enclave, campaign).ToList();

            var filteredList = list.Select(n => new NPCToCsv() {Id = n.Id, Email = n.Email}).ToList();

            var stream = new MemoryStream();
            TextWriter streamWriter = new StreamWriter(stream);
            engine.WriteStream(streamWriter, filteredList);
            streamWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "text/csv", $"{Guid.NewGuid()}.csv");
        }

        /// <summary>
        /// Delete All NPCs in a specific enclave
        /// </summary>
        /// <param name="enclave"></param>
        /// <param name="campaign"></param>
        /// <returns></returns>
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.OK)]
        [SwaggerOperation("deleteEnclave")]
        [HttpDelete("{campaign}/{enclave}")]
        public void DeleteEnclave(string campaign, string enclave)
        {
            var npcFilter = this.BuildEnclaveFilter(campaign, enclave);
            _mongo.DeleteMany(npcFilter);
            
            var ipFilter = Builders<NPCIpAddress>.Filter.Eq("Enclave", enclave);
            _mongoIps.DeleteMany(ipFilter);
        }
        
        /// <summary>
        /// Get a CSV file containing all of the requested properties of NPCs in an enclave
        /// </summary>
        /// <param name="campaign"></param>
        /// <param name="enclave"></param>
        /// <param name="fieldsToReturn"></param>
        /// <returns></returns>
        [HttpPost("{campaign}/{enclave}/custom")]
        public IActionResult GetReducedNpcs(string campaign, string enclave, [FromBody] string[] fieldsToReturn)
        {
            var filter = BuildEnclaveFilter(campaign, enclave);
            var npcList = _mongo.Find(filter).ToList();
            Dictionary<string, Dictionary<string, string>> npcDetails = new Dictionary<string, Dictionary<string, string>>();
            
            foreach (var npc in npcList) {
                Dictionary<string, string> npcProperties = new NPCReduced(fieldsToReturn, npc).PropertySelection;
                var name = npc.Name;
                var npcName = name.ToString();
                npcDetails[npcName] = npcProperties;
            }

            //return npcDetails;
            
            EnclaveReducedCsv enclaveCsv = new EnclaveReducedCsv(fieldsToReturn, npcDetails);
            var stream = new MemoryStream();
            TextWriter streamWriter = new StreamWriter(stream);
            streamWriter.Write(enclaveCsv.CsvData);
            streamWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "text/csv", $"{Guid.NewGuid()}.csv");
            
            
        }
        
    }
}