# Estructura de Desglose de Trabajo (EDT / WBS)

Estructura jerárquica y secuencial de las tareas planificadas para el proyecto inmobiliario.

```mermaid
flowchart TD
    ROOT["Proyecto Inmobiliaria"]

    %% Fase 1: Análisis
    FASE1["1. Análisis y Requisitos"]
    ROOT --> FASE1
    FASE1_1["1.1 Definición general de Casos de Uso"]
    FASE1_2["1.2 Especificación detallada de cada Caso de Uso"]
    FASE1 --> FASE1_1
    FASE1 --> FASE1_2

    %% Fase 2: Diseño de Datos
    FASE2["2. Modelado y Persistencia"]
    ROOT --> FASE2
    FASE2_1["2.1 Diagrama Entidad-Relación (DER)"]
    FASE2_2["2.2 Script SQL de creación e inicialización de BD"]
    FASE2 --> FASE2_1
    FASE2 --> FASE2_2

    %% Fase 3: Documentación y Setup
    FASE3["3. Configuración y Documentación"]
    ROOT --> FASE3
    FASE3_1["3.1 Actualización de README.md"]
    FASE3_1_1["3.1.1 Instrucciones para levantar e inicializar BD"]
    FASE3_1_2["3.1.2 Instrucciones para ejecutar la aplicación"]
    FASE3 --> FASE3_1
    FASE3_1 --> FASE3_1_1
    FASE3_1 --> FASE3_1_2

    %% Fase 4: Desarrollo Primera Entrega
    FASE4["4. Desarrollo: Primera Entrega"]
    ROOT --> FASE4
    FASE4_1["4.1 Configuración de conexión y capa de datos"]
    FASE4_2["4.2 ABM de Propietarios"]
    FASE4_3["4.3 ABM de Inquilinos"]
    FASE4_4["4.4 Validación de criterios de entrega"]
    FASE4 --> FASE4_1
    FASE4 --> FASE4_2
    FASE4 --> FASE4_3
    FASE4 --> FASE4_4
```

## Secuencia y Dependencias

```mermaid
flowchart LR
    CU["Casos de Uso"] --> DET_CU["Detalle de Casos de Uso"]
    DET_CU --> DER["Diagrama Entidad-Relación"]
    DER --> SQL["Script SQL (Creación / Init)"]
    SQL --> DOC["README (Setup BD y Run App)"]
    DOC --> DEV1["Primera Entrega (ABM Propietarios e Inquilinos)"]
```

## Detalle de Paquetes de Trabajo

| Código | Tarea | Entregable / Resultado |
| :--- | :--- | :--- |
| **1.1** | Definición de Casos de Uso | Lista de casos de uso y actores identificados. |
| **1.2** | Detalle de Casos de Uso | Plantillas con flujos principales, alternativos y pre/postcondiciones. |
| **2.1** | Diagrama Entidad-Relación (DER) | Modelo conceptual/lógico con entidades, atributos y relaciones. |
| **2.2** | Script SQL | Archivo `.sql` reproducible para creación de tablas y datos semilla. |
| **3.1** | Setup en [README.md](README.md) | Guía paso a paso para levantar BD y correr la app ASP.NET Core. |
| **4.0** | [Primera Entrega](entregas-y-revisiones/primera-entrega.md) | ABM funcional de Propietarios e Inquilinos con ASP.NET MVC. |
