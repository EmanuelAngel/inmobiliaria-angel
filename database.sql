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
-- 5. TABLA: USUARIO
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS USUARIO (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    avatar VARCHAR(255) NULL,
    rol VARCHAR(20) NOT NULL,
    activo TINYINT(1) NOT NULL DEFAULT 1,
    CONSTRAINT chk_usuario_rol CHECK (rol IN ('Administrador', 'Empleado'))
) ENGINE = InnoDB;


-- -----------------------------------------------------------------------------
-- 6. TABLA: RESERVA
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


-- -----------------------------------------------------------------------------
-- 7. TABLA: IMAGEN_INMUEBLE
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS IMAGEN_INMUEBLE (
    id INT AUTO_INCREMENT PRIMARY KEY,
    inmueble_id INT NOT NULL,
    url VARCHAR(255) NOT NULL,
    CONSTRAINT fk_imagen_inmueble FOREIGN KEY (inmueble_id) REFERENCES INMUEBLE (id),
    INDEX idx_imagen_inmueble_id (inmueble_id)
) ENGINE = InnoDB;


-- =============================================================================
-- DATOS SEMILLA / PRUEBA INICIALES
-- =============================================================================
-- Credenciales de prueba:
-- Admin 1:    admin@inmobiliaria.test / Canelones666
-- Admin 2:    carlos.admin@inmobiliaria.test / Canelones666
-- Empleado 1: empleado@inmobiliaria.test / Milanguche$$$
-- Empleada 2: sofia.empleada@inmobiliaria.test / Milanguche$$$
INSERT INTO
    USUARIO (
        nombre,
        apellido,
        email,
        password_hash,
        avatar,
        rol,
        activo
    )
VALUES
    (
        'Lucía',
        'Méndez',
        'admin@inmobiliaria.test',
        'AQAAAAIAAYagAAAAEH50qbl0w853Wbb5EeGdhF9GwAL0Gjh/F23ZIh1Ty8VHwWXOLQCnnswyGd4485or8Q==',
        '/img/usuarios/user-admin-1.webp',
        'Administrador',
        1
    ),
    (
        'Esteban',
        'Pérez',
        'empleado@inmobiliaria.test',
        'AQAAAAIAAYagAAAAEKE2i9A0YGLlpYClj3gW/FQ5pEHTz/8X5jtBRkQAHl9d+KuXc1PdmJD6+RNVTG4m+w==',
        '/img/usuarios/user-employee-1.webp',
        'Empleado',
        1
    ),
    (
        'Carlos',
        'Vargas',
        'carlos.admin@inmobiliaria.test',
        'AQAAAAIAAYagAAAAEH50qbl0w853Wbb5EeGdhF9GwAL0Gjh/F23ZIh1Ty8VHwWXOLQCnnswyGd4485or8Q==',
        '/img/usuarios/user-admin-2.png',
        'Administrador',
        1
    ),
    (
        'Sofía',
        'Martínez',
        'sofia.empleada@inmobiliaria.test',
        'AQAAAAIAAYagAAAAEKE2i9A0YGLlpYClj3gW/FQ5pEHTz/8X5jtBRkQAHl9d+KuXc1PdmJD6+RNVTG4m+w==',
        '/img/usuarios/user-employee-2.jpg',
        'Empleado',
        1
    );


INSERT INTO
    PROPIETARIO (nombre, apellido, dni, email, telefono, activo)
VALUES
    (
        'Juan Carlos',
        'Pérez',
        '20111222',
        'juan.perez@email.com',
        '2664111222',
        1
    ),
    (
        'María Elena',
        'Gómez',
        '27333444',
        'maria.gomez@email.com',
        '2664333444',
        1
    ),
    (
        'Roberto',
        'Fernández',
        '18555666',
        'roberto.fernandez@email.com',
        '2664555666',
        1
    ),
    (
        'Claudia Marcela',
        'Morales',
        '24888999',
        'claudia.morales@email.com',
        '2664888999',
        1
    ),
    (
        'Gustavo Adolfo',
        'Rivas',
        '22444111',
        'gustavo.rivas@email.com',
        '2664444111',
        1
    ),
    (
        'Osvaldo Darío',
        'Torres',
        '16222333',
        'osvaldo.torres@email.com',
        '2664222333',
        0
    );


INSERT INTO
    INQUILINO (dni, nombre_completo, email, telefono, activo)
