namespace inmobiliaria_lab2.Repositories;

public interface IRepositorio<T>
{
    IList<T> ObtenerLista(int nroDePagina = 1, int tamDePagina = 12);
    T? ObtenerPorId(int id);
    int ObtenerCantidad();
    int Alta(T entidad);
    int Modificacion(T entidad);
    int Baja(int id);
}
