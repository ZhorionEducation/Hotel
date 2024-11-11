﻿using Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hotel.ViewModels;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

[AuthorizePermission("PermisoRol")]
public class PermisoRolController : Controller
{
    private readonly HotelContext _context;

    public PermisoRolController(HotelContext context)
    {
        _context = context;
    }

    [AuthorizePermission("PermisoRol")]
    public async Task<IActionResult> Index()
    {
        var permisos = await _context.Permisos.ToListAsync();
        var roles = await _context.Roles.ToListAsync();

        var viewModel = new PermisoRolIndexViewModel
        {
            Permisos = permisos,
            Roles = roles
        };

        return View("Index", viewModel);
    }

    [HttpPost]
    [AuthorizePermission("PermisoRol")]
    public async Task<IActionResult> AddPermisoToRol(Guid rolId, Guid permisoId)
    {
        var rol = await _context.Roles.Include(r => r.Permisos).FirstOrDefaultAsync(r => r.Id == rolId);
        if (rol == null)
        {
            return NotFound(new { message = "Rol no encontrado" });
        }

        var permiso = await _context.Permisos.FindAsync(permisoId);
        if (permiso == null)
        {
            return NotFound(new { message = "Permiso no encontrado" });
        }

        if (!rol.Permisos.Contains(permiso))
        {
            rol.Permisos.Add(permiso);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [AuthorizePermission("PermisoRol")]
    public async Task<IActionResult> EditPermiso(Permiso permiso)
    {
        if (ModelState.IsValid)
        {
            _context.Update(permiso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return PartialView("_ErrorModal");
    }

    [HttpPost]
    [AuthorizePermission("PermisoRol")]
    public async Task<IActionResult> DeletePermiso(Guid id)
    {
        var permiso = await _context.Permisos.FindAsync(id);
        if (permiso == null) return NotFound();

        _context.Permisos.Remove(permiso);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [AuthorizePermission("PermisoRol")]
    public async Task<IActionResult> DeleteRol(Guid id)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null) return NotFound();

        _context.Roles.Remove(rol);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [AuthorizePermission("PermisoRol")]
    public async Task<IActionResult> Details(Guid id, bool isPermiso)
    {
        Console.WriteLine($"Detalles solicitados para ID: {id}, isPermiso: {isPermiso}");

        if (isPermiso)
        {
            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso == null)
            {
                Console.WriteLine($"Permiso con ID: {id} no encontrado.");
                return NotFound();
            }

            return PartialView("_DetailsPermiso", permiso);
        }
        else
        {
            var rol = await _context.Roles
                .Include(r => r.Usuarios)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (rol == null)
            {
                Console.WriteLine($"Rol con ID: {id} no encontrado.");
                return NotFound();
            }

            var viewModel = new RoleDetailsViewModel
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                Estado = rol.Estado,
                CantidadUsuarios = rol.Usuarios?.Count ?? 0,
                NombresUsuarios = rol.Usuarios?.Select(u => u.NombreUsuario).ToList() ?? new List<string>()
            };

            Console.WriteLine($"Rol: {viewModel.Nombre}, Cantidad de Usuarios: {viewModel.CantidadUsuarios}");
            foreach (var nombreUsuario in viewModel.NombresUsuarios)
            {
                Console.WriteLine($"Usuario: {nombreUsuario}");
            }

            return PartialView("_DetailsRol", viewModel);
        }
    }

    [HttpPost]
    [AuthorizePermission("PermisoRol")]
    public async Task<IActionResult> CreatePermiso(Permiso permiso)
    {
        if (ModelState.IsValid)
        {
            permiso.Id = Guid.NewGuid();
            _context.Permisos.Add(permiso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return PartialView("_ErrorModal");
    }

    [HttpPost]
    [AuthorizePermission("PermisoRol")]
    public async Task<IActionResult> CreateRol(Role rol)
    {
        if (ModelState.IsValid)
        {
            rol.Id = Guid.NewGuid();
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return PartialView("_ErrorModal");
    }

    [HttpPost]
    [AuthorizePermission("PermisoRol")]
    public async Task<IActionResult> ChangeEstadoRol(Guid id, [FromBody] EstadoChangeModel model)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null)
        {
            Console.WriteLine($"Rol con ID: {id} no encontrado.");
            return Json(new { success = false, message = "Rol no encontrado" });
        }

        Console.WriteLine($"Cambiando estado del rol con ID: {id} a {model.Estado}");
        rol.Estado = model.Estado;
        await _context.SaveChangesAsync();
        Console.WriteLine($"Estado del rol con ID: {id} cambiado a {model.Estado}");

        return Json(new { success = true });
    }

    public async Task<IActionResult> GetRolDetails(Guid id)
    {
        var rol = await _context.Roles
            .Include(r => r.Permisos)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rol == null)
            return Json(new { success = false, message = "Rol no encontrado" });

        return Json(new { 
            success = true, 
            nombre = rol.Nombre,
            permisos = rol.Permisos.Select(p => p.Id).ToList()
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditRol([FromForm] Guid Id, [FromForm] string Nombre, [FromForm] string PermisosIds)
    {
        try
        {
            var rol = await _context.Roles
                .Include(r => r.Permisos)
                .FirstOrDefaultAsync(r => r.Id == Id);

            if (rol == null)
                return NotFound(new { success = false, message = "Rol no encontrado" });

            if (string.IsNullOrWhiteSpace(Nombre))
                return BadRequest(new { success = false, message = "El nombre es requerido" });

            rol.Nombre = Nombre;

            // Limpiar permisos existentes
            rol.Permisos.Clear();

            if (!string.IsNullOrEmpty(PermisosIds))
            {
                var permisoIdList = JsonSerializer.Deserialize<List<Guid>>(PermisosIds);
                var permisos = await _context.Permisos
                    .Where(p => permisoIdList.Contains(p.Id))
                    .ToListAsync();

                foreach (var permiso in permisos)
                {
                    rol.Permisos.Add(permiso);
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Rol actualizado correctamente" });
        }
        catch (JsonException)
        {
            return BadRequest(new { success = false, message = "Error al procesar los permisos seleccionados" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error al actualizar el rol: {ex.Message}" });
        }
    }

    public class EstadoChangeModel
    {
        public bool Estado { get; set; }
    }
}
