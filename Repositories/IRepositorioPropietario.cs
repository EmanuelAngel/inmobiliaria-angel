using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public interface IRepositorioPropietario : IRepositorio<Propietario>
{
    Propietario? ObtenerPorDni(string dni);
    Propietario? ObtenerPorEmail(string email);
}
