using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hotel.Models;

public class Huesped
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El nombre del huésped es requerido")]
    [RegularExpression(@"^[a-zA-Z\s]{1,20}$", ErrorMessage = "El nombre del huésped debe ser alfabético y no exceder los 20 caracteres")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "El documento de identidad es requerido")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "El documento de identidad debe ser numérico y tener 11 dígitos")]
    public string? DocumentoIdentidad { get; set; }

    [Required]
    public Guid ReservaId { get; set; }

    [ForeignKey("ReservaId")]
    public Reserva? Reserva { get; set; }
}