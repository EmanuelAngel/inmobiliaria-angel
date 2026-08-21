using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public interface IRepositorioInquilino : IRepositorio<Inquilino>
{
    Inquilino? ObtenerPorDni(string dni);
    Inquilino? ObtenerPorEmail(string email);
}
