using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public interface IRepositorioUsuario : IRepositorio<Usuario>
{
    Usuario? ObtenerPorId(int id, bool soloActivos);
    Usuario? ObtenerPorEmail(string email, bool soloActivos = true);
    IList<Usuario> ObtenerLista(string? estado, int nroDePagina = 1, int tamDePagina = 12);
    int ObtenerCantidad(string? estado);
    IList<Usuario> Buscar(string q, int limite = 20);
    int ActualizarPassword(int id, string passwordHash);
    int ActualizarAvatar(int id, string? avatar);
    int Activar(int id);
}
