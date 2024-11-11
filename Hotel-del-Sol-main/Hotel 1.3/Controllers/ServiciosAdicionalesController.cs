using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel.Models;

namespace Hotel.Controllers
{
    [AuthorizePermission("Servicios")]
    public class ServiciosAdicionalesController : Controller
    {
        private readonly HotelContext _context;

        public ServiciosAdicionalesController(HotelContext context)
        {
            _context = context;
        }

        [AuthorizePermission("Servicios")]
        public IActionResult IndexUsuarios()
        {
            var servicios = _context.ServiciosAdicionales.ToList();
            return View(servicios);
        }

        // GET: ServiciosAdicionales
        [AuthorizePermission("Servicios")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiciosAdicionales.ToListAsync());
        }

        // GET: ServiciosAdicionales/Details/5
        [AuthorizePermission("Servicios")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviciosAdicionale = await _context.ServiciosAdicionales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviciosAdicionale == null)
            {
                return NotFound();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DetailsServicio", serviciosAdicionale);
            }

            return View(serviciosAdicionale);
        }

        // GET: ServiciosAdicionales/Create
        [AuthorizePermission("Servicios")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiciosAdicionales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("Servicios")]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Precio,Imagen,Activo")] ServiciosAdicionale serviciosAdicionale)
        {
            if (ModelState.IsValid)
            {
                serviciosAdicionale.Id = Guid.NewGuid();
                _context.Add(serviciosAdicionale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviciosAdicionale);
        }

        // GET: ServiciosAdicionales/Edit/5
        [AuthorizePermission("Servicios")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviciosAdicionale = await _context.ServiciosAdicionales.FindAsync(id);
            if (serviciosAdicionale == null)
            {
                return NotFound();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EditServicio", serviciosAdicionale);
            }

            return View(serviciosAdicionale);
        }

        // POST: ServiciosAdicionales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("Servicios")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nombre,Descripcion,Precio,Imagen,Activo")] ServiciosAdicionale serviciosAdicionale)
        {
            if (id != serviciosAdicionale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviciosAdicionale);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, id = serviciosAdicionale.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiciosAdicionaleExists(serviciosAdicionale.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EditServicio", serviciosAdicionale);
            }
            return View(serviciosAdicionale);
        }

        // GET: ServiciosAdicionales/Delete/5
        [AuthorizePermission("Servicios")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviciosAdicionale = await _context.ServiciosAdicionales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviciosAdicionale == null)
            {
                return NotFound();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DeleteServicio", serviciosAdicionale);
            }

            return View(serviciosAdicionale);
        }

        // POST: ServiciosAdicionales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("Servicios")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var serviciosAdicionale = await _context.ServiciosAdicionales.FindAsync(id);
            if (serviciosAdicionale != null)
            {
                _context.ServiciosAdicionales.Remove(serviciosAdicionale);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiciosAdicionaleExists(Guid id)
        {
            return _context.ServiciosAdicionales.Any(e => e.Id == id);
        }
    }
}
