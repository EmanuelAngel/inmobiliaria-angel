# Proyecto Inmobiliaria

El sistema trata de la informatización de la gestión de alquileres de propiedades inmuebles que realiza una **agencia** inmobiliaria.

## Entidades

- **Propietario**. Es el dueño de uno o varios inmuebles.
- **Inmueble**. Son las propiedades que se dan en alquiler a los inquilinos, a través de un contrato.
- **Inquilino**. Es quien contrata el alquiler de un inmueble.
- **Contrato**. Un contrato lleva un inquilino, un inmueble, un monto y fechas desde y hasta.
- **Pago**. Un contrato tiene varios pagos, uno por cada mes.
- **Usuario**. El sistema prevé acceso con email y contraseña. Existen dos roles: Administrador y empleado. **Solo los administradores pueden eliminar entidades.** Solo los administradores pueden gestionar a otros usuarios. Los empleados pueden manipular su propio perfil.

## Funcionalidades

- En la inmobiliaria se alquilan diferentes tipos de inmuebles para un uso determinado.
- Los propietarios de los inmuebles los ofrecen a la agencia para que ésta les busque inquilinos y hacer un contrato de **alquiler** por un tiempo determinado. Claro está que el propietario está obligado a aceptar el inquilino que proponga la agencia si cumple las condiciones estipuladas.
- Se sabe que un propietario es dueño de uno o varios inmuebles. Cada inmueble será propiedad de un único propietario.
- Un inquilino puede llegar a participar de varios contratos de alquiler, pero cada inquilino es único responsable de su contrato.
- Así mismo, cada contrato de **alquiler** tiene asociado un solo inmueble. Aunque a lo largo del tiempo ese inmueble aparece en varios otros contratos de **alquiler** no vigentes.
- Cada contrato de alquiler tiene asociados pagos con información sobre: el número de pago, fecha de pago e importe.
- Cuando un propietario entrega un inmueble, la agencia le pide la dirección, uso (solamente comercial o residencial), tipo (local, depósito, casa, departamento, etc.), cantidad de ambientes, coordenadas y el precio del inmueble. Se debe poder administrar (ABM) los tipos de los inmuebles.
- Si el propietario no estaba ingresado desde antes, hay que agregarlo al sistema solicitando su DNI, apellido, nombre y sus datos de contacto.
- El propietario puede solicitar que se suspenda temporalmente la oferta de uno de sus inmuebles. Esto hará que dicho inmueble no aparezca en los listados de inmuebles para alquilar. No afecta a los alquileres ya creados.
- Cuando el inquilino viene a alquilar un local se lo entrevista solicitando sus datos personales. ABM inquilino. DNI, nombre completo y datos de contacto.
- Luego indica las características del inmueble (uso de local, tipo, ambientes y precio aproximado) y las fechas para alquilar. La agencia lleva a cabo un método para búsqueda de inmuebles que no estén ocupados en esas fechas. Si encuentra algunos adecuados, se entrega una lista de inmuebles. Si al nuevo inquilino le interesa algún inmueble se crea el contrato de alquiler.
- Para los contratos, se deben registrar la fecha de inicio y fecha de finalización del mismo (se deben controlar las fechas), el monto de alquiler mensual en pesos y un vínculo entre la propiedad inmueble y el inquilino. Se debe volver a verificar que el inmueble no esté ocupado en esas fechas por otro contrato.
- Cuando el inquilino viene a pagar el alquiler, quedará registrado el número de pago (1 a "n" para cada contrato), la fecha en la que se realizó el pago, un detalle de lo pagado ("Mes X" o "Abono mes mayo", por ejemplo) y el importe. En los pagos, al editar, solo se puede editar el concepto, no el monto, fecha o número de pago. La eliminación debe ser un cambio de estado, es decir que se siguen mostrando pero especificando que el pago está anulado.
- El inquilino puede terminar antes el contrato si lo desea, pero pagando una multa. Esta fecha debe quedar registrada en el sistema. En caso de terminar el alquiler, se debe registrar la fecha de cuándo se hará efectiva la terminación y calcular la multa. Si se cumplió menos de la mitad del tiempo original **de alquiler**, deberá pagar 2 (dos) meses extra de alquiler. Caso contrario, sólo uno. Además, se debe revisar que no deba meses de alquiler. El sistema debe informar el valor de la multa y permitir cargarlo en los "pagos" del contrato en la misma pantalla. Si el inquilino no paga en el momento, debe poder volverse a ver esta información (multa, meses adeudados, etc). Cabe mencionar que no se debería perder la fecha de finalización original. Esto no solo evita perder información, si no que permite recrear el cálculo de la multa.
- El sistema debe permitir fácilmente renovar un contrato de alquiler. Esto generará un **nuevo** alquiler, con un nuevo monto y fechas, pero con el mismo inquilino e inmueble.
- El sistema debe contar con acceso por usuario y contraseña. Existen dos roles: empleado y administrador. Los administradores pueden eliminar entidades y gestionar a otros usuarios. Los empleados solo pueden manipular su propio perfil (cambiar datos personales, contraseña y avatar). Se debe registrar qué usuario creó un contrato y, en caso que corresponda, quién lo terminó. Similar para pagos, quién lo creó y, en caso de que corresponda, quién lo anuló. Esta información de auditoría sólo es visible para administradores y en una vista de detalles de la entidad correspondiente.

## Informes

- Listar todos los inmuebles y su dueño, que estén en el sistema. Permitir filtrar por disponibilidad (no de fechas, sino de la propiedad "Estado" o "Disponible").
- Listar todos los inmuebles que le correspondan a un propietario.
- Listar todos los contratos de alquiler que se encuentren vigentes (por fecha desde y hasta).
- Listar todos los contratos, incluyendo su inquilino, de un inmueble en particular.
- Listar todos los contratos que terminen en 30, 60 o 90 días (permitir elegir o especificar plazo).
- Listar los pagos realizados para un contrato en particular. Permitir cargar un nuevo pago a ese contrato desde la pantalla del listado.
- Dadas dos fechas posibles de un **contrato** (inicio y fin), listar todos los inmuebles que no estén ocupados en algún contrato entre esas fechas.
