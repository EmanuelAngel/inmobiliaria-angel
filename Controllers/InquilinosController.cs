using Microsoft.AspNetCore.Mvc;
using inmobiliaria_lab2.Models;
using inmobiliaria_lab2.Repositories;

namespace inmobiliaria_lab2.Controllers;

public class InquilinosController(IRepositorioInquilino repositorio) : Controller
{
    private readonly IRepositorioInquilino _repositorio = repositorio;

    // GET: Inquilinos
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

    // GET: Inquilinos/Details/5
    public IActionResult Details(int id)
    {
        var inquilino = _repositorio.ObtenerPorId(id);

        if (inquilino == null)
        {
            return NotFound();
        }

        return View(inquilino);
    }

    // GET: Inquilinos/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Inquilinos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Inquilino inquilino)
    {
        if (!ModelState.IsValid)
        {
            return View(inquilino);
        }

        var existenteDni = _repositorio.ObtenerPorDni(inquilino.Dni);
        if (existenteDni != null)
        {
            ModelState.AddModelError(nameof(inquilino.Dni), "Ya existe un inquilino registrado con este DNI.");
            return View(inquilino);
        }

        if (!string.IsNullOrWhiteSpace(inquilino.Email))
        {
            var existenteEmail = _repositorio.ObtenerPorEmail(inquilino.Email);
            if (existenteEmail != null)
            {
                ModelState.AddModelError(nameof(inquilino.Email), "Ya existe un inquilino registrado con este correo electrónico.");
                return View(inquilino);
            }
        }

        var idGenerado = _repositorio.Alta(inquilino);
        TempData["Mensaje"] = "Inquilino registrado exitosamente.";
        TempData["Id"] = idGenerado;
        return RedirectToAction(nameof(Index));
    }

    // GET: Inquilinos/Edit/5
    public IActionResult Edit(int id)
    {
        var inquilino = _repositorio.ObtenerPorId(id);
        if (inquilino == null)
        {
            return NotFound();
        }

        return View(inquilino);
    }

    // POST: Inquilinos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Inquilino inquilino)
    {
        if (id != inquilino.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(inquilino);
        }

        var existenteDni = _repositorio.ObtenerPorDni(inquilino.Dni);
        if (existenteDni != null && existenteDni.Id != inquilino.Id)
        {
            ModelState.AddModelError(nameof(inquilino.Dni), "Ya existe otro inquilino registrado con este DNI.");
            return View(inquilino);
        }

        if (!string.IsNullOrWhiteSpace(inquilino.Email))
        {
            var existenteEmail = _repositorio.ObtenerPorEmail(inquilino.Email);
            if (existenteEmail != null && existenteEmail.Id != inquilino.Id)
            {
                ModelState.AddModelError(nameof(inquilino.Email), "Ya existe otro inquilino registrado con este correo electrónico.");
                return View(inquilino);
            }
        }

        _repositorio.Modificacion(inquilino);
        TempData["Mensaje"] = "Inquilino actualizado exitosamente.";
        TempData["Id"] = inquilino.Id;
        return RedirectToAction(nameof(Index));
    }

    // GET: Inquilinos/Delete/5
    public IActionResult Delete(int id)
    {
        var inquilino = _repositorio.ObtenerPorId(id);
        if (inquilino == null)
        {
            return NotFound();
        }

        return View(inquilino);
    }

    // POST: Inquilinos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _repositorio.Baja(id);
            TempData["Mensaje"] = "Inquilino dado de baja exitosamente.";
        }
        catch (Exception)
        {
            TempData["Error"] = "No se puede eliminar el inquilino porque tiene reservas o registros vinculados.";
        }

        return RedirectToAction(nameof(Index));
    }
}
