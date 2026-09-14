using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

/// <summary>
/// Representa una imagen asociada a la galería de un inmueble.
/// </summary>
public class ImagenInmueble
{
    [Display(Name = "Código")]
    public int Id { get; set; }

    /// <summary>Clave foránea hacia <see cref="Inmueble"/>.</summary>
    [Required(ErrorMessage = "El inmueble es obligatorio")]
    [Display(Name = "Inmueble")]
    public int InmuebleId { get; set; }

    [Required(ErrorMessage = "La ruta o URL de la imagen es obligatoria")]
    [StringLength(255, ErrorMessage = "La URL no puede superar los 255 caracteres")]
    [Display(Name = "Imagen")]
    public string Url { get; set; } = string.Empty;
}
