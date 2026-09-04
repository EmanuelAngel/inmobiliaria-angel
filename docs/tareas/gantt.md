# Cronograma del Proyecto (Diagrama de Gantt)

Diagrama temporal basado en las fechas estipuladas y confirmadas para las entregas y revisiones del proyecto.

```mermaid
gantt
    title Cronograma de Entregas y Fases
    dateFormat YYYY-MM-DD
    axisFormat %d/%m

    section Fase Inicial y Setup
    Analisis y DER                         :done, a1, 2026-08-15, 2026-08-18
    Script SQL y Documentacion Base        :done, a2, 2026-08-18, 2026-08-19

    section Primera Entrega
    ABM de Propietarios e Inquilinos       :done, pe1, 2026-08-19, 2026-08-20
    Hito - Primera Entrega                 :done, milestone, m1, 2026-08-20, 0d
    Primera Revision Cruzada               :done, rev1, 2026-08-21, 2026-08-24

    section Segunda Entrega y Revisión
    ABM Inmuebles Reservas y Navegacion    :active, se1, 2026-08-21, 2026-09-03
    Hito - Segunda Entrega                 :milestone, m2, 2026-09-03, 0d
    Segunda Revision Cruzada               :rev2, 2026-09-04, 2026-09-08

    section Entrega Final
    Autenticacion Roles y Auditoria        :ef1, 2026-09-07, 2026-09-11
    Gestion de Pagos Multas y Estados      :ef2, 2026-09-11, 2026-09-14
    Filtros de Disponibilidad y Cierre     :ef3, 2026-09-14, 2026-09-17
    Hito - Entrega Final                   :milestone, m3, 2026-09-17, 0d
```

## Referencias de Hitos y Entregas

| Hito / Actividad | Fecha / Rango | Entregables Principales |
| :--- | :--- | :--- |
| [**Primera Entrega**](../entregas-y-revisiones/primera-entrega.md) | `2026-08-20` (Realizada) | Repositorio base, DER, Script SQL, README, ABM Propietarios e Inquilinos. |
| [**Primera Revisión**](../entregas-y-revisiones/primera-revision.md) | `2026-08-21` a `2026-08-24` | Informe técnico de revisión cruzada entre grupos. |
| [**Segunda Entrega**](../entregas-y-revisiones/segunda-entrega.md) | `2026-09-03` | ABM Inmuebles y Reservas/Contratos, Vistas de Detalle, Menú y estilos. |
| [**Segunda Revisión**](../entregas-y-revisiones/segunda-revision.md) | `2026-09-04` a `2026-09-08` | Informe técnico de revisión cruzada sobre segunda entrega. |
| [**Entrega Final**](../entregas-y-revisiones/entrega-final.md) | `2026-09-07` a `2026-09-17` | Auth (Login/Roles), Pagos/Multas, Consultas/Filtros, Auditoría y entrega final completa (100%). |
