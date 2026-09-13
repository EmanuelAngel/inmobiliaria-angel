using inmobiliaria_lab2.Models;

namespace inmobiliaria_lab2.Repositories;

/// <summary>
/// Interfaz para el acceso a datos de las imágenes de galería de inmuebles.
/// </summary>
public interface IRepositorioImagenInmueble
{
    /// <summary>
    /// Obtiene todas las imágenes asociadas a un inmueble específico.
    /// </summary>
    IList<ImagenInmueble> ObtenerPorInmueble(int inmuebleId);

    /// <summary>
    /// Obtiene una imagen por su identificador primario.
    /// </summary>
    ImagenInmueble? ObtenerPorId(int id);

    /// <summary>
    /// Inserta una nueva imagen en la base de datos y retorna su identificador autogenerado.
    /// </summary>
    int Alta(ImagenInmueble imagen);

    /// <summary>
    /// Elimina físicamente el registro de la imagen de la base de datos.
    /// </summary>
    int Borrar(int id);
}
