using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public interface IRepositorioReserva : IRepositorio<Reserva>
{
    /// <summary>
    /// Verifica si un inmueble está disponible en el rango de fechas dado.
    /// </summary>
    /// <param name="inmuebleId">ID del inmueble a verificar.</param>
    /// <param name="desde">Fecha de inicio del período a consultar.</param>
    /// <param name="hasta">Fecha de fin del período a consultar.</param>
    /// <param name="excluirReservaId">ID de reserva a excluir de la verificación (usar al editar).</param>
    /// <returns><c>true</c> si el inmueble está libre; <c>false</c> si hay superposición.</returns>
    bool VerificarDisponibilidad(int inmuebleId, DateOnly desde, DateOnly hasta, int? excluirReservaId = null);

    IList<Reserva> ObtenerPorInmueble(int inmuebleId);
    IList<Reserva> ObtenerLista(string? estado, int nroDePagina = 1, int tamDePagina = 10);
    int ObtenerCantidad(string? estado = null);
}
