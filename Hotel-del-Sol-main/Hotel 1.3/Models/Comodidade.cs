using System;
using System.Collections.Generic;

namespace Hotel.Models;

public partial class Comodidade
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Imagen { get; set; }

    public bool? Activo { get; set; }

    public decimal Precio { get; set; }

    public virtual ICollection<Habitacione> Habitaciones { get; set; } = new List<Habitacione>(); // Nueva relación
}
