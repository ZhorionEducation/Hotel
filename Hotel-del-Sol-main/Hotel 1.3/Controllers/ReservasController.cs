using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Security.Claims;

namespace Hotel.Controllers
{
    [AuthorizePermission("Reservas")]
    public class ReservasController : Controller
    {
        private readonly HotelContext _context;

        public ReservasController(HotelContext context)
        {
            _context = context;
        }

        // GET: Reservas
        [AuthorizePermission("Reservas")]
        public async Task<IActionResult> Index()
        {
            var hotelContext = _context.Reservas.Include(r => r.Habitacion).Include(r => r.Usuario).Include(r => r.Comodidads).Include(r => r.Servicios);
            return View(await hotelContext.ToListAsync());

        }

        // GET: Reservas/MisReservas
        public async Task<IActionResult> MisReservas()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Usuarios", "Create");
            }

            var userName = User.Identity.Name;
            var user = await _context.Usuarios.SingleOrDefaultAsync(u => u.NombreUsuario == userName);
            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var userReservas = _context.Reservas
                .Include(r => r.Habitacion)
                .Include(r => r.Comodidads)
                .Include(r => r.Servicios)
                .Where(r => r.UsuarioId == user.Id);

            return View(await userReservas.ToListAsync());
        }

        // GET: Reservas/Details/5
        [AuthorizePermission("Reservas")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Habitacion)
                .Include(r => r.Usuario)
                .Include(r => r.Comodidads)
                .Include(r => r.Servicios)
                .Include(r => r.Huespedes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DetailsReserva", reserva);
            }
            return View(reserva);
        }

        // GET: Reservas/Create
        [AuthorizePermission("Reservas")]
        public IActionResult Create()
        {
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "NumeroHabitacion");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "NombreUsuario"); //Cambiado a id, NombreUsuario para que en la vista se visualice el nombre pero valide ID
            ViewData["ComodidadId"] = new SelectList(_context.Comodidades, "Id", "Nombre");
            ViewData["ServicioAdicionalId"] = new SelectList(_context.ServiciosAdicionales, "Id", "Nombre");
            return View();
        }

        // POST: Reservas/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("Reservas")]
        public async Task<IActionResult> Create([Bind("Id,UsuarioId,HabitacionId,FechaInicio,FechaFin,PrecioTotal,NumeroAcompanantes,FechaReserva,Comodidads,Servicios")] Reserva reserva, List<Guid> Comodidads, List<Guid> Servicios, List<Huesped> Huespedes)
        {
            if (ModelState.IsValid)
            {
                reserva.Id = Guid.NewGuid();
                foreach (var ComodidadId in Comodidads)
                {
                    var comodidad = await _context.Comodidades.FindAsync(ComodidadId);
                    if (comodidad != null)
                    {
                        reserva.Comodidads.Add(comodidad);
                    }
                }

                foreach (var ServicioAdicionalId in Servicios)
                {
                    var servicio = await _context.ServiciosAdicionales.FindAsync(ServicioAdicionalId);
                    if (servicio != null)
                    {
                        reserva.Servicios.Add(servicio);
                    }
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId != null)
                {
                    // Buscar el usuario en la base de datos utilizando el UserId
                    var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

                    if (usuario != null)
                    {
                        // Crear un nuevo objeto Huesped para el usuario que hace la reserva
                        var usuarioHuesped = new Huesped
                        {
                            Id = Guid.NewGuid(),
                            ReservaId = reserva.Id,
                            Nombre = usuario.Nombre,
                            DocumentoIdentidad = usuario.NumeroDocumento.ToString() // Asumiendo que NumeroDocumento es el DocumentoIdentidad
                        };

                        // Agregar el usuario que hace la reserva a la lista de Huespedes
                        reserva.Huespedes.Add(usuarioHuesped);
                    }
                }

                foreach (var huesped in Huespedes)
                {
                    huesped.Id = Guid.NewGuid();
                    huesped.ReservaId = reserva.Id;
                    reserva.Huespedes.Add(huesped);
                }

                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            // Si hay un error, se deben recargar las listas de selección
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "NumeroHabitacion", reserva.HabitacionId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "NombreUsuario", reserva.UsuarioId);
            ViewData["ComodidadId"] = new SelectList(_context.Comodidades, "Id", "Nombre", reserva.Comodidads.Select(c => c.Id));
            ViewData["ServicioAdicionalId"] = new SelectList(_context.ServiciosAdicionales, "Id", "Nombre", reserva.Servicios.Select(s => s.Id));

            return View(reserva);
        }


        // GET: Reservas/Edit/5
        [AuthorizePermission("Reservas")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var reserva = await _context.Reservas
                .Include(r => r.Comodidads)
                .Include(r => r.Servicios)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "NumeroHabitacion", reserva.HabitacionId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "NombreUsuario", reserva.UsuarioId);
            ViewData["ComodidadId"] = new MultiSelectList(_context.Comodidades, "Id", "Nombre", reserva.Comodidads.Select(c => c.Id));
            ViewData["ServicioAdicionalId"] = new MultiSelectList(_context.ServiciosAdicionales, "Id", "Nombre", reserva.Servicios.Select(s => s.Id));
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EditReserva", reserva);
            }
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("Reservas")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UsuarioId,HabitacionId,FechaInicio,FechaFin,PrecioTotal,NumeroAcompanantes,FechaReserva")] Reserva reserva, List<Guid> Comodidads, List<Guid> Servicios)
        {
            if (id != reserva.Id)
            {
                return Json(new { success = false, message = "ID desajustado" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var reservaToUpdate = await _context.Reservas
                        .Include(r => r.Comodidads)
                        .Include(r => r.Servicios)
                        .FirstOrDefaultAsync(r => r.Id == id);

                    if (reservaToUpdate == null)
                    {
                        return Json(new { success = false, message = "Reserva no encontrada" });
                    }

                    _context.Entry(reservaToUpdate).CurrentValues.SetValues(reserva);

                    reservaToUpdate.Comodidads = new List<Comodidade>();
                    if (Comodidads != null)
                    {
                        foreach (var comodidadId in Comodidads)
                        {
                            var comodidad = await _context.Comodidades.FindAsync(comodidadId);
                            if (comodidad != null)
                            {
                                reservaToUpdate.Comodidads.Add(comodidad);
                            }
                        }
                    }

                    reservaToUpdate.Servicios = new List<ServiciosAdicionale>();
                    if (Servicios != null)
                    {
                        foreach (var servicioId in Servicios)
                        {
                            var servicio = await _context.ServiciosAdicionales.FindAsync(servicioId);
                            if (servicio != null)
                            {
                                reservaToUpdate.Servicios.Add(servicio);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    return Json(new { success = true, id = reserva.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
                    {
                        return Json(new { success = false, message = "Reserva not found" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Concurrency error" });
                    }
                }
            }

            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "NumeroHabitacion", reserva.HabitacionId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "NombreUsuario", reserva.UsuarioId);
            ViewData["ComodidadId"] = new MultiSelectList(_context.Comodidades, "Id", "Nombre", Comodidads);
            ViewData["ServicioAdicionalId"] = new MultiSelectList(_context.ServiciosAdicionales, "Id", "Nombre", Servicios);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EditReserva", reserva);
            }
            return View(reserva);
        }


        // Obtener precio de la reserva al reservar
        // Obtener precio de la reserva al reservar
        [HttpGet]
        public IActionResult GetPrecioHabitacion(Guid id)
        {
            var habitacion = _context.Habitaciones
                .Where(h => h.Id == id)
                .Select(h => h.PrecioPorNoche)
                .FirstOrDefault();
            if (habitacion != 0)
            {
                return Json(habitacion);
            }
            return Json(0);
        }

        [HttpGet]
        public IActionResult GetPrecioServicio(Guid id)
        {
            var servicio = _context.ServiciosAdicionales
                .Where(s => s.Id == id)
                .Select(s => s.Precio)
                .FirstOrDefault();
            if (servicio != 0)
            {
                return Json(servicio);
            }
            return Json(0);
        }


        [HttpGet]
        public IActionResult GetPrecioComodidad(Guid id)
        {
            var comodidad = _context.Comodidades
                .Where(c => c.Id == id)
                .Select(c => c.Precio)
                .FirstOrDefault();
            if (comodidad != 0)
            {
                return Json(comodidad);
            }
            return Json(0);
        }


        // GET: Reservas/Delete/5
        [AuthorizePermission("Reservas")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Habitacion)
                .Include(r => r.Usuario)
                .Include(r => r.Comodidads)
                .Include(r => r.Servicios)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DeleteReserva", reserva);
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizePermission("Reservas")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Servicios)
                .Include(r => r.Comodidads)
                .Include(r => r.Pagos)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva != null)
            {
                // Eliminar los pagos asociados
                foreach (var pago in reserva.Pagos.ToList())
                {
                    _context.Pagos.Remove(pago);
                }

                // Eliminar las relaciones entre la reserva y servicios adicionales
                reserva.Servicios.Clear();

                // Eliminar las relaciones entre la reserva y comodidades
                reserva.Comodidads.Clear();

                // Eliminar la reserva
                _context.Reservas.Remove(reserva);

                await _context.SaveChangesAsync();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(Guid id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
