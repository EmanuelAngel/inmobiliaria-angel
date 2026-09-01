using Microsoft.AspNetCore.Mvc;
using inmobiliaria_lab2.Models;
using inmobiliaria_lab2.Repositories;

namespace inmobiliaria_lab2.Controllers;

public class InmueblesController(
    IRepositorioInmueble repositorioInmueble,
    IRepositorioPropietario repositorioPropietario,
    IRepositorioTipoInmueble repositorioTipoInmueble
) : Controller
{
    private readonly IRepositorioInmueble _repositorioInmueble = repositorioInmueble;
    private readonly IRepositorioPropietario _repositorioPropietario = repositorioPropietario;
    private readonly IRepositorioTipoInmueble _repositorioTipoInmueble = repositorioTipoInmueble;

    // GET: Inmuebles
    public IActionResult Index(int pagina = 1, int tamDePagina = 10, string? estado = null)
    {
        pagina = Math.Max(1, pagina);
        tamDePagina = Math.Clamp(tamDePagina, 1, 50);

        var lista = _repositorioInmueble.ObtenerPorDisponibilidad(estado, pagina, tamDePagina);
        var total = _repositorioInmueble.ObtenerCantidad(estado);

        ViewBag.Pagina = pagina;
        ViewBag.TamDePagina = tamDePagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamDePagina);
        ViewBag.TotalRegistros = total;
        ViewBag.EstadoFiltro = estado;
        ViewBag.Id = TempData["Id"];

        if (TempData.ContainsKey("Mensaje"))
        {
            ViewBag.Mensaje = TempData["Mensaje"];
        }
        if (TempData.ContainsKey("Error"))
        {
            ViewBag.Error = TempData["Error"];
        }

        return View(lista);
    }

    // GET: Inmuebles/Details/5
    public IActionResult Details(int id)
    {
        var inmueble = _repositorioInmueble.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound();
        }

        return View(inmueble);
    }

    // GET: Inmuebles/Create
    public IActionResult Create()
    {
        CargarTiposInmueble();
        return View();
    }

    // POST: Inmuebles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Inmueble inmueble)
    {
        if (!ModelState.IsValid)
        {
            if (inmueble.PropietarioId > 0)
            {
                inmueble.Propietario = _repositorioPropietario.ObtenerPorId(inmueble.PropietarioId);
            }
            CargarTiposInmueble();
            return View(inmueble);
        }

        inmueble.Direccion = inmueble.Direccion.Trim();
        var idGenerado = _repositorioInmueble.Alta(inmueble);

        TempData["Mensaje"] = "Inmueble registrado exitosamente.";
        TempData["Id"] = idGenerado;
        return RedirectToAction(nameof(Index));
    }

    // GET: Inmuebles/Edit/5
    public IActionResult Edit(int id)
    {
        var inmueble = _repositorioInmueble.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound();
        }

        CargarTiposInmueble();
        return View(inmueble);
    }

    // POST: Inmuebles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Inmueble inmueble)
    {
        if (id != inmueble.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            if (inmueble.PropietarioId > 0)
            {
                inmueble.Propietario = _repositorioPropietario.ObtenerPorId(inmueble.PropietarioId);
            }
            CargarTiposInmueble();
            return View(inmueble);
        }

        inmueble.Direccion = inmueble.Direccion.Trim();
        _repositorioInmueble.Modificacion(inmueble);

        TempData["Mensaje"] = "Inmueble actualizado exitosamente.";
        TempData["Id"] = inmueble.Id;
        return RedirectToAction(nameof(Index));
    }

    // GET: Inmuebles/Delete/5
    public IActionResult Delete(int id)
    {
        var inmueble = _repositorioInmueble.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound();
        }

        return View(inmueble);
    }

    // POST: Inmuebles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _repositorioInmueble.Baja(id);
        TempData["Mensaje"] = "Inmueble suspendido exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Inmuebles/CambiarEstado
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarEstado(int id, string estado)
    {
        if (estado != EstadoInmueble.Disponible.ToString() && estado != EstadoInmueble.Suspendido.ToString())
        {
            TempData["Error"] = "Estado inválido.";
            return RedirectToAction(nameof(Index));
        }

        _repositorioInmueble.CambiarEstado(id, estado);
        TempData["Mensaje"] = $"Estado del inmueble actualizado a '{estado}'.";
        return RedirectToAction(nameof(Index));
    }

    private void CargarTiposInmueble()
    {
        ViewBag.TiposInmueble = _repositorioTipoInmueble.ObtenerTodos();
    }
}
