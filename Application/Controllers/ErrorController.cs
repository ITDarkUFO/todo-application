using Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Application.Controllers
{
    [Route("error")]
    public class ErrorController : Controller
    {
        [Route("")]
        [Route("{errorCode}")]
        public IActionResult ErrorPage(int errorCode)
        {
            return View(errorCode);
        }

        [Route("404")]
        public IActionResult NotFoundPage()
        {
            return View();
        }

        [Route("accessdenied")]
        public IActionResult AccessDeniedPage()
        {
            return View();
        }
    }
}
