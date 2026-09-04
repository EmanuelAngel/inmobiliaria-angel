using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public interface IRepositorioInquilino : IRepositorio<Inquilino>
{
    IList<Inquilino> Buscar(string q, int limite = 20);
    Inquilino? ObtenerPorDni(string dni);
    Inquilino? ObtenerPorEmail(string email);
}
