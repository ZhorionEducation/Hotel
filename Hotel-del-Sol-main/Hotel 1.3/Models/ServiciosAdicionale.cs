using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Models;

public partial class ServiciosAdicionale
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    public decimal Precio { get; set; }

    public string? Imagen { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
