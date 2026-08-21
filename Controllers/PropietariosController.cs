using Microsoft.AspNetCore.Mvc;
using inmobiliaria_lab2.Models;
using inmobiliaria_lab2.Repositories;

namespace inmobiliaria_lab2.Controllers;

public class PropietariosController(IRepositorioPropietario repositorio) : Controller
{
    private readonly IRepositorioPropietario _repositorio = repositorio;

    // GET: Propietarios
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

        return View(lista);
    }

    // GET: Propietarios/Details/5
    public IActionResult Details(int id)
    {
        var propietario = _repositorio.ObtenerPorId(id);

        if (propietario == null)
        {
            // ¿Mejor 404 o pasar ID = 0 a la vista?
            return NotFound();
        }

        return View(propietario);
    }

    // GET: Propietarios/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Propietarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Propietario propietario)
    {
        if (!ModelState.IsValid)
        {
            return View(propietario);
        }

        var existenteDni = _repositorio.ObtenerPorDni(propietario.Dni);
        if (existenteDni != null)
        {
            ModelState.AddModelError(nameof(propietario.Dni), "Ya existe un propietario registrado con este DNI.");
            return View(propietario);
        }

        var existenteEmail = _repositorio.ObtenerPorEmail(propietario.Email);
        if (existenteEmail != null)
        {
            ModelState.AddModelError(nameof(propietario.Email), "Ya existe un propietario registrado con este correo electrónico.");
            return View(propietario);
        }

        var idGenerado = _repositorio.Alta(propietario);
        TempData["Mensaje"] = "Propietario registrado exitosamente.";
        TempData["Id"] = idGenerado;
        return RedirectToAction(nameof(Index));
    }

    // GET: Propietarios/Edit/5
    public IActionResult Edit(int id)
    {
        var propietario = _repositorio.ObtenerPorId(id);
        if (propietario == null)
        {
            return NotFound();
        }

        return View(propietario);
    }

    // POST: Propietarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Propietario propietario)
    {
        if (id != propietario.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(propietario);
        }

        var existenteDni = _repositorio.ObtenerPorDni(propietario.Dni);
        if (existenteDni != null && existenteDni.Id != propietario.Id)
        {
            ModelState.AddModelError(nameof(propietario.Dni), "Ya existe otro propietario registrado con este DNI.");
            return View(propietario);
        }

        var existenteEmail = _repositorio.ObtenerPorEmail(propietario.Email);
        if (existenteEmail != null && existenteEmail.Id != propietario.Id)
        {
            ModelState.AddModelError(nameof(propietario.Email), "Ya existe otro propietario registrado con este correo electrónico.");
            return View(propietario);
        }

        _repositorio.Modificacion(propietario);
        TempData["Mensaje"] = "Propietario actualizado exitosamente.";
        TempData["Id"] = propietario.Id;
        return RedirectToAction(nameof(Index));
    }

    // GET: Propietarios/Delete/5
    public IActionResult Delete(int id)
    {
        var propietario = _repositorio.ObtenerPorId(id);
        if (propietario == null)
        {
            return NotFound();
        }

        return View(propietario);
    }

    // POST: Propietarios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _repositorio.Baja(id);
            TempData["Mensaje"] = "Propietario eliminado exitosamente.";
        }
        catch (Exception)
        {
            TempData["Error"] = "No se puede eliminar el propietario porque tiene inmuebles o registros vinculados.";
        }

        return RedirectToAction(nameof(Index));
    }
}
