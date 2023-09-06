using Microsoft.AspNetCore.Mvc;
using MiParcial.Models;
using System.Diagnostics;

namespace MiParcial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpContextAccessor _session;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor session)
        {
            _logger = logger;
            _session = session;
        }

        public IActionResult Index()
        {
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");


            //session no iniciada
            if (_session.HttpContext != null &&  _session.HttpContext.Session.GetString("nameUser") == null)
            {
                return RedirectToAction("Index", "Access");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}