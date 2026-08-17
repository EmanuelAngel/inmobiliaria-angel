# Inmobiliaria Laboratorio 2

El sistema trata de la informatización de la gestión de alquileres
temporarios de propiedades inmuebles que realiza una agencia
inmobiliaria.

---

## Integrantes del Grupo

* **Emanuel Angel** - *emanuelangelsbr@gmail.com* - [@EmanuelAngel](https://github.com/EmanuelAngel) - Discord: `angel.emanuel`

---

## Modelado de Datos

A continuación se presenta el esquema del modelo de datos correspondiente a la aplicación:

### Diagrama Entidad-Relación (DER)

```mermaid
erDiagram
    PROPIETARIO {
        int id PK
        string nombre
        string apellido
        string dni
        string email
        string telefono
    }

    TIPO_INMUEBLE {
        int id PK
        string descripcion
    }

    INMUEBLE {
        int id PK
        int propietario_id FK
        int tipo_id FK
        string direccion
        int cupo
        decimal precio_por_dia
        decimal porcentaje_senia
        string coordenadas
        string imagen_portada
        string estado
    }

    IMAGEN_INMUEBLE {
        int id PK
        int inmueble_id FK
        string url
    }

    INQUILINO {
        int id PK
        string dni
        string nombre_completo
        string email
        string telefono
    }

    RESERVA {
        int id PK
        int inquilino_id FK
        int inmueble_id FK
        int usuario_creacion_id FK
        int usuario_terminacion_id FK
        date fecha_desde
        date fecha_hasta
        date fecha_fin_anticipado
        decimal monto_por_dia
        string estado
    }

    PAGO {
        int id PK
        int reserva_id FK
        int usuario_creacion_id FK
        int usuario_anulacion_id FK
        string concepto
        date fecha
        decimal importe
        string estado
    }

    USUARIO {
        int id PK
        string email
        string password_hash
        string nombre
        string apellido
        string avatar
        string rol
    }

    PROPIETARIO ||--o{ INMUEBLE : posee
    TIPO_INMUEBLE ||--o{ INMUEBLE : clasifica
    INMUEBLE ||--o{ IMAGEN_INMUEBLE : tiene
    INQUILINO ||--o{ RESERVA : realiza
    INMUEBLE ||--o{ RESERVA : cubre
    RESERVA ||--o{ PAGO : tiene
    USUARIO ||--o{ RESERVA : crea
    USUARIO ||--o{ RESERVA : termina
    USUARIO ||--o{ PAGO : crea
    USUARIO ||--o{ PAGO : anula
```

### Creación e inicialización de la base de datos

**Requisitos:**

- MySQL. Versión utilizada en el proyecto: 8.0.46.0

#### Instrucciones

1. Iniciar una sesión de terminal en la carpeta del proyecto.
2. Ejecutar el comando para crear e inicializar la base de datos.
    - Windows: `mysql -u root -p --default-character-set=utf8mb4 < database.sql`
    - Linux: `mysql -u root -p < database.sql`
3. Ingresar la contraseña del usuario `root` para iniciar el comando de MySQL.

---
