using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

public class TipoInmueble
{
    [Display(Name = "Código")]
    public int Id { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(100, ErrorMessage = "La descripción no puede superar los 100 caracteres")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}
