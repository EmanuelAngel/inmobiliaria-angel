using MySqlConnector;
using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public class RepositorioInquilino(IConfiguration configuration) : RepositorioBase(configuration), IRepositorioInquilino
{
    public IList<Inquilino> ObtenerLista(int nroDePagina = 1, int tamDePagina = 12)
    {
        var lista = new List<Inquilino>();
        var offset = (Math.Max(1, nroDePagina) - 1) * tamDePagina;

        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                dni,
                nombre_completo,
                email,
                telefono,
                activo
            FROM
                INQUILINO
            WHERE
                activo = 1
            ORDER BY
                nombre_completo
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

    public Inquilino? ObtenerPorId(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                dni,
                nombre_completo,
                email,
                telefono,
                activo
            FROM
                INQUILINO
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

    public Inquilino? ObtenerPorDni(string dni)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                dni,
                nombre_completo,
                email,
                telefono,
                activo
            FROM
                INQUILINO
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

    public Inquilino? ObtenerPorEmail(string email)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                dni,
                nombre_completo,
                email,
                telefono,
                activo
            FROM
                INQUILINO
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

    public int Alta(Inquilino inquilino)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            INSERT INTO
                INQUILINO (dni, nombre_completo, email, telefono)
            VALUES
                (@dni, @nombre_completo, @email, @telefono);

            SELECT
                LAST_INSERT_ID();
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@dni", inquilino.Dni);
        comando.Parameters.AddWithValue("@nombre_completo", inquilino.NombreCompleto);
        comando.Parameters.AddWithValue("@email", (object?)inquilino.Email ?? DBNull.Value);
        comando.Parameters.AddWithValue("@telefono", (object?)inquilino.Telefono ?? DBNull.Value);

        conexion.Open();
        var resultado = comando.ExecuteScalar();
        if (resultado != null && resultado != DBNull.Value)
        {
            var idGenerado = Convert.ToInt32(resultado);
            inquilino.Id = idGenerado;
            return idGenerado;
        }

        return 0;
    }

    public int Modificacion(Inquilino inquilino)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE INQUILINO
            SET
                dni = @dni,
                nombre_completo = @nombre_completo,
                email = @email,
                telefono = @telefono
            WHERE
                id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", inquilino.Id);
        comando.Parameters.AddWithValue("@dni", inquilino.Dni);
        comando.Parameters.AddWithValue("@nombre_completo", inquilino.NombreCompleto);
        comando.Parameters.AddWithValue("@email", (object?)inquilino.Email ?? DBNull.Value);
        comando.Parameters.AddWithValue("@telefono", (object?)inquilino.Telefono ?? DBNull.Value);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int Baja(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE INQUILINO
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
                INQUILINO
            WHERE
                activo = 1;
        """;

        using var comando = new MySqlCommand(query, conexion);

        conexion.Open();
        var resultado = comando.ExecuteScalar();

        return Convert.ToInt32(resultado);
    }

    public IList<Inquilino> Buscar(string q, int limite = 20)
    {
        var lista = new List<Inquilino>();
        var termino = $"%{q}%";

        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                dni,
                nombre_completo,
                email,
                telefono,
                activo
            FROM
                INQUILINO
            WHERE
                activo = 1
                AND (
                    nombre_completo LIKE @q
                    OR dni LIKE @q
                )
            ORDER BY
                nombre_completo
            LIMIT
                @limite;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@q", termino);
        comando.Parameters.AddWithValue("@limite", limite);

        conexion.Open();
        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(Mapear(reader));
        }

        return lista;
    }

    private static Inquilino Mapear(MySqlDataReader reader)
    {
        var emailOrdinal = reader.GetOrdinal("email");
        var telefonoOrdinal = reader.GetOrdinal("telefono");

        return new Inquilino
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Dni = reader.GetString(reader.GetOrdinal("dni")),
            NombreCompleto = reader.GetString(reader.GetOrdinal("nombre_completo")),
            Email = reader.IsDBNull(emailOrdinal) ? null : reader.GetString(emailOrdinal),
            Telefono = reader.IsDBNull(telefonoOrdinal) ? null : reader.GetString(telefonoOrdinal),
            Activo = reader.GetBoolean(reader.GetOrdinal("activo"))
        };
    }
}
