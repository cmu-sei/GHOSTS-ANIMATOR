using System.Collections.Generic;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Api.Infrastructure.Social;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace Ghosts.Animator.Api.Controllers;

[Route("social/")]
public class SocialController : Controller
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly ApplicationConfiguration _configuration;
    
    public SocialController()
    {
        this._configuration = Program.Configuration;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.IsEnabled = this._configuration.SocialGraph.IsEnabled;
        if (!this._configuration.SocialGraph.IsEnabled)
        {
            return View();
        }

        var path = SocialGraphManager.GetSocialGraphFile();
        if (!System.IO.File.Exists(path))
        {
            ViewBag.IsEnabled = false;
            return View();
        }
        
        var graphs = JsonConvert.DeserializeObject<List<SocialGraph>>(System.IO.File.ReadAllText(path));
        _log.Info($"SocialGraph loaded from disk...");
        
        return View(graphs);
    }

    [HttpGet("{id}")]
    public IActionResult Detail(string id)
    {
        return View();
    }
    
    [HttpGet("{id}/interactions")]
    public IActionResult Interactions(string id)
    {
        return View();
    }
}