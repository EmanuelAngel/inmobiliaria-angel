# Cronograma del Proyecto (Diagrama de Gantt)

Diagrama temporal basado en las fechas tentativas estipuladas para las entregas y revisiones del proyecto.

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
    Primera Revision Cruzada               :active, rev1, 2026-08-21, 2026-08-24

    section Segunda Entrega
    ABM Inmuebles Reservas y Navegacion    :se1, 2026-08-21, 2026-08-27
    Hito - Segunda Entrega                 :milestone, m2, 2026-08-27, 0d

    section Entrega Final
    Autenticacion Roles y Auditoria        :ef1, 2026-08-28, 2026-09-15
    Gestion de Pagos Multas y Estados      :ef2, 2026-09-16, 2026-09-30
    Filtros de Disponibilidad y Cierre     :ef3, 2026-10-01, 2026-10-09
    Hito - Entrega Final                   :milestone, m3, 2026-10-10, 0d
```

## Referencias de Hitos y Entregas

| Hito / Actividad | Fecha Tentativa | Entregables Principales |
| :--- | :--- | :--- |
| [**Primera Entrega**](../entregas-y-revisiones/primera-entrega.md) | `2026-08-20` | Repositorio base, DER, Script SQL, README, ABM Propietarios e Inquilinos. |
| [**Primera Revisión**](../entregas-y-revisiones/primera-revision.md) | Post-entrega | Informe técnico de revisión cruzada entre grupos. |
| [**Segunda Entrega**](../entregas-y-revisiones/segunda-entrega.md) | `2026-08-27` | ABM Inmuebles y Reservas/Contratos, Vistas de Detalle, Menú y estilos. |
| [**Entrega Final**](../entregas-y-revisiones/entrega-final.md) | `2026-10-10` | Auth (Login/Roles), Pagos/Multas, Consultas/Filtros, Auditoría y entrega final completa. |
