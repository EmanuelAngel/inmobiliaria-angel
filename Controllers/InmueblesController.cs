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

        // La paginación se construye aquí y no en la vista para mantener la vista "tonta":
        // los filtros activos (estado, etc.) se incluyen en ValoresRuta para que al paginar
        // no se pierdan. El controlador los conoce; la vista no.
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

    // GET: Inmuebles/CambiarEstado/5?estado=Suspendido
    public IActionResult CambiarEstado(int id, string? estado = null)
    {
        var inmueble = _repositorioInmueble.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(estado))
        {
            estado = inmueble.Estado == EstadoInmueble.Disponible
                ? EstadoInmueble.Suspendido.ToString()
                : EstadoInmueble.Disponible.ToString();
        }

        if (estado != EstadoInmueble.Disponible.ToString() && estado != EstadoInmueble.Suspendido.ToString())
        {
            TempData["Error"] = "Estado inválido.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.NuevoEstado = estado;
        return View(inmueble);
    }

    // POST: Inmuebles/CambiarEstado
    [HttpPost, ActionName("CambiarEstado")]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarEstadoConfirmado(int id, string estado)
    {
        if (estado != EstadoInmueble.Disponible.ToString() && estado != EstadoInmueble.Suspendido.ToString())
        {
            TempData["Error"] = "Estado inválido.";
            return RedirectToAction(nameof(Index));
        }

        _repositorioInmueble.CambiarEstado(id, estado);
        TempData["Mensaje"] = estado == EstadoInmueble.Disponible.ToString()
            ? "Inmueble habilitado exitosamente."
            : "Inmueble suspendido exitosamente.";
        TempData["Id"] = id;
        return RedirectToAction(nameof(Index));
    }

    private void CargarTiposInmueble()
    {
        ViewBag.TiposInmueble = _repositorioTipoInmueble.ObtenerTodos();
    }

    // GET: Inmuebles/Buscar?q=san+martin
    [HttpGet]
    public IActionResult Buscar(string? q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Json(new { results = Array.Empty<object>() });

        var resultados = _repositorioInmueble.Buscar(q)
            .Select(i => new
            {
                id = i.Id,
                text = $"{i.Direccion} ({i.Tipo?.Descripcion})"
            });

        return Json(new { results = resultados });
    }
}
