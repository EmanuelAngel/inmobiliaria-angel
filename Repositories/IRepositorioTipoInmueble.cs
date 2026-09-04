using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public interface IRepositorioTipoInmueble : IRepositorio<TipoInmueble>
{
    TipoInmueble? ObtenerPorDescripcion(string descripcion);
    IList<TipoInmueble> ObtenerTodos();
}
