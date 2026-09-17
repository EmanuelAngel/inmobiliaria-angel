using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria_lab2.Models;
using inmobiliaria_lab2.Repositories;

namespace inmobiliaria_lab2.Controllers;

[Authorize]
public class PagosController(
    IRepositorioPago repositorioPago,
    IRepositorioReserva repositorioReserva,
    IRepositorioUsuario repositorioUsuario
) : Controller
{
    private readonly IRepositorioPago _repositorioPago = repositorioPago;
    private readonly IRepositorioReserva _repositorioReserva = repositorioReserva;
    private readonly IRepositorioUsuario _repositorioUsuario = repositorioUsuario;

    // GET: Pagos
    public IActionResult Index(int? reservaId = null, string? estado = null, int pagina = 1, int tamDePagina = 10)
    {
        pagina = Math.Max(1, pagina);
        tamDePagina = Math.Clamp(tamDePagina, 1, 50);

        var pagos = _repositorioPago.ObtenerLista(reservaId, estado, pagina, tamDePagina);
        var total = _repositorioPago.ObtenerCantidad(reservaId, estado);

        var valoresRuta = new Dictionary<string, string>();
        if (reservaId.HasValue)
            valoresRuta["reservaId"] = reservaId.Value.ToString();
        if (!string.IsNullOrEmpty(estado))
            valoresRuta["estado"] = estado;

        var viewModel = new PagosIndexViewModel
        {
            Pagos = pagos,
            ReservaIdFiltro = reservaId,
            EstadoFiltro = estado,
            Paginacion = new PaginacionViewModel
            {
                PaginaActual = pagina,
                TamDePagina = tamDePagina,
                TotalPaginas = (int)Math.Ceiling((double)total / tamDePagina),
                TotalRegistros = total,
                ValoresRuta = valoresRuta
            }
        };

        if (reservaId.HasValue)
        {
            var reserva = _repositorioReserva.ObtenerPorId(reservaId.Value);
            if (reserva != null)
            {
                viewModel.ReservaContexto = reserva;
                viewModel.TotalReserva = reserva.MontoTotal;
                viewModel.TotalPagado = _repositorioPago.ObtenerTotalPagado(reservaId.Value);
            }
        }

        if (TempData.ContainsKey("Mensaje"))
            ViewBag.Mensaje = TempData["Mensaje"];
        if (TempData.ContainsKey("Error"))
            ViewBag.Error = TempData["Error"];

        return View(viewModel);
    }

    // GET: Pagos/Details/5
    public IActionResult Details(int id)
    {
        var pago = _repositorioPago.ObtenerPorId(id);
        if (pago == null)
            return NotFound();

        if (User.IsInRole("Administrador"))
        {
            if (pago.UsuarioCreacionId > 0)
            {
                ViewBag.UsuarioCreacion = _repositorioUsuario.ObtenerPorId(pago.UsuarioCreacionId, soloActivos: false);
            }
            if (pago.UsuarioAnulacionId.HasValue)
            {
                ViewBag.UsuarioAnulacion = _repositorioUsuario.ObtenerPorId(pago.UsuarioAnulacionId.Value, soloActivos: false);
            }
        }

        return View(pago);
    }

    // GET: Pagos/Create
    public IActionResult Create(int? reservaId = null)
    {
        if (reservaId.HasValue)
        {
            var reserva = _repositorioReserva.ObtenerPorId(reservaId.Value);
            if (reserva == null)
                return NotFound();

            var totalPagado = _repositorioPago.ObtenerTotalPagado(reservaId.Value);
            var saldoPendiente = Math.Max(0, reserva.MontoTotal - totalPagado);

            if (saldoPendiente <= 0)
            {
                TempData["Error"] = "La reserva ya se encuentra totalmente saldada.";
                return RedirectToAction(nameof(Index), new { reservaId = reservaId.Value });
            }

            ViewBag.Reserva = reserva;
            ViewBag.SaldoPendiente = saldoPendiente;

            var modelo = new Pago
            {
                ReservaId = reservaId.Value,
                Reserva = reserva,
                Fecha = DateOnly.FromDateTime(DateTime.Today),
                Importe = saldoPendiente,
                Concepto = totalPagado == 0 ? "Seña de reserva" : "Pago de alquiler"
            };

            return View(modelo);
        }

        // Si no viene reservaId, enviamos lista de reservas activas para selección
        ViewBag.ReservasActivas = _repositorioReserva.ObtenerLista(estado: "Activa", nroDePagina: 1, tamDePagina: 100);
        return View(new Pago { Fecha = DateOnly.FromDateTime(DateTime.Today) });
    }

    // POST: Pagos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Pago pago)
    {
        var reserva = _repositorioReserva.ObtenerPorId(pago.ReservaId);
        if (reserva == null)
        {
            ModelState.AddModelError(nameof(pago.ReservaId), "La reserva especificada no existe.");
            return ReenviarCreateConContexto(pago, null);
        }

        var totalPagado = _repositorioPago.ObtenerTotalPagado(pago.ReservaId);
        var saldoPendiente = Math.Max(0, reserva.MontoTotal - totalPagado);

        if (pago.Importe <= 0)
        {
            ModelState.AddModelError(nameof(pago.Importe), "El importe debe ser mayor a 0.");
        }
        else if (pago.Importe > saldoPendiente)
        {
            ModelState.AddModelError(nameof(pago.Importe),
                $"El importe (${pago.Importe:N2}) supera el saldo pendiente de la reserva (${saldoPendiente:N2}).");
        }

        if (!ModelState.IsValid)
        {
            return ReenviarCreateConContexto(pago, reserva, saldoPendiente);
        }

        var currentUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : 0;
        pago.UsuarioCreacionId = currentUserId;
        pago.Estado = EstadoPago.Activo;

        var nuevoId = _repositorioPago.Alta(pago);
        TempData["Mensaje"] = "Pago registrado exitosamente.";
        TempData["Id"] = nuevoId;

        return RedirectToAction(nameof(Index), new { reservaId = pago.ReservaId });
    }

    // GET: Pagos/Edit/5
    public IActionResult Edit(int id)
    {
        var pago = _repositorioPago.ObtenerPorId(id);
        if (pago == null)
            return NotFound();

        if (pago.Estado == EstadoPago.Anulado)
        {
            TempData["Error"] = "Un pago anulado no puede ser modificado.";
            return RedirectToAction(nameof(Index), new { reservaId = pago.ReservaId });
        }

        var viewModel = new PagoEditViewModel
        {
            Id = pago.Id,
            ReservaId = pago.ReservaId,
            InquilinoNombre = pago.Reserva?.Inquilino?.NombreCompleto,
            InmuebleDireccion = pago.Reserva?.Inmueble?.Direccion,
            Fecha = pago.Fecha,
            Importe = pago.Importe,
            Estado = pago.Estado,
            Concepto = pago.Concepto
        };

        return View(viewModel);
    }

    // POST: Pagos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, PagoEditViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            var pagoExistente = _repositorioPago.ObtenerPorId(id);
            if (pagoExistente != null)
            {
                model.InquilinoNombre = pagoExistente.Reserva?.Inquilino?.NombreCompleto;
                model.InmuebleDireccion = pagoExistente.Reserva?.Inmueble?.Direccion;
                model.Fecha = pagoExistente.Fecha;
                model.Importe = pagoExistente.Importe;
                model.Estado = pagoExistente.Estado;
            }
            return View(model);
        }

        _repositorioPago.ModificarConcepto(id, model.Concepto);
        TempData["Mensaje"] = "Concepto de pago actualizado exitosamente.";
        return RedirectToAction(nameof(Index), new { reservaId = model.ReservaId });
    }

    // GET: Pagos/Delete/5 (Anular)
    [Authorize(Roles = "Administrador")]
    public IActionResult Delete(int id)
    {
        var pago = _repositorioPago.ObtenerPorId(id);
        if (pago == null)
            return NotFound();

        if (pago.Estado == EstadoPago.Anulado)
        {
            TempData["Error"] = "El pago ya se encuentra anulado.";
            return RedirectToAction(nameof(Index), new { reservaId = pago.ReservaId });
        }

        return View(pago);
    }

    // POST: Pagos/Delete/5 (Confirmación de Anulación)
    [Authorize(Roles = "Administrador")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var pago = _repositorioPago.ObtenerPorId(id);
        if (pago == null)
            return NotFound();

        var currentUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : 0;
        _repositorioPago.Anular(id, currentUserId);

        TempData["Mensaje"] = "Pago anulado exitosamente.";
        return RedirectToAction(nameof(Index), new { reservaId = pago.ReservaId });
    }

    private IActionResult ReenviarCreateConContexto(Pago pago, Reserva? reserva, decimal? saldoPendiente = null)
    {
        if (reserva != null)
        {
            ViewBag.Reserva = reserva;
            ViewBag.SaldoPendiente = saldoPendiente ?? Math.Max(0, reserva.MontoTotal - _repositorioPago.ObtenerTotalPagado(reserva.Id));
            pago.Reserva = reserva;
        }
        else
        {
            ViewBag.ReservasActivas = _repositorioReserva.ObtenerLista(estado: "Activa", nroDePagina: 1, tamDePagina: 100);
        }

        return View(pago);
    }
}
