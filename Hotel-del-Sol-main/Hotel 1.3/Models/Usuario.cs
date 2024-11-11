using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Models;

public partial class Usuario
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    public string NombreUsuario { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    public string CorreoElectronico { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Contrasena { get; set; } = null!;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "El nombre solo puede contener letras.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "El apellido solo puede contener letras.")]
    public string Apellido { get; set; } = null!;

    [Phone(ErrorMessage = "El número de teléfono no es válido.")]
    public string? Telefono { get; set; }

    [DataType(DataType.Date)]
    [CustomValidation(typeof(Usuario), nameof(ValidarFechaNacimiento))]
    public DateOnly? FechaNacimiento { get; set; }

    public Guid? GeneroId { get; set; }

    public Guid? RolId { get; set; }

    public string? ImagenPerfil { get; set; }

    public DateOnly? FechaRegistro { get; set; }

    public bool? Activo { get; set; }  

    public long? NumeroDocumento { get; set; } // Nuevo campo

    public Guid? TipoDocumentoId { get; set; } // Nuevo campo

    public virtual Genero? Genero { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual Role? Rol { get; set; }

    public virtual TipoDocumento? TipoDocumento { get; set; } // Nueva relación

    public static ValidationResult? ValidarFechaNacimiento(DateOnly? fechaNacimiento, ValidationContext context)
    {
        if (fechaNacimiento.HasValue && fechaNacimiento.Value > DateOnly.FromDateTime(DateTime.Now))
        {
            return new ValidationResult("La fecha de nacimiento no puede ser una fecha futura.");
        }
        return ValidationResult.Success;
    }
}
