---
fecha-confirmada: 2026-09-07 al 2026-09-17
---

Vamos finalizando la aplicación web para una inmobiliaria, utilizando ASP.Net Core. La narrativa es la misma de las entregas anteriores.

Sintetizando lo pedido en la narrativa serían los siguientes puntos:

- ABM de Propietarios.
- ABM de Inquilinos.
- ABM de Inmuebles y tipos de inmuebles
- ABM de Contratos.
- ABM de Pagos.
- ABM de Usuarios. Se debe distinguir entre los roles de administradores y empleados. Solo los administradores pueden gestionar otros usuarios (ABM).
- Edición de perfil para el usuario logueado. Permitir cambiar contraseña, cambiar foto de perfil y quitar la foto de perfil.
- Uso de autenticación para poder operar (login).
- Uso de políticas de autorización para limitar las bajas exclusivamente al rol "Administrador".
- Listar los inmuebles que estén disponibles (estado disponible, no por fechas).
- Listar todos los inmuebles que le correspondan a un propietario.
- Listar todos los contratos de alquiler que se encuentren vigentes (por fechas desde/hasta).
- Listar todos los contratos de un inmueble en particular.
- Listar los pagos realizados para un contrato en particular. Permitir cargar un nuevo pago a ese contrato desde la pantalla del listado.
- Dadas dos fechas posibles de un contrato (inicio y fin), listar todos los inmuebles que no estén ocupados en algún contrato entre esas fechas.
- Controlar que no existe superposición de fechas de contratos al crear/editar contratos.
- Permitir renovar contratos (crea otro pre cargando datos del original).
- Permitir terminar tempranamente contratos indicando la multa y permitir cargar el pago de esa multa. 
- Registrar qué usuario creó un contrato y, en caso que corresponda, quien lo terminó. Similar para pagos, quién lo creó y, en caso que corresponda, quien lo anuló. Esta información de auditoría sólo es visible para administradores y en una vista de detalles de la entidad correspondiente.
- Usabilidad para el usuario, como menú de navegación, listados con datos representativos, nombres de campos apropiados, ortografía, accesos directos, notificaciones (resultado de operaciones y errores entre otras), validaciones de datos, etc.

Se debe entregar enlace al repositorio, incluyendo:

1. Archivo gitignore correspondiente al proyecto
2. Diagrama de entidad-relación o de clases
3. Base de datos
4. Usuario y contraseña de un administrador y de un empleado
