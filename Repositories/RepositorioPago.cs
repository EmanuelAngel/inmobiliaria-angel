using System.Data;
using inmobiliaria_lab2.Models;
using MySqlConnector;

namespace inmobiliaria_lab2.Repositories;

public class RepositorioPago(IConfiguration configuration) : RepositorioBase(configuration), IRepositorioPago
{
    private static Pago MapearBase(MySqlDataReader reader)
    {
        return new Pago
        {
            Id = reader.GetInt32("id"),
            ReservaId = reader.GetInt32("reserva_id"),
            UsuarioCreacionId = reader.GetInt32("usuario_creacion_id"),
            UsuarioAnulacionId = reader.IsDBNull(reader.GetOrdinal("usuario_anulacion_id"))
                ? null
                : reader.GetInt32("usuario_anulacion_id"),
            Concepto = reader.GetString("concepto"),
            Fecha = DateOnly.FromDateTime(reader.GetDateTime("fecha")),
            Importe = reader.GetDecimal("importe"),
            Estado = Enum.Parse<EstadoPago>(reader.GetString("estado"), ignoreCase: true)
        };
    }

    private static Pago MapearConJoins(MySqlDataReader reader)
    {
        var pago = MapearBase(reader);

        // Reserva e Inquilino
        pago.Reserva = new Reserva
        {
            Id = pago.ReservaId,
            InquilinoId = reader.GetInt32("reserva_inquilino_id"),
            InmuebleId = reader.GetInt32("reserva_inmueble_id"),
            FechaDesde = DateOnly.FromDateTime(reader.GetDateTime("reserva_fecha_desde")),
            FechaHasta = DateOnly.FromDateTime(reader.GetDateTime("reserva_fecha_hasta")),
            MontoPorDia = reader.GetDecimal("reserva_monto_por_dia"),
            Estado = Enum.Parse<EstadoReserva>(reader.GetString("reserva_estado"), ignoreCase: true),
            Inquilino = new Inquilino
            {
                Id = reader.GetInt32("reserva_inquilino_id"),
                Dni = reader.GetString("inquilino_dni"),
                NombreCompleto = reader.GetString("inquilino_nombre"),
                Email = reader.IsDBNull(reader.GetOrdinal("inquilino_email")) ? null : reader.GetString("inquilino_email"),
                Telefono = reader.IsDBNull(reader.GetOrdinal("inquilino_telefono")) ? null : reader.GetString("inquilino_telefono")
            },
            Inmueble = new Inmueble
            {
                Id = reader.GetInt32("reserva_inmueble_id"),
                Direccion = reader.GetString("inmueble_direccion")
            }
        };

        // Auditoría — Usuario Creador
        pago.UsuarioCreacion = new Usuario
        {
            Id = pago.UsuarioCreacionId,
            Nombre = reader.GetString("creador_nombre"),
            Apellido = reader.GetString("creador_apellido"),
            Email = reader.GetString("creador_email")
        };

        // Auditoría — Usuario Anulador (si fue anulado)
        if (pago.UsuarioAnulacionId.HasValue && !reader.IsDBNull(reader.GetOrdinal("anulador_nombre")))
        {
            pago.UsuarioAnulacion = new Usuario
            {
                Id = pago.UsuarioAnulacionId.Value,
                Nombre = reader.GetString("anulador_nombre"),
                Apellido = reader.GetString("anulador_apellido"),
                Email = reader.GetString("anulador_email")
            };
        }

        return pago;
    }

    public IList<Pago> ObtenerLista(int nroDePagina = 1, int tamDePagina = 10)
        => ObtenerLista(null, null, nroDePagina, tamDePagina);

