using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public interface IRepositorioUsuario : IRepositorio<Usuario>
{
    Usuario? ObtenerPorEmail(string email);
    IList<Usuario> Buscar(string q, int limite = 20);
    int ActualizarPassword(int id, string passwordHash);
    int ActualizarAvatar(int id, string? avatar);
}
