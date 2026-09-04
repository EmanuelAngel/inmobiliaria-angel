using Microsoft.AspNetCore.Mvc;
using inmobiliaria_lab2.Models;
using inmobiliaria_lab2.Repositories;

namespace inmobiliaria_lab2.Controllers;

public class ReservasController(
    IRepositorioReserva repositorioReserva,
    IRepositorioInquilino repositorioInquilino,
    IRepositorioInmueble repositorioInmueble
) : Controller
{
    private readonly IRepositorioReserva _repositorioReserva = repositorioReserva;
    private readonly IRepositorioInquilino _repositorioInquilino = repositorioInquilino;
    private readonly IRepositorioInmueble _repositorioInmueble = repositorioInmueble;

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
    public IActionResult Delete(int id)
    {
        var reserva = _repositorioReserva.ObtenerPorId(id);
        if (reserva == null)
            return NotFound();

        return View(reserva);
    }

    // POST: Reservas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _repositorioReserva.Baja(id);
        TempData["Mensaje"] = "Reserva cancelada exitosamente.";
        return RedirectToAction(nameof(Index));
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
