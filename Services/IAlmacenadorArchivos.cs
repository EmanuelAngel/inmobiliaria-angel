namespace inmobiliaria_lab2.Services;

/// <summary>
/// Abstracción para el almacenamiento y eliminación física de archivos en el sistema.
/// </summary>
public interface IAlmacenadorArchivos
{
    /// <summary>
    /// Guarda un archivo subido en el almacenamiento local dentro de wwwroot/uploads/{subcarpeta}.
    /// </summary>
    /// <param name="archivo">Archivo recibido vía multipart/form-data.</param>
    /// <param name="subcarpeta">Subcarpeta destino dentro de uploads (ej: "inmuebles/portadas", "inmuebles/galeria").</param>
    /// <returns>Ruta relativa web accesible (ej: "/uploads/inmuebles/portadas/{guid}.jpg").</returns>
    Task<string> GuardarArchivoAsync(IFormFile archivo, string subcarpeta);

    /// <summary>
    /// Elimina físicamente un archivo del disco de forma segura e idempotente.
    /// </summary>
    /// <param name="rutaRelativa">Ruta relativa web almacenada en BD (ej: "/uploads/inmuebles/...").</param>
    void BorrarArchivo(string? rutaRelativa);
}
