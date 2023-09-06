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
    public class CursosController : Controller
    {
        private readonly Cc101020Context _context;
        private readonly IHttpContextAccessor _session;

        List<string> permisos = new List<string> { "Administrador" };

        public CursosController(Cc101020Context context, IHttpContextAccessor session)
        {
            _context = context;
            _session = session;
        }

        // GET: Cursos
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

        // GET: Cursos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cursos == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .Include(c => c.Profesor)
                .FirstOrDefaultAsync(m => m.CursoId == id);
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // GET: Cursos/Create
        public IActionResult Create()
        {
            // Concatena Nombre y Apellido en una sola cadena
            var profesores = _context.Profesores.Select(p => new
            {
                Id = p.Id,
                NombreCompleto = $"Id: {p.Id} - Nombre: {p.Nombre} {p.Apellido}"
            }).ToList();

            ViewData["ProfesorId"] = new SelectList(profesores, "Id", "NombreCompleto");
            return View();
        }

        // POST: Cursos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CursoId,NombreCurso,ProfesorId")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "Id", curso.ProfesorId);
            return View(curso);
        }

        // GET: Cursos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cursos == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
            {
                return NotFound();
            }
            //ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "Id", curso.ProfesorId);
            // Concatena Nombre y Apellido en una sola cadena
            var profesores = _context.Profesores.Select(p => new
            {
                Id = p.Id,
                NombreCompleto = $"Id: {p.Id} - Nombre: {p.Nombre} {p.Apellido}"
            }).ToList();

            ViewData["ProfesorId"] = new SelectList(profesores, "Id", "NombreCompleto");
            return View(curso);
        }

        // POST: Cursos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CursoId,NombreCurso,ProfesorId")] Curso curso)
        {
            if (id != curso.CursoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(curso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursoExists(curso.CursoId))
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
            ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "Id", curso.ProfesorId);
            return View(curso);
        }

        // GET: Cursos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cursos == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .Include(c => c.Profesor)
                .FirstOrDefaultAsync(m => m.CursoId == id);
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // POST: Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cursos == null)
            {
                return Problem("Entity set 'Cc101020Context.Cursos'  is null.");
            }
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                _context.Cursos.Remove(curso);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CursoExists(int id)
        {
          return (_context.Cursos?.Any(e => e.CursoId == id)).GetValueOrDefault();
        }
    }
}
