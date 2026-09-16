// Instancia global de Notyf configurada con la paleta arquitectónica del proyecto
window.notyf = typeof Notyf !== 'undefined' ? new Notyf({
    duration: 4000,
    position: { x: 'right', y: 'bottom' },
    dismissible: true,
    types: [
        {
            type: 'success',
            background: '#166534',
            icon: {
                className: 'bi bi-check-circle-fill',
                tagName: 'i',
                color: '#ffffff'
            }
        },
        {
            type: 'error',
            background: '#991b1b',
            duration: 0,
            dismissible: true,
            icon: {
                className: 'bi bi-exclamation-triangle-fill',
                tagName: 'i',
                color: '#ffffff'
            }
        },
        {
            type: 'warning',
            background: '#b45309',
            icon: {
                className: 'bi bi-exclamation-circle-fill',
                tagName: 'i',
                color: '#ffffff'
            }
        },
        {
            type: 'info',
            background: '#2b506e',
            icon: {
                className: 'bi bi-info-circle-fill',
                tagName: 'i',
                color: '#ffffff'
            }
        }
    ]
}) : null;

document.addEventListener("DOMContentLoaded", function () {
    // Tooltips
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

    // Selector de tema claro / oscuro / sistema
    (function () {
        const ICONS = { light: 'bi-sun-fill', dark: 'bi-moon-stars-fill', auto: 'bi-circle-half' };
        const icon = document.getElementById('themeIcon');

        function applyTheme(theme) {
            const resolved = theme === 'auto'
                ? (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light')
                : theme;
            document.documentElement.setAttribute('data-bs-theme', resolved);
            localStorage.setItem('theme', theme);
            if (icon) {
                icon.className = 'bi ' + ICONS[theme];
            }
        }

        // Inicializar ícono con el tema guardado
        const saved = localStorage.getItem('theme') || 'auto';
        if (icon) icon.className = 'bi ' + ICONS[saved];

        // Clicks en las opciones del dropdown
        document.querySelectorAll('[data-theme]').forEach(function (btn) {
            btn.addEventListener('click', function () {
                applyTheme(this.dataset.theme);
            });
        });

        // Reaccionar a cambio del sistema operativo cuando está en modo auto
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function () {
            if ((localStorage.getItem('theme') || 'auto') === 'auto') {
                applyTheme('auto');
            }
        });
    })();

    // Alternar visibilidad de contraseña (botón ojo)
    document.addEventListener('click', function (e) {
        const toggleBtn = e.target.closest('[data-password-toggle]');
        if (!toggleBtn) return;

        e.preventDefault();
        const inputGroup = toggleBtn.closest('.input-group');
        const targetSelector = toggleBtn.getAttribute('data-target');
        const input = targetSelector
            ? document.querySelector(targetSelector)
            : (inputGroup ? inputGroup.querySelector('input') : null);

        if (!input) return;

        const isPassword = input.type === 'password';
        input.type = isPassword ? 'text' : 'password';

        const icon = toggleBtn.querySelector('i');
        if (icon) {
            icon.classList.toggle('bi-eye', !isPassword);
            icon.classList.toggle('bi-eye-slash', isPassword);
        }

        toggleBtn.setAttribute('aria-label', isPassword ? 'Ocultar contraseña' : 'Mostrar contraseña');
    });
});
