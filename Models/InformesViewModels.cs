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

/// <summary>
/// Elemento del ranking de inmuebles más reservados en un período dado.
/// </summary>
public class InmuebleRankingItem
{
    public Inmueble Inmueble { get; set; } = null!;
    public int TotalReservas { get; set; }
}

/// <summary>
/// Elemento de inmueble disponible sin reservas recientes o nunca reservado.
/// </summary>
public class InmuebleSinReservaItem
{
    public Inmueble Inmueble { get; set; } = null!;
    public DateOnly? UltimaReserva { get; set; }
    public int? DiasSinReserva { get; set; }
}

/// <summary>
/// ViewModel para el informe de rendimiento de inmuebles (más reservados y sin actividad reciente).
/// </summary>
public class RendimientoInmueblesViewModel
{
    public string TabActiva { get; set; } = "mas-reservados";

    public int DiasInactividad { get; set; } = 30;

    public IList<InmuebleRankingItem> MasReservados { get; set; } = [];

    public IList<InmuebleSinReservaItem> SinReservas { get; set; } = [];

    // Aliases para compatibilidad con la especificación y diseño
    public string Tab { get => TabActiva; set => TabActiva = value; }
    public int DiasSinReservas { get => DiasInactividad; set => DiasInactividad = value; }
    public IList<InmuebleRankingItem> TopReservados { get => MasReservados; set => MasReservados = value; }
}

