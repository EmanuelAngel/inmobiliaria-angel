namespace inmobiliaria_lab2.Services;

/// <summary>
/// Implementación local de almacenamiento de archivos dentro del directorio wwwroot/uploads.
/// </summary>
public class AlmacenadorArchivosLocal(IWebHostEnvironment webHostEnvironment) : IAlmacenadorArchivos
{
    private static readonly HashSet<string> ExtensionesPermitidas = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp",
        ".gif"
    };

    private const long TamanoMaximoBytes = 5 * 1024 * 1024; // 5 MB

    public async Task<string> GuardarArchivoAsync(IFormFile archivo, string subcarpeta)
    {
        ArgumentNullException.ThrowIfNull(archivo);

        if (archivo.Length == 0)
        {
            throw new InvalidOperationException("El archivo está vacío.");
        }

        if (archivo.Length > TamanoMaximoBytes)
        {
            throw new InvalidOperationException("El archivo supera el tamaño máximo permitido de 5 MB.");
        }

        var extension = Path.GetExtension(archivo.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !ExtensionesPermitidas.Contains(extension))
        {
            throw new InvalidOperationException($"La extensión '{extension}' no está permitida. Formatos válidos: JPG, JPEG, PNG, WEBP, GIF.");
        }

        var subcarpetaLimpia = subcarpeta.Trim('/', '\\').Replace('/', Path.DirectorySeparatorChar);
        var uploadsPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads", subcarpetaLimpia);

        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var nombreArchivo = $"{Guid.NewGuid()}{extension.ToLowerInvariant()}";
        var rutaFisica = Path.Combine(uploadsPath, nombreArchivo);

        await using (var stream = new FileStream(rutaFisica, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        var subcarpetaWeb = subcarpetaLimpia.Replace('\\', '/');
        return $"/uploads/{subcarpetaWeb}/{nombreArchivo}";
    }

    public void BorrarArchivo(string? rutaRelativa)
    {
        if (string.IsNullOrWhiteSpace(rutaRelativa))
        {
            return;
        }

        try
        {
            var rutaNormalizada = rutaRelativa.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            var rutaCompleta = Path.GetFullPath(Path.Combine(webHostEnvironment.WebRootPath, rutaNormalizada));

            // Protección contra Path Traversal: verificar que el archivo esté estrictamente dentro de WebRootPath
            var webRootFullPath = Path.GetFullPath(webHostEnvironment.WebRootPath);
            if (!rutaCompleta.StartsWith(webRootFullPath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (File.Exists(rutaCompleta))
            {
                File.Delete(rutaCompleta);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            // Silencioso e idempotente para evitar que una falla de I/O bloquee transacciones
        }
    }
}
