using MySqlConnector;
using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public class RepositorioTipoInmueble(IConfiguration configuration) : RepositorioBase(configuration), IRepositorioTipoInmueble
{
    public IList<TipoInmueble> ObtenerLista(int nroDePagina = 1, int tamDePagina = 12)
    {
        var lista = new List<TipoInmueble>();
        var offset = (Math.Max(1, nroDePagina) - 1) * tamDePagina;

        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                descripcion,
                activo
            FROM
                TIPO_INMUEBLE
            WHERE
                activo = 1
            ORDER BY
                descripcion
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

    public IList<TipoInmueble> ObtenerTodos()
    {
        var lista = new List<TipoInmueble>();

        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                descripcion,
                activo
            FROM
                TIPO_INMUEBLE
            WHERE
                activo = 1
            ORDER BY
                descripcion;
        """;

        using var comando = new MySqlCommand(query, conexion);

        conexion.Open();

        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(Mapear(reader));
        }

        return lista;
    }

    public TipoInmueble? ObtenerPorId(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                descripcion,
                activo
            FROM
                TIPO_INMUEBLE
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

    public TipoInmueble? ObtenerPorDescripcion(string descripcion)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                descripcion,
                activo
            FROM
                TIPO_INMUEBLE
            WHERE
                descripcion = @descripcion
                AND activo = 1;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@descripcion", descripcion);

        conexion.Open();
        using var reader = comando.ExecuteReader();

        if (reader.Read())
        {
            return Mapear(reader);
        }

        return null;
    }

    public int Alta(TipoInmueble tipoInmueble)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            INSERT INTO
                TIPO_INMUEBLE (descripcion)
            VALUES
                (@descripcion);

            SELECT
                LAST_INSERT_ID();
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@descripcion", tipoInmueble.Descripcion);

        conexion.Open();
        var resultado = comando.ExecuteScalar();
        if (resultado != null && resultado != DBNull.Value)
        {
            var idGenerado = Convert.ToInt32(resultado);
            tipoInmueble.Id = idGenerado;
            return idGenerado;
        }

        return 0;
    }

    public int Modificacion(TipoInmueble tipoInmueble)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE TIPO_INMUEBLE
            SET
                descripcion = @descripcion
            WHERE
                id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", tipoInmueble.Id);
        comando.Parameters.AddWithValue("@descripcion", tipoInmueble.Descripcion);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int Baja(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE TIPO_INMUEBLE
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
                TIPO_INMUEBLE
            WHERE
                activo = 1;
        """;

        using var comando = new MySqlCommand(query, conexion);

        conexion.Open();

        var resultado = comando.ExecuteScalar();

        return Convert.ToInt32(resultado);
    }

    private static TipoInmueble Mapear(MySqlDataReader reader)
    {
        return new TipoInmueble
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Descripcion = reader.GetString(reader.GetOrdinal("descripcion")),
            Activo = reader.GetBoolean(reader.GetOrdinal("activo"))
        };
    }
}
