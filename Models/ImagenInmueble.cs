using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

/// <summary>
/// Representa una imagen asociada a la galería de un inmueble.
/// </summary>
/// <remarks>
/// <para>
/// <b>Patrón FK + Propiedad de navegación:</b><br/>
/// Expone <see cref="InmuebleId"/> como clave foránea persistente y <see cref="Inmueble"/> como
/// navegación opcional cuando el repositorio hace JOIN con la tabla <c>INMUEBLE</c>.
/// </para>
/// </remarks>
public class ImagenInmueble
{
    [Display(Name = "Código")]
    public int Id { get; set; }

    /// <summary>Clave foránea hacia <see cref="Inmueble"/>. Siempre poblada.</summary>
    [Required(ErrorMessage = "El inmueble es obligatorio")]
    [Display(Name = "Inmueble")]
    public int InmuebleId { get; set; }

    /// <summary>Inmueble al que pertenece la imagen. Solo poblado cuando el repositorio hace JOIN.</summary>
    public Inmueble? Inmueble { get; set; }

    [Required(ErrorMessage = "La ruta o URL de la imagen es obligatoria")]
    [StringLength(255, ErrorMessage = "La URL no puede superar los 255 caracteres")]
    [Display(Name = "Imagen")]
    public string Url { get; set; } = string.Empty;
}
