-- =============================================================================
-- BASE DE DATOS: INMOBILIARIA LAB 2 (1ra Entrega: Propietarios e Inquilinos)
-- Motor: MySQL 8.0+
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
