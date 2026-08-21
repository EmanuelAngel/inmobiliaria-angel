using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

public class Propietario
{
    [Display(Name = "Código")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El DNI es obligatorio")]
    [StringLength(20, ErrorMessage = "El DNI no puede superar los 20 caracteres")]
    [Display(Name = "DNI")]
    public string Dni { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
    [StringLength(150, ErrorMessage = "El email no puede superar los 150 caracteres")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [StringLength(50, ErrorMessage = "El teléfono no puede superar los 50 caracteres")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [Display(Name = "Nombre Completo")]
    public string NombreCompleto => $"{Nombre} {Apellido}";
}

