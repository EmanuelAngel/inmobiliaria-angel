using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

/// <summary>
/// ViewModel para la edición de pagos.
/// Restringe estrictamente la modificación al campo Concepto.
/// </summary>
public class PagoEditViewModel
{
    public int Id { get; set; }

    [Display(Name = "Reserva")]
    public int ReservaId { get; set; }

    [Display(Name = "Inquilino")]
    public string? InquilinoNombre { get; set; }

    [Display(Name = "Inmueble")]
    public string? InmuebleDireccion { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Pago")]
    public DateOnly Fecha { get; set; }

    [DataType(DataType.Currency)]
    [Display(Name = "Importe")]
    public decimal Importe { get; set; }

    [Display(Name = "Estado")]
    public EstadoPago Estado { get; set; }

    [Required(ErrorMessage = "El concepto es obligatorio")]
    [StringLength(255, ErrorMessage = "El concepto no puede superar los 255 caracteres")]
    [Display(Name = "Concepto")]
    public string Concepto { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel para el listado híbrido de pagos (global o contextual por reserva).
/// </summary>
public class PagosIndexViewModel
{
    public IList<Pago> Pagos { get; set; } = [];

    /// <summary>Poblado cuando el listado se filtra por una reserva específica.</summary>
    public Reserva? ReservaContexto { get; set; }

    public decimal TotalReserva { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal SaldoPendiente => Math.Max(0, TotalReserva - TotalPagado);

    public PaginacionViewModel Paginacion { get; set; } = new();

    public int? ReservaIdFiltro { get; set; }
    public string? EstadoFiltro { get; set; }
}
