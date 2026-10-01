# Design Brief: HabitHub Dashboard (Mobile-First)

Documento de especificación de interfaz y UX para la propuesta de Habit Tracker, derivado del proceso de descubrimiento con la skill **Impeccable**.

---

## 1. Dirección Visual y Principios de Diseño

| Parámetro | Definición |
|---|---|
| **Enfoque de diseño** | **Mobile-First**: layout vertical de una sola columna apilada, optimizado para uso rápido con una sola mano. |
| **Estilo estético** | **Estructurado, uniforme y limpio**: tarjetas de ancho completo apiladas verticalmente, con idéntica anatomía y altura consistente. |
| **Tema visual** | **Modo Claro (Light Theme) como principal**: fondos claros con sombras suaves, bordes nítidos y badges de acento temáticos por categoría. |
| **Navegación** | Top Bar fija con saludo, fecha y avatar de usuario. Bottom Nav flotante reducida a 3 secciones clave: *Today*, *Habits*, *Articles*. |

---

## 2. Topología de Navegación

### A. Pantallas Móviles (< 768px)
- **Top App Bar:** Saludo personalizado (*"Good morning, Alex!"*), fecha actual y avatar del usuario a la derecha (elimina la necesidad de pestaña de perfil en la barra inferior).
- **Filtros de categoría:** Chips horizontales compactos (*All*, *Health*, *Work*, *Study*).
- **Feed principal:** Pila vertical de tarjetas uniformes (una debajo de la otra), con espaciado constante y sin layouts asimétricos.
- **Bottom Navigation Bar (3 destinos):**
  1. 📅 **Today:** Dashboard de check-ins diarios.
  2. 📋 **Habits:** Listado general, configuración y altas de hábitos.
  3. 📰 **Articles:** Blog de ciencia del hábito curado por el Admin.

---

## 3. Anatomía de la Tarjeta Uniforme (Últimos 7 Días)

Cada tarjeta comparte una estructura geométrica idéntica para evitar sobrecarga cognitiva:

```
┌──────────────────────────────────────────────────────────┐
│ [Salud]  Hydration                         [-] 6/8 cups [+]│  <-- Header, Categoría y Stepper
├──────────────────────────────────────────────────────────┤
│   Mon    Tue    Wed    Thu    Fri    Sat    Sun          │  <-- Tira semanal de 7 días
│  [ ✔ ]  [ ✔ ]  [ ✔ ]  [ ✔ ]  [ ✔ ]  [ ✔ ]  [ ✔ ]         │  (Mini-heatmap de consistencia)
└──────────────────────────────────────────────────────────┘
```

- **Mini-heatmap semanal:** Solo muestra los **últimos 7 días** (Lunes a Domingo) en microcuadros redondeados con check de completitud. 
- **Heatmap histórico (30/60/365 días):** Se delega a la vista de detalle de cada hábito al tocar la tarjeta, manteniendo el feed diario limpio y veloz.

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
