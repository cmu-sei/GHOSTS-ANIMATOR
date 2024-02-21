using System.Threading;
using Ghosts.Animator.Api.Infrastructure.Animations;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MongoDB.Bson;
using NLog;

namespace Ghosts.Animator.Api.Controllers;

[Route("/animations/")]
[ApiExplorerSettings(IgnoreApi = true)]
public class AnimationsController : Controller
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    private readonly ApplicationConfiguration _configuration;
    private readonly IManageableHostedService _animationsManager;

    public AnimationsController(IManageableHostedService animationsManager)
    {
        this._configuration = Program.Configuration;
        this._animationsManager = animationsManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.FullAutonomy = _configuration.Animations.FullAutonomy.ToJson();
        ViewBag.SocialSharing = _configuration.Animations.SocialSharing.ToJson();
        ViewBag.SocialBelief = _configuration.Animations.SocialBelief.ToJson();
        ViewBag.Chat = _configuration.Animations.Chat.ToJson();
        ViewBag.SocialGraph = _configuration.Animations.SocialGraph.ToJson();
        
        return View(new AnimationConfiguration());
    }
    
    [HttpPost("start")]
    public IActionResult Start(AnimationConfiguration configuration, [FromForm] string jobConfiguration)
    {
        configuration.JobConfiguration = jobConfiguration;
        _animationsManager.StartJob(configuration, new CancellationToken());
        return RedirectToAction("Index");
    }
    
    [HttpPost("stop")]
    public IActionResult Stop(string jobId)
    {
        _animationsManager.StopJob(jobId);
        return RedirectToAction("Index");
    }
}