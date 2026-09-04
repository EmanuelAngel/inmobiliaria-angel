using MySqlConnector;
using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public class RepositorioReserva(IConfiguration configuration) : RepositorioBase(configuration), IRepositorioReserva
{
    public IList<Reserva> ObtenerLista(int nroDePagina = 1, int tamDePagina = 10)
        => ObtenerLista(null, nroDePagina, tamDePagina);

    public IList<Reserva> ObtenerLista(string? estado, int nroDePagina = 1, int tamDePagina = 10)
    {
        var lista = new List<Reserva>();
        var offset = (Math.Max(1, nroDePagina) - 1) * tamDePagina;

        using var conexion = new MySqlConnection(ConnectionString);

        var sql = string.IsNullOrWhiteSpace(estado)
            ? """
                SELECT
                    r.id,
                    r.inquilino_id,
                    r.inmueble_id,
                    r.usuario_creacion_id,
                    r.usuario_terminacion_id,
                    r.fecha_desde,
                    r.fecha_hasta,
                    r.fecha_fin_anticipado,
                    r.monto_por_dia,
                    r.estado,
                    iq.nombre_completo,
                    iq.dni,
                    iq.email,
                    iq.telefono,
                    iq.activo AS inquilino_activo,
                    im.direccion,
                    im.propietario_id,
                    im.tipo_id,
                    im.cupo,
                    im.precio_por_dia AS inmueble_precio_por_dia,
                    im.porcentaje_senia,
                    im.latitud,
                    im.longitud,
                    im.imagen_portada,
                    im.estado AS inmueble_estado,
                    t.descripcion AS tipo_descripcion,
                    t.activo AS tipo_activo
                FROM
                    RESERVA r
                    INNER JOIN INQUILINO iq ON r.inquilino_id = iq.id
                    INNER JOIN INMUEBLE im ON r.inmueble_id = im.id
                    INNER JOIN TIPO_INMUEBLE t ON im.tipo_id = t.id
                ORDER BY
                    r.id DESC
                LIMIT
                    @limite
                OFFSET
                    @offset;
            """
            : """
                SELECT
                    r.id,
                    r.inquilino_id,
                    r.inmueble_id,
                    r.usuario_creacion_id,
                    r.usuario_terminacion_id,
                    r.fecha_desde,
                    r.fecha_hasta,
                    r.fecha_fin_anticipado,
                    r.monto_por_dia,
                    r.estado,
                    iq.nombre_completo,
                    iq.dni,
                    iq.email,
                    iq.telefono,
                    iq.activo AS inquilino_activo,
                    im.direccion,
                    im.propietario_id,
                    im.tipo_id,
                    im.cupo,
                    im.precio_por_dia AS inmueble_precio_por_dia,
                    im.porcentaje_senia,
                    im.latitud,
                    im.longitud,
                    im.imagen_portada,
                    im.estado AS inmueble_estado,
                    t.descripcion AS tipo_descripcion,
                    t.activo AS tipo_activo
                FROM
                    RESERVA r
                    INNER JOIN INQUILINO iq ON r.inquilino_id = iq.id
                    INNER JOIN INMUEBLE im ON r.inmueble_id = im.id
                    INNER JOIN TIPO_INMUEBLE t ON im.tipo_id = t.id
                WHERE
                    r.estado = @estado
                ORDER BY
                    r.id DESC
                LIMIT
                    @limite
                OFFSET
                    @offset;
            """;

        using var comando = new MySqlCommand(sql, conexion);
        if (!string.IsNullOrWhiteSpace(estado))
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

    public Reserva? ObtenerPorId(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                r.id,
                r.inquilino_id,
                r.inmueble_id,
                r.usuario_creacion_id,
                r.usuario_terminacion_id,
                r.fecha_desde,
                r.fecha_hasta,
                r.fecha_fin_anticipado,
                r.monto_por_dia,
                r.estado,
                iq.nombre_completo,
                iq.dni,
                iq.email,
                iq.telefono,
                iq.activo AS inquilino_activo,
                im.direccion,
                im.propietario_id,
                im.tipo_id,
                im.cupo,
                im.precio_por_dia AS inmueble_precio_por_dia,
                im.porcentaje_senia,
                im.latitud,
                im.longitud,
                im.imagen_portada,
                im.estado AS inmueble_estado,
                t.descripcion AS tipo_descripcion,
                t.activo AS tipo_activo
            FROM
                RESERVA r
                INNER JOIN INQUILINO iq ON r.inquilino_id = iq.id
                INNER JOIN INMUEBLE im ON r.inmueble_id = im.id
                INNER JOIN TIPO_INMUEBLE t ON im.tipo_id = t.id
            WHERE
                r.id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);

        conexion.Open();
        using var reader = comando.ExecuteReader();
        return reader.Read() ? MapearConJoins(reader) : null;
    }

    public int ObtenerCantidad() => ObtenerCantidad(null);

    public int ObtenerCantidad(string? estado = null)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        var query = string.IsNullOrWhiteSpace(estado)
            ? """
                SELECT
                    COUNT(id)
                FROM
                    RESERVA;
            """
            : """
                SELECT
                    COUNT(id)
                FROM
                    RESERVA
                WHERE
                    estado = @estado;
            """;

        using var comando = new MySqlCommand(query, conexion);
        if (!string.IsNullOrWhiteSpace(estado))
            comando.Parameters.AddWithValue("@estado", estado);

        conexion.Open();
        return Convert.ToInt32(comando.ExecuteScalar());
    }

    public int Alta(Reserva reserva)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            INSERT INTO
                RESERVA (
                    inquilino_id,
                    inmueble_id,
                    fecha_desde,
                    fecha_hasta,
                    monto_por_dia,
                    estado
                )
            VALUES
                (
                    @inquilino_id,
                    @inmueble_id,
                    @fecha_desde,
                    @fecha_hasta,
                    @monto_por_dia,
                    @estado
                );

            SELECT
                LAST_INSERT_ID();
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@inquilino_id", reserva.InquilinoId);
        comando.Parameters.AddWithValue("@inmueble_id", reserva.InmuebleId);
        comando.Parameters.AddWithValue("@fecha_desde", reserva.FechaDesde.ToDateTime(TimeOnly.MinValue));
        comando.Parameters.AddWithValue("@fecha_hasta", reserva.FechaHasta.ToDateTime(TimeOnly.MinValue));
        comando.Parameters.AddWithValue("@monto_por_dia", reserva.MontoPorDia);
        comando.Parameters.AddWithValue("@estado", reserva.Estado.ToString());

        conexion.Open();
        var resultado = comando.ExecuteScalar();
        if (resultado != null && resultado != DBNull.Value)
        {
            var idGenerado = Convert.ToInt32(resultado);
            reserva.Id = idGenerado;
            return idGenerado;
        }

        return 0;
    }

    public int Modificacion(Reserva reserva)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE RESERVA
            SET
                inquilino_id = @inquilino_id,
                inmueble_id = @inmueble_id,
                fecha_desde = @fecha_desde,
                fecha_hasta = @fecha_hasta,
                monto_por_dia = @monto_por_dia
            WHERE
                id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", reserva.Id);
        comando.Parameters.AddWithValue("@inquilino_id", reserva.InquilinoId);
        comando.Parameters.AddWithValue("@inmueble_id", reserva.InmuebleId);
        comando.Parameters.AddWithValue("@fecha_desde", reserva.FechaDesde.ToDateTime(TimeOnly.MinValue));
        comando.Parameters.AddWithValue("@fecha_hasta", reserva.FechaHasta.ToDateTime(TimeOnly.MinValue));
        comando.Parameters.AddWithValue("@monto_por_dia", reserva.MontoPorDia);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int Baja(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE RESERVA
            SET
                estado = @estado
            WHERE
                id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);
        comando.Parameters.AddWithValue("@estado", EstadoReserva.Cancelada.ToString());

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public bool VerificarDisponibilidad(int inmuebleId, DateOnly desde, DateOnly hasta, int? excluirReservaId = null)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                COUNT(id)
            FROM
                RESERVA
            WHERE
                inmueble_id = @inmuebleId
                AND estado = @estado
                AND fecha_desde < @hasta
                AND fecha_hasta > @desde
                AND (
                    @excluirId IS NULL
                    OR id != @excluirId
                );
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@inmuebleId", inmuebleId);
        comando.Parameters.AddWithValue("@estado", EstadoReserva.Activa.ToString());
        comando.Parameters.AddWithValue("@desde", desde.ToDateTime(TimeOnly.MinValue));
        comando.Parameters.AddWithValue("@hasta", hasta.ToDateTime(TimeOnly.MinValue));
        comando.Parameters.AddWithValue("@excluirId", (object?)excluirReservaId ?? DBNull.Value);

        conexion.Open();
        return Convert.ToInt32(comando.ExecuteScalar()) == 0;
    }

    public IList<Reserva> ObtenerPorInmueble(int inmuebleId)
    {
        var lista = new List<Reserva>();

        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                r.id,
                r.inquilino_id,
                r.inmueble_id,
                r.usuario_creacion_id,
                r.usuario_terminacion_id,
                r.fecha_desde,
                r.fecha_hasta,
                r.fecha_fin_anticipado,
                r.monto_por_dia,
                r.estado,
                iq.nombre_completo,
                iq.dni,
                iq.email,
                iq.telefono,
                iq.activo AS inquilino_activo,
                im.direccion,
                im.propietario_id,
                im.tipo_id,
                im.cupo,
                im.precio_por_dia AS inmueble_precio_por_dia,
                im.porcentaje_senia,
                im.latitud,
                im.longitud,
                im.imagen_portada,
                im.estado AS inmueble_estado,
                t.descripcion AS tipo_descripcion,
                t.activo AS tipo_activo
            FROM
                RESERVA r
                INNER JOIN INQUILINO iq ON r.inquilino_id = iq.id
                INNER JOIN INMUEBLE im ON r.inmueble_id = im.id
                INNER JOIN TIPO_INMUEBLE t ON im.tipo_id = t.id
            WHERE
                r.inmueble_id = @inmuebleId
            ORDER BY
                r.id DESC;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@inmuebleId", inmuebleId);

        conexion.Open();
        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(MapearConJoins(reader));
        }

        return lista;
    }

    // ── Mappers ───────────────────────────────────────────────────────────────

    private static Reserva MapearBase(MySqlDataReader reader)
    {
        var estadoRaw = reader.GetString(reader.GetOrdinal("estado"));
        var estado = Enum.TryParse<EstadoReserva>(estadoRaw, true, out var parsed)
            ? parsed
            : EstadoReserva.Activa;

        var fechaFinOrdinal = reader.GetOrdinal("fecha_fin_anticipado");

        return new Reserva
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            InquilinoId = reader.GetInt32(reader.GetOrdinal("inquilino_id")),
            InmuebleId = reader.GetInt32(reader.GetOrdinal("inmueble_id")),
            FechaDesde = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("fecha_desde"))),
            FechaHasta = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("fecha_hasta"))),
            FechaFinAnticipado = reader.IsDBNull(fechaFinOrdinal)
                ? null
                : DateOnly.FromDateTime(reader.GetDateTime(fechaFinOrdinal)),
            MontoPorDia = reader.GetDecimal(reader.GetOrdinal("monto_por_dia")),
            Estado = estado,
            UsuarioCreacionId = reader.IsDBNull(reader.GetOrdinal("usuario_creacion_id"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("usuario_creacion_id")),
            UsuarioTerminacionId = reader.IsDBNull(reader.GetOrdinal("usuario_terminacion_id"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("usuario_terminacion_id")),
        };
    }

    private static Reserva MapearConJoins(MySqlDataReader reader)
    {
        var reserva = MapearBase(reader);

        reserva.Inquilino = new Inquilino
        {
            Id = reader.GetInt32(reader.GetOrdinal("inquilino_id")),
            NombreCompleto = reader.GetString(reader.GetOrdinal("nombre_completo")),
            Dni = reader.GetString(reader.GetOrdinal("dni")),
            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
            Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
            Activo = reader.GetBoolean(reader.GetOrdinal("inquilino_activo")),
        };

        var inmuebleEstadoRaw = reader.GetString(reader.GetOrdinal("inmueble_estado"));
        var inmuebleEstado = Enum.TryParse<EstadoInmueble>(inmuebleEstadoRaw, true, out var parsedEstado)
            ? parsedEstado
            : EstadoInmueble.Disponible;

        reserva.Inmueble = new Inmueble
        {
            Id = reader.GetInt32(reader.GetOrdinal("inmueble_id")),
            PropietarioId = reader.GetInt32(reader.GetOrdinal("propietario_id")),
            TipoId = reader.GetInt32(reader.GetOrdinal("tipo_id")),
            Direccion = reader.GetString(reader.GetOrdinal("direccion")),
            Cupo = reader.GetInt32(reader.GetOrdinal("cupo")),
            PrecioPorDia = reader.GetDecimal(reader.GetOrdinal("inmueble_precio_por_dia")),
            PorcentajeSenia = reader.GetDecimal(reader.GetOrdinal("porcentaje_senia")),
            Latitud = reader.IsDBNull(reader.GetOrdinal("latitud")) ? null : reader.GetDecimal(reader.GetOrdinal("latitud")),
            Longitud = reader.IsDBNull(reader.GetOrdinal("longitud")) ? null : reader.GetDecimal(reader.GetOrdinal("longitud")),
            ImagenPortada = reader.IsDBNull(reader.GetOrdinal("imagen_portada")) ? null : reader.GetString(reader.GetOrdinal("imagen_portada")),
            Estado = inmuebleEstado,
            Tipo = new TipoInmueble
            {
                Id = reader.GetInt32(reader.GetOrdinal("tipo_id")),
                Descripcion = reader.GetString(reader.GetOrdinal("tipo_descripcion")),
                Activo = reader.GetBoolean(reader.GetOrdinal("tipo_activo")),
            },
        };

        return reserva;
    }
}