    public IList<Pago> ObtenerLista(int? reservaId, string? estado, int nroDePagina = 1, int tamDePagina = 10)
    {
        var lista = new List<Pago>();
        using var conexion = new MySqlConnection(ConnectionString);

        var query = """
            SELECT
                p.id,
                p.reserva_id,
                p.usuario_creacion_id,
                p.usuario_anulacion_id,
                p.concepto,
                p.fecha,
                p.importe,
                p.estado,
                r.inquilino_id AS reserva_inquilino_id,
                r.inmueble_id AS reserva_inmueble_id,
                r.fecha_desde AS reserva_fecha_desde,
                r.fecha_hasta AS reserva_fecha_hasta,
                r.monto_por_dia AS reserva_monto_por_dia,
                r.estado AS reserva_estado,
                iq.dni AS inquilino_dni,
                iq.nombre_completo AS inquilino_nombre,
                iq.email AS inquilino_email,
                iq.telefono AS inquilino_telefono,
                im.direccion AS inmueble_direccion,
                uc.nombre AS creador_nombre,
                uc.apellido AS creador_apellido,
                uc.email AS creador_email,
                ua.nombre AS anulador_nombre,
                ua.apellido AS anulador_apellido,
                ua.email AS anulador_email
            FROM
                PAGO p
                INNER JOIN RESERVA r ON p.reserva_id = r.id
                INNER JOIN INQUILINO iq ON r.inquilino_id = iq.id
                INNER JOIN INMUEBLE im ON r.inmueble_id = im.id
                INNER JOIN USUARIO uc ON p.usuario_creacion_id = uc.id
                LEFT JOIN USUARIO ua ON p.usuario_anulacion_id = ua.id
            WHERE
                1 = 1
        """;

        if (reservaId.HasValue)
            query += " AND p.reserva_id = @reservaId";
        if (!string.IsNullOrWhiteSpace(estado))
            query += " AND p.estado = @estado";

        query += """
            
            ORDER BY
                p.id DESC
            LIMIT
                @tamDePagina
            OFFSET
                @offset;
        """;

        using var comando = new MySqlCommand(query, conexion);
        if (reservaId.HasValue)
            comando.Parameters.AddWithValue("@reservaId", reservaId.Value);
        if (!string.IsNullOrWhiteSpace(estado))
            comando.Parameters.AddWithValue("@estado", estado);

        comando.Parameters.AddWithValue("@tamDePagina", tamDePagina);
        comando.Parameters.AddWithValue("@offset", (nroDePagina - 1) * tamDePagina);

        conexion.Open();
        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(MapearConJoins(reader));
        }

