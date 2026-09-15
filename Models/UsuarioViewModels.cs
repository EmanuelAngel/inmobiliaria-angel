using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace inmobiliaria_lab2.Models;

public class LoginUsuarioViewModel
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
    [StringLength(150, ErrorMessage = "El correo electrónico no puede superar los 150 caracteres")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

public class CrearUsuarioViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
    [StringLength(150, ErrorMessage = "El correo electrónico no puede superar los 150 caracteres")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es obligatorio")]
    [Display(Name = "Rol")]
    public RolUsuario Rol { get; set; } = RolUsuario.Empleado;

    [Display(Name = "Foto de perfil (avatar)")]
    public IFormFile? AvatarFile { get; set; }
}

public class EditarUsuarioViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
    [StringLength(150, ErrorMessage = "El correo electrónico no puede superar los 150 caracteres")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es obligatorio")]
    [Display(Name = "Rol")]
    public RolUsuario Rol { get; set; } = RolUsuario.Empleado;

    public string? AvatarActual { get; set; }

    [Display(Name = "Cambiar foto de perfil")]
    public IFormFile? AvatarFile { get; set; }

    [Display(Name = "Quitar foto de perfil")]
    public bool QuitarAvatar { get; set; }
}

public class PerfilUsuarioViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
    [StringLength(150, ErrorMessage = "El correo electrónico no puede superar los 150 caracteres")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Rol")]
    public RolUsuario Rol { get; set; }

    public string? AvatarActual { get; set; }

    [Display(Name = "Cambiar foto de perfil")]
    public IFormFile? AvatarFile { get; set; }

    [Display(Name = "Quitar foto de perfil")]
    public bool QuitarAvatar { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Contraseña actual")]
    public string? ClaveActual { get; set; }

    [StringLength(100, MinimumLength = 8, ErrorMessage = "La nueva contraseña debe tener entre 8 y 100 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string? NuevaClave { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nueva contraseña")]
    [Compare("NuevaClave", ErrorMessage = "La nueva contraseña y la confirmación no coinciden")]
    public string? ConfirmarClave { get; set; }
}

public class UsuarioListadoViewModel
{
    public int Id { get; set; }
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public string? Avatar { get; set; }
    public bool Activo { get; set; }
}

public class UsuarioDetalleViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    [Display(Name = "Nombre completo")]
    public string NombreCompleto => $"{Nombre} {Apellido}";
    public string Email { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public string? Avatar { get; set; }
    public bool Activo { get; set; }
}
