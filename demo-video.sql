-- -----------------------------------------------------------------------------
-- demo-video.sql
-- Estado de datos reproducible para el video demo del portfolio.
--
-- Problema que resuelve: las semillas de database.sql usan fechas absolutas de
-- 2026-08/09/10. Pasadas esas fechas, todas las reservas activas quedan
-- vencidas y la pantalla de "Finalizar anticipadamente" calcula:
--     dias_restantes = 0  ->  baseEconomica = 0  ->  multa = $0,00
-- Este script crea una reserva Activa con fechas RELATIVAS a hoy, de modo que
-- el transcurrido sea menor al 50% y la multa aplique al 50% del saldo restante
-- (el tramo con el número grande, que es el que se quiere mostrar en cámara).
--
-- Uso:
--     mysql -u <usuario> -p inmobiliaria_dev < demo-video.sql
--
-- Re-ejecutable: antes de cada toma vuelve a dejar la reserva demo en estado
-- inicial (Activa, sin pago de multa). No toca ninguna otra reserva.
--
-- IMPORTANTE: este script es solo para el entorno local de grabación. No forma
-- parte del entregable ni se ejecuta desde la aplicación.
-- -----------------------------------------------------------------------------

SET @id_demo  = 900;  -- id reservado para la demo (fuera del rango de las semillas)
SET @inmueble = 1;
SET @inquilino = 1;
SET @usuario = 2;     -- usuario_creacion_id (empleado)


-- Reset idempotente: si ya se grabó una toma y la reserva quedó Cancelada con
-- su pago de multa registrado, se vuelve al punto de partida.
-- El DELETE físico es aceptable SOLO acá: la baja lógica es una regla de la
-- aplicación, no del script de grabación.
DELETE FROM PAGO   WHERE reserva_id = @id_demo;
DELETE FROM RESERVA WHERE id = @id_demo;


-- Reserva demo: 3 días transcurridos de 14 pactados (~21% del plazo).
-- Con el plazo a menos de la mitad, la regla de negocio aplica multa del 50%
-- sobre el saldo restante (11 días por el precio del inmueble).
-- monto_por_dia se toma del inmueble: la reserva guarda el precio al momento
-- de la creación, así la pantalla de detalle queda coherente.
INSERT INTO RESERVA (
    id,
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
SELECT
    @id_demo,
    @inquilino,
    i.id,
    @usuario,
    NULL,
    CURDATE() - INTERVAL 3 DAY,
    CURDATE() + INTERVAL 11 DAY,
    NULL,
    i.precio_por_dia,
    'Activa'
FROM INMUEBLE i
WHERE i.id = @inmueble;


-- El inmueble tiene que estar ofrecido para que también aparezca en
-- Informes/Disponibilidad durante la toma.
UPDATE INMUEBLE SET estado = 'Disponible' WHERE id = @inmueble;


-- -----------------------------------------------------------------------------
-- Verificación: replica el cálculo de ReservasController.CalcularLiquidacion
-- (ReservasController.cs:415-441) sin abrir la aplicación.
-- La columna multa_esperada debe coincidir EXACTO con el número grande que
-- muestra Views/Reservas/FinalizarAnticipada.cshtml en la fecha de grabación.
-- Si no coincide, no grabes: revisá que hayas vuelto a correr este script hoy.
-- -----------------------------------------------------------------------------
SELECT
    d.reserva                   AS reserva,
    d.fecha_desde,
    d.fecha_hasta,
    d.dias_totales,
    d.dias_transcurridos,
    d.dias_restantes,
    ROUND(d.dias_transcurridos / d.dias_totales * 100, 2) AS pct_transcurrido,
    d.pct_multa,
    ROUND(d.dias_restantes * d.monto_por_dia * d.pct_multa / 100, 2) AS multa_esperada
FROM (
    SELECT
        r.id AS reserva,
        r.fecha_desde,
        r.fecha_hasta,
        r.monto_por_dia,
        -- Math.Max(..., 1) del controlador: el plazo nunca es cero.
        GREATEST(DATEDIFF(r.fecha_hasta, r.fecha_desde), 1) AS dias_totales,
        -- Math.Clamp(hoy - desde, 0, dias_totales)
        LEAST(
            GREATEST(DATEDIFF(CURDATE(), r.fecha_desde), 0),
            GREATEST(DATEDIFF(r.fecha_hasta, r.fecha_desde), 1)
        ) AS dias_transcurridos,
        -- Math.Max(0, hasta - hoy)
        GREATEST(DATEDIFF(r.fecha_hasta, CURDATE()), 0) AS dias_restantes,
        CASE
            WHEN LEAST(
                     GREATEST(DATEDIFF(CURDATE(), r.fecha_desde), 0),
                     GREATEST(DATEDIFF(r.fecha_hasta, r.fecha_desde), 1)
                 ) / GREATEST(DATEDIFF(r.fecha_hasta, r.fecha_desde), 1) * 100 < 50
            THEN 50
            ELSE 25
        END AS pct_multa
    FROM RESERVA r
    WHERE r.id = @id_demo
) d;
