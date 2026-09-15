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
}
