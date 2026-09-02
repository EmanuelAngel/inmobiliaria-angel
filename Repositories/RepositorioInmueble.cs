using MySqlConnector;
using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public class RepositorioInmueble(IConfiguration configuration) : RepositorioBase(configuration), IRepositorioInmueble
{
    public IList<Inmueble> ObtenerLista(int nroDePagina = 1, int tamDePagina = 12)
    {
        var lista = new List<Inmueble>();
        var offset = (Math.Max(1, nroDePagina) - 1) * tamDePagina;

        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                i.id,
                i.propietario_id,
                i.tipo_id,
                i.direccion,
                i.cupo,
                i.precio_por_dia,
                i.porcentaje_senia,
                i.latitud,
                i.longitud,
                i.imagen_portada,
                i.estado,
                p.nombre AS propietario_nombre,
                p.apellido AS propietario_apellido,
                p.dni AS propietario_dni,
                p.email AS propietario_email,
                p.telefono AS propietario_telefono,
                p.activo AS propietario_activo,
                t.descripcion AS tipo_descripcion,
                t.activo AS tipo_activo
            FROM
                INMUEBLE i
                INNER JOIN PROPIETARIO p ON i.propietario_id = p.id
                INNER JOIN TIPO_INMUEBLE t ON i.tipo_id = t.id
            ORDER BY
                i.id DESC
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
            lista.Add(MapearConJoins(reader));
        }

        return lista;
    }

    public IList<Inmueble> ObtenerPorDisponibilidad(string? estado, int nroDePagina = 1, int tamDePagina = 12)
    {
        if (string.IsNullOrWhiteSpace(estado))
        {
            return ObtenerLista(nroDePagina, tamDePagina);
        }

        var lista = new List<Inmueble>();
        var offset = (Math.Max(1, nroDePagina) - 1) * tamDePagina;

        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                i.id,
                i.propietario_id,
                i.tipo_id,
                i.direccion,
                i.cupo,
                i.precio_por_dia,
                i.porcentaje_senia,
                i.latitud,
                i.longitud,
                i.imagen_portada,
                i.estado,
                p.nombre AS propietario_nombre,
                p.apellido AS propietario_apellido,
                p.dni AS propietario_dni,
                p.email AS propietario_email,
                p.telefono AS propietario_telefono,
                p.activo AS propietario_activo,
                t.descripcion AS tipo_descripcion,
                t.activo AS tipo_activo
            FROM
                INMUEBLE i
                INNER JOIN PROPIETARIO p ON i.propietario_id = p.id
                INNER JOIN TIPO_INMUEBLE t ON i.tipo_id = t.id
            WHERE
                i.estado = @estado
            ORDER BY
                i.id DESC
            LIMIT
                @limite
            OFFSET
                @offset;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@estado", estado);
        comando.Parameters.AddWithValue("@limite", tamDePagina);
        comando.Parameters.AddWithValue("@offset", offset);

        conexion.Open();
        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(MapearConJoins(reader));
        }

        return lista;
    }

    public IList<Inmueble> ObtenerPorPropietario(int propietarioId)
    {
        var lista = new List<Inmueble>();

        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                i.id,
                i.propietario_id,
                i.tipo_id,
                i.direccion,
                i.cupo,
                i.precio_por_dia,
                i.porcentaje_senia,
                i.latitud,
                i.longitud,
                i.imagen_portada,
                i.estado,
                p.nombre AS propietario_nombre,
                p.apellido AS propietario_apellido,
                p.dni AS propietario_dni,
                p.email AS propietario_email,
                p.telefono AS propietario_telefono,
                p.activo AS propietario_activo,
                t.descripcion AS tipo_descripcion,
                t.activo AS tipo_activo
            FROM
                INMUEBLE i
                INNER JOIN PROPIETARIO p ON i.propietario_id = p.id
                INNER JOIN TIPO_INMUEBLE t ON i.tipo_id = t.id
            WHERE
                i.propietario_id = @propietarioId
            ORDER BY
                i.id DESC;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@propietarioId", propietarioId);

        conexion.Open();
        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(MapearConJoins(reader));
        }

        return lista;
    }

    public Inmueble? ObtenerPorId(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                i.id,
                i.propietario_id,
                i.tipo_id,
                i.direccion,
                i.cupo,
                i.precio_por_dia,
                i.porcentaje_senia,
                i.latitud,
                i.longitud,
                i.imagen_portada,
                i.estado,
                p.nombre AS propietario_nombre,
                p.apellido AS propietario_apellido,
                p.dni AS propietario_dni,
                p.email AS propietario_email,
                p.telefono AS propietario_telefono,
                p.activo AS propietario_activo,
                t.descripcion AS tipo_descripcion,
                t.activo AS tipo_activo
            FROM
                INMUEBLE i
                INNER JOIN PROPIETARIO p ON i.propietario_id = p.id
                INNER JOIN TIPO_INMUEBLE t ON i.tipo_id = t.id
            WHERE
                i.id = @id;
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

    public int Alta(Inmueble inmueble)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            INSERT INTO
                INMUEBLE (
                    propietario_id,
                    tipo_id,
                    direccion,
                    cupo,
                    precio_por_dia,
                    porcentaje_senia,
                    latitud,
                    longitud,
                    imagen_portada,
                    estado
                )
            VALUES
                (
                    @propietario_id,
                    @tipo_id,
                    @direccion,
                    @cupo,
                    @precio_por_dia,
                    @porcentaje_senia,
                    @latitud,
                    @longitud,
                    @imagen_portada,
                    @estado
                );

            SELECT
                LAST_INSERT_ID();
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@propietario_id", inmueble.PropietarioId);
        comando.Parameters.AddWithValue("@tipo_id", inmueble.TipoId);
        comando.Parameters.AddWithValue("@direccion", inmueble.Direccion);
        comando.Parameters.AddWithValue("@cupo", inmueble.Cupo);
        comando.Parameters.AddWithValue("@precio_por_dia", inmueble.PrecioPorDia);
        comando.Parameters.AddWithValue("@porcentaje_senia", inmueble.PorcentajeSenia);
        comando.Parameters.AddWithValue("@latitud", (object?)inmueble.Latitud ?? DBNull.Value);
        comando.Parameters.AddWithValue("@longitud", (object?)inmueble.Longitud ?? DBNull.Value);
        comando.Parameters.AddWithValue("@imagen_portada", (object?)inmueble.ImagenPortada ?? DBNull.Value);
        comando.Parameters.AddWithValue("@estado", inmueble.Estado.ToString());

        conexion.Open();
        var resultado = comando.ExecuteScalar();
        if (resultado != null && resultado != DBNull.Value)
        {
            var idGenerado = Convert.ToInt32(resultado);
            inmueble.Id = idGenerado;
            return idGenerado;
        }

        return 0;
    }

    public int Modificacion(Inmueble inmueble)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE INMUEBLE
            SET
                propietario_id = @propietario_id,
                tipo_id = @tipo_id,
                direccion = @direccion,
                cupo = @cupo,
                precio_por_dia = @precio_por_dia,
                porcentaje_senia = @porcentaje_senia,
                latitud = @latitud,
                longitud = @longitud,
                imagen_portada = @imagen_portada,
                estado = @estado
            WHERE
                id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", inmueble.Id);
        comando.Parameters.AddWithValue("@propietario_id", inmueble.PropietarioId);
        comando.Parameters.AddWithValue("@tipo_id", inmueble.TipoId);
        comando.Parameters.AddWithValue("@direccion", inmueble.Direccion);
        comando.Parameters.AddWithValue("@cupo", inmueble.Cupo);
        comando.Parameters.AddWithValue("@precio_por_dia", inmueble.PrecioPorDia);
        comando.Parameters.AddWithValue("@porcentaje_senia", inmueble.PorcentajeSenia);
        comando.Parameters.AddWithValue("@latitud", (object?)inmueble.Latitud ?? DBNull.Value);
        comando.Parameters.AddWithValue("@longitud", (object?)inmueble.Longitud ?? DBNull.Value);
        comando.Parameters.AddWithValue("@imagen_portada", (object?)inmueble.ImagenPortada ?? DBNull.Value);
        comando.Parameters.AddWithValue("@estado", inmueble.Estado.ToString());

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int Baja(int id)
    {
        // En inmueble la baja lógica suspende la oferta de la propiedad
        return CambiarEstado(id, EstadoInmueble.Suspendido.ToString());
    }

    public int CambiarEstado(int id, string nuevoEstado)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE INMUEBLE
            SET
                estado = @estado
            WHERE
                id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);
        comando.Parameters.AddWithValue("@estado", nuevoEstado);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int ObtenerCantidad()
    {
        return ObtenerCantidad(null);
    }

    public int ObtenerCantidad(string? estado = null)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        var query = string.IsNullOrWhiteSpace(estado)
            ? """
                SELECT
                    COUNT(id)
                FROM
                    INMUEBLE;
            """
            : """
                SELECT
                    COUNT(id)
                FROM
                    INMUEBLE
                WHERE
                    estado = @estado;
            """;

        using var comando = new MySqlCommand(query, conexion);
        if (!string.IsNullOrWhiteSpace(estado))
        {
            comando.Parameters.AddWithValue("@estado", estado);
        }

        conexion.Open();
        var resultado = comando.ExecuteScalar();
        return Convert.ToInt32(resultado);
    }

    private static Inmueble MapearBase(MySqlDataReader reader)
    {
        var estadoRaw = reader.GetString(reader.GetOrdinal("estado"));
        var estado = Enum.TryParse<EstadoInmueble>(estadoRaw, true, out var parsed)
            ? parsed
            : EstadoInmueble.Disponible;

        return new Inmueble
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            PropietarioId = reader.GetInt32(reader.GetOrdinal("propietario_id")),
            TipoId = reader.GetInt32(reader.GetOrdinal("tipo_id")),
            Direccion = reader.GetString(reader.GetOrdinal("direccion")),
            Cupo = reader.GetInt32(reader.GetOrdinal("cupo")),
            PrecioPorDia = reader.GetDecimal(reader.GetOrdinal("precio_por_dia")),
            PorcentajeSenia = reader.GetDecimal(reader.GetOrdinal("porcentaje_senia")),
            Latitud = reader.IsDBNull(reader.GetOrdinal("latitud")) ? null : reader.GetDecimal(reader.GetOrdinal("latitud")),
            Longitud = reader.IsDBNull(reader.GetOrdinal("longitud")) ? null : reader.GetDecimal(reader.GetOrdinal("longitud")),
            ImagenPortada = reader.IsDBNull(reader.GetOrdinal("imagen_portada")) ? null : reader.GetString(reader.GetOrdinal("imagen_portada")),
            Estado = estado
        };
    }

    private static Inmueble MapearConJoins(MySqlDataReader reader)
    {
        var inmueble = MapearBase(reader);

        inmueble.Propietario = new Propietario
        {
            Id = reader.GetInt32(reader.GetOrdinal("propietario_id")),
            Nombre = reader.GetString(reader.GetOrdinal("propietario_nombre")),
            Apellido = reader.GetString(reader.GetOrdinal("propietario_apellido")),
            Dni = reader.GetString(reader.GetOrdinal("propietario_dni")),
            Email = reader.GetString(reader.GetOrdinal("propietario_email")),
            Telefono = reader.GetString(reader.GetOrdinal("propietario_telefono")),
            Activo = reader.GetBoolean(reader.GetOrdinal("propietario_activo"))
        };

        inmueble.Tipo = new TipoInmueble
        {
            Id = reader.GetInt32(reader.GetOrdinal("tipo_id")),
            Descripcion = reader.GetString(reader.GetOrdinal("tipo_descripcion")),
            Activo = reader.GetBoolean(reader.GetOrdinal("tipo_activo"))
        };

        return inmueble;
    }
}
