// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace Ghosts.Animator.Api.Controllers;

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
    /// Returns name and Id for all NPCs in the system (caution, could return a large amount of data)
    /// </summary>
    /// <returns>IEnumerable&lt;NpcNameId&gt;</returns>
    [ProducesResponseType(typeof(IEnumerable<NpcNameId>), (int) HttpStatusCode.OK)]
    [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IEnumerable<NpcNameId>))]
    [SwaggerOperation("getNPCList")]
    [HttpGet("list")]
    public IEnumerable<NpcNameId> List()
    {
        return _mongo.Find(_ => true).ToList().Select(item => new NpcNameId() { Id = item.Id, Name = item.Name.ToString() }).ToList();
    }
        
    /// <summary>
    /// Get NPC by specific Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(NpcProfile), (int) HttpStatusCode.OK)]
    [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(NpcProfile))]
    [SwaggerOperation("getNPCById")]
    [HttpGet("{id:guid}")]
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
    [HttpDelete("{id:guid}")]
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
    [HttpGet("{id:guid}/photo")]
    public IActionResult GetPhotoById(Guid id)
    {
        //get npc and find image
        var npc = _mongo.Find(x => x.Id == id).FirstOrDefault();
        if (npc == null) return NotFound();
        //load image as stream
        var stream = new FileStream(npc.PhotoLink, FileMode.Open);
        return File(stream, "image/jpg", $"{npc.Name.ToString().Replace(" ", "_")}.jpg");
    }
        
    /// <summary>
    /// Create one NPC (handy for syncing up from ghosts core api)
    /// </summary>
    /// <returns>NPC Profile</returns>
    [ProducesResponseType(typeof(NpcProfile), (int) HttpStatusCode.OK)]
    [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(NpcProfile))]
    [SwaggerOperation("createNpc")]
    [HttpPost]
    public NpcProfile Create(NpcProfile npcProfile, bool generate)
    {
        NpcProfile npc;
        if (generate)
        {
            npc = NPC.TransformTo(Npc.Generate(MilitaryUnits.GetServiceBranch()));
            npc.Name = npcProfile.Name;
            npc.Email = npcProfile.Email;
        }
        else
        {
            npc = npcProfile;
        }
        
        npc.Id = Guid.NewGuid();
        npc.Created = DateTime.UtcNow;
        
        _mongo.InsertOne(NPC.TransformTo(npc), new InsertOneOptions { BypassDocumentValidation = false});
        return npc;
    }
        
    /// <summary>
    /// Get a subset of details about a specific NPC
    /// </summary>
    /// <param name="npcId"></param>
    /// <param name="fieldsToReturn"></param>
    /// <returns></returns>
    [HttpPost("npc/{npcId:guid}")]
    public object GetNpcReduced(Guid npcId, [FromBody] string[] fieldsToReturn)
    {
        var npc = _mongo.Find(x => x.Id == npcId).FirstOrDefault();
        return new NPCReduced(fieldsToReturn, npc).PropertySelection;
    }
}