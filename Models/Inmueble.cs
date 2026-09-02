using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

public enum EstadoInmueble
{
    [Display(Name = "Disponible")]
    Disponible = 1,

    [Display(Name = "Suspendido")]
    Suspendido = 2
}

/// <summary>
/// Representa un inmueble ofertado por un propietario.
/// </summary>
/// <remarks>
/// <para>
/// <b>Patrón FK + Propiedad de navegación:</b><br/>
/// Cada relación expone dos propiedades con responsabilidades distintas:
/// </para>
/// <list type="table">
///   <item>
///     <term><c>PropietarioId</c> / <c>TipoId</c></term>
///     <description>Clave foránea. Siempre poblada. Usar para persistencia (INSERT/UPDATE) y filtros sin JOIN.</description>
///   </item>
///   <item>
///     <term><c>Propietario?</c> / <c>Tipo?</c></term>
///     <description>Propiedad de navegación. Solo poblada cuando el repositorio usa <c>MapearConJoins</c>. Usar en vistas y lógica de presentación.</description>
///   </item>
/// </list>
/// <para>
/// Nunca asumir que la propiedad de navegación está poblada.
/// El repositorio decide qué mapper usar; la vista depende solo de lo que el controlador le pasa.
/// </para>
/// </remarks>
public class Inmueble
{
    [Display(Name = "Código")]
    public int Id { get; set; }

    /// <summary>Clave foránea hacia <see cref="Propietario"/>. Siempre poblada.</summary>
    [Required(ErrorMessage = "Debe seleccionar un propietario")]
    [Display(Name = "Propietario")]
    public int PropietarioId { get; set; }

    /// <summary>Propietario del inmueble. Solo poblado cuando el repositorio hace JOIN con la tabla propietarios.</summary>
    public Propietario? Propietario { get; set; }

    /// <summary>Clave foránea hacia <see cref="TipoInmueble"/>. Siempre poblada.</summary>
    [Required(ErrorMessage = "Debe seleccionar un tipo de inmueble")]
    [Display(Name = "Tipo de Inmueble")]
    public int TipoId { get; set; }

    /// <summary>Tipo del inmueble. Solo poblado cuando el repositorio hace JOIN con la tabla tipos_inmueble.</summary>
    public TipoInmueble? Tipo { get; set; }

    [Required(ErrorMessage = "La dirección es obligatoria")]
    [StringLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres")]
    [Display(Name = "Dirección")]
    public string Direccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El cupo es obligatorio")]
    [Range(1, 100, ErrorMessage = "El cupo debe ser de al menos 1 persona")]
    [Display(Name = "Cupo (personas)")]
    public int Cupo { get; set; } = 1;

    [Required(ErrorMessage = "El precio por día es obligatorio")]
    [Range(1.0, 100000000.0, ErrorMessage = "El precio por día debe ser mayor a 0")]
    [DataType(DataType.Currency)]
    [Display(Name = "Precio por Día")]
    public decimal PrecioPorDia { get; set; }

    [Required(ErrorMessage = "El porcentaje de seña es obligatorio")]
    [Range(0, 100, ErrorMessage = "El porcentaje de seña debe estar entre 0 y 100")]
    [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
    [Display(Name = "Porcentaje de Seña (%)")]
    public decimal PorcentajeSenia { get; set; }

    [Display(Name = "Latitud")]
    [Range(-90.0, 90.0, ErrorMessage = "La latitud debe estar entre -90 y 90")]
    public decimal? Latitud { get; set; }

    [Display(Name = "Longitud")]
    [Range(-180.0, 180.0, ErrorMessage = "La longitud debe estar entre -180 y 180")]
    public decimal? Longitud { get; set; }

    [StringLength(255, ErrorMessage = "La ruta de la imagen no puede superar los 255 caracteres")]
    [Display(Name = "Imagen de Portada")]
    public string? ImagenPortada { get; set; }

    [Display(Name = "Estado")]
    public EstadoInmueble Estado { get; set; } = EstadoInmueble.Disponible;
}
