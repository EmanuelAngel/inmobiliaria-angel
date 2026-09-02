# Contexto del Proyecto para LLMs y Agentes de IA

Este archivo concentra el contexto técnico, arquitectónico y de dominio necesario para trabajar en el proyecto sin ambigüedades.

---

## Stack Tecnológico

| Capa | Tecnología |
|---|---|
| Framework web | ASP.NET Core MVC (.NET 10) |
| Vistas | Razor Views |
| Base de datos | MySQL 8.0.46 |
| Acceso a datos | Repository Pattern con ADO.NET puro (`MySqlConnector`, sin ORM) |
| Lenguaje | C# |

---

## Repositorio de Referencia (Cátedra)

- **URL:** [marianoluzza/inmobiliariaULP](https://github.com/marianoluzza/inmobiliariaULP)
- **Aclaración:** Repositorio de ejemplo provisto por el profesor (con actualizaciones a lo largo del tiempo). **No está completo** ni define la arquitectura definitiva del proyecto; debe tomarse únicamente como material de consulta y guía de referencia.

---

## Nomenclatura Canónica del Dominio

> **Regla absoluta:** La [narrativa del proyecto](docs/narrativa.md) es la fuente de verdad. Documentos de entregas previas pueden contener terminología obsoleta y NO deben tomarse como referencia para nombrar entidades o funcionalidades.

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

- **Patrón:** Repository Pattern (interfaces en `Models/` o `Repositories/`, e.g. `IRepositorio<T>`).
- **Acceso a datos:** ADO.NET puro (`MySqlConnection`, `MySqlCommand`, `MySqlDataReader`).
- **Sin ORM:** No usar Entity Framework Core ni Dapper. Las consultas SQL se escriben explícitamente a mano parametrizadas para evitar SQL Injection.
- **Inyección de Dependencias:** Cada repositorio recibe `IConfiguration` o la cadena de conexión por DI en [Program.cs](Program.cs).

---

## Convenciones de Modelos

### Patrón FK + Propiedad de navegación

La FK (`int`) es **siempre obligatoria**. La propiedad de navegación (`Entidad?`) se agrega **solo cuando una vista o lógica concreta la necesita**.

| Propiedad | Tipo | Garantía | Uso |
|-----------|------|----------|-----|
| `PropietarioId`, `TipoId` | `int` (requerido) | Siempre poblada | Persistencia (INSERT/UPDATE), filtros sin JOIN |
| `Propietario?`, `Tipo?` | objeto nullable | Solo con JOIN explícito | Vistas, lógica de presentación — agregar solo si hay uso real |

**No anticipar relaciones.** Si la pantalla solo necesita mostrar un nombre, usar un ViewModel con `string` es suficiente. La navigation property se incorpora al modelo cuando el controlador la necesita para armar la respuesta.

**Regla para el repositorio:** cada repositorio expone dos mappers privados: `MapearBase` (puebla solo los campos propios de la entidad, siempre incluye las FKs) y `MapearConJoins` (llama a `MapearBase` y agrega las propiedades de navegación). Los métodos de lectura eligen cuál usar según si su `SELECT` incluye JOINs o no.

**Regla para el controlador/vista:** nunca asumir que la propiedad de navegación está poblada. Si la vista la necesita, el controlador debe usar el método de repositorio que llama a `MapearConJoins`.

---

## Estructura del Proyecto

```
inmobiliaria-lab2/
├── Controllers/        # Controladores MVC
├── Models/             # Modelos de dominio y ViewModels
├── Repositories/       # Interfaces e implementaciones de acceso a datos (ADO.NET)
├── Views/              # Vistas Razor organizadas por controlador
├── wwwroot/            # Recursos estáticos (CSS, JS, librerías frontend)
├── docs/               # Documentación interna y narrativa
├── database.sql        # Script de inicialización y datos semilla
├── appsettings.json    # Configuración y ConnectionStrings
├── AGENTS.md           # Contexto para asistentes y agentes
└── Program.cs          # Entry point y configuración de servicios / DI
```

---

## Estado del Proyecto

- **Modalidad:** Individual (Emanuel Angel).
- **Base de datos / Script SQL:** Inicializado ([database.sql](database.sql)).
- **Documentación / README:** Creado con DER e instrucciones de despliegue.
- **Primera entrega:** Realizada (ABM de Propietarios e Inquilinos).
- **En curso / Próximo hito:** [Segunda entrega](docs/entregas-y-revisiones/segunda-entrega.md) — ABM Inmuebles y Reservas.

---

## Reglas de Negocio Clave

1. **Suspensión de Inmuebles:** Un propietario puede suspender la oferta de su inmueble (no aparece en listados pero no afecta reservas activas).
2. **Superposición de Reservas:** Las reservas NO pueden solaparse en fechas para el mismo inmueble; debe validarse al crear y editar.
3. **Cancelación Anticipada y Multas:**
   - Si transcurrió menos del 50% del tiempo: multa = 50% del saldo restante.
   - Si transcurrió 50% o más: multa = 25% del saldo restante.
   - El pago de la multa debe registrarse en la misma pantalla para hacer efectiva la finalización.
4. **Extensión de Reserva:** Genera una *nueva* reserva vinculada al mismo inquilino e inmueble (no muta la original).
5. **Pagos:** Al editar un pago solo se permite modificar el concepto. La eliminación es lógica (cambio de estado a *anulado*).
6. **Auditoría:** Se registra usuario creador/terminador de reservas y creador/anulador de pagos (visible solo para administradores en vista de detalle).
7. **Seguridad y Roles:**
   - `Administrador`: gestión total, gestión de usuarios y puede ejecutar bajas sobre cualquier entidad.
   - `Empleado`: gestión operativa y edición de su propio perfil.
8. **Bajas — convención universal:** Todas las entidades usan **baja lógica** (`activo TINYINT(1) DEFAULT 1`). Nunca se usa `DELETE` físico. El método `Baja(int id)` en todos los repositorios hace `UPDATE SET activo = 0`. Todas las queries de listado, búsqueda por id, por dni y por email filtran `AND activo = 1`. La funcionalidad de reactivación (`Activar`) queda pendiente hasta implementar el sistema de roles.
9. **Consultas y Listados:** Paginación resuelta en servidor y dropdowns con búsqueda asíncrona/server-side.

---

## Convención de Commits

- **Estándar:** Conventional Commits (`tipo(scope): descripción`).
- **Idioma:** Español para el tipo de cambio, scope y descripción (orientado a la revisión entre compañeros).
- **Regla:** Verbo en infinitivo (`agregar`, `crear`, `modificar`, `corregir`), todo en minúsculas y sin punto final.
- **Ejemplo:**
  ```bash
  git commit -m "feat(repo): agregar interfaz de repositorio base genérico"
  ```
