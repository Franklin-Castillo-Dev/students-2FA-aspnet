using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiParcial.Models;

namespace MiParcial.Controllers
{
    public class InscripcionesController : Controller
    {
        private readonly Cc101020Context _context;
        private readonly IHttpContextAccessor _session;

        List<string> permisos = new List<string> { "Estudiante", "Administrador" };

        public InscripcionesController(Cc101020Context context, IHttpContextAccessor session)
        {
            _context = context;
            _session = session;
        }

        // GET: Inscripciones
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

            if (_session.HttpContext.Session.GetString("permisosUser") == "Estudiante")
            {

                int? idUsuarioLogin = _session.HttpContext.Session.GetInt32("idUser");

                // Verificar si el EstudianteId es válido (no nulo ni vacío)
                if (!string.IsNullOrEmpty(idUsuarioLogin.ToString()))
                {
                    // Obtener el estudiante actual utilizando el EstudianteId de la sesión
                    var estudianteActual = await _context.Estudiantes.FindAsync(idUsuarioLogin);

                    var cc101020Context_Student = _context.Inscripciones.Include(i => i.Curso).Include(i => i.Estudiante).Where(i => i.EstudianteId == estudianteActual.EstudianteId);
                    return View(await cc101020Context_Student.ToListAsync());

                }

            }

            //Administrador
            var cc101020Context_Admin = _context.Inscripciones.Include(i => i.Curso).Include(i => i.Estudiante);
            return View(await cc101020Context_Admin.ToListAsync());

            //var cc101020Context = _context.Inscripciones.Include(i => i.Curso).Include(i => i.Estudiante);
            //return View(await cc101020Context.ToListAsync());
        }

        // GET: Inscripciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Inscripciones == null)
            {
                return NotFound();
            }

            var inscripcione = await _context.Inscripciones
                .Include(i => i.Curso)
                .Include(i => i.Estudiante)
                .FirstOrDefaultAsync(m => m.InscripcionId == id);
            if (inscripcione == null)
            {
                return NotFound();
            }

