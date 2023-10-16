// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Threading.Tasks;
using Ghosts.Animator.Api.Infrastructure.ContentServices.Ollama;
using Ghosts.Animator.Api.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ghosts.Animator.Api.Controllers;

[Route("/")]
public class HomeController : Controller
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        var x = new OllamaFormatterService();
        var o = await x.GenerateNextAction(new NPC(),"why is the sky blue?");
        return Ok(o);

    }
}