using Microsoft.AspNetCore.Mvc;

namespace Application.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Route("admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
