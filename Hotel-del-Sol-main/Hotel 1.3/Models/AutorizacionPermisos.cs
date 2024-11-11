using Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

public class AuthorizePermissionAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _permission;

    public AuthorizePermissionAttribute(string permission)
    {
        _permission = permission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userId = Guid.Parse(userIdClaim);
        var dbContext = context.HttpContext.RequestServices.GetService(typeof(HotelContext)) as HotelContext;
        var usuario = dbContext.Usuarios
            .Include(u => u.Rol)
            .ThenInclude(r => r.Permisos)
            .FirstOrDefault(u => u.Id == userId);

        if (usuario == null || !usuario.Rol.Permisos.Any(p => p.Nombre == _permission))
        {
            context.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/AccessDenied.cshtml",
                StatusCode = 403
            };
        }
    }
}