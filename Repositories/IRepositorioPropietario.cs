using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public interface IRepositorioPropietario : IRepositorio<Propietario>
{
    IList<Propietario> Buscar(string q, int limite = 20);
    Propietario? ObtenerPorDni(string dni);
    Propietario? ObtenerPorEmail(string email);
}
