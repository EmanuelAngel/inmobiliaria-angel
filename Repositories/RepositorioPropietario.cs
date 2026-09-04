using MySqlConnector;
using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public class RepositorioPropietario(IConfiguration configuration) : RepositorioBase(configuration), IRepositorioPropietario
{

    public IList<Propietario> ObtenerLista(int nroDePagina = 1, int tamDePagina = 12)
    {
        var lista = new List<Propietario>();
        var offset = (Math.Max(1, nroDePagina) - 1) * tamDePagina;

        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                nombre,
                apellido,
                dni,
                email,
                telefono,
                activo
            FROM
                PROPIETARIO
            WHERE
                activo = 1
            ORDER BY
                apellido,
                nombre
            LIMIT
                @limite
            OFFSET
                @offset;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@limite", tamDePagina);
        comando.Parameters.AddWithValue("@offset", offset);

        conexion.Open();

        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(Mapear(reader));
        }

        return lista;
    }

    public IList<Propietario> Buscar(string q, int limite = 20)
    {
        var lista = new List<Propietario>();
        limite = Math.Clamp(limite, 1, 50);

        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                nombre,
                apellido,
                dni,
                email,
                telefono,
                activo
            FROM
                PROPIETARIO
            WHERE
                activo = 1
                AND (
                    nombre LIKE @patron
                    OR apellido LIKE @patron
                    OR dni LIKE @patron
                )
            ORDER BY
                apellido,
                nombre
            LIMIT
                @limite;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@patron", $"%{q.Trim()}%");
        comando.Parameters.AddWithValue("@limite", limite);

        conexion.Open();

        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(Mapear(reader));
        }

        return lista;
    }

    public Propietario? ObtenerPorId(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);

        const string query = """
            SELECT
                id,
                nombre,
                apellido,
                dni,
                email,
                telefono,
                activo
            FROM
                PROPIETARIO
            WHERE
                id = @id
                AND activo = 1;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);

        conexion.Open();
        using var reader = comando.ExecuteReader();

        if (reader.Read())
        {
            return Mapear(reader);
        }

        return null;
    }

    public Propietario? ObtenerPorDni(string dni)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                nombre,
                apellido,
                dni,
                email,
                telefono,
                activo
            FROM
                PROPIETARIO
            WHERE
                dni = @dni
                AND activo = 1;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@dni", dni);

        conexion.Open();
        using var reader = comando.ExecuteReader();

        if (reader.Read())
        {
            return Mapear(reader);
        }

        return null;
    }

    public Propietario? ObtenerPorEmail(string email)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                nombre,
                apellido,
                dni,
                email,
                telefono,
                activo
            FROM
                PROPIETARIO
            WHERE
                email = @email
                AND activo = 1;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@email", email);

        conexion.Open();
        using var reader = comando.ExecuteReader();

        if (reader.Read())
        {
            return Mapear(reader);
        }

        return null;
    }

    public int Alta(Propietario propietario)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            INSERT INTO
                PROPIETARIO (nombre, apellido, dni, email, telefono)
            VALUES
                (@nombre, @apellido, @dni, @email, @telefono);

            SELECT
                LAST_INSERT_ID();
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@nombre", propietario.Nombre);
        comando.Parameters.AddWithValue("@apellido", propietario.Apellido);
        comando.Parameters.AddWithValue("@dni", propietario.Dni);
        comando.Parameters.AddWithValue("@email", propietario.Email);
        comando.Parameters.AddWithValue("@telefono", propietario.Telefono);

        conexion.Open();
        var resultado = comando.ExecuteScalar();
        if (resultado != null && resultado != DBNull.Value)
        {
            var idGenerado = Convert.ToInt32(resultado);
            propietario.Id = idGenerado;
            return idGenerado;
        }

        return 0;
    }

    public int Modificacion(Propietario propietario)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE PROPIETARIO
            SET
                nombre = @nombre,
                apellido = @apellido,
                dni = @dni,
                email = @email,
                telefono = @telefono
            WHERE
                id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", propietario.Id);
        comando.Parameters.AddWithValue("@nombre", propietario.Nombre);
        comando.Parameters.AddWithValue("@apellido", propietario.Apellido);
        comando.Parameters.AddWithValue("@dni", propietario.Dni);
        comando.Parameters.AddWithValue("@email", propietario.Email);
        comando.Parameters.AddWithValue("@telefono", propietario.Telefono);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int Baja(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE PROPIETARIO
            SET
                activo = 0
            WHERE
                id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int ObtenerCantidad()
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                COUNT(id)
            FROM
                PROPIETARIO
            WHERE
                activo = 1;
        """;

        using var comando = new MySqlCommand(query, conexion);

        conexion.Open();

        var resultado = comando.ExecuteScalar();

        return Convert.ToInt32(resultado);
    }

    private static Propietario Mapear(MySqlDataReader reader)
    {
        return new Propietario
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
            Apellido = reader.GetString(reader.GetOrdinal("apellido")),
            Dni = reader.GetString(reader.GetOrdinal("dni")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            Telefono = reader.GetString(reader.GetOrdinal("telefono")),
            Activo = reader.GetBoolean(reader.GetOrdinal("activo"))
        };
    }
}
