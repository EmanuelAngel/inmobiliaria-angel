using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

public class Inquilino
{
    [Display(Name = "Código")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El DNI es obligatorio")]
    [StringLength(20, ErrorMessage = "El DNI no puede superar los 20 caracteres")]
    [Display(Name = "DNI")]
    public string Dni { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre completo no puede superar los 200 caracteres")]
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
    [StringLength(150, ErrorMessage = "El email no puede superar los 150 caracteres")]
    [Display(Name = "Correo Electrónico")]
    public string? Email { get; set; }

    [StringLength(50, ErrorMessage = "El teléfono no puede superar los 50 caracteres")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }
}
