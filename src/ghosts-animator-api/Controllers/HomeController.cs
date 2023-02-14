// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

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
}