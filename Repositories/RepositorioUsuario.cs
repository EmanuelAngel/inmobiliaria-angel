using MySqlConnector;
using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

public class RepositorioUsuario(IConfiguration configuration) : RepositorioBase(configuration), IRepositorioUsuario
{
    public IList<Usuario> ObtenerLista(int nroDePagina = 1, int tamDePagina = 12)
        => ObtenerLista(null, nroDePagina, tamDePagina);

    public IList<Usuario> ObtenerLista(string? estado, int nroDePagina = 1, int tamDePagina = 12)
    {
        var lista = new List<Usuario>();
        var offset = (Math.Max(1, nroDePagina) - 1) * tamDePagina;

        using var conexion = new MySqlConnection(ConnectionString);

        var whereClause = string.Equals(estado, "inactivos", StringComparison.OrdinalIgnoreCase)
            ? "WHERE activo = 0"
            : string.Equals(estado, "todos", StringComparison.OrdinalIgnoreCase)
                ? ""
                : "WHERE activo = 1";

        var query = $"""
            SELECT
                id,
                nombre,
                apellido,
                email,
                password_hash,
                avatar,
                rol,
                activo
            FROM
                USUARIO
            {whereClause}
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

    public IList<Usuario> Buscar(string q, int limite = 20)
    {
        var lista = new List<Usuario>();
        limite = Math.Clamp(limite, 1, 50);

        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            SELECT
                id,
                nombre,
                apellido,
                email,
                password_hash,
                avatar,
                rol,
                activo
            FROM
                USUARIO
            WHERE
                activo = 1
                AND (
                    nombre LIKE @patron
                    OR apellido LIKE @patron
                    OR email LIKE @patron
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

    public Usuario? ObtenerPorId(int id)
        => ObtenerPorId(id, true);

    public Usuario? ObtenerPorId(int id, bool soloActivos)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        var query = soloActivos
            ? """
                SELECT
                    id,
                    nombre,
                    apellido,
                    email,
                    password_hash,
                    avatar,
                    rol,
                    activo
                FROM
                    USUARIO
                WHERE
                    id = @id
                    AND activo = 1;
            """
            : """
                SELECT
                    id,
                    nombre,
                    apellido,
                    email,
                    password_hash,
                    avatar,
                    rol,
                    activo
                FROM
                    USUARIO
                WHERE
                    id = @id;
            """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);

        conexion.Open();

        using var reader = comando.ExecuteReader();
        return reader.Read() ? Mapear(reader) : null;
    }

    public Usuario? ObtenerPorEmail(string email)
        => ObtenerPorEmail(email, true);

    public Usuario? ObtenerPorEmail(string email, bool soloActivos = true)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        var query = soloActivos
            ? """
                SELECT
                    id,
                    nombre,
                    apellido,
                    email,
                    password_hash,
                    avatar,
                    rol,
                    activo
                FROM
                    USUARIO
                WHERE
                    email = @email
                    AND activo = 1;
            """
            : """
                SELECT
                    id,
                    nombre,
                    apellido,
                    email,
                    password_hash,
                    avatar,
                    rol,
                    activo
                FROM
                    USUARIO
                WHERE
                    email = @email;
            """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@email", email.Trim());

        conexion.Open();

        using var reader = comando.ExecuteReader();
        return reader.Read() ? Mapear(reader) : null;
    }

    public int Alta(Usuario usuario)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            INSERT INTO
                USUARIO (
                    nombre,
                    apellido,
                    email,
                    password_hash,
                    avatar,
                    rol,
                    activo
                )
            VALUES
                (
                    @nombre,
                    @apellido,
                    @email,
                    @password_hash,
                    @avatar,
                    @rol,
                    @activo
                );

            SELECT
                LAST_INSERT_ID();
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@nombre", usuario.Nombre.Trim());
        comando.Parameters.AddWithValue("@apellido", usuario.Apellido.Trim());
        comando.Parameters.AddWithValue("@email", usuario.Email.Trim());
        comando.Parameters.AddWithValue("@password_hash", usuario.PasswordHash);
        comando.Parameters.AddWithValue("@avatar", (object?)usuario.Avatar ?? DBNull.Value);
        comando.Parameters.AddWithValue("@rol", usuario.Rol.ToString());
        comando.Parameters.AddWithValue("@activo", usuario.Activo ? 1 : 0);

        conexion.Open();

        var resultado = comando.ExecuteScalar();
        if (resultado is not null and not DBNull)
        {
            usuario.Id = Convert.ToInt32(resultado);
            return usuario.Id;
        }

        return 0;
    }

    public int Modificacion(Usuario usuario)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE
                USUARIO
            SET
                nombre = @nombre,
                apellido = @apellido,
                email = @email,
                avatar = @avatar,
                rol = @rol
            WHERE
                id = @id
                AND activo = 1;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", usuario.Id);
        comando.Parameters.AddWithValue("@nombre", usuario.Nombre.Trim());
        comando.Parameters.AddWithValue("@apellido", usuario.Apellido.Trim());
        comando.Parameters.AddWithValue("@email", usuario.Email.Trim());
        comando.Parameters.AddWithValue("@avatar", (object?)usuario.Avatar ?? DBNull.Value);
        comando.Parameters.AddWithValue("@rol", usuario.Rol.ToString());

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int ActualizarPassword(int id, string passwordHash)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE
                USUARIO
            SET
                password_hash = @password_hash
            WHERE
                id = @id
                AND activo = 1;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);
        comando.Parameters.AddWithValue("@password_hash", passwordHash);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int ActualizarAvatar(int id, string? avatar)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE
                USUARIO
            SET
                avatar = @avatar
            WHERE
                id = @id
                AND activo = 1;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);
        comando.Parameters.AddWithValue("@avatar", (object?)avatar ?? DBNull.Value);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int Baja(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE
                USUARIO
            SET
                activo = 0
            WHERE
                id = @id
                AND activo = 1;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int Activar(int id)
    {
        using var conexion = new MySqlConnection(ConnectionString);
        const string query = """
            UPDATE
                USUARIO
            SET
                activo = 1
            WHERE
                id = @id;
        """;

        using var comando = new MySqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@id", id);

        conexion.Open();
        return comando.ExecuteNonQuery();
    }

    public int ObtenerCantidad()
        => ObtenerCantidad(null);

    public int ObtenerCantidad(string? estado)
    {
        using var conexion = new MySqlConnection(ConnectionString);

        var whereClause = string.Equals(estado, "inactivos", StringComparison.OrdinalIgnoreCase)
            ? "WHERE activo = 0"
            : string.Equals(estado, "todos", StringComparison.OrdinalIgnoreCase)
                ? ""
                : "WHERE activo = 1";

        var query = $"""
            SELECT
                COUNT(id)
            FROM
                USUARIO
            {whereClause};
        """;

        using var comando = new MySqlCommand(query, conexion);
        conexion.Open();
        return Convert.ToInt32(comando.ExecuteScalar());
    }

    private static Usuario Mapear(MySqlDataReader reader)
    {
        var rolStr = reader.GetString(reader.GetOrdinal("rol"));
        if (!Enum.TryParse<RolUsuario>(rolStr, true, out var rol))
        {
            rol = RolUsuario.Empleado;
        }

        return new Usuario
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
            Apellido = reader.GetString(reader.GetOrdinal("apellido")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
            Avatar = reader.IsDBNull(reader.GetOrdinal("avatar")) ? null : reader.GetString(reader.GetOrdinal("avatar")),
            Rol = rol,
            Activo = reader.GetBoolean(reader.GetOrdinal("activo"))
        };
    }
}