VALUES
    (
        '35444555',
        'Martín Gómez',
        'martin.gomez@email.com',
        '2664555666',
        1
    ),
    (
        '38777888',
        'Ana Rossi',
        'ana.rossi@email.com',
        '2664777888',
        1
    ),
    (
        '40123987',
        'Lucas Benítez',
        'lucas.benitez@email.com',
        '2664123987',
        1
    ),
    (
        '42333111',
        'Valentina Solís',
        'valentina.solis@email.com',
        '2664333111',
        1
    ),
    (
        '36999000',
        'Marcos Del Valle',
        'marcos.delvalle@email.com',
        '2664999000',
        1
    ),
    (
        '33222111',
        'Federico Ruiz',
        'federico.ruiz@email.com',
        '2664222111',
        0
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
        'Av. Illia 456, San Luis',
        4,
        45000.00,
        20.00,
        -33.29800000,
        -66.33500000,
        '/img/inmuebles/1.JPG',
        'Disponible'
    ),
    (
        1,
        2,
        'San Martín 1234, 3° B, San Luis',
        2,
        32000.00,
        30.00,
        -33.30100000,
        -66.33800000,
        '/img/inmuebles/2.JPG',
        'Disponible'
    ),
    (
        2,
        4,
        'Circuito Los Funes Km 8, Potrero de los Funes',
        6,
        85000.00,
        25.00,
        -33.22500000,
        -66.23000000,
        '/img/inmuebles/base-outdoor-1.webp',
        'Disponible'
    ),
    (
        3,
        3,
        'Pringles 789, PB A, San Luis',
        1,
        26000.00,
        20.00,
        -33.30000000,
        -66.33200000,
        '/img/inmuebles/3.webp',
        'Suspendido'
    ),
    (
        1,
        1,
        'Los Eucaliptos 142, Juana Koslay',
        5,
        60000.00,
        20.00,
        -33.27500000,
        -66.26000000,
        '/img/inmuebles/5.jpg',
        'Disponible'
    ),
    (
        4,
        5,
        'Av. del Sol 850, Villa de Merlo',
        3,
        55000.00,
        20.00,
        -32.34200000,
        -65.01300000,
        '/img/inmuebles/6.jpg',
        'Disponible'
    ),
    (
        5,
        4,
        'Costanera Norte 310, El Volcán',
        4,
        70000.00,
        25.00,
        -33.24800000,
        -66.17500000,
        '/img/inmuebles/base-outdoor-2.jpg',
        'Disponible'
    ),
    (
        2,
        2,
        'Av. Serrana Mza 12 Casa 4, La Punta',
        2,
        29000.00,
        20.00,
        -33.18300000,
        -66.31200000,
        '/img/inmuebles/7.jpg',
        'Suspendido'
    );


-- -----------------------------------------------------------------------------
-- 6. DATOS: IMAGEN_INMUEBLE (Galería)
-- -----------------------------------------------------------------------------
INSERT INTO
    IMAGEN_INMUEBLE (inmueble_id, url)
VALUES
    (1, '/img/inmuebles/8.jpg'),
    (1, '/img/inmuebles/9.jpg'),
    (2, '/img/inmuebles/10.webp'),
    (2, '/img/inmuebles/base-1.jpg'),
    (3, '/img/inmuebles/base-outdoor-3.jpg'),
    (3, '/img/inmuebles/base-outdoor-4.jpg'),
    (4, '/img/inmuebles/11.webp'),
    (5, '/img/inmuebles/12.webp'),
    (5, '/img/inmuebles/base-2.jpg'),
    (6, '/img/inmuebles/4.webp'),
    (6, '/img/inmuebles/13.webp'),
    (7, '/img/inmuebles/14.jpg'),
    (7, '/img/inmuebles/15.webp'),
    (8, '/img/inmuebles/16.jpeg');


-- -----------------------------------------------------------------------------
-- 7. DATOS: RESERVA
-- -----------------------------------------------------------------------------
INSERT INTO
    RESERVA (
        inquilino_id,
        inmueble_id,
        usuario_creacion_id,
        usuario_terminacion_id,
        fecha_desde,
        fecha_hasta,
        fecha_fin_anticipado,
        monto_por_dia,
        estado
    )
VALUES
    (
        1,
        1,
        2,
        NULL,
        '2026-09-14',
        '2026-09-24',
        NULL,
        45000.00,
        'Activa'
    ),
    (
        2,
        3,
        4,
        NULL,
        '2026-09-08',
        '2026-09-18',
        NULL,
        85000.00,
        'Activa'
    ),
    (
        3,
        2,
        2,
        NULL,
        '2026-10-01',
        '2026-10-10',
        NULL,
        32000.00,
        'Activa'
    ),
    (
        4,
        5,
        1,
        NULL,
        '2026-08-01',
        '2026-08-10',
        NULL,
        60000.00,
        'Finalizada'
    ),
    (
        5,
        6,
        2,
        1,
        '2026-08-15',
        '2026-08-25',
        '2026-08-20',
        55000.00,
        'Cancelada'
    );
