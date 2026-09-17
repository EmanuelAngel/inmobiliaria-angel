using inmobiliaria_lab2.Models;
using MySqlConnector;

namespace inmobiliaria_lab2.Repositories;

public interface IRepositorioPago : IRepositorio<Pago>
{
    /// <summary>
    /// Obtiene todos los pagos asociados a una reserva.
    /// </summary>
    IList<Pago> ObtenerPorReserva(int reservaId, bool soloActivos = false);

    /// <summary>
    /// Calcula la suma de todos los pagos activos de una reserva.
    /// </summary>
    decimal ObtenerTotalPagado(int reservaId);

    /// <summary>
    /// Lista paginada con filtros opcionales por reserva y por estado ('Activo' / 'Anulado').
    /// </summary>
    IList<Pago> ObtenerLista(int? reservaId, string? estado, int nroDePagina = 1, int tamDePagina = 10);

    /// <summary>
    /// Cuenta la cantidad de registros con filtros opcionales.
    /// </summary>
    int ObtenerCantidad(int? reservaId, string? estado = null);

    /// <summary>
    /// Modifica exclusivamente el concepto de un pago activo.
    /// </summary>
    int ModificarConcepto(int id, string nuevoConcepto);

    /// <summary>
    /// Anula un pago activo y registra qué usuario administrador lo anuló.
    /// </summary>
    int Anular(int id, int usuarioAnulacionId);

    /// <summary>
    /// Inserción de pago con soporte opcional para transacciones externas.
    /// </summary>
    int Alta(Pago pago, MySqlConnection? conexion, MySqlTransaction? transaccion);
}
