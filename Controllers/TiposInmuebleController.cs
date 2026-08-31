using Microsoft.AspNetCore.Mvc;
using inmobiliaria_lab2.Models;
using inmobiliaria_lab2.Repositories;

namespace inmobiliaria_lab2.Controllers;

public class TiposInmuebleController(IRepositorioTipoInmueble repositorio) : Controller
{
    private readonly IRepositorioTipoInmueble _repositorio = repositorio;

    // GET: TiposInmueble
    public IActionResult Index(int pagina = 1, int tamDePagina = 10)
    {
        pagina = Math.Max(1, pagina);
        tamDePagina = Math.Clamp(tamDePagina, 1, 50);

        var lista = _repositorio.ObtenerLista(pagina, tamDePagina);
        var total = _repositorio.ObtenerCantidad();

        ViewBag.Pagina = pagina;
        ViewBag.TamDePagina = tamDePagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamDePagina);
        ViewBag.TotalRegistros = total;
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

    // GET: TiposInmueble/Details/5
    public IActionResult Details(int id)
    {
        var tipoInmueble = _repositorio.ObtenerPorId(id);

        if (tipoInmueble == null)
        {
            return NotFound();
        }

        return View(tipoInmueble);
    }

    // GET: TiposInmueble/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TiposInmueble/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TipoInmueble tipoInmueble)
    {
        if (!ModelState.IsValid)
        {
            return View(tipoInmueble);
        }

        var existente = _repositorio.ObtenerPorDescripcion(tipoInmueble.Descripcion.Trim());
        if (existente != null)
        {
            ModelState.AddModelError(nameof(tipoInmueble.Descripcion), "Ya existe un tipo de inmueble registrado con esta descripción.");
            return View(tipoInmueble);
        }

        tipoInmueble.Descripcion = tipoInmueble.Descripcion.Trim();
        var idGenerado = _repositorio.Alta(tipoInmueble);
        TempData["Mensaje"] = "Tipo de inmueble registrado exitosamente.";
        TempData["Id"] = idGenerado;
        return RedirectToAction(nameof(Index));
    }

    // GET: TiposInmueble/Edit/5
    public IActionResult Edit(int id)
    {
        var tipoInmueble = _repositorio.ObtenerPorId(id);
        if (tipoInmueble == null)
        {
            return NotFound();
        }

        return View(tipoInmueble);
    }

    // POST: TiposInmueble/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, TipoInmueble tipoInmueble)
    {
        if (id != tipoInmueble.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(tipoInmueble);
        }

        var existente = _repositorio.ObtenerPorDescripcion(tipoInmueble.Descripcion.Trim());
        if (existente != null && existente.Id != tipoInmueble.Id)
        {
            ModelState.AddModelError(nameof(tipoInmueble.Descripcion), "Ya existe otro tipo de inmueble registrado con esta descripción.");
            return View(tipoInmueble);
        }

        tipoInmueble.Descripcion = tipoInmueble.Descripcion.Trim();
        _repositorio.Modificacion(tipoInmueble);
        TempData["Mensaje"] = "Tipo de inmueble actualizado exitosamente.";
        TempData["Id"] = tipoInmueble.Id;
        return RedirectToAction(nameof(Index));
    }

    // GET: TiposInmueble/Delete/5
    public IActionResult Delete(int id)
    {
        var tipoInmueble = _repositorio.ObtenerPorId(id);
        if (tipoInmueble == null)
        {
            return NotFound();
        }

        return View(tipoInmueble);
    }

    // POST: TiposInmueble/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _repositorio.Baja(id);
        TempData["Mensaje"] = "Tipo de inmueble dado de baja exitosamente.";
        return RedirectToAction(nameof(Index));
    }
}
