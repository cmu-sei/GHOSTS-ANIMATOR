using System;
using System.Linq;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Ghosts.Animator.Api.Controllers;

[Controller]
[Produces("application/json")]
[Route("view-activities/")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ViewActivitiesController : Controller
{
    private readonly IMongoCollection<NPC> _mongo;
        
    public ViewActivitiesController(DatabaseSettings.IApplicationDatabaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _mongo = database.GetCollection<NPC>(settings.CollectionNameNPCs);
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        var list = _mongo.Find(x => true).ToList().OrderBy(o => o.Enclave).ThenBy(o=>o.Team);
        return View("Index", list);
    }
    
    [HttpGet("{id:guid}")]
    public IActionResult Detail(Guid id)
    {
        var o = _mongo.Find(x => x.Id == id).FirstOrDefault();
        return View("Detail", o);
    }
}