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
    public class UsuariosController : Controller
    {
        private readonly HotelContext _context;

        public UsuariosController(HotelContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var hotelContext = _context.Usuarios.Include(u => u.Genero).Include(u => u.Rol);
            return View(await hotelContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        // GET: Usuarios/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios
            .Include(u => u.Genero)
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null)
        {
            return NotFound();
        }

        return PartialView("_DetailsUsuarios", usuario); // Vista parcial con los detalles del usuario
    }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre");
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Nombre", "0B9A405E-EFB9-4E0F-A3AF-29854CBB0A1E");
            ViewData["TipoDocumentoId"] = new SelectList(_context.TiposDocumento, "Id", "Nombre");

            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombreUsuario,CorreoElectronico,Contrasena,Nombre,Apellido,Telefono,FechaNacimiento,GeneroId,RolId,ImagenPerfil,FechaRegistro,Activo,NumeroDocumento,TipoDocumentoId")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Id = Guid.NewGuid();

                // Asignar el Id del rol predefinido
                usuario.RolId = new Guid("FB0BBBB3-1F6E-4796-AEAA-9402B9CFE19B");

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", usuario.GeneroId);
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Nombre", "0B9A405E-EFB9-4E0F-A3AF-29854CBB0A1E");

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", usuario.GeneroId);
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Nombre", usuario.RolId);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EditUsuarios", usuario);
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,NombreUsuario,CorreoElectronico,Contrasena,Nombre,Apellido,Telefono,FechaNacimiento,GeneroId,RolId,ImagenPerfil,FechaRegistro,Activo")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
               
            }
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", usuario.GeneroId);
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Nombre", usuario.RolId);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EditUsuarios", usuario);
            }
            return View(usuario);
        }

        // GET: Usuarios/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Usuarios/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string nombreUsuario, string contrasena)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .ThenInclude(r => r.Permisos)
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Contrasena == contrasena);

            if (usuario != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim("FullName", $"{usuario.Nombre} {usuario.Apellido}"),
                    new Claim(ClaimTypes.Role, usuario.Rol.Nombre),
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim("UserId", usuario.Id.ToString())
                    
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Login");
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
            return View();
        }

        // POST: Usuarios/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Genero)
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DeleteUsuarios", usuario);
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(Guid id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
