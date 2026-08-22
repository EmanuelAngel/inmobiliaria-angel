# Contexto del Proyecto para LLMs y Colaboradores

Este archivo concentra el contexto técnico y de dominio necesario para trabajar en el proyecto sin ambigüedades.

---

## Stack Tecnológico

| Capa | Tecnología |
|---|---|
| Framework web | ASP.NET Core MVC (.NET 10) |
| Vistas | Razor Views |
| Base de datos | MySQL |
| Acceso a datos | Repository Pattern con ADO.NET puro (sin ORM) |
| Lenguaje | C# |

---

## Nomenclatura Canónica del Dominio

> **Regla absoluta:** la [narrativa del proyecto](./narrativa.md) es la fuente de verdad. Cualquier documento de entrega (especialmente `entregas-y-revisiones/entrega-final.md`) puede usar terminología del año anterior y NO debe tomarse como referencia para nombrar entidades o funcionalidades.

| Término canónico | NO usar |
|---|---|
| **Reserva** | ~~Contrato~~ |
| **Inmueble** | ~~Propiedad~~ |
| **Inquilino** | — |
| **Propietario** | — |
| **Pago** | — |
| **Usuario** | — |

---

## Arquitectura

- **Patrón:** Repository Pattern
- **Acceso a datos:** ADO.NET puro — `SqlConnection` / `MySqlConnection`, `SqlCommand`, `SqlDataReader`
- Sin ORM: ni Entity Framework Core ni Dapper. Las queries son SQL explícito escrito a mano.
- Cada repositorio recibe la cadena de conexión vía inyección de dependencias (o desde `IConfiguration`).

---

## Estructura del Proyecto

```
inmobiliaria-lab2/
├── Controllers/        # Controladores MVC
├── Models/             # Modelos de dominio y ViewModels
├── Views/              # Vistas Razor organizadas por controlador
├── wwwroot/            # Recursos estáticos (CSS, JS, imágenes)
├── appsettings.json    # Configuración (cadena de conexión MySQL, etc.)
└── Program.cs          # Entry point y configuración de servicios
```

> La carpeta `docs/` es personal, no se sube al repositorio.

---

## Estado del Proyecto
 
- **Modalidad:** Individual (Emanuel Angel).
- **Convenciones de código:** Definidas (ver [AGENTS.md](../AGENTS.md)).
- **Script SQL / DER:** Completado e inicializado.
- **Primera entrega:** Realizada (ABM de Propietarios e Inquilinos).
- **Próximo hito:** Segunda entrega (ABM Inmuebles y Reservas).

---

## Reglas de Negocio Clave (resumen rápido)

Para el detalle completo, leer la [narrativa](./narrativa.md). Puntos no obvios:

- Un inmueble puede ser **suspendido** por el propietario: deja de aparecer en listados pero no afecta reservas vigentes.
- Las **reservas no se superponen**: debe validarse al crear y al editar.
- **Cancelación anticipada:** si se cumplió menos de la mitad del tiempo, multa = 50% del restante. Caso contrario, 25%. El pago de la multa se registra en la misma pantalla y es obligatorio para finalizar.
- **Extensión de reserva:** se crea una nueva reserva (no se modifica la original), con mismo inquilino e inmueble.
- **Pagos:** solo se puede editar el concepto, no el monto ni la fecha. La eliminación es cambio de estado (anulado), no borrado físico.
- **Auditoría:** quién creó/terminó una reserva y quién creó/anuló un pago. Solo visible para administradores en vista de detalle.
- **Roles:** `Administrador` (puede eliminar entidades y gestionar usuarios) y `Empleado` (solo gestiona su propio perfil).
- **Listados:** paginado por servidor. Dropdowns con búsqueda server-side (no traer todos los valores).