            return View(inscripcione);
        }

        // GET: Inscripciones/Create
        public async Task<ActionResult> Create()
        {
            // Concatena Curso en una sola cadena
            var cursos = _context.Cursos.Select(p => new
            {
                CursoId = p.CursoId,
                nombre = $"Id: {p.CursoId} - Nombre: {p.NombreCurso}"
            }).ToList();

            int? idUsuarioLogin = _session.HttpContext.Session.GetInt32("idUser");

            if (_session.HttpContext.Session.GetString("permisosUser") == "Estudiante")
            {
                // Verificar si el EstudianteId es válido (no nulo ni vacío)
                if (!string.IsNullOrEmpty(idUsuarioLogin.ToString()))
                {
                    // Obtener el estudiante actual utilizando el EstudianteId de la sesión
                    var estudianteActual = await _context.Estudiantes.FindAsync(idUsuarioLogin);

                    // Crear un objeto SelectListItem con el EstudianteId de la sesión como valor y nombre
                    var estudianteSeleccionado = new SelectListItem
                    {
                        Value = estudianteActual.EstudianteId.ToString(),
                        Text = $"Id: {estudianteActual.EstudianteId} - Nombre: {estudianteActual.Nombre}"
                    };

                    // Crear una lista de SelectListItem que contiene solo el estudiante seleccionado
                    var estudiantes = new List<SelectListItem> { estudianteSeleccionado };

                    // Establecer el SelectList en ViewData con el estudiante seleccionado
                    ViewData["EstudianteId"] = new SelectList(estudiantes, "Value", "Text");
                }
                else
                {
                    // Create a list with just an empty item
                    var emptyItem = new SelectListItem
                    {
                        Value = string.Empty,
                        Text = "No Hay estudiante validado."
                    };

                    var estudiantes = new List<SelectListItem> { emptyItem };

                    ViewData["EstudianteId"] = new SelectList(estudiantes, "Value", "Text");
                }
            }

            if (_session.HttpContext.Session.GetString("permisosUser") == "Administrador")
            {
                var cc101020Context_Admin = _context.Inscripciones.Include(i => i.Curso).Include(i => i.Estudiante);
                // Concatena Nombre y Apellido en una sola cadena
                var estudiantes = _context.Estudiantes.Select(p => new
                {
                    Id = p.EstudianteId,
                    NombreCompleto = $"Id: {p.EstudianteId} - Nombre: {p.Nombre}"
                }).ToList();

                ViewData["EstudianteId"] = new SelectList(estudiantes, "Id", "NombreCompleto");
            }


            ViewData["CursoId"] = new SelectList(cursos, "CursoId", "nombre");


            return View();
            //ViewData["CursoId"] = new SelectList(_context.Cursos, "CursoId", "CursoId");
            //ViewData["EstudianteId"] = new SelectList(_context.Estudiantes, "EstudianteId", "EstudianteId");
            //return View();
        }

        // POST: Inscripciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InscripcionId,EstudianteId,CursoId")] Inscripcione inscripcione)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inscripcione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CursoId"] = new SelectList(_context.Cursos, "CursoId", "NombreCurso", inscripcione.CursoId);
            ViewData["EstudianteId"] = new SelectList(_context.Estudiantes, "EstudianteId", "Nombre", inscripcione.EstudianteId);
            return View(inscripcione);
        }

        //// GET: Inscripciones/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var inscripcion = await _context.Inscripciones
        //                        .Where(i => i.InscripcionId == id)
        //                        .FirstOrDefaultAsync();

        //    if (inscripcion == null)
        //    {
        //        return NotFound();
        //    }

        //    // Concatena Curso en una sola cadena
        //    var cursos = _context.Cursos.Select(p => new
        //    {
        //        CursoId = p.CursoId,
        //        nombre = $"Id: {p.CursoId} - Nombre: {p.NombreCurso}"
        //    }).ToList();

        //    //ViewData["CursoId"] = new SelectList(_context.Cursos, "CursoId", "CursoId", inscripcion.CursoId);
        //    ViewData["CursoId"] = new SelectList(cursos, "CursoId", "NombreCurso", inscripcion.CursoId);

        //    int? idUsuarioLogin = _session.HttpContext.Session.GetInt32("idUser");

        //    //Vista Estudiante
        //    if (_session.HttpContext.Session.GetString("permisosUser") == "Estudiante")
        //    {
        //        // Obtener el estudiante actual utilizando el EstudianteId de la sesión
        //        var estudianteActual = await _context.Estudiantes.FindAsync(idUsuarioLogin);

        //        // Crear un objeto SelectListItem con el EstudianteId de la sesión como valor y nombre
        //        var estudianteSeleccionado = new SelectListItem
        //        {
        //            Value = estudianteActual.EstudianteId.ToString(),
        //            Text = $"Id: {estudianteActual.EstudianteId} - Nombre: {estudianteActual.Nombre}"
        //        };

        //        // Crear una lista de SelectListItem que contiene solo el estudiante seleccionado
        //        var estudiantes = new List<SelectListItem> { estudianteSeleccionado };

        //        // Establecer el SelectList en ViewData con el estudiante seleccionado
        //        ViewData["EstudianteId"] = new SelectList(estudiantes, "Value", "Text");
        //    }
        //    //Vista Admin
        //    else if (_session.HttpContext.Session.GetString("permisosUser") == "Administrador")
        //    {
        //        ViewData["EstudianteId"] = new SelectList(_context.Estudiantes, "EstudianteId", "Nombre", inscripcion.EstudianteId);
        //    }
        //    // Vista error
        //    else
        //    {
        //        // Create a list with just an empty item
        //        var emptyItem = new SelectListItem
        //        {
        //            Value = string.Empty,
        //            Text = "No Hay estudiante validado."
        //        };

        //        var estudiantes = new List<SelectListItem> { emptyItem };

        //        ViewData["EstudianteId"] = new SelectList(estudiantes, "Value", "Text");
        //    }                            

        //    return View(inscripcion);
            
        //}

        //// POST: Inscripciones/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("InscripcionId,EstudianteId,CursoId")] Inscripcione inscripcione)
        //{
        //    if (id != inscripcione.InscripcionId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(inscripcione);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!InscripcioneExists(inscripcione.InscripcionId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CursoId"] = new SelectList(_context.Cursos, "CursoId", "CursoId", inscripcione.CursoId);
        //    ViewData["EstudianteId"] = new SelectList(_context.Estudiantes, "EstudianteId", "EstudianteId", inscripcione.EstudianteId);
        //    return View(inscripcione);
        //}

        // GET: Inscripciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Inscripciones == null)
            {
                return NotFound();
            }

            var inscripcione = await _context.Inscripciones
                .Include(i => i.Curso)
                .Include(i => i.Estudiante)
                .FirstOrDefaultAsync(m => m.InscripcionId == id);
            if (inscripcione == null)
            {
                return NotFound();
            }

            return View(inscripcione);
        }

        // POST: Inscripciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Inscripciones == null)
            {
                return Problem("Entity set 'Cc101020Context.Inscripciones'  is null.");
            }
            var inscripcione = await _context.Inscripciones.FindAsync(id);
            if (inscripcione != null)
            {
                _context.Inscripciones.Remove(inscripcione);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InscripcioneExists(int id)
        {
          return (_context.Inscripciones?.Any(e => e.InscripcionId == id)).GetValueOrDefault();
        }
    }
}
