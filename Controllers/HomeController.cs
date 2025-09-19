using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria_angel.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria_angel.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly string _conn = "Server=localhost;User=root;Password=;Database=inmobiliaria_angel_test;Port=3306";

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        List<UsuarioTest> usuariosTest = [];

        using (var conn = new MySqlConnection(_conn))
        {
            var query = $@"
                SELECT
                    id,
                    nombre,
                    fecha_registro
                FROM
                    usuarios;
            ";

            using (MySqlCommand command = new(query, conn))
            {
                conn.Open();

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    usuariosTest.Add(new UsuarioTest
                    {
                        Id = reader.GetInt32("id"),
                        Nombre = reader.GetString("nombre"),
                        FechaRegistro = reader.GetString("fecha_registro"),
                    });
                }

                conn.Close();
            }
        }

        return View(usuariosTest);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
