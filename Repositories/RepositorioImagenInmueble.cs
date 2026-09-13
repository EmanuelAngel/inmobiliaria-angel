using MySqlConnector;
using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

/// <summary>
/// Implementación con ADO.NET puro del repositorio de imágenes de galería de inmuebles.
/// </summary>
public class RepositorioImagenInmueble(IConfiguration configuration) : RepositorioBase(configuration), IRepositorioImagenInmueble
{
    public IList<ImagenInmueble> ObtenerPorInmueble(int inmuebleId)
    {
        var lista = new List<ImagenInmueble>();
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                inmueble_id,
                url
            FROM
                IMAGEN_INMUEBLE
            WHERE
                inmueble_id = @inmueble_id
            ORDER BY
                id ASC;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@inmueble_id", inmuebleId);

        conexion.Open();
        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(MapearBase(reader));
        }

        return lista;
    }

    public ImagenInmueble? ObtenerPorId(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                img.id,
                img.inmueble_id,
                img.url,
                i.direccion AS inmueble_direccion
            FROM
                IMAGEN_INMUEBLE img
                INNER JOIN INMUEBLE i ON img.inmueble_id = i.id
            WHERE
                img.id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);

        conexion.Open();
        using var reader = comando.ExecuteReader();
        if (reader.Read())
        {
            return MapearConJoins(reader);
        }

        return null;
    }

    public int Alta(ImagenInmueble imagen)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            INSERT INTO
                IMAGEN_INMUEBLE (inmueble_id, url)
            VALUES
                (@inmueble_id, @url);

            SELECT
                LAST_INSERT_ID();
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@inmueble_id", imagen.InmuebleId);
        comando.Parameters.AddWithValue("@url", imagen.Url);

        conexion.Open();
        var resultado = comando.ExecuteScalar();
        if (resultado != null && resultado != DBNull.Value)
        {
            var idGenerado = Convert.ToInt32(resultado);
            imagen.Id = idGenerado;
            return idGenerado;
        }

        return 0;
    }

    public int Borrar(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            DELETE FROM IMAGEN_INMUEBLE
            WHERE
                id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    private static ImagenInmueble MapearBase(MySqlDataReader reader)
    {
        return new ImagenInmueble
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            InmuebleId = reader.GetInt32(reader.GetOrdinal("inmueble_id")),
            Url = reader.GetString(reader.GetOrdinal("url"))
        };
    }

    private static ImagenInmueble MapearConJoins(MySqlDataReader reader)
    {
        var imagen = MapearBase(reader);
        imagen.Inmueble = new Inmueble
        {
            Id = imagen.InmuebleId,
            Direccion = reader.GetString(reader.GetOrdinal("inmueble_direccion"))
        };
        return imagen;
    }
}
