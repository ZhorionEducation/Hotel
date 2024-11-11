using System;
using System.Collections.Generic;

namespace Hotel.Models;

public partial class Pago
{
    public Guid Id { get; set; }

    public Guid? ReservaId { get; set; }

    public string MetodoPago { get; set; } = null!;

    public DateOnly FechaPago { get; set; }

    public decimal Monto { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Reserva? Reserva { get; set; }
}
