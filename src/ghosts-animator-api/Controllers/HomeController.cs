using Microsoft.AspNetCore.Mvc;

namespace Ghosts.Animator.Api.Controllers;

[Route("/")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}