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