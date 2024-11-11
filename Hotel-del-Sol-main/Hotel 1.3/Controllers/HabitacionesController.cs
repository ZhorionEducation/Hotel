using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel.Models;
using Microsoft.Extensions.Hosting;


namespace Hotel.Controllers
{
    [AuthorizePermission("Habitaciones")]
    public class HabitacionesController : Controller
    {
        private readonly HotelContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HabitacionesController(HotelContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Habitaciones
        [AuthorizePermission("Habitaciones")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Habitaciones.ToListAsync());
        }

        // GET: Habitaciones/Details/5
        [AuthorizePermission("Habitaciones")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacione = await _context.Habitaciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habitacione == null)
            {
                return NotFound();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DetailsHabitacion", habitacione);
            }
            return View(habitacione);
        }

        // GET: Habitaciones/Create
        [AuthorizePermission("Habitaciones")]
        public IActionResult Create()
        {
            ViewData["Comodidades"] = new SelectList(_context.Comodidades, "Id", "Nombre");
            return View();
        }

        // POST: Habitaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("Habitaciones")]
        public async Task<IActionResult> Create([Bind("Id,NumeroHabitacion,Descripcion,Capacidad,PrecioPorNoche,Activo")] Habitacione habitacione, IFormFile imagen, List<Guid> Comodidades)
        {
            if (ModelState.IsValid)
            {
                habitacione.Id = Guid.NewGuid();

                if (imagen != null && imagen.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img/habitaciones");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imagen.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(fileStream);
                    }

                    habitacione.Imagen = uniqueFileName;
                }

                // Manejo de las comodidades
                if (Comodidades != null && Comodidades.Any())
                {
                    foreach (var comodidadId in Comodidades)
                    {
                        var comodidad = await _context.Comodidades.FindAsync(comodidadId);
                        if (comodidad != null)
                        {
                            habitacione.Comodidades.Add(comodidad);
                        }
                    }
                }

                _context.Add(habitacione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Comodidades"] = new SelectList(_context.Comodidades, "Id", "Nombre", Comodidades);
            return View(habitacione);
        }

        // GET: Habitaciones/Edit/5
        [AuthorizePermission("Habitaciones")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacione = await _context.Habitaciones
                .Include(h => h.Comodidades)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (habitacione == null)
            {
                return NotFound();
            }

            // Obtener todas las comodidades
            var todasLasComodidades = await _context.Comodidades.ToListAsync();

            // Obtener los IDs de las comodidades seleccionadas
            var comodidadesSeleccionadas = habitacione.Comodidades
                .Select(c => c.Id)
                .ToList();

            // Crear MultiSelectList
            ViewBag.Comodidades = new MultiSelectList(
                todasLasComodidades,
                "Id",
                "Nombre",
                comodidadesSeleccionadas
            );

            return PartialView("_EditHabitacion", habitacione);
        }

        // POST: Habitaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("Habitaciones")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,NumeroHabitacion,Descripcion,Capacidad,PrecioPorNoche,Activo,Imagen")] Habitacione habitacione, IFormFile nuevaImagen, List<Guid> Comodidades)
        {
            if (id != habitacione.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingHabitacione = await _context.Habitaciones
                        .Include(h => h.Comodidades)
                        .FirstOrDefaultAsync(h => h.Id == id);

                    if (existingHabitacione == null)
                    {
                        return NotFound();
                    }

                    if (nuevaImagen != null && nuevaImagen.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img/habitaciones");

                        if (!string.IsNullOrEmpty(existingHabitacione.Imagen))
                        {
                            string oldImagePath = Path.Combine(uploadsFolder, existingHabitacione.Imagen);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + nuevaImagen.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await nuevaImagen.CopyToAsync(fileStream);
                        }

                        existingHabitacione.Imagen = uniqueFileName;
                    }

                    existingHabitacione.NumeroHabitacion = habitacione.NumeroHabitacion;
                    existingHabitacione.Descripcion = habitacione.Descripcion;
                    existingHabitacione.Capacidad = habitacione.Capacidad;
                    existingHabitacione.PrecioPorNoche = habitacione.PrecioPorNoche;
                    existingHabitacione.Activo = habitacione.Activo;

                    // Manejar las comodidades
                    existingHabitacione.Comodidades.Clear();
                    if (Comodidades != null && Comodidades.Any())
                    {
                        foreach (var comodidadId in Comodidades)
                        {
                            var comodidad = await _context.Comodidades.FindAsync(comodidadId);
                            if (comodidad != null)
                            {
                                existingHabitacione.Comodidades.Add(comodidad);
                            }
                        }
                    }

                    _context.Update(existingHabitacione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitacioneExists(habitacione.Id))
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
            ViewBag.Comodidades = new MultiSelectList(_context.Comodidades, "Id", "Nombre", Comodidades);
            return PartialView("_EditHabitacion", habitacione);
        }



        // GET: Habitaciones/Delete/5
        [AuthorizePermission("Habitaciones")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacione = await _context.Habitaciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habitacione == null)
            {
                return NotFound();
            }

            return View(habitacione);
        }

        // POST: Habitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("Habitaciones")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var habitacione = await _context.Habitaciones.FindAsync(id);
            if (habitacione != null)
            {
                _context.Habitaciones.Remove(habitacione);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HabitacioneExists(Guid id)
        {
            return _context.Habitaciones.Any(e => e.Id == id);
        }
    }
}
