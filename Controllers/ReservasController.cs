using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria_lab2.Models;
using inmobiliaria_lab2.Repositories;

namespace inmobiliaria_lab2.Controllers;

[Authorize]
public class ReservasController(
    IRepositorioReserva repositorioReserva,
    IRepositorioInquilino repositorioInquilino,
    IRepositorioInmueble repositorioInmueble,
    IRepositorioUsuario repositorioUsuario
) : Controller
{
    private readonly IRepositorioReserva _repositorioReserva = repositorioReserva;
    private readonly IRepositorioInquilino _repositorioInquilino = repositorioInquilino;
    private readonly IRepositorioInmueble _repositorioInmueble = repositorioInmueble;
    private readonly IRepositorioUsuario _repositorioUsuario = repositorioUsuario;

    // GET: Reservas
    public IActionResult Index(int pagina = 1, int tamDePagina = 10, string? estado = null)
    {
        pagina = Math.Max(1, pagina);
        tamDePagina = Math.Clamp(tamDePagina, 1, 50);

        var lista = _repositorioReserva.ObtenerLista(estado, pagina, tamDePagina);
        var total = _repositorioReserva.ObtenerCantidad(estado);

        var valoresRuta = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(estado))
            valoresRuta["estado"] = estado;

        ViewBag.Paginacion = new PaginacionViewModel
        {
            PaginaActual = pagina,
            TamDePagina = tamDePagina,
            TotalPaginas = (int)Math.Ceiling((double)total / tamDePagina),
            TotalRegistros = total,
            ValoresRuta = valoresRuta
        };
        ViewBag.EstadoFiltro = estado;
        ViewBag.Id = TempData["Id"];

        if (TempData.ContainsKey("Mensaje"))
            ViewBag.Mensaje = TempData["Mensaje"];
        if (TempData.ContainsKey("Error"))
            ViewBag.Error = TempData["Error"];

        return View(lista);
    }

    // GET: Reservas/Details/5
    public IActionResult Details(int id)
    {
        var reserva = _repositorioReserva.ObtenerPorId(id);
        if (reserva == null)
            return NotFound();

        if (User.IsInRole("Administrador"))
        {
            if (reserva.UsuarioCreacionId.HasValue)
            {
                ViewBag.UsuarioCreacion = _repositorioUsuario.ObtenerPorId(reserva.UsuarioCreacionId.Value, soloActivos: false);
            }
            if (reserva.UsuarioTerminacionId.HasValue)
            {
                ViewBag.UsuarioTerminacion = _repositorioUsuario.ObtenerPorId(reserva.UsuarioTerminacionId.Value, soloActivos: false);
            }
        }

        return View(reserva);
    }

    // GET: Reservas/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Reservas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Reserva reserva)
    {
        reserva.UsuarioCreacionId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : null;

        if (!ModelState.IsValid)
        {
            RepoblarNavegacion(reserva);
            return View(reserva);
        }

        if (reserva.FechaHasta < reserva.FechaDesde)
        {
            ModelState.AddModelError(nameof(reserva.FechaHasta),
                "La fecha de finalización no puede ser anterior a la fecha de inicio.");
            RepoblarNavegacion(reserva);
            return View(reserva);
        }

        if (!_repositorioReserva.VerificarDisponibilidad(reserva.InmuebleId, reserva.FechaDesde, reserva.FechaHasta))
        {
            ModelState.AddModelError(nameof(reserva.InmuebleId),
                "El inmueble ya tiene una reserva activa en ese período. Elija otras fechas.");
            RepoblarNavegacion(reserva);
            return View(reserva);
        }

        var idGenerado = _repositorioReserva.Alta(reserva);
        TempData["Mensaje"] = "Reserva registrada exitosamente.";
        TempData["Id"] = idGenerado;
        return RedirectToAction(nameof(Index));
    }

    // GET: Reservas/Edit/5
    public IActionResult Edit(int id)
    {
        var reserva = _repositorioReserva.ObtenerPorId(id);
        if (reserva == null)
            return NotFound();

        return View(reserva);
    }

    // POST: Reservas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Reserva reserva)
    {
        if (id != reserva.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            RepoblarNavegacion(reserva);
            return View(reserva);
        }

        if (reserva.FechaHasta < reserva.FechaDesde)
        {
            ModelState.AddModelError(nameof(reserva.FechaHasta),
                "La fecha de finalización no puede ser anterior a la fecha de inicio.");
            RepoblarNavegacion(reserva);
            return View(reserva);
        }

        // excluirReservaId = reserva.Id para evitar falsa superposición consigo misma
        if (!_repositorioReserva.VerificarDisponibilidad(reserva.InmuebleId, reserva.FechaDesde, reserva.FechaHasta, reserva.Id))
        {
            ModelState.AddModelError(nameof(reserva.InmuebleId),
                "El inmueble ya tiene una reserva activa en ese período. Elija otras fechas.");
            RepoblarNavegacion(reserva);
            return View(reserva);
        }

        _repositorioReserva.Modificacion(reserva);
        TempData["Mensaje"] = "Reserva actualizada exitosamente.";
        TempData["Id"] = reserva.Id;
        return RedirectToAction(nameof(Index));
    }

    // GET: Reservas/Delete/5
    [Authorize(Roles = "Administrador")]
    public IActionResult Delete(int id)
    {
        var reserva = _repositorioReserva.ObtenerPorId(id);
        if (reserva == null)
            return NotFound();

        return View(reserva);
    }

    // POST: Reservas/Delete/5
    [Authorize(Roles = "Administrador")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var usuarioId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : (int?)null;
        _repositorioReserva.Baja(id, usuarioId);
        TempData["Mensaje"] = "Reserva cancelada exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Reservas/FinalizarAnticipada/5
    public IActionResult FinalizarAnticipada(int id)
    {
        var reserva = _repositorioReserva.ObtenerPorId(id);
        if (reserva == null)
            return NotFound();

        if (reserva.Estado != EstadoReserva.Activa)
        {
            TempData["Error"] = "Solo se pueden finalizar anticipadamente reservas activas.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var fechaFinAnticipado = hoy;
        if (fechaFinAnticipado < reserva.FechaDesde)
            fechaFinAnticipado = reserva.FechaDesde;
        else if (fechaFinAnticipado >= reserva.FechaHasta)
            fechaFinAnticipado = reserva.FechaHasta > reserva.FechaDesde ? reserva.FechaHasta.AddDays(-1) : reserva.FechaDesde;

        var model = new ReservaFinalizarViewModel
        {
            ReservaId = reserva.Id,
            Reserva = reserva,
            FechaHastaOriginal = reserva.FechaHasta,
            FechaFinAnticipado = fechaFinAnticipado,
            ConceptoPago = "Multa por cancelación anticipada"
        };

        CalcularLiquidacion(reserva, model);

        return View(model);
    }

    // POST: Reservas/FinalizarAnticipada/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult FinalizarAnticipada(int id, ReservaFinalizarViewModel model)
    {
        if (id != model.ReservaId)
            return BadRequest();

        var reserva = _repositorioReserva.ObtenerPorId(id);
        if (reserva == null)
            return NotFound();

        if (reserva.Estado != EstadoReserva.Activa)
        {
            TempData["Error"] = "Solo se pueden finalizar anticipadamente reservas activas.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (model.FechaFinAnticipado < reserva.FechaDesde)
        {
            ModelState.AddModelError(nameof(model.FechaFinAnticipado),
                "La fecha efectiva de terminación no puede ser anterior a la fecha de inicio de la reserva.");
        }

        if (model.FechaFinAnticipado >= reserva.FechaHasta)
        {
            ModelState.AddModelError(nameof(model.FechaFinAnticipado),
                "La fecha efectiva de terminación debe ser anterior a la fecha de finalización original.");
        }

        CalcularLiquidacion(reserva, model);

        if (!ModelState.IsValid)
        {
            model.Reserva = reserva;
            return View(model);
        }

        var usuarioId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : 0;
        if (usuarioId <= 0)
        {
            ModelState.AddModelError(string.Empty, "No se pudo identificar al usuario autenticado para registrar la operación.");
            model.Reserva = reserva;
            return View(model);
        }

        var pagoMulta = new Pago
        {
            ReservaId = id,
            Concepto = string.IsNullOrWhiteSpace(model.ConceptoPago)
                ? "Multa por cancelación anticipada"
                : model.ConceptoPago.Trim(),
            Fecha = DateOnly.FromDateTime(DateTime.Today),
            Importe = model.MontoMulta,
            Estado = EstadoPago.Activo,
            UsuarioCreacionId = usuarioId
        };

        try
        {
            _repositorioReserva.FinalizarConMulta(id, model.FechaFinAnticipado, usuarioId, pagoMulta);
            TempData["Mensaje"] = $"Reserva #{id} finalizada anticipadamente. Se registró el cobro de la multa por {model.MontoMulta:C}.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Error al procesar la finalización anticipada: " + ex.Message);
            model.Reserva = reserva;
            return View(model);
        }
    }

    // GET: Reservas/Extender/5
    public IActionResult Extender(int id)
    {
        var reserva = _repositorioReserva.ObtenerPorId(id);
        if (reserva == null)
            return NotFound();

        if (reserva.Estado != EstadoReserva.Activa)
        {
            TempData["Error"] = "Solo se pueden extender reservas activas.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var model = new ReservaExtenderViewModel
        {
            ReservaOriginalId = reserva.Id,
            ReservaOriginal = reserva,
            InmuebleId = reserva.InmuebleId,
            InquilinoId = reserva.InquilinoId,
            FechaDesde = reserva.FechaHasta,
            FechaHasta = reserva.FechaHasta.AddDays(1),
            MontoPorDia = reserva.MontoPorDia
        };

        return View(model);
    }

    // POST: Reservas/Extender/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Extender(int id, ReservaExtenderViewModel model)
    {
        if (id != model.ReservaOriginalId && id != model.ReservaId)
            return BadRequest();

        var original = _repositorioReserva.ObtenerPorId(id);
        if (original == null)
            return NotFound();

        if (original.Estado != EstadoReserva.Activa)
        {
            TempData["Error"] = "Solo se pueden extender reservas activas.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // Forzar inviolabilidad de datos base tomados de la reserva original
        model.FechaDesde = original.FechaHasta;
        model.InmuebleId = original.InmuebleId;
        model.InquilinoId = original.InquilinoId;
        model.ReservaOriginal = original;

        if (model.FechaHasta <= model.FechaDesde)
        {
            ModelState.AddModelError(nameof(model.FechaHasta),
                "La fecha de finalización debe ser posterior a la fecha de inicio.");
        }

        if (model.MontoPorDia <= 0m)
        {
            ModelState.AddModelError(nameof(model.MontoPorDia),
                "El monto por día debe ser mayor a 0.");
        }

        if (ModelState.IsValid)
        {
            if (!_repositorioReserva.VerificarDisponibilidad(original.InmuebleId, model.FechaDesde, model.FechaHasta))
            {
                ModelState.AddModelError(nameof(model.FechaHasta),
                    "El inmueble ya tiene una reserva activa en ese período. Elija otras fechas.");
            }
        }

        if (!ModelState.IsValid)
        {
            model.ReservaOriginal = original;
            return View(model);
        }

        // TODO: Habría que refactorizar en algún momento el cómo se obtienen los valores de los claims
        var usuarioId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : (int?)null;

        var nuevaReserva = new Reserva
        {
            InquilinoId = original.InquilinoId,
            InmuebleId = original.InmuebleId,
            FechaDesde = model.FechaDesde,
            FechaHasta = model.FechaHasta,
            MontoPorDia = model.MontoPorDia,
            Estado = EstadoReserva.Activa,
            UsuarioCreacionId = usuarioId
        };

        try
        {
            var nuevoId = _repositorioReserva.Alta(nuevaReserva);
            TempData["Mensaje"] = $"Reserva extendida exitosamente con el código #{nuevoId}.";
            return RedirectToAction(nameof(Details), new { id = nuevoId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Error al registrar la extensión de la reserva: " + ex.Message);
            model.ReservaOriginal = original;
            return View(model);
        }
    }

    private static void CalcularLiquidacion(Reserva reserva, ReservaFinalizarViewModel model)
    {
        model.ReservaId = reserva.Id;
        model.Reserva = reserva;
        model.FechaHastaOriginal = reserva.FechaHasta;

        var diasTotales = reserva.FechaHasta.DayNumber - reserva.FechaDesde.DayNumber;
        if (diasTotales <= 0)
            diasTotales = 1;

        var diasTranscurridos = Math.Clamp(model.FechaFinAnticipado.DayNumber - reserva.FechaDesde.DayNumber, 0, diasTotales);
        var diasRestantes = Math.Max(0, reserva.FechaHasta.DayNumber - model.FechaFinAnticipado.DayNumber);

        var porcentajeTranscurrido = Math.Round((decimal)diasTranscurridos / diasTotales * 100m, 2);
        var porcentajeMulta = porcentajeTranscurrido < 50m ? 50m : 25m;
        var baseEconomica = diasRestantes * reserva.MontoPorDia;
        var montoMulta = Math.Round(baseEconomica * (porcentajeMulta / 100m), 2);

        model.DiasTotales = diasTotales;
        model.DiasTranscurridos = diasTranscurridos;
        model.DiasRestantes = diasRestantes;
        model.PorcentajeTranscurrido = porcentajeTranscurrido;
        model.PorcentajeMulta = porcentajeMulta;
        model.BaseEconomica = baseEconomica;
        model.MontoMulta = montoMulta;
    }

    /// <summary>
    /// Repuebla las navigation properties del modelo para que los select2
    /// puedan mostrar el valor pre-seleccionado al volver a renderizar el formulario.
    /// </summary>
    private void RepoblarNavegacion(Reserva reserva)
    {
        if (reserva.InquilinoId > 0)
            reserva.Inquilino = _repositorioInquilino.ObtenerPorId(reserva.InquilinoId);
        if (reserva.InmuebleId > 0)
            reserva.Inmueble = _repositorioInmueble.ObtenerPorId(reserva.InmuebleId);
    }
}
