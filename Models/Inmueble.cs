using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

public enum EstadoInmueble
{
    [Display(Name = "Disponible")]
    Disponible = 1,

    [Display(Name = "Suspendido")]
    Suspendido = 2
}

public class Inmueble
{
    [Display(Name = "Código")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un propietario")]
    [Display(Name = "Propietario")]
    public int PropietarioId { get; set; }

    public Propietario? Propietario { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un tipo de inmueble")]
    [Display(Name = "Tipo de Inmueble")]
    public int TipoId { get; set; }

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
    [Range(0.0, 100.0, ErrorMessage = "El porcentaje de seña debe estar entre 0 y 100")]
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
