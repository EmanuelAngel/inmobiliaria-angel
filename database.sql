-- =============================================================================
-- BASE DE DATOS: INMOBILIARIA LAB 2 (2da Entrega: Inmuebles y Reservas)
-- Motor: MySQL 8.0.46
-- Codificación: utf8mb4
-- IDEMPOTENTE: si la base ya existe, la elimina y la recrea desde cero.
-- =============================================================================
DROP DATABASE IF EXISTS inmobiliaria_dev;


CREATE DATABASE inmobiliaria_dev CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;


USE inmobiliaria_dev;


-- -----------------------------------------------------------------------------
-- 1. TABLA: PROPIETARIO
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS PROPIETARIO (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    dni VARCHAR(20) NOT NULL UNIQUE,
    email VARCHAR(150) NOT NULL,
    telefono VARCHAR(50) NOT NULL,
    activo TINYINT(1) NOT NULL DEFAULT 1
) ENGINE = InnoDB;


-- -----------------------------------------------------------------------------
-- 2. TABLA: INQUILINO
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS INQUILINO (
    id INT AUTO_INCREMENT PRIMARY KEY,
    dni VARCHAR(20) NOT NULL UNIQUE,
    nombre_completo VARCHAR(200) NOT NULL,
    email VARCHAR(150) NULL,
    telefono VARCHAR(50) NULL,
    activo TINYINT(1) NOT NULL DEFAULT 1
) ENGINE = InnoDB;


-- -----------------------------------------------------------------------------
-- 3. TABLA: TIPO_INMUEBLE
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS TIPO_INMUEBLE (
    id INT AUTO_INCREMENT PRIMARY KEY,
    descripcion VARCHAR(100) NOT NULL UNIQUE,
    activo TINYINT(1) NOT NULL DEFAULT 1
) ENGINE = InnoDB;


-- -----------------------------------------------------------------------------
-- 4. TABLA: INMUEBLE
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS INMUEBLE (
    id INT AUTO_INCREMENT PRIMARY KEY,
    propietario_id INT NOT NULL,
    tipo_id INT NOT NULL,
    direccion VARCHAR(200) NOT NULL,
    cupo INT NOT NULL,
    precio_por_dia DECIMAL(10, 2) NOT NULL,
    porcentaje_senia DECIMAL(5, 2) NOT NULL,
    latitud DECIMAL(10, 8) NULL,
    longitud DECIMAL(11, 8) NULL,
    imagen_portada VARCHAR(255) NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'Disponible',
    CONSTRAINT chk_inmueble_cupo CHECK (cupo > 0),
    CONSTRAINT chk_inmueble_precio CHECK (precio_por_dia > 0),
    CONSTRAINT chk_inmueble_senia CHECK (
        porcentaje_senia >= 0
        AND porcentaje_senia <= 100
    ),
    CONSTRAINT chk_inmueble_estado CHECK (estado IN ('Disponible', 'Suspendido')),
    CONSTRAINT fk_inmueble_propietario FOREIGN KEY (propietario_id) REFERENCES PROPIETARIO (id),
    CONSTRAINT fk_inmueble_tipo FOREIGN KEY (tipo_id) REFERENCES TIPO_INMUEBLE (id)
) ENGINE = InnoDB;


-- -----------------------------------------------------------------------------
-- 5. TABLA: RESERVA
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS RESERVA (
    id INT AUTO_INCREMENT PRIMARY KEY,
    inquilino_id INT NOT NULL,
    inmueble_id INT NOT NULL,
    usuario_creacion_id INT NULL,
    usuario_terminacion_id INT NULL,
    fecha_desde DATE NOT NULL,
    fecha_hasta DATE NOT NULL,
    fecha_fin_anticipado DATE NULL,
    monto_por_dia DECIMAL(10, 2) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'Activa',
    CONSTRAINT chk_reserva_monto CHECK (monto_por_dia > 0),
    CONSTRAINT chk_reserva_fechas CHECK (fecha_hasta >= fecha_desde),
    CONSTRAINT chk_reserva_estado CHECK (estado IN ('Activa', 'Finalizada', 'Cancelada')),
    CONSTRAINT fk_reserva_inquilino FOREIGN KEY (inquilino_id) REFERENCES INQUILINO (id),
    CONSTRAINT fk_reserva_inmueble FOREIGN KEY (inmueble_id) REFERENCES INMUEBLE (id)
) ENGINE = InnoDB;


-- =============================================================================
-- DATOS SEMILLA / PRUEBA INICIALES
-- =============================================================================
INSERT INTO
    PROPIETARIO (nombre, apellido, dni, email, telefono)
VALUES
    (
        'Juan Carlos',
        'Pérez',
        '20111222',
        'juan.perez@email.com',
        '2664111222'
    ),
    (
        'María Elena',
        'Gómez',
        '27333444',
        'maria.gomez@email.com',
        '2664333444'
    ),
    (
        'Roberto',
        'Fernández',
        '18555666',
        'roberto.fernandez@email.com',
        '2664555666'
    );


INSERT INTO
    INQUILINO (dni, nombre_completo, email, telefono)
VALUES
    (
        '35444555',
        'Martín Gómez',
        'martin.gomez@email.com',
        '2664555666'
    ),
    (
        '38777888',
        'Ana Rossi',
        'ana.rossi@email.com',
        '2664777888'
    ),
    (
        '40123987',
        'Lucas Benítez',
        'lucas.benitez@email.com',
        '2664123987'
    );


INSERT INTO
    TIPO_INMUEBLE (descripcion)
VALUES
    ('Casa'),
    ('Departamento'),
    ('Monoambiente'),
    ('Cabaña'),
    ('Loft');


INSERT INTO
    INMUEBLE (
        propietario_id,
        tipo_id,
        direccion,
        cupo,
        precio_por_dia,
        porcentaje_senia,
        latitud,
        longitud,
        imagen_portada,
        estado
    )
VALUES
    (
        1,
        1,
        'Av. Illia 456',
        4,
        45000.00,
        20.00,
        -33.29800000,
        -66.33500000,
        '/img/inmuebles/casa-illia.jpg',
        'Disponible'
    ),
    (
        1,
        2,
        'San Martín 1234, 3° B',
        2,
        30000.00,
        30.00,
        -33.30100000,
        -66.33800000,
        '/img/inmuebles/depto-sanmartin.jpg',
        'Disponible'
    ),
    (
        2,
        4,
        'Ruta 1 Km 5, Potrero de los Funes',
        6,
        75000.00,
        25.00,
        -33.22500000,
        -66.23000000,
        '/img/inmuebles/cabana-potrero.jpg',
        'Disponible'
    ),
    (
        3,
        3,
        'Pringles 789',
        2,
        25000.00,
        20.00,
        -33.30000000,
        -66.33200000,
        '/img/inmuebles/mono-pringles.jpg',
        'Suspendido'
    );


INSERT INTO
    RESERVA (
        inquilino_id,
        inmueble_id,
        fecha_desde,
        fecha_hasta,
        monto_por_dia,
        estado
    )
VALUES
    (
        1,
        1,
        '2026-09-10',
        '2026-09-15',
        45000.00,
        'Activa'
    ),
    (
        2,
        3,
        '2026-09-20',
        '2026-09-25',
        75000.00,
        'Activa'
    ),
    (
        3,
        2,
        '2026-08-01',
        '2026-08-07',
        30000.00,
        'Finalizada'
    );
