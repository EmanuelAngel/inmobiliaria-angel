# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

Personal administrativo y operativo de la agencia inmobiliaria:
- **Empleados:** Operadores de mostrador que gestionan la atención a inquilinos y propietarios, consultan disponibilidad de inmuebles, registran contratos de reserva, cargan pagos y aplican cancelaciones anticipadas con multas.
- **Administradores:** Encargados de supervisión de la agencia con facultades exclusivas de auditoría, gestión de usuarios/roles y ejecución de bajas del sistema.

## Product Purpose

Sistema integral de gestión de alquileres temporarios para "Inmobiliaria Angel". Facilita el ciclo completo de negocio: oferta y catalogación de inmuebles por propietario, búsqueda por disponibilidad de fechas sin solapamiento, registro de reservas, cobranza escalonada (señas y liquidaciones), renovación/extensión, y penalizaciones por rescisión anticipada según el tiempo transcurrido.

## Positioning

Herramienta de operación interna y mostrador (back-office) de alta confiabilidad. A diferencia de portales públicos de anuncios, prioriza la precisión transaccional estricta: validación estricta de no solapamiento temporal de reservas, cálculo algorítmico de multas (50% o 25% del saldo según si transcurrió menos o más de la mitad del plazo) con registro forzoso del pago para efectivizar la cancelación, y trazabilidad total con auditoría por rol.

## Operating Context

- Entorno de trabajo: Mostrador de atención y oficinas administrativas, utilizado en jornadas completas de escritorio en navegadores de escritorio.
- Modo de interacción predominante: **Operate** (orientado a la productividad, escaneo rápido de listados densos, carga veloz de formularios transaccionales y precisión numérica).
- Tareas repetitivas: Cruce de fechas de reservas, emisión y anulación de recibos de pago, y altas de inquilinos durante la entrevista presencial o telefónica.

## Capabilities and Constraints

- **Stack innegociable:** ASP.NET Core MVC (.NET 10), Razor Views, MySQL 8, ADO.NET puro (`MySqlConnector`, sin ORM como EF Core o Dapper), y Bootstrap 5 para el diseño de interfaces.
- **Nomenclatura canónica de dominio:** Inmueble (no "propiedad"), Reserva (no "contrato"), Inquilino, Propietario, Pago, Usuario.
- **Regla de bajas:** Baja lógica universal (`activo = 1/0`). Nunca se utiliza `DELETE` físico.
- **Reglas de negocio clave:**
  - Inmuebles: Suspensión temporal de oferta sin alterar reservas activas; cupo, fotos y precio por día.
  - Reservas: Validación estricta anti-solapamiento de fechas por inmueble.
  - Pagos: Edición restringida exclusivamente al concepto (monto y fecha inmutables); eliminación lógica como anulación.
  - Cancelación anticipada: Si transcurrió < 50% del plazo, multa del 50% del saldo; si transcurrió >= 50%, multa del 25%. El pago de la multa debe cargarse y cancelarse en la misma pantalla para efectivizar la terminación.
  - Auditoría: Registro de usuario creador/terminador de reservas y creador/anulador de pagos visible únicamente para administradores en la vista de detalle.

## Brand Commitments

- **Identidad:** "Inmobiliaria Angel".
- **Tono y personalidad visual:** Profesional, sobria, confiable, proptech moderna. Debe distanciarse de la plantilla genérica azul de Bootstrap sin alterar la compatibilidad con las clases estándar de Bootstrap 5.
- **Soporte de temas:** Soporte fluido tanto para modo claro (Light) como para modo oscuro (Dark), preservando la legibilidad de controles y componentes Select2.

## Evidence on Hand

- Documentación exhaustiva en `docs/narrativa.md`, `docs/captura-de-requisitos/`, y `AGENTS.md`.
- Base de datos relacional modelada con script de inicialización y datos semilla en `database.sql`.
- Vistas Razor completas de gestión para Propietarios, Inquilinos, Inmuebles, Tipos de Inmueble y Reservas.

## Product Principles

1. **Precisión transaccional sobre adorno:** La interfaz debe facilitar la verificación de fechas, cálculos de multas y saldos sin ambigüedades ni distracciones.
2. **Claridad y escaneabilidad densa:** Tablas legibles, alineación numérica tabular para importes e IDs, y estados visuales claros (disponible, suspendido, anulado, completado).
3. **Robustez de mostrador:** Flujos rápidos para carga de datos en vivo durante la atención de clientes, minimizando clics y evitando bloqueos innecesarios.
4. **Respeto a los roles:** Separación estricta de affordances entre empleados y administradores, ocultando datos de auditoría sensible a operadores no autorizados.

## Accessibility & Inclusion

- Contraste visual estricto (mínimo 4.5:1 para texto normal, 3:1 para texto grande) tanto en modo claro como en modo oscuro.
- Foco visible accesible (`:focus-visible`) para navegación por teclado en formularios densos.
- Diferenciación de estados no dependiente únicamente del color (combinación de insignias con texto descriptivo e iconos).
