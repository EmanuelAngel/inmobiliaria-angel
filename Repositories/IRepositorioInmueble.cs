using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public interface IRepositorioInmueble : IRepositorio<Inmueble>
{
    IList<Inmueble> Buscar(string q, int limite = 20);
    IList<Inmueble> ObtenerPorPropietario(int propietarioId);
    IList<Inmueble> ObtenerPorDisponibilidad(string? estado, int nroDePagina = 1, int tamDePagina = 12);
    int ObtenerCantidad(string? estado = null);
    int CambiarEstado(int id, string nuevoEstado);
}
