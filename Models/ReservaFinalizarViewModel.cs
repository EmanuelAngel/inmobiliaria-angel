using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

/// <summary>
/// ViewModel para la liquidación y cobro de multa por terminación anticipada de una reserva.
/// </summary>
public class ReservaFinalizarViewModel
{
    public int ReservaId { get; set; }

    public Reserva? Reserva { get; set; }

    [Display(Name = "Fecha Hasta Original")]
    public DateOnly FechaHastaOriginal { get; set; }

    [Required(ErrorMessage = "La fecha de fin anticipado es obligatoria")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha Efectiva de Terminación")]
    public DateOnly FechaFinAnticipado { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public int DiasTotales { get; set; }
    public int DiasTranscurridos { get; set; }
    public int DiasRestantes { get; set; }

    public decimal PorcentajeTranscurrido { get; set; }
    public decimal PorcentajeMulta { get; set; }

    [DataType(DataType.Currency)]
    [Display(Name = "Base Económica (Días no gozados)")]
    public decimal BaseEconomica { get; set; }

    [DataType(DataType.Currency)]
    [Display(Name = "Monto de Multa")]
    public decimal MontoMulta { get; set; }

    [Required(ErrorMessage = "El concepto del pago es obligatorio")]
    [StringLength(255, ErrorMessage = "El concepto no puede superar los 255 caracteres")]
    [Display(Name = "Concepto de Pago")]
    public string ConceptoPago { get; set; } = "Multa por cancelación anticipada";
}
