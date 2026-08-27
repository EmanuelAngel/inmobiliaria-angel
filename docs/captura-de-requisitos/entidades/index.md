# Casos de Uso — Entidades Base

> **Fuente de verdad:** [`../../narrativa.md`](../../narrativa.md)  
> **Terminología canónica:** ver [`../../AGENTS.md`](../../AGENTS.md)

---

## Propietario

```mermaid
flowchart LR
    Emp([Empleado])
    Adm([Administrador])
    Adm -.-|hereda| Emp

    subgraph sgP[Propietario]
        P01((Listar))
        P02((Alta))
        P03((Editar))
        P04((Eliminar))
        P05((Ver detalle))
    end

    Emp --> P01
    Emp --> P02
    Emp --> P03
    Emp --> P05
    Adm --> P04
```

| ID | Caso de uso | Actor | Detalle / Diagrama | Ref. narrativa |
|----|-------------|-------|--------------------|----------------|
| **[CU-P01](./CU-P01-listar-propietarios.md)** | **Listar propietarios** | Empleado, Admin | 📄 [Ver detalle y secuencia](./CU-P01-listar-propietarios.md) | 1, 2, 32, 33 |
| **[CU-P02](./CU-P02-alta-propietario.md)** | **Alta de propietario** | Empleado, Admin | 📄 [Ver detalle y secuencia](./CU-P02-alta-propietario.md) | 1, 2 |
| CU-P03 | Edición de propietario | Empleado, Admin | — | 1, 2 |
| **[CU-P04](./CU-P04-eliminar-propietario.md)** | **Eliminar propietario** | Admin | 📄 [Ver detalle y secuencia](./CU-P04-eliminar-propietario.md) | 21 |
| CU-P05 | Ver detalle de propietario | Empleado, Admin | Incluye sus inmuebles asociados | 2 |

---

## Inmueble

```mermaid
flowchart LR
    Emp([Empleado])
    Adm([Administrador])
    Adm -.-|hereda| Emp

    subgraph sgI[Inmueble]
        I01((Listar))
        I02((Alta))
        I03((Editar))
        I04((Eliminar))
        I05((Suspender / reactivar))
        I06((Ver por propietario))
        I07((Buscar disponibles))
    end

    subgraph sgTI[Tipo de Inmueble]
        TI01((Listar))
        TI02((Alta))
        TI03((Editar))
        TI04((Eliminar))
    end

    Emp --> I01
    Emp --> I02
    Emp --> I03
    Emp --> I05
    Emp --> I06
    Emp --> I07
    Emp --> TI01
    Adm --> I04
    Adm --> TI02
    Adm --> TI03
    Adm --> TI04
```

| ID | Caso de uso | Actor | Notas | Ref. narrativa |
|----|-------------|-------|-------|----------------|
| CU-I01 | Listar inmuebles | Empleado, Admin | Filtro por estado (disponible/suspendido) | 8, 24 |
| CU-I02 | Alta de inmueble | Empleado, Admin | Requiere propietario; imagen portada y galería | 6 |
| CU-I03 | Edición de inmueble | Empleado, Admin | | 6 |
| CU-I04 | Eliminar inmueble | Admin | Baja lógica (`activo = 0`) | 21 |
| CU-I05 | Suspender / reactivar inmueble | Empleado, Admin | No afecta reservas vigentes | 8 |
| CU-I06 | Ver inmuebles de un propietario | Empleado, Admin | Sublistado en detalle de propietario | 2, 25 |
| CU-I07 | Buscar inmuebles disponibles por fechas | Empleado, Admin | Base para crear reserva | 10, 31 |

### Tipo de Inmueble

| ID | Caso de uso | Actor | Notas | Ref. narrativa |
|----|-------------|-------|-------|----------------|
| CU-TI01 | Listar tipos de inmueble | Empleado, Admin | | 7 |
| CU-TI02 | Alta de tipo | Admin | | 7 |
| CU-TI03 | Edición de tipo | Admin | | 7 |
| CU-TI04 | Eliminar tipo | Admin | Validar que no existan inmuebles asociados | 7, 21 |

---

## Inquilino

```mermaid
flowchart LR
    Emp([Empleado])
    Adm([Administrador])
    Adm -.-|hereda| Emp

    subgraph sgIN[Inquilino]
        IN01((Listar))
        IN02((Alta))
        IN03((Editar))
        IN04((Eliminar))
    end

    Emp --> IN01
    Emp --> IN02
    Emp --> IN03
    Adm --> IN04
```

| ID | Caso de uso | Actor | Notas | Ref. narrativa |
|----|-------------|-------|-------|----------------|
| CU-IN01 | Listar inquilinos | Empleado, Admin | Paginado por servidor | 9 |
| CU-IN02 | Alta de inquilino | Empleado, Admin | DNI, nombre, datos de contacto | 9 |
| CU-IN03 | Edición de inquilino | Empleado, Admin | | 9 |
| CU-IN04 | Eliminar inquilino | Admin | Baja lógica (`activo = 0`) | 9, 21 |

---

## Usuario

```mermaid
flowchart LR
    Anon([Anónimo])
    Emp([Empleado])
    Adm([Administrador])
    Adm -.-|hereda| Emp

    subgraph sgAcceso[Acceso]
        U01((Iniciar sesión))
        U02((Cerrar sesión))
        U03((Editar perfil propio))
    end

    subgraph sgU[Gestión de usuarios]
        UU04((Listar usuarios))
        UU05((Alta usuario))
        UU06((Editar usuario))
        UU07((Eliminar usuario))
    end

    Anon --> U01
    Emp --> U01
    Emp --> U02
    Emp --> U03
    Adm --> UU04
    Adm --> UU05
    Adm --> UU06
    Adm --> UU07
```

| ID | Caso de uso | Actor | Notas | Ref. narrativa |
|----|-------------|-------|-------|----------------|
| CU-U01 | Iniciar sesión | Anónimo | Email + contraseña | 20 |
| CU-U02 | Cerrar sesión | Empleado, Admin | | 20 |
| CU-U03 | Editar perfil propio | Empleado, Admin | Datos personales, contraseña, avatar | 22 |
| CU-U04 | Listar usuarios | Admin | Solo administradores | 21 |
| CU-U05 | Alta de usuario | Admin | Asignar rol (Empleado / Admin) | 21 |
| CU-U06 | Editar usuario | Admin | Incluye cambio de rol y estado | 21 |
| CU-U07 | Eliminar usuario | Admin | Baja lógica (`activo = 0`) | 21 |

---

## Estado

- [x] Entidades relevadas
- [x] Bajas lógicas formalizadas
- [x] Diagramas de secuencia y especificación para Propietarios (CU-P01, CU-P02, CU-P04)
