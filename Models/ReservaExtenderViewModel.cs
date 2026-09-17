using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

/// <summary>
/// ViewModel para la extensión y renovación consecutiva de una reserva activa.
/// </summary>
public class ReservaExtenderViewModel
{
    [Display(Name = "Reserva Original")]
    public int ReservaOriginalId { get; set; }

    /// <summary>
    /// Alias de conveniencia para compatibilidad con el parámetro de ruta y modelos existentes.
    /// </summary>
    public int ReservaId
    {
        get => ReservaOriginalId;
        set => ReservaOriginalId = value;
    }

    /// <summary>
    /// Instancia de la reserva original con navegación cargada (Inquilino e Inmueble) para la vista.
    /// </summary>
    public Reserva? ReservaOriginal { get; set; }

    /// <summary>
    /// Alias de conveniencia para ReservaOriginal.
    /// </summary>
    public Reserva? Reserva
    {
        get => ReservaOriginal;
        set => ReservaOriginal = value;
    }

    public int InmuebleId { get; set; }

    public int InquilinoId { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha Desde (Consecutiva)")]
    public DateOnly FechaDesde { get; set; }

    [Required(ErrorMessage = "La fecha de finalización es obligatoria")]
    [DataType(DataType.Date)]
    [Display(Name = "Nueva Fecha de Finalización")]
    public DateOnly FechaHasta { get; set; }

    [Required(ErrorMessage = "El monto por día es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto por día debe ser mayor a 0")]
    [DataType(DataType.Currency)]
    [Display(Name = "Monto por Día")]
    public decimal MontoPorDia { get; set; }

    // ── Helpers calculados ──────────────────────────────────────────────────

    /// <summary>
    /// Cantidad de días de extensión. Retorna 0 si la fecha hasta no es posterior a la fecha desde.
    /// </summary>
    [Display(Name = "Días de Extensión")]
    public int DiasTotal => FechaHasta > FechaDesde ? FechaHasta.DayNumber - FechaDesde.DayNumber : 0;

    /// <summary>
    /// Costo total proyectado para la extensión.
    /// </summary>
    [DataType(DataType.Currency)]
    [Display(Name = "Monto Total Estimado")]
    public decimal MontoTotal => DiasTotal * MontoPorDia;
}
