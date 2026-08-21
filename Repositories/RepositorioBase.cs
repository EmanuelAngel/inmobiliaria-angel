namespace inmobiliaria_lab2.Repositories;

public abstract class RepositorioBase
{
    protected readonly string ConnectionString;

    protected RepositorioBase(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");
    }
}
