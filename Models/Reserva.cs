using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

public enum EstadoReserva
{
    [Display(Name = "Activa")]
    Activa = 1,

    [Display(Name = "Finalizada")]
    Finalizada = 2,

    [Display(Name = "Cancelada")]
    Cancelada = 3
}

/// <summary>
/// Representa una reserva de alquiler temporal de un inmueble por un inquilino.
/// </summary>
/// <remarks>
/// <para>
/// <b>Patrón FK + Propiedad de navegación:</b><br/>
/// Cada relación expone dos propiedades con responsabilidades distintas:
/// </para>
/// <list type="table">
///   <item>
///     <term><c>InquilinoId</c> / <c>InmuebleId</c></term>
///     <description>Clave foránea. Siempre poblada. Usar para persistencia (INSERT/UPDATE) y filtros sin JOIN.</description>
///   </item>
///   <item>
///     <term><c>Inquilino?</c> / <c>Inmueble?</c></term>
///     <description>Propiedad de navegación. Solo poblada cuando el repositorio usa <c>MapearConJoins</c>. Usar en vistas y lógica de presentación.</description>
///   </item>
/// </list>
/// <para>
/// Nunca asumir que la propiedad de navegación está poblada.
/// El repositorio decide qué mapper usar; la vista depende solo de lo que el controlador le pasa.
/// </para>
/// </remarks>
public class Reserva
{
    [Display(Name = "Código")]
    public int Id { get; set; }

    /// <summary>Clave foránea hacia <see cref="Inquilino"/>. Siempre poblada.</summary>
    [Required(ErrorMessage = "Debe seleccionar un inquilino")]
    [Display(Name = "Inquilino")]
    public int InquilinoId { get; set; }

    /// <summary>Inquilino de la reserva. Solo poblado cuando el repositorio hace JOIN.</summary>
    public Inquilino? Inquilino { get; set; }

    /// <summary>Clave foránea hacia <see cref="Inmueble"/>. Siempre poblada.</summary>
    [Required(ErrorMessage = "Debe seleccionar un inmueble")]
    [Display(Name = "Inmueble")]
    public int InmuebleId { get; set; }

    /// <summary>Inmueble de la reserva. Solo poblado cuando el repositorio hace JOIN.</summary>
    public Inmueble? Inmueble { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha Desde")]
    public DateOnly FechaDesde { get; set; }

    [Required(ErrorMessage = "La fecha de finalización es obligatoria")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha Hasta")]
    public DateOnly FechaHasta { get; set; }

    /// <summary>
    /// Fecha de terminación anticipada. Se guarda cuando el inquilino termina antes.
    /// No se expone en UI hasta implementar el módulo de cancelaciones con multa.
    /// </summary>
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Fin Anticipado")]
    public DateOnly? FechaFinAnticipado { get; set; }

    [Required(ErrorMessage = "El monto por día es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto por día debe ser mayor a 0")]
    [DataType(DataType.Currency)]
    [Display(Name = "Monto por Día")]
    public decimal MontoPorDia { get; set; }

    [Display(Name = "Estado")]
    public EstadoReserva Estado { get; set; } = EstadoReserva.Activa;

    // Auditoría — se persisten en BD, sin UI hasta implementar autenticación.
    public int? UsuarioCreacionId { get; set; }
    public int? UsuarioTerminacionId { get; set; }

    // ── Helpers calculados (no mapeados a BD) ─────────────────────────────────

    /// <summary>Cantidad de días de la reserva (fecha hasta - fecha desde).</summary>
    public int DiasTotal => FechaHasta.DayNumber - FechaDesde.DayNumber;

    /// <summary>Monto total de la reserva (días × monto por día).</summary>
    [DataType(DataType.Currency)]
    [Display(Name = "Monto Total")]
    public decimal MontoTotal => DiasTotal * MontoPorDia;
}
