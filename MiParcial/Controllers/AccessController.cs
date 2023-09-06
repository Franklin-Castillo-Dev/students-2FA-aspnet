using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiParcial.Models;
using MiParcial.ViewModel;

namespace MiParcial.Controllers
{
    public class AccessController : Controller
    {

        private readonly Cc101020Context _context;

        private readonly IHttpContextAccessor _session;


        public AccessController(Cc101020Context context, IHttpContextAccessor session)
        {
            _context = context;
            _session = session;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string Usuario, string Password)
        {
            try
            {
                // Realizamos Busqueda del Usuario a la Base
                var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Email == Usuario);

                // Si el Usuario no Existe
                if (user == null)
                {
                    // Retornamos error y volvemos a la misma vista
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                    return View("Index");
                }

                // Si al Contra es Incorrecta
                if (!BCrypt.Net.BCrypt.Verify(Password, user.Password))
                {
                    // Retornamos error y volvemos a la misma vista
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                    return View("Index");
                }

                // Inicio de sesión exitoso, guardamos datos y redirigir a la página de inicio (index)
                _session.HttpContext.Session.SetInt32("idUser", user.Id);
                _session.HttpContext.Session.SetString("nameUser", user.Email);
                _session.HttpContext.Session.SetString("permisosUser", user.Permiso);

                return RedirectToAction("Index", "Home");


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error durante el inicio de sesión.");
                return View("Index");

            }
        }

        // GET: Access/Create
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistroViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el Correo ya existe.
                var correoExistente = await _context.Users.FirstOrDefaultAsync(e => e.Email == viewModel.User.Email);
                if (correoExistente != null)
                {
                    ModelState.AddModelError(string.Empty, "Correo ya registrado. Elija otro.");
                    return View(viewModel); // Devolver la vista con el mensaje de error.
                }

                //Configuramos Usuario
                viewModel.User.Permiso = "Estudiante";
                viewModel.User.Password = BCrypt.Net.BCrypt.HashPassword(viewModel.User.Password);

                // Guardar Usuario
                _context.Add(viewModel.User);
                await _context.SaveChangesAsync();
                                                
                // Asignar el UserId al Estudiante 
                viewModel.Estudiante.UserId = viewModel.User.Id;
                
                //Guardamos ahora Estudiante
                _context.Add(viewModel.Estudiante);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);

        }

        public IActionResult EntrarInvitado()
        {
            try
            {
              

                // Inicio de sesión exitoso, redirigir a la página de inicio (index)
                _session.HttpContext.Session.SetInt32("idUser", 0);
                _session.HttpContext.Session.SetString("nameUser", "Publico");
                _session.HttpContext.Session.SetString("permisosUser", "Publico");

                return RedirectToAction("Index", "Home");


            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Access");
            }

        }


        public IActionResult Logout()
        {
            // Elimina las variables de sesión relacionadas con la sesión del usuario
            _session.HttpContext.Session.Clear(); // O puedes eliminar solo las específicas que necesitas

            // Redirige a la página de inicio de sesión u otra página que desees después de cerrar sesión
            return RedirectToAction("Index", "Access");
        }

    }
}
