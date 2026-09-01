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
});
