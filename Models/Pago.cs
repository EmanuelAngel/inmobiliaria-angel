using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

public enum EstadoPago
{
    [Display(Name = "Activo")]
    Activo = 1,

    [Display(Name = "Anulado")]
    Anulado = 2
}

/// <summary>
/// Representa una transacción económica (pago ordinario, seña o multa) asociada a una reserva.
/// </summary>
/// <remarks>
/// <para>
/// <b>Patrón FK + Propiedad de navegación:</b><br/>
/// Cada relación expone su clave foránea (<c>ReservaId</c>, <c>UsuarioCreacionId</c>, <c>UsuarioAnulacionId</c>)
/// y su propiedad de navegación nullable correspondiente (<c>Reserva?</c>, <c>UsuarioCreacion?</c>, <c>UsuarioAnulacion?</c>).
/// </para>
/// </remarks>
public class Pago
{
    [Display(Name = "Código")]
    public int Id { get; set; }

    /// <summary>Clave foránea hacia <see cref="Reserva"/>. Siempre poblada.</summary>
    [Required(ErrorMessage = "Debe seleccionar una reserva")]
    [Display(Name = "Reserva")]
    public int ReservaId { get; set; }

    /// <summary>Reserva a la que pertenece el pago. Solo poblada cuando el repositorio hace JOIN.</summary>
    public Reserva? Reserva { get; set; }

    [Required(ErrorMessage = "El concepto es obligatorio")]
    [StringLength(255, ErrorMessage = "El concepto no puede superar los 255 caracteres")]
    [Display(Name = "Concepto")]
    public string Concepto { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de pago es obligatoria")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Pago")]
    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "El importe es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El importe debe ser mayor a 0")]
    [DataType(DataType.Currency)]
    [Display(Name = "Importe")]
    public decimal Importe { get; set; }

    [Display(Name = "Estado")]
    public EstadoPago Estado { get; set; } = EstadoPago.Activo;

    // ── Auditoría ────────────────────────────────────────────────────────────
    public int UsuarioCreacionId { get; set; }
    public Usuario? UsuarioCreacion { get; set; }

    public int? UsuarioAnulacionId { get; set; }
    public Usuario? UsuarioAnulacion { get; set; }
}
