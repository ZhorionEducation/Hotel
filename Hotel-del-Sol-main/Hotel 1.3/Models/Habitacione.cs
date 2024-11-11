using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Models;

public partial class Habitacione
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El número de la habitación es requerido")]
    public string NumeroHabitacion { get; set; } = null!;

    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "La capacidad de la habitación es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser un número positivo")]
    public int Capacidad { get; set; }

    [Required(ErrorMessage = "El precio por noche es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio por noche debe ser un valor positivo")]
    public decimal PrecioPorNoche { get; set; }

    
    public string? Imagen { get; set; } 

    public bool? Activo { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Comodidade> Comodidades { get; set; } = new List<Comodidade>(); // Nueva relación
}
