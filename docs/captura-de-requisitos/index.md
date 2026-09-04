# Captura de Requisitos — Índice General

> **Fuente de verdad:** [`../narrativa.md`](../narrativa.md)  
> **Terminología canónica y convenciones:** ver [`../../AGENTS.md`](../../AGENTS.md)  
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

### [Entidades Base](./entidades/index.md)

| ID | Caso de uso | Actor | Detalle / Diagrama |
|----|-------------|-------|--------------------|
| [CU-P01](./entidades/CU-P01-listar-propietarios.md) | **Listar propietarios** | Empleado, Admin | 📄 [Especificación y Secuencia](./entidades/CU-P01-listar-propietarios.md) |
| [CU-P02](./entidades/CU-P02-alta-propietario.md) | **Alta de propietario** | Empleado, Admin | 📄 [Especificación y Secuencia](./entidades/CU-P02-alta-propietario.md) |
| CU-P03 | Edición de propietario | Empleado, Admin | — |
| [CU-P04](./entidades/CU-P04-eliminar-propietario.md) | **Eliminar propietario** | Admin | 📄 [Especificación y Secuencia](./entidades/CU-P04-eliminar-propietario.md) |
| CU-P05 | Ver detalle de propietario | Empleado, Admin | — |
| CU-I01 | Listar inmuebles | Empleado, Admin | — |
| CU-I02 | Alta de inmueble | Empleado, Admin | — |
| CU-I03 | Edición de inmueble | Empleado, Admin | — |
| CU-I04 | Eliminar inmueble | Admin | — |
| CU-I05 | Suspender / reactivar inmueble | Empleado, Admin | — |
| CU-I06 | Ver inmuebles de un propietario | Empleado, Admin | — |
| CU-I07 | Buscar inmuebles disponibles por fechas | Empleado, Admin | — |
| CU-TI01 | Listar tipos de inmueble | Empleado, Admin | — |
| CU-TI02 | Alta de tipo | Admin | — |
| CU-TI03 | Edición de tipo | Admin | — |
| CU-TI04 | Eliminar tipo | Admin | — |
| CU-IN01 | Listar inquilinos | Empleado, Admin | — |
| CU-IN02 | Alta de inquilino | Empleado, Admin | — |
| CU-IN03 | Edición de inquilino | Empleado, Admin | — |
| CU-IN04 | Eliminar inquilino | Admin | — |
| CU-U01 | Iniciar sesión | Anónimo | — |
| CU-U02 | Cerrar sesión | Empleado, Admin | — |
| CU-U03 | Editar perfil propio | Empleado, Admin | — |
| CU-U04 | Listar usuarios | Admin | — |
| CU-U05 | Alta de usuario | Admin | — |
| CU-U06 | Editar usuario | Admin | — |
| CU-U07 | Eliminar usuario | Admin | — |

### [Reservas y Pagos](./reservas/index.md)

| ID | Caso de uso | Actor | Detalle / Diagrama |
|----|-------------|-------|--------------------|
| CU-R01 | Listar reservas vigentes | Empleado, Admin | — |
| CU-R02 | Listar reservas por vencimiento | Empleado, Admin | — |
| [CU-R03](./reservas/CU-R03-alta-reserva.md) | **Alta de reserva** | Empleado, Admin | 📄 [Especificación y Secuencia](./reservas/CU-R03-alta-reserva.md) |
| CU-R04 | Ver detalle de reserva | Empleado, Admin | — |
| [CU-R05](./reservas/CU-R05-cancelar-reserva.md) | **Cancelar reserva anticipadamente** | Empleado, Admin | 📄 [Especificación y Secuencia](./reservas/CU-R05-cancelar-reserva.md) |
| [CU-R06](./reservas/CU-R06-extender-reserva.md) | **Extender / renovar reserva** | Empleado, Admin | 📄 [Especificación y Secuencia](./reservas/CU-R06-extender-reserva.md) |
| CU-PA01 | Listar pagos de una reserva | Empleado, Admin | — |
| CU-PA02 | Alta de pago | Empleado, Admin | — |
| CU-PA03 | Editar pago | Empleado, Admin | — |
| CU-PA04 | Anular pago | Admin | — |

### [Informes](./informes/index.md)

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

## Puntos abiertos y convenciones fijadas

| CU | Punto | Definición / Regla |
|----|-------|--------------------|
| CU-P04 | Bajas de entidades | **Baja lógica universal** (`activo = 0`). Nunca `DELETE` físico. |
| CU-U06 | Edición de usuario | Incluye cambio de rol y estado. |
| CU-R03 | Pago de seña en alta | Configurable en el formulario de alta según porcentaje del inmueble. |
| CU-PA04 | Anulación de pagos | Baja lógica (`activo = 0`), restringido a Admin. |

---

## Estado general

- [x] Entidades relevadas
- [x] Reservas y Pagos relevados
- [x] Informes relevados
- [x] Puntos abiertos resueltos según convención de dominio
- [x] Estructura modular organizada por carpetas
- [x] Casos de uso de Propietarios documentados con código real (CU-P01, CU-P02, CU-P04)
- [x] Casos de uso complejos documentados con especificación y diagramas de secuencia (CU-R03, CU-R05, CU-R06)
