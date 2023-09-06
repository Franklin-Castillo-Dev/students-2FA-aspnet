using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiParcial.Models;

namespace MiParcial.Controllers
{
    public class UsersController : Controller
    {
        private readonly Cc101020Context _context;
        
        private readonly IHttpContextAccessor _session;

        List<string> selectPermisos = new List<string> { "Publico", "Estudiante", "Administrador" };

        List<string> permisos = new List<string> { "Administrador" };


        public UsersController(Cc101020Context context, IHttpContextAccessor session)
        {
            _context = context;
            _session = session;
        }

        // GET: Users
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


            return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'Cc101020Context.Users'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["tiposPermisos"] = new SelectList(selectPermisos);
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Edad,Permiso,Password")] User user)
        {
            if (ModelState.IsValid)
            {

                // Verificar si el Correo ya existe.
                var correoExistente = await _context.Users.FirstOrDefaultAsync(e => e.Email == user.Email);
                if (correoExistente != null)
                {                    
                    ModelState.AddModelError(string.Empty, "Correo ya registrado. Elija otro.");
                    return View(user); // Devolver la vista con el mensaje de error.
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            //quitamos la encriptada
            user.Password = "";
            ViewData["tiposPermisos"] = new SelectList(selectPermisos);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Edad,Permiso,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Cc101020Context.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            //Validar que si exista.
            if (user == null)
            {
                // Puedes manejar el caso donde el usuario no existe, por ejemplo, redireccionando a una página de error.
                return NotFound(); // Devolver una respuesta 404 (Not Found).
            }
            // Verificar si el usuario está vinculado a un estudiante.
            var estudiante = await _context.Estudiantes.FirstOrDefaultAsync(e => e.UserId == id);
            if (estudiante != null)
            {
                // Si hay un estudiante vinculado, mandar mensaje error a la vista.
                ModelState.AddModelError(string.Empty, "No se puede eliminar el usuario porque está vinculado a un estudiante.");
                return View(user); // Devolver la vista con el mensaje de error.

            }

            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
