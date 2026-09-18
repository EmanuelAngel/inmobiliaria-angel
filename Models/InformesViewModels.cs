using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_lab2.Models;

/// <summary>
/// ViewModel para el filtro y reporte de disponibilidad de inmuebles por rango de fechas.
/// </summary>
public class DisponibilidadFiltroViewModel
{
    [Display(Name = "Fecha Desde")]
    [DataType(DataType.Date)]
    public DateOnly? FechaDesde { get; set; }

    [Display(Name = "Fecha Hasta")]
    [DataType(DataType.Date)]
    public DateOnly? FechaHasta { get; set; }

    public IList<Inmueble> Inmuebles { get; set; } = [];

    public bool BusquedaRealizada { get; set; }
}

/// <summary>
/// ViewModel para el monitoreo de reservas vigentes y próximas a finalizar.
/// </summary>
public class MonitoreoReservasViewModel
{
    public string TabActiva { get; set; } = "vigentes";

    public int DiasVencimiento { get; set; } = 30;

    public IList<Reserva> ReservasVigentes { get; set; } = [];

    public IList<Reserva> ReservasPorVencer { get; set; } = [];

    // Aliases para compatibilidad con la especificación y diseño
    public string Tab { get => TabActiva; set => TabActiva = value; }
    public int Dias { get => DiasVencimiento; set => DiasVencimiento = value; }
    public IList<Reserva> Vigentes { get => ReservasVigentes; set => ReservasVigentes = value; }
    public IList<Reserva> PorVencer { get => ReservasPorVencer; set => ReservasPorVencer = value; }
}
