# CU-R06 — Extender / Renovar Reserva

> **Módulo:** Reservas y Pagos  
> **Actor principal:** Empleado, Administrador  
> **Fuente de verdad:** [`../../narrativa.md`](../../narrativa.md) (Ítem 19)  
> **Reglas de negocio:** ver [`../../AGENTS.md`](../../AGENTS.md)

---

## 1. Descripción
Permite prolongar el período de ocupación de un inquilino sobre un inmueble. Por regla de arquitectura del dominio, esta operación **no modifica la reserva existente**, sino que genera una **nueva reserva** vinculada al mismo inquilino e inmueble, iniciando inmediatamente tras la finalización de la original.

---

## 2. Precondiciones
- El usuario autenticado posee rol `Empleado` o `Administrador`.
- La reserva original existe y está activa (`activo = 1`).
- El inmueble no tiene otras reservas superpuestas durante el período de extensión solicitado (`fecha_fin_original` hasta `nueva_fecha_fin`).

---

## 3. Postcondiciones
- Se crea una **nueva entidad Reserva** en el sistema con su propio ciclo de vida, importes y auditoría del usuario creador.
- La reserva original permanece intacta en sus fechas, importes y pagos históricos.

---

## 4. Reglas de Negocio Aplicadas
1. **RN-R06-1 (Inmutabilidad del contrato original):** La reserva original NO sufre modificaciones en su `fecha_fin` ni en sus registros contables.
2. **RN-R06-2 (Continuidad de fechas):** La `fecha_inicio` de la nueva reserva debe coincidir con la fecha de fin de la reserva previa (o el día posterior, según convención del negocio).
3. **RN-R06-3 (No superposición):** Se debe validar que no existan reservas posteriores ya confirmadas en el nuevo rango de fechas.
4. **RN-R06-4 (Auditoría):** La nueva reserva registra `id_usuario_creador` del operador actual.

---

## 5. Flujo Principal
1. El usuario selecciona la opción "Extender / Renovar" desde la vista de detalle de una reserva existente.
2. El sistema precarga los datos del formulario de alta:
   - Inmueble e Inquilino vinculados (solo lectura).
   - `FechaInicio` establecida en la `FechaFin` de la reserva anterior.
3. El usuario ingresa la nueva `FechaFin` y el nuevo monto pactado para la extensión.
4. El sistema valida la disponibilidad del inmueble en el nuevo rango solicitado.
5. El usuario confirma la creación de la nueva reserva.
6. El sistema persiste la nueva reserva y opcionalmente asocia una referencia a la reserva precedente si correspondiera.
7. El sistema redirige al detalle de la nueva reserva.

---

## 6. Flujos Alternativos y Excepciones
- **FA1 — Superposición con otra reserva futura:** Si existe otra reserva ya registrada para ese inmueble dentro del período de extensión, el sistema rechaza la operación e informa las fechas en conflicto.

---

## 7. Diagrama de Secuencia

```mermaid
sequenceDiagram
    autonumber
    actor Usuario as Empleado / Admin
    participant View as Vista (Razor - Extender)
    participant Ctrl as ReservasController
    participant RepoR as RepositorioReserva
    participant DB as Base de Datos (MySQL)

    Usuario->>Ctrl: GET /Reservas/Extender(idOriginal)
    activate Ctrl
    Ctrl->>RepoR: ObtenerPorId(idOriginal)
    RepoR->>DB: SELECT * FROM reservas WHERE id = @idOriginal
    DB-->>RepoR: Reserva Original
    RepoR-->>Ctrl: Datos de Reserva
    Ctrl-->>View: Presenta Formulario con datos precargados (FechaInicio = FechaFinOriginal)
    deactivate Ctrl

    Usuario->>View: Ingresa nueva FechaFin y confirma
    View->>Ctrl: POST /Reservas/Extender (ExtenderReservaViewModel)
    
    activate Ctrl
    Ctrl->>RepoR: ValidarSuperposicion(inmuebleId, nuevaFechaInicio, nuevaFechaFin)
    RepoR->>DB: SELECT COUNT(*) FROM reservas WHERE id_inmueble = @inmuebleId AND activo = 1 AND (fecha_inicio <= @nuevaFin AND fecha_fin >= @nuevaInicio)
    DB-->>RepoR: count

    alt Existe conflicto de fechas (count > 0)
        Ctrl-->>View: Error ("El inmueble no está disponible en el período de extensión solicitado")
    else Fechas disponibles
        Ctrl->>RepoR: Guardar(nuevaReserva, usuarioLogueadoId)
        RepoR->>DB: INSERT INTO reservas (id_inmueble, id_inquilino, fecha_inicio, fecha_fin, importe, id_usuario_creador) VALUES (...)
        DB-->>RepoR: nuevo_id_reserva
        
        Ctrl-->>View: Redirección a Detalle(nuevo_id_reserva) con mensaje de confirmación
    end
    deactivate Ctrl
```
