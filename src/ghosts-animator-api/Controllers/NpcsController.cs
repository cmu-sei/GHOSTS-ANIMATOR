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
using System.Net;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace Ghosts.Animator.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class NPCsController : ControllerBase
    {
        private readonly IMongoCollection<NPC> _mongo;

        public NPCsController(DatabaseSettings.IApplicationDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _mongo = database.GetCollection<NPC>(settings.CollectionNameNPCs);
        }
        
        /// <summary>
        /// Returns all generated NPCs in the system (caution, could return a large amount of data)
        /// </summary>
        /// <returns>IEnumerable&lt;NpcProfile&gt;</returns>
        [ProducesResponseType(typeof(IEnumerable<NpcProfile>), (int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IEnumerable<NpcProfile>))]
        [SwaggerOperation("getNPCs")]
        [HttpGet]
        public IEnumerable<NpcProfile> Get()
        {
            return _mongo.Find(x => true).ToList();
        }
        
        /// <summary>
        /// Get NPC by specific Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(NpcProfile), (int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(NpcProfile))]
        [SwaggerOperation("getNPCById")]
        [HttpGet("{id}")]
        public NpcProfile GetById(Guid id)
        {
            return _mongo.Find(x => x.Id == id).FirstOrDefault();
        }
        
        /// <summary>
        /// Delete NPC by specific Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.OK)]
        [SwaggerOperation("deleteNPCById")]
        [HttpDelete("{id}")]
        public void DeleteById(Guid id)
        {
             //_mongo.Find(x => x.Id == id).FirstOrDefault();
             _mongo.DeleteOne(x => x.Id == id);
        }
        
        /// <summary>
        /// Get NPC photo by specific Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(IActionResult), (int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IActionResult))]
        [SwaggerOperation("getNpcAvatarById")]
        [HttpGet("{id}/photo")]
        public IActionResult GetPhotoById(Guid id)
        {
            //get npc and find image
            var npc = _mongo.Find(x => x.Id == id).FirstOrDefault();
            //load image as stream
            var stream = new FileStream(npc.PhotoLink, FileMode.Open);
            return File(stream, "image/jpg", $"{npc.Name.ToString().Replace(" ", "_")}.jpg");
        }
        
        /// <summary>
        /// Generate random NPC by random service branch
        /// </summary>
        /// <returns>NPC Profile</returns>
        [ProducesResponseType(typeof(NpcProfile), (int) HttpStatusCode.OK)]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(NpcProfile))]
        [SwaggerOperation("generateNpc")]
        [HttpPost]
        public NpcProfile GenerateOne()
        {
            var npc = NPC.TransformTo(Npc.Generate(MilitaryUnits.GetServiceBranch()));
            _mongo.InsertOne(npc, new InsertOneOptions { BypassDocumentValidation = false});
            return npc;
        }
        
        /// <summary>
        /// Get a subset of details about a specific NPC
        /// </summary>
        /// <param name="npcId"></param>
        /// <param name="fieldsToReturn"></param>
        /// <returns></returns>
        [HttpPost("npc/{npcId}")]
        public object GetNpcReduced(Guid npcId, [FromBody] string[] fieldsToReturn)
        {
            var npc = _mongo.Find(x => x.Id == npcId).FirstOrDefault();
            return new NPCReduced(fieldsToReturn, npc).PropertySelection;
        }
    }
}