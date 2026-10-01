# Design Brief: HabitHub Dashboard (Mobile-First)

Documento de especificación de interfaz y UX para la propuesta de Habit Tracker, derivado del proceso de descubrimiento con la skill **Impeccable**.

---

## 1. Dirección Visual y Principios de Diseño

| Parámetro | Definición |
|---|---|
| **Enfoque de diseño** | **Mobile-First**: la experiencia primordial se concibe para pantallas de smartphone y se expande progresivamente a desktop. |
| **Estilo estético** | **Detallado, táctil e informativo**: adiós al minimalismo vacío; tarjetas ricas en datos, bordes sutiles, micro-interacciones claras y jerarquía visual densa pero ordenada. |
| **Soporte de temas** | **Dual (Light / Dark) de alto contraste**: fondos oscuros profundos (con contrastes nítidos para legibilidad bajo sol o de noche) y paleta diurna limpia. |
| **Acentos temáticos** | **Codificación por color según categoría**: verde esmeralda para Salud/Cuerpo, violeta/índigo para Estudio/Dev, ámbar para Enfoque, etc. |

---

## 2. Topología de Navegación

### A. Pantallas Móviles (< 768px)
- **Top App Bar fija:** Saludo/fecha de hoy, racha global acumulada y avatar del usuario con acceso rápido a perfil.
- **Área de contenido scrolleable:** Listado vertical de tarjetas de hábitos de "Hoy", con filtros rápidos horizontales por categoría en chips deslizables.
- **Bottom Navigation Bar fija (inferior):** Barra de navegación con 4 destinos clave de fácil alcance con el pulgar:
  1. 🏠 **Hoy:** Dashboard diario de check-ins rápidos.
  2. 📊 **Mis Hábitos:** Gestión detallada, archivo y creación de nuevos hábitos.
  3. 📚 **Artículos:** Módulo de ciencia de hábitos y lecturas curadas por el Admin.
  4. 👤 **Perfil:** Ajustes, avatar y administración de cuenta.

### B. Pantallas Desktop (≥ 768px)
- **Sidebar lateral colapsable:** A la izquierda, absorbe la navegación del bottom bar más acciones de soporte y gestión.
- **Layout de dos columnas:**
  - Columna principal (65%): Feed de hábitos de hoy con mini-heatmaps expandidos.
  - Columna lateral (35%): Resumen del día, artículo destacado de la semana y accesos rápidos de auditoría.

---

## 3. Anatomía Detallada de la Tarjeta de Hábito

Cada tarjeta de hábito se aleja del to-do plano y se estructura como un módulo de control táctil con densidad informativa:

```
┌──────────────────────────────────────────────────────────┐
│ [Icono] Salud • Agua diaria                     [Frec: 1D]│  <-- Header + Categoría
│ Meta: 6/8 Vasos (75%)                          [🔥 14 días]│  <-- Racha y progreso
├──────────────────────────────────────────────────────────┤
│   [ - ]   6 vasos alcanzados   [ + ]                     │  <-- Stepper interactivo
│   ██████████████████░░░░░░░░░░░░░░░ (Barra de progreso)   │
├──────────────────────────────────────────────────────────┤
│ Mini-Heatmap (Últimos 30 días):                           │
│ 🟩🟩🟩⬜🟩🟩🟩🟩🟩🟩🟩🟩⬜🟩🟩🟩🟩🟩🟩🟩🟩🟩🟩🟩🟩🟩🟩🟩🟩  │  <-- Consistencia individual
├──────────────────────────────────────────────────────────┤
│ [📎 Adjuntar evidencia/nota]       [Último registro: 14:30]│  <-- Acciones secundarias
└──────────────────────────────────────────────────────────┘
```

### Variantes de Interacción según Tipo de Hábito:
1. **Cuantitativo (ej. agua, páginas leídas):**
   - Stepper táctil grande `[ - ]` y `[ + ]` para sumar unidades sin abrir modales.
   - Micro-barra de progreso con el color de la categoría que se llena en tiempo real.
2. **Booleano (ej. meditar, tender la cama):**
   - Botón táctil tipo switch/toggle de alta respuesta con micro-animación de éxito al completarse.
3. **Abstinencia / Quit Habit (ej. sin azúcar, sin fumar):**
   - Contador en días, horas y minutos desde el último reinicio.
   - Botón secundario para registrar recaída con nota de aprendizaje.

### Mini-Heatmap Integrado por Hábito:
- Matriz compacta de cuadros (últimos 30 a 60 días) incrustada en la parte inferior de la tarjeta.
- Permite ver de un solo vistazo la constancia específica de **ese** hábito sin tener que navegar a una pantalla de estadísticas separada.

---

## 4. Flujo de Subida de Evidencia (Requerimiento Cátedra)

- Dentro de cada tarjeta de hábito, un icono discreto de clip `[📎]` permite abrir un drawer inferior (*bottom sheet* en móvil) para:
  1. Agregar una nota o reflexión del día.
  2. Subir un archivo de comprobante (foto de lectura, captura de código, comprobante o PDF).
- Si la tarjeta ya tiene archivo adjunto, muestra una miniatura badge con acceso directo para visualizarlo o reemplazarlo.
