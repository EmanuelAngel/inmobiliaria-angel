using Microsoft.AspNetCore.Mvc;
using inmobiliaria_lab2.Models;
using inmobiliaria_lab2.Repositories;
using inmobiliaria_lab2.Services;

namespace inmobiliaria_lab2.Controllers;

public class InmueblesController(
    IRepositorioInmueble repositorioInmueble,
    IRepositorioPropietario repositorioPropietario,
    IRepositorioTipoInmueble repositorioTipoInmueble,
    IRepositorioImagenInmueble repositorioImagenInmueble,
    IAlmacenadorArchivos almacenadorArchivos
) : Controller
{
    private readonly IRepositorioInmueble _repositorioInmueble = repositorioInmueble;
    private readonly IRepositorioPropietario _repositorioPropietario = repositorioPropietario;
    private readonly IRepositorioTipoInmueble _repositorioTipoInmueble = repositorioTipoInmueble;
    private readonly IRepositorioImagenInmueble _repositorioImagenInmueble = repositorioImagenInmueble;
    private readonly IAlmacenadorArchivos _almacenadorArchivos = almacenadorArchivos;

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

        ViewBag.Galeria = _repositorioImagenInmueble.ObtenerPorInmueble(id);
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
    public async Task<IActionResult> Create(Inmueble inmueble, IFormFile? archivoPortada)
    {
        string? rutaPortadaGuardada = null;

        if (archivoPortada != null)
        {
            try
            {
                rutaPortadaGuardada = await _almacenadorArchivos.GuardarArchivoAsync(archivoPortada, "inmuebles/portadas");
                inmueble.ImagenPortada = rutaPortadaGuardada;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ImagenPortada", ex.Message);
            }
        }

        if (!ModelState.IsValid)
        {
            // Rollback del archivo guardado si la validación falla
            if (!string.IsNullOrEmpty(rutaPortadaGuardada))
            {
                _almacenadorArchivos.BorrarArchivo(rutaPortadaGuardada);
                inmueble.ImagenPortada = null;
            }

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
    public async Task<IActionResult> Edit(int id, Inmueble inmueble, IFormFile? archivoPortada, bool eliminarPortada = false)
    {
        if (id != inmueble.Id)
        {
            return BadRequest();
        }

        var inmuebleExistente = _repositorioInmueble.ObtenerPorId(id);
        if (inmuebleExistente == null)
        {
            return NotFound();
        }

        string? nuevaPortadaGuardada = null;

        if (eliminarPortada)
        {
            inmueble.ImagenPortada = null;
        }
        else if (archivoPortada != null)
        {
            try
            {
                nuevaPortadaGuardada = await _almacenadorArchivos.GuardarArchivoAsync(archivoPortada, "inmuebles/portadas");
                inmueble.ImagenPortada = nuevaPortadaGuardada;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ImagenPortada", ex.Message);
            }
        }
        else
        {
            // Conserva la portada actual si no se envió archivo nuevo ni se pidió eliminar
            inmueble.ImagenPortada = inmuebleExistente.ImagenPortada;
        }

        if (!ModelState.IsValid)
        {
            // Rollback del nuevo archivo si falló la validación
            if (!string.IsNullOrEmpty(nuevaPortadaGuardada))
            {
                _almacenadorArchivos.BorrarArchivo(nuevaPortadaGuardada);
                inmueble.ImagenPortada = inmuebleExistente.ImagenPortada;
            }

            if (inmueble.PropietarioId > 0)
            {
                inmueble.Propietario = _repositorioPropietario.ObtenerPorId(inmueble.PropietarioId);
            }
            CargarTiposInmueble();
            return View(inmueble);
        }

        // Si se reemplazó o eliminó con éxito, borramos la portada anterior del disco
        if (eliminarPortada || nuevaPortadaGuardada != null)
        {
            if (!string.IsNullOrEmpty(inmuebleExistente.ImagenPortada))
            {
                _almacenadorArchivos.BorrarArchivo(inmuebleExistente.ImagenPortada);
            }
        }

        inmueble.Direccion = inmueble.Direccion.Trim();
        _repositorioInmueble.Modificacion(inmueble);

        TempData["Mensaje"] = "Inmueble actualizado exitosamente.";
        TempData["Id"] = inmueble.Id;
        return RedirectToAction(nameof(Index));
    }

    // GET: Inmuebles/Galeria/5
    public IActionResult Galeria(int id)
    {
        var inmueble = _repositorioInmueble.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound();
        }

        ViewBag.Galeria = _repositorioImagenInmueble.ObtenerPorInmueble(id);
        return View(inmueble);
    }

    // POST: Inmuebles/SubirImagenes/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubirImagenes(int id, List<IFormFile>? imagenes)
    {
        var inmueble = _repositorioInmueble.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound();
        }

        if (imagenes == null || imagenes.Count == 0)
        {
            TempData["Error"] = "No se seleccionó ningún archivo de imagen para subir.";
            return RedirectToAction(nameof(Galeria), new { id });
        }

        int subidasExitosas = 0;
        var errores = new List<string>();

        foreach (var archivo in imagenes)
        {
            try
            {
                var url = await _almacenadorArchivos.GuardarArchivoAsync(archivo, "inmuebles/galeria");
                _repositorioImagenInmueble.Alta(new ImagenInmueble
                {
                    InmuebleId = id,
                    Url = url
                });
                subidasExitosas++;
            }
            catch (Exception ex)
            {
                errores.Add($"{archivo.FileName}: {ex.Message}");
            }
        }

        if (subidasExitosas > 0)
        {
            TempData["Mensaje"] = $"Se subieron {subidasExitosas} imagen(es) correctamente a la galería.";
        }
        if (errores.Count > 0)
        {
            TempData["Error"] = $"Errores al subir: {string.Join("; ", errores)}";
        }

        return RedirectToAction(nameof(Galeria), new { id });
    }

    // POST: Inmuebles/EliminarImagen
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EliminarImagen(int id, int inmuebleId)
    {
        var imagen = _repositorioImagenInmueble.ObtenerPorId(id);
        if (imagen != null)
        {
            _almacenadorArchivos.BorrarArchivo(imagen.Url);
            _repositorioImagenInmueble.Borrar(id);
            TempData["Mensaje"] = "Imagen eliminada de la galería exitosamente.";
        }
        else
        {
            TempData["Error"] = "No se encontró la imagen a eliminar.";
        }

        return RedirectToAction(nameof(Galeria), new { id = inmuebleId });
    }

    // POST: Inmuebles/EliminarPortada/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EliminarPortada(int id)
    {
        var inmueble = _repositorioInmueble.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(inmueble.ImagenPortada))
        {
            _almacenadorArchivos.BorrarArchivo(inmueble.ImagenPortada);
            inmueble.ImagenPortada = null;
            _repositorioInmueble.Modificacion(inmueble);
            TempData["Mensaje"] = "Imagen de portada eliminada correctamente.";
        }

        return RedirectToAction(nameof(Edit), new { id });
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
