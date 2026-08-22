# Captura de Requisitos — Índice General

> **Fuente de verdad:** [narrativa.md](../narrativa.md)  
> **Terminología canónica:** ver [contexto-proyecto.md](../contexto-proyecto.md)  
> **Ítems numerados de la narrativa:** 1–23 (funcionalidades), 24–31 (informes), 32–34 (otros requerimientos)

---

## Requerimientos no funcionales (de la narrativa)

| # | Requerimiento |
|---|---------------|
| 32 | Paginado por servidor en todos los listados |
| 33 | Búsquedas resueltas en el servidor |
| 34 | Dropdowns con búsqueda server-side (no traer todos los valores) |

---

## Casos de Uso

### [Entidades Base](./casos-de-uso-entidades.md)

| ID | Caso de uso | Actor |
|----|-------------|-------|
| CU-P01 | Listar propietarios | Empleado, Admin |
| CU-P02 | Alta de propietario | Empleado, Admin |
| CU-P03 | Edición de propietario | Empleado, Admin |
| CU-P04 | Eliminar propietario | Admin |
| CU-P05 | Ver detalle de propietario | Empleado, Admin |
| CU-I01 | Listar inmuebles | Empleado, Admin |
| CU-I02 | Alta de inmueble | Empleado, Admin |
| CU-I03 | Edición de inmueble | Empleado, Admin |
| CU-I04 | Eliminar inmueble | Admin |
| CU-I05 | Suspender / reactivar inmueble | Empleado, Admin |
| CU-I06 | Ver inmuebles de un propietario | Empleado, Admin |
| CU-I07 | Buscar inmuebles disponibles por fechas | Empleado, Admin |
| CU-TI01 | Listar tipos de inmueble | Empleado, Admin |
| CU-TI02 | Alta de tipo | Admin |
| CU-TI03 | Edición de tipo | Admin |
| CU-TI04 | Eliminar tipo | Admin |
| CU-IN01 | Listar inquilinos | Empleado, Admin |
| CU-IN02 | Alta de inquilino | Empleado, Admin |
| CU-IN03 | Edición de inquilino | Empleado, Admin |
| CU-IN04 | Eliminar inquilino | Admin |
| CU-U01 | Iniciar sesión | Anónimo |
| CU-U02 | Cerrar sesión | Empleado, Admin |
| CU-U03 | Editar perfil propio | Empleado, Admin |
| CU-U04 | Listar usuarios | Admin |
| CU-U05 | Alta de usuario | Admin |
| CU-U06 | Editar usuario | Admin |
| CU-U07 | Eliminar usuario | Admin |

### [Reservas y Pagos](./casos-de-uso-reservas.md)

| ID | Caso de uso | Actor |
|----|-------------|-------|
| CU-R01 | Listar reservas vigentes | Empleado, Admin |
| CU-R02 | Listar reservas por vencimiento | Empleado, Admin |
| CU-R03 | Alta de reserva | Empleado, Admin |
| CU-R04 | Ver detalle de reserva | Empleado, Admin |
| CU-R05 | Cancelar reserva anticipadamente | Empleado, Admin |
| CU-R06 | Extender / renovar reserva | Empleado, Admin |
| CU-PA01 | Listar pagos de una reserva | Empleado, Admin |
| CU-PA02 | Alta de pago | Empleado, Admin |
| CU-PA03 | Editar pago | Empleado, Admin |
| CU-PA04 | Anular pago | Admin |

### [Informes](./casos-de-uso-informes.md)

| ID | Caso de uso | Actor | Nuevo CU |
|----|-------------|-------|----------|
| CU-INF01 | Listar inmuebles con propietario y filtro de disponibilidad | Empleado, Admin | — ver CU-I01 |
| CU-INF02 | Listar inmuebles de un propietario específico | Empleado, Admin | — ver CU-I06 |
| CU-INF03 | Inmuebles más reservados (últimos 365 días) | Empleado, Admin | ✅ |
| CU-INF04 | Inmuebles sin reservas en últimos X días | Empleado, Admin | ✅ |
| CU-INF05 | Listar inmuebles disponibles entre dos fechas | Empleado, Admin | — ver CU-I07 |
| CU-INF06 | Listar reservas vigentes | Empleado, Admin | — ver CU-R01 |
| CU-INF07 | Listar reservas que vencen en X días | Empleado, Admin | — ver CU-R02 |
| CU-INF08 | Listar pagos de una reserva + cargar nuevo pago | Empleado, Admin | — ver CU-PA01, CU-PA02 |

---

## Puntos abiertos ⚠️

| CU | Pregunta |
|----|----------|
| CU-P04 | ¿Baja lógica o física para propietarios? |
| CU-U06 | ¿Editar usuario incluye cambio de rol? |
| CU-R03 | ¿El pago de seña se genera automáticamente al crear la reserva o se carga a mano? |
| CU-PA04 | ¿Solo Admin puede anular pagos? Confirmar con docente |

---

## Estado general

- [x] Entidades relevadas
- [x] Reservas y Pagos relevados
- [x] Informes relevados
- [ ] Puntos abiertos resueltos
- [ ] Detalle de cada CU redactado
