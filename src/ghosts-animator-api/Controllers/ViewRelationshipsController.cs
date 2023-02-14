// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Linq;
using System.Text;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Ghosts.Animator.Api.Controllers;

[Controller]
[Produces("application/json")]
[Route("view-relationships/")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ViewRelationshipsController : Controller
{
    private readonly IMongoCollection<NPC> _mongo;
        
    public ViewRelationshipsController(DatabaseSettings.IApplicationDatabaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _mongo = database.GetCollection<NPC>(settings.CollectionNameNPCs);
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View("Index");
    }
        
    [HttpGet("profile/{id:guid}")]
    public IActionResult Profile(Guid id)
    {
        var npc = _mongo.Find(x => x.Id == id).FirstOrDefault();
        return View("Profile", npc);
    }
        
    [HttpGet("files/data1.csv")]
    public FileResult Download()
    {
        const string fileName = "data1.csv";
        var content = new StringBuilder("npc_id,source,target,type").Append(Environment.NewLine);

        var list = _mongo.Find(x => true).ToList().OrderBy(o => o.Enclave).ThenBy(o=>o.Team);
            
        NPC previousNpc = null;
        var enclave = string.Empty;
        var team = string.Empty;
        var campaign = string.Empty;
            
        foreach (var npc in list)
        {
            if (previousNpc == null)
            {
                campaign = npc.Campaign;
            }
                
            if (previousNpc == null || previousNpc.Enclave != npc.Enclave)
            {
                enclave = npc.Enclave;
                content.Append(',').Append(campaign).Append(',').Append(enclave).Append(",CAMPAIGN").Append(Environment.NewLine);
            }
                
            if (string.IsNullOrEmpty(team) || previousNpc?.Team != npc.Team)
            {
                team = $"{enclave}/{npc.Team}";
                content.Append(',').Append(enclave).Append(',').Append(team).Append(",TEAM").Append(Environment.NewLine);
            }

            content.Append(npc.Id).Append(',').Append(team).Append(',').Append(npc.Name).Append(',').Append(npc.Enclave).Append('-').Append(npc.Team).Append(Environment.NewLine);
            previousNpc = npc;
        }
        var fileBytes = Encoding.ASCII.GetBytes(content.ToString());
        return File(fileBytes, "text/csv", fileName);
    }
}