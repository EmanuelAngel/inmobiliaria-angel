namespace inmobiliaria_lab2.Models;

public class PaginacionViewModel
{
    public int PaginaActual { get; set; } = 1;
    public int TotalPaginas { get; set; } = 1;
    public int TamDePagina { get; set; } = 10;
    public int TotalRegistros { get; set; } = 0;
    public string Accion { get; set; } = "Index";
    public string? Controlador { get; set; }
    public Dictionary<string, string> ValoresRuta { get; set; } = new();

    public Dictionary<string, string> ObtenerRuta(int pagina)
    {
        var ruta = new Dictionary<string, string>(ValoresRuta)
        {
            ["pagina"] = pagina.ToString(),
            ["tamDePagina"] = TamDePagina.ToString()
        };
        return ruta;
    }
}
