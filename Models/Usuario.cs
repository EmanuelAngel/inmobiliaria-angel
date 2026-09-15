using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

public enum RolUsuario
{
    [Display(Name = "Administrador")]
    Administrador = 1,

    [Display(Name = "Empleado")]
    Empleado = 2
}

public class Usuario
{
    public int Id { get; set; }


    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
    [StringLength(150, ErrorMessage = "El correo electrónico no puede superar los 150 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Avatar { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio")]
    public RolUsuario Rol { get; set; } = RolUsuario.Empleado;

    public bool Activo { get; set; } = true;

    [Display(Name = "Nombre completo")]
    public string NombreCompleto => $"{Nombre} {Apellido}";
}
