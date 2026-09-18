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
