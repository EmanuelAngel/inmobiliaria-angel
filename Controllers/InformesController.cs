using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria_lab2.Models;
using inmobiliaria_lab2.Repositories;

namespace inmobiliaria_lab2.Controllers;

[Authorize]
public class InformesController(
    IRepositorioInmueble repositorioInmueble,
    IRepositorioReserva repositorioReserva
) : Controller
{
    private readonly IRepositorioInmueble _repositorioInmueble = repositorioInmueble;
    private readonly IRepositorioReserva _repositorioReserva = repositorioReserva;

    // GET: Informes
    public IActionResult Index()
    {
        return View();
    }

    // GET: Informes/Disponibilidad
    public IActionResult Disponibilidad(DateOnly? desde, DateOnly? hasta)
    {
        var model = new DisponibilidadFiltroViewModel
        {
            FechaDesde = desde,
            FechaHasta = hasta
        };

        if (!desde.HasValue && !hasta.HasValue)
        {
            return View(model);
        }

        if (!desde.HasValue || !hasta.HasValue)
        {
            ModelState.AddModelError(string.Empty, "Debe especificar ambas fechas (desde y hasta) para realizar la búsqueda.");
            model.BusquedaRealizada = true;
            return View(model);
        }

        if (desde.Value > hasta.Value)
        {
            ModelState.AddModelError(string.Empty, "La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'.");
            model.BusquedaRealizada = true;
            return View(model);
        }

        model.Inmuebles = _repositorioInmueble.ObtenerDisponiblesEntreFechas(desde.Value, hasta.Value);
        model.BusquedaRealizada = true;

        return View(model);
    }

    // GET: Informes/MonitoreoReservas (Próximo - Fase 3)
    public IActionResult MonitoreoReservas()
    {
        TempData["Mensaje"] = "El módulo de Monitoreo de Reservas estará disponible próximamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Informes/RendimientoInmuebles (Próximo - Fase 4)
    public IActionResult RendimientoInmuebles()
    {
        TempData["Mensaje"] = "El módulo de Rendimiento de Inmuebles estará disponible próximamente.";
        return RedirectToAction(nameof(Index));
    }
}
