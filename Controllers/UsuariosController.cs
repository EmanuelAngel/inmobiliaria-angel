using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria_lab2.Models;
using inmobiliaria_lab2.Repositories;
using inmobiliaria_lab2.Services;

namespace inmobiliaria_lab2.Controllers;

[Authorize]
public class UsuariosController(
    IRepositorioUsuario repositorioUsuario,
    IAlmacenadorArchivos almacenadorArchivos,
    ILogger<UsuariosController> logger) : Controller
{
    private readonly IRepositorioUsuario _repoUsuario = repositorioUsuario;
    private readonly IAlmacenadorArchivos _almacenadorArchivos = almacenadorArchivos;
    private readonly ILogger<UsuariosController> _logger = logger;

    #region Autenticación (Login / Logout)

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginUsuarioViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginUsuarioViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var usuario = _repoUsuario.ObtenerPorEmail(model.Email);
        if (usuario == null || !usuario.Activo)
        {
            _logger.LogWarning("Intento fallido de login: usuario inexistente o inactivo ({Email})", model.Email);
            ModelState.AddModelError(string.Empty, "Correo electrónico o contraseña incorrectos.");
            return View(model);
        }

        var passwordHasher = new PasswordHasher<Usuario>();
        var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, model.Password);
        if (resultado == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Intento fallido de login: contraseña incorrecta ({Email})", model.Email);
            ModelState.AddModelError(string.Empty, "Correo electrónico o contraseña incorrectos.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Email),
            new(ClaimTypes.GivenName, usuario.NombreCompleto),
            new(ClaimTypes.Role, usuario.Rol.ToString()),
            new("Avatar", usuario.Avatar ?? string.Empty)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        _logger.LogInformation("Usuario {Email} inició sesión correctamente con rol {Rol}", usuario.Email, usuario.Rol);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("Usuario {Email} cerró sesión", User.Identity?.Name);
        return RedirectToAction("Login", "Usuarios");
    }

    #endregion

    #region Administración de Usuarios (Solo Administrador)

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public IActionResult Index(int pagina = 1, int tamDePagina = 10)
    {
        pagina = Math.Max(1, pagina);
        tamDePagina = Math.Clamp(tamDePagina, 1, 50);

        var lista = _repoUsuario.ObtenerLista(pagina, tamDePagina);
        var total = _repoUsuario.ObtenerCantidad();

        var model = lista.Select(u => new UsuarioListadoViewModel
        {
            Id = u.Id,
            NombreCompleto = u.NombreCompleto,
            Email = u.Email,
            Rol = u.Rol,
            Avatar = u.Avatar,
            Activo = u.Activo
        }).ToList();

        ViewBag.Paginacion = new PaginacionViewModel
        {
            PaginaActual = pagina,
            TamDePagina = tamDePagina,
            TotalPaginas = (int)Math.Ceiling((double)total / tamDePagina),
            TotalRegistros = total,
            Controlador = "Usuarios",
            Accion = "Index"
        };

        return View(model);
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public IActionResult Detalles(int id)
    {
        var usuario = _repoUsuario.ObtenerPorId(id);
        if (usuario == null)
        {
            return NotFound();
        }

        var model = new UsuarioDetalleViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Rol = usuario.Rol,
            Avatar = usuario.Avatar,
            Activo = usuario.Activo
        };

        return View(model);
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public IActionResult Crear()
    {
        return View(new CrearUsuarioViewModel());
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearUsuarioViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existente = _repoUsuario.ObtenerPorEmail(model.Email);
        if (existente != null)
        {
            ModelState.AddModelError(nameof(model.Email), "El correo electrónico ya se encuentra registrado.");
            return View(model);
        }

        var usuario = new Usuario
        {
            Nombre = model.Nombre.Trim(),
            Apellido = model.Apellido.Trim(),
            Email = model.Email.Trim(),
            Rol = model.Rol,
            Activo = true
        };

        var passwordHasher = new PasswordHasher<Usuario>();
        usuario.PasswordHash = passwordHasher.HashPassword(usuario, model.Password);

        if (model.AvatarFile != null && model.AvatarFile.Length > 0)
        {
            try
            {
                usuario.Avatar = await _almacenadorArchivos.GuardarArchivoAsync(model.AvatarFile, "usuarios/avatares");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(model.AvatarFile), ex.Message);
                return View(model);
            }
        }

        _repoUsuario.Alta(usuario);
        _logger.LogInformation("Usuario {Email} con rol {Rol} creado correctamente.", usuario.Email, usuario.Rol);

        TempData["Mensaje"] = "Usuario creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public IActionResult Editar(int id)
    {
        var usuario = _repoUsuario.ObtenerPorId(id);
        if (usuario == null)
        {
            return NotFound();
        }

        var model = new EditarUsuarioViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Rol = usuario.Rol,
            AvatarActual = usuario.Avatar
        };

        return View(model);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, EditarUsuarioViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var usuario = _repoUsuario.ObtenerPorId(id);
        if (usuario == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.AvatarActual = usuario.Avatar;
            return View(model);
        }

        if (!string.Equals(usuario.Email, model.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            var existente = _repoUsuario.ObtenerPorEmail(model.Email);
            if (existente != null && existente.Id != id)
            {
                ModelState.AddModelError(nameof(model.Email), "El correo electrónico ya se encuentra registrado por otro usuario.");
                model.AvatarActual = usuario.Avatar;
                return View(model);
            }
        }

        usuario.Nombre = model.Nombre.Trim();
        usuario.Apellido = model.Apellido.Trim();
        usuario.Email = model.Email.Trim();
        usuario.Rol = model.Rol;

        if (model.QuitarAvatar)
        {
            if (!string.IsNullOrEmpty(usuario.Avatar))
            {
                _almacenadorArchivos.BorrarArchivo(usuario.Avatar);
                usuario.Avatar = null;
            }
        }
        else if (model.AvatarFile != null && model.AvatarFile.Length > 0)
        {
            try
            {
                if (!string.IsNullOrEmpty(usuario.Avatar))
                {
                    _almacenadorArchivos.BorrarArchivo(usuario.Avatar);
                }
                usuario.Avatar = await _almacenadorArchivos.GuardarArchivoAsync(model.AvatarFile, "usuarios/avatares");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(model.AvatarFile), ex.Message);
                model.AvatarActual = usuario.Avatar;
                return View(model);
            }
        }

        _repoUsuario.Modificacion(usuario);
        _logger.LogInformation("Usuario ID {Id} ({Email}) actualizado por administrador.", usuario.Id, usuario.Email);

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId == id.ToString())
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Email),
                new(ClaimTypes.GivenName, usuario.NombreCompleto),
                new(ClaimTypes.Role, usuario.Rol.ToString()),
                new("Avatar", usuario.Avatar ?? string.Empty)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

        TempData["Mensaje"] = "Usuario modificado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId == id.ToString())
        {
            TempData["Error"] = "No puedes eliminar tu propia cuenta de usuario.";
            return RedirectToAction(nameof(Index));
        }

        var usuario = _repoUsuario.ObtenerPorId(id);
        if (usuario == null)
        {
            return NotFound();
        }

        var model = new UsuarioDetalleViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Rol = usuario.Rol,
            Avatar = usuario.Avatar,
            Activo = usuario.Activo
        };

        return View(model);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    [ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public IActionResult EliminarConfirmado(int id)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId == id.ToString())
        {
            TempData["Error"] = "No puedes eliminar tu propia cuenta de usuario.";
            return RedirectToAction(nameof(Index));
        }

        var usuario = _repoUsuario.ObtenerPorId(id);
        if (usuario == null)
        {
            return NotFound();
        }

        _repoUsuario.Baja(id);
        _logger.LogInformation("Usuario ID {Id} dado de baja correctamente.", id);

        TempData["Mensaje"] = "Usuario eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Perfil Propio (Cualquier Usuario Autenticado)

    [HttpGet]
    public IActionResult Perfil()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(idClaim, out var id))
        {
            return Challenge();
        }

        var usuario = _repoUsuario.ObtenerPorId(id);
        if (usuario == null)
        {
            return NotFound();
        }

        var model = new PerfilUsuarioViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Rol = usuario.Rol,
            AvatarActual = usuario.Avatar
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(PerfilUsuarioViewModel model)
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(idClaim, out var currentUserId) || currentUserId != model.Id)
        {
            return Forbid();
        }

        var usuario = _repoUsuario.ObtenerPorId(currentUserId);
        if (usuario == null)
        {
            return NotFound();
        }

        model.Rol = usuario.Rol;
        model.AvatarActual = usuario.Avatar;

        var passwordHasher = new PasswordHasher<Usuario>();
        bool deseaCambiarClave = !string.IsNullOrWhiteSpace(model.NuevaClave);

        if (deseaCambiarClave)
        {
            if (string.IsNullOrWhiteSpace(model.ClaveActual))
            {
                ModelState.AddModelError(nameof(model.ClaveActual), "Debe ingresar su contraseña actual para establecer una nueva contraseña.");
            }
            else
            {
                var resultadoVerificacion = passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, model.ClaveActual);
                if (resultadoVerificacion == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError(nameof(model.ClaveActual), "La contraseña actual es incorrecta.");
                }
            }
        }
        else if (!string.IsNullOrWhiteSpace(model.ClaveActual))
        {
            ModelState.AddModelError(nameof(model.NuevaClave), "Debe ingresar una nueva contraseña.");
        }

        if (!string.Equals(usuario.Email, model.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            var existente = _repoUsuario.ObtenerPorEmail(model.Email);
            if (existente != null && existente.Id != currentUserId)
            {
                ModelState.AddModelError(nameof(model.Email), "El correo electrónico ya se encuentra registrado por otro usuario.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        usuario.Nombre = model.Nombre.Trim();
        usuario.Apellido = model.Apellido.Trim();
        usuario.Email = model.Email.Trim();

        if (model.QuitarAvatar)
        {
            if (!string.IsNullOrEmpty(usuario.Avatar))
            {
                _almacenadorArchivos.BorrarArchivo(usuario.Avatar);
                usuario.Avatar = null;
            }
        }
        else if (model.AvatarFile != null && model.AvatarFile.Length > 0)
        {
            try
            {
                if (!string.IsNullOrEmpty(usuario.Avatar))
                {
                    _almacenadorArchivos.BorrarArchivo(usuario.Avatar);
                }
                usuario.Avatar = await _almacenadorArchivos.GuardarArchivoAsync(model.AvatarFile, "usuarios/avatares");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(model.AvatarFile), ex.Message);
                return View(model);
            }
        }

        _repoUsuario.Modificacion(usuario);

        if (deseaCambiarClave)
        {
            var nuevoHash = passwordHasher.HashPassword(usuario, model.NuevaClave!);
            _repoUsuario.ActualizarPassword(usuario.Id, nuevoHash);
            usuario.PasswordHash = nuevoHash;
            _logger.LogInformation("Usuario ID {Id} actualizó su contraseña.", usuario.Id);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Email),
            new(ClaimTypes.GivenName, usuario.NombreCompleto),
            new(ClaimTypes.Role, usuario.Rol.ToString()),
            new("Avatar", usuario.Avatar ?? string.Empty)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        _logger.LogInformation("Perfil de usuario ID {Id} actualizado correctamente.", usuario.Id);

        TempData["Mensaje"] = "Perfil actualizado correctamente.";
        return RedirectToAction(nameof(Perfil));
    }

    #endregion
}
