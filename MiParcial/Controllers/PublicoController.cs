using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiParcial.Models;

namespace MiParcial.Controllers
{
    public class PublicoController : Controller
    {
        private readonly Cc101020Context _context;
        private readonly IHttpContextAccessor _session;

        List<string> permisos = new List<string> { "Publico", "Administrador" };

        public PublicoController(Cc101020Context context, IHttpContextAccessor session)
        {
            _context = context;
            _session = session;
        }

        // GET: Profesores
        public async Task<IActionResult> Index()
        {
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");


            //session no iniciada
            if (_session.HttpContext != null && _session.HttpContext.Session.GetString("nameUser") == null)
            {
                return RedirectToAction("Index", "Access");
            }

            //Verificar permisos                        
            if (_session.HttpContext != null && !permisos.Contains(_session.HttpContext.Session.GetString("permisosUser")))
            {
                return RedirectToAction("Index", "Home");
            }


            var cc101020Context = _context.Cursos.Include(c => c.Profesor);
            return View(await cc101020Context.ToListAsync());
        }
    }
}
