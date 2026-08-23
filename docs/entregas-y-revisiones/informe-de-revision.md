# Informe de Revisión Cruzada — Primera Entrega

## Identificación

|                        |                                          |
| ---------------------- | ---------------------------------------- |
| **Grupo revisor**      | 4. Angel Emanuel                         |
| **Grupo revisado**     | 5. Mauricio Barca                        |
| **Fecha de revisión**  | 2026-08-22                               |
| **Repositorio**        | [Link](https://github.com/Mbarca89/lab2) |
| **Commit de revisión** | 9017da022294ce925d7dd8877d0fc149db5a24aa |
## Hallazgos

### 1. Uso de asincronismo no cubierto por la cátedra aún

|                   |                                                                                                                                                                                                                                                     |
| ----------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Ubicación**     | `Data/PropietarioRepository.cs`, `Data/InquilinoRepository.cs`                                                                                                                                                                                      |
| **Descripción**   | Todos los métodos de acceso a datos usan `async`/`await` (`GetAllAsync`, `CreateAsync`, etc.).                                                                                                                                                      |
| **Justificación** | Si bien el asincronismo en nuestro caso es la práctica recomendada (yo también lo hubiera hecho desde el inicio) si no fuera porque los profesores dejaron en claro que hasta que no veamos asincronismo en clase teníamos que hacer todo síncrono. |
| **Importancia**   | Menor                                                                                                                                                                                                                                               |
| **Recomendación** | Intentar seguir los lineamientos de los profesores.                                                                                                                                                                                                 |
### 2. Interpolación de nombres de columna en consultas SQL

|                   |                                                                                                                                                                                                                                                                                                                                                              |
| ----------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Ubicación**     | `Data/PropietarioRepository.cs` (método `ExistsAsync`, líneas 38-48)                                                                                                                                                                                                                                                                                         |
| **Descripción**   | El método privado `ExistsAsync` recibe el nombre de la columna como string y lo interpola directamente en la consulta SQL con `$"... WHERE {column} = @Value..."`.                                                                                                                                                                                           |
| **Justificación** | Si bien en este caso no hay riesgo real de SQL injection (los valores `"Dni"` y `"Email"` están hardcodeados en los métodos públicos), el patrón de concatenar nombres de columna en SQL podría escalar mal si alguien lo extiende sin tomar la misma precaución. Pero como está en un grupo como único integrante tiene sentido que se haya permitido esto. |
| **Importancia**   | Sugerencia                                                                                                                                                                                                                                                                                                                                                   |
| **Recomendación** | Considerar usar métodos separados con queries explícitas para cada caso, evitando la interpolación en SQL.                                                                                                                                                                                                                                                   |

### 3. Elección de motor de base de datos (SQL Server en lugar de MySQL)

|                   |                                                                                                                                                                                                                                                                                                                                                                      |
| ----------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Ubicación**     | `Inmobiliaria.csproj`, `script.sql`, `appsettings.json`                                                                                                                                                                                                                                                                                                              |
| **Descripción**   | Se utilizó Microsoft SQL Server (`Microsoft.Data.SqlClient`) como motor de base de datos en lugar de MySQL.                                                                                                                                                                                                                                                          |
| **Justificación** | **Si no mal recuerdo**, los profesores mencionaron intentar trabajar con MySQL para estandarizar los entornos de entrega y revisión. Usar otra tecnología puede generar inconvenientes a otros grupos al momento de revisar (en mi caso por velocidad de internet al descargar SQL Server). Pero fuera de eso, el script y la configuración funcionan correctamente. |
| **Importancia**   | Menor                                                                                                                                                                                                                                                                                                                                                                |
| **Recomendación** | Intentar seguir los lineamientos de los profesores para estandarizar el entorno común de trabajo y revisión.                                                                                                                                                                                                                                                         |

## Aspectos positivos

- Usa el *data annotation* `[Phone]` para validar los campos que son números de teléfono. Fue más allá de lo que mostró el profesor para mejorar la validación de los modelos.
- El script de la base de datos `./script.sql` es idempotente. Se puede ejecutar múltiples veces sin romper nada. 
- Buena práctica usando `sealed` en clases que no deberían usarse para heredar.
- Los repositorios implementan correctamente el patrón `async/await` para todas las operaciones de bases de datos lo cual es la práctica recomendada para I/O en .NET.
- Reutiliza lógica de acceso a datos mediante el método privado `ExecuteWriteAsync`, que centraliza la apertura de conexión y el armado de parámetros para las operaciones de escritura (`Create` y `Update`), evitando código repetido.
- En los controladores, al editar un propietario o inquilino, valida la unicidad de DNI y Email excluyendo al registro actual (pasando `excludedId` al repositorio). Esto evita que la validación rechace los propios datos del registro que se está editando.
- Extrae el formulario de propietarios e inquilinos en una vista parcial (`_Form.cshtml`) reutilizada tanto en Create como en Edit, evitando duplicar el marcado.
- En las vistas de listado, verifica si el modelo tiene registros (`!Model.Any()`) y muestra un mensaje informativo cuando la tabla está vacía.
- Mantiene convenciones de nomenclatura consistentes. Métodos técnicos y acciones en inglés respetando los estándares .NET/REST (`GetAllAsync`, `CreateAsync`, `Details`, etc.) y entidades de dominio en español sin mezclar.

## Conclusión general

El trabajo cumple con lo requerido para la primer entrega, no hay cosas extras que revisar o faltantes. El script de sql no falla, la aplicación compila y opera correctamente en los flujos principales.

Destaco:
- El script de sql idempotente.
- En general el DRY aplicado:
	- La componetización del form de las vistas.
	- El método privado `ExecuteWriteAsync` para reutilización en métodos de Create y Edit.

Los primeros hallazgos son menores:
- El uso del asincronismo se adelanta al contenido visto, pero es algo que yo también hubiera hecho desde un inicio. Y si el compañero desconocía esto, me parece bueno que haya ido por este enfoque por su cuenta. Tampoco hay problema de comprensión ya que en mí caso (plan viejo, desconozco plan actual) ya había visto asincronismo en JS.
- La interpolación de strings en sql no representa un riesgo real en este caso, pero para un futuro si se une otro compañero al grupo o en trabajo en equipo esto sí sería una cosa a tener en cuenta.

Más allá de los detalles mencionados y del tiempo extra que llevó preparar el entorno por la diferencia de motor de base de datos, el trabajo está muy bien , el código es impecable y fácil de seguir. Es más simple y limpio que el mío, por lo que me dieron ganas de seguir esta forma de trabajar.

Muy bien Mauricio, éxitos en las próximas entregas.