        return lista;
    }

    public Pago? ObtenerPorId(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                p.id,
                p.reserva_id,
                p.usuario_creacion_id,
                p.usuario_anulacion_id,
                p.concepto,
                p.fecha,
                p.importe,
                p.estado,
                r.inquilino_id AS reserva_inquilino_id,
                r.inmueble_id AS reserva_inmueble_id,
                r.fecha_desde AS reserva_fecha_desde,
                r.fecha_hasta AS reserva_fecha_hasta,
                r.monto_por_dia AS reserva_monto_por_dia,
                r.estado AS reserva_estado,
                iq.dni AS inquilino_dni,
                iq.nombre_completo AS inquilino_nombre,
                iq.email AS inquilino_email,
                iq.telefono AS inquilino_telefono,
                im.direccion AS inmueble_direccion,
                uc.nombre AS creador_nombre,
                uc.apellido AS creador_apellido,
                uc.email AS creador_email,
                ua.nombre AS anulador_nombre,
                ua.apellido AS anulador_apellido,
                ua.email AS anulador_email
            FROM
                PAGO p
                INNER JOIN RESERVA r ON p.reserva_id = r.id
                INNER JOIN INQUILINO iq ON r.inquilino_id = iq.id
                INNER JOIN INMUEBLE im ON r.inmueble_id = im.id
                INNER JOIN USUARIO uc ON p.usuario_creacion_id = uc.id
                LEFT JOIN USUARIO ua ON p.usuario_anulacion_id = ua.id
            WHERE
                p.id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);

        conexion.Open();
        using var reader = comando.ExecuteReader();
        return reader.Read() ? MapearConJoins(reader) : null;
    }

    public IList<Pago> ObtenerPorReserva(int reservaId, bool soloActivos = false)
    {
        var lista = new List<Pago>();
        using var conexion = new MySqlConnection(ConnectionString);

        var query = """
            SELECT
                p.id,
                p.reserva_id,
                p.usuario_creacion_id,
                p.usuario_anulacion_id,
                p.concepto,
                p.fecha,
                p.importe,
                p.estado,
                r.inquilino_id AS reserva_inquilino_id,
                r.inmueble_id AS reserva_inmueble_id,
                r.fecha_desde AS reserva_fecha_desde,
                r.fecha_hasta AS reserva_fecha_hasta,
                r.monto_por_dia AS reserva_monto_por_dia,
                r.estado AS reserva_estado,
                iq.dni AS inquilino_dni,
                iq.nombre_completo AS inquilino_nombre,
                iq.email AS inquilino_email,
                iq.telefono AS inquilino_telefono,
                im.direccion AS inmueble_direccion,
                uc.nombre AS creador_nombre,
                uc.apellido AS creador_apellido,
                uc.email AS creador_email,
                ua.nombre AS anulador_nombre,
                ua.apellido AS anulador_apellido,
                ua.email AS anulador_email
            FROM
                PAGO p
                INNER JOIN RESERVA r ON p.reserva_id = r.id
                INNER JOIN INQUILINO iq ON r.inquilino_id = iq.id
                INNER JOIN INMUEBLE im ON r.inmueble_id = im.id
                INNER JOIN USUARIO uc ON p.usuario_creacion_id = uc.id
                LEFT JOIN USUARIO ua ON p.usuario_anulacion_id = ua.id
            WHERE
                p.reserva_id = @reservaId
        """;

        if (soloActivos)
            query += " AND p.estado = 'Activo'";

        query += """
            
            ORDER BY
                p.id ASC;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@reservaId", reservaId);

        conexion.Open();
        using var reader = comando.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(MapearConJoins(reader));
        }

        return lista;
    }

    public decimal ObtenerTotalPagado(int reservaId)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                COALESCE(SUM(importe), 0)
            FROM
                PAGO
            WHERE
                reserva_id = @reservaId
                AND estado = 'Activo';
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@reservaId", reservaId);

        conexion.Open();
        return Convert.ToDecimal(comando.ExecuteScalar());
    }

    public int ObtenerCantidad() => ObtenerCantidad(null, null);

    public int ObtenerCantidad(int? reservaId, string? estado = null)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        var query = """
            SELECT
                COUNT(id)
            FROM
                PAGO
            WHERE
                1 = 1
        """;

        if (reservaId.HasValue)
            query += " AND reserva_id = @reservaId";
        if (!string.IsNullOrWhiteSpace(estado))
            query += " AND estado = @estado";

        query += ";";

        using var comando = new MySqlCommand(query, conexion);
        if (reservaId.HasValue)
            comando.Parameters.AddWithValue("@reservaId", reservaId.Value);
        if (!string.IsNullOrWhiteSpace(estado))
            comando.Parameters.AddWithValue("@estado", estado);

        conexion.Open();
        return Convert.ToInt32(comando.ExecuteScalar());
    }

    public int Alta(Pago pago) => Alta(pago, null, null);

    public int Alta(Pago pago, MySqlConnection? conexion, MySqlTransaction? transaccion)
    {
        var debeCerrar = false;
        if (conexion == null)
        {
            conexion = new MySqlConnection(ConnectionString);
            conexion.Open();
            debeCerrar = true;
        }

        try
        {
            const string query = """
                INSERT INTO
                    PAGO (
                        reserva_id,
                        usuario_creacion_id,
                        usuario_anulacion_id,
                        concepto,
                        fecha,
                        importe,
                        estado
                    )
                VALUES
                    (
                        @reserva_id,
                        @usuario_creacion_id,
                        @usuario_anulacion_id,
                        @concepto,
                        @fecha,
                        @importe,
                        @estado
                    );

                SELECT
                    LAST_INSERT_ID();
            """;

            using var comando = new MySqlCommand(query, conexion, transaccion);
            comando.Parameters.AddWithValue("@reserva_id", pago.ReservaId);
            comando.Parameters.AddWithValue("@usuario_creacion_id", pago.UsuarioCreacionId);
            comando.Parameters.AddWithValue("@usuario_anulacion_id", (object?)pago.UsuarioAnulacionId ?? DBNull.Value);
            comando.Parameters.AddWithValue("@concepto", pago.Concepto);
            comando.Parameters.AddWithValue("@fecha", pago.Fecha.ToDateTime(TimeOnly.MinValue));
            comando.Parameters.AddWithValue("@importe", pago.Importe);
            comando.Parameters.AddWithValue("@estado", pago.Estado.ToString());

            var nuevoId = Convert.ToInt32(comando.ExecuteScalar());
            pago.Id = nuevoId;
            return nuevoId;
        }
        finally
        {
            if (debeCerrar)
            {
                conexion.Dispose();
            }
        }
    }

    public int Modificacion(Pago pago) => ModificarConcepto(pago.Id, pago.Concepto);

    public int ModificarConcepto(int id, string nuevoConcepto)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE PAGO
            SET
                concepto = @concepto
            WHERE
                id = @id
                AND estado = 'Activo';
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);
        comando.Parameters.AddWithValue("@concepto", nuevoConcepto);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int Baja(int id) => Anular(id, 0);

    public int Anular(int id, int usuarioAnulacionId)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE PAGO
            SET
                estado = 'Anulado',
                usuario_anulacion_id = @usuario_anulacion_id
            WHERE
                id = @id
                AND estado = 'Activo';
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);
        comando.Parameters.AddWithValue("@usuario_anulacion_id", usuarioAnulacionId);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }
}
