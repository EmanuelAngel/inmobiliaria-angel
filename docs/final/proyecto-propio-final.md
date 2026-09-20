---
fecha-tentativa: 2026-11-06
---

# Objetivo

Desarrollar un sistema utilizando cualquier framework backend (express.js, asp.net core, etc) sumado a cualquier framework frontend dedicado (react, vue, angular, svelte, etc) sobre una temática cualquiera a elección propia (no se está ligado a dictado por la cátedra).

## Requerimientos mínimos

- Al menos 4 clases/tablas relacionadas entre si con al menos una relación 1 a muchos.
- Seguridad con login. Uso de Authorize y roles. Alguna funcionalidad restringida por rol. Avatar en los usuarios.
- Uso de archivos, adicional al avatar en el usuario.
- Al menos un CRUD debe estar hecho con el framework frontend elegido y toda su funcionalidad vía ajax.
- Listados con paginado (salvo tablas muy chicas que no crecen con el tiempo). No se debe traer todos los elementos y paginar eso. Puede ser via ajax o con peticiones tradicionales, pero se debe servir cada página por petición.
- Al seleccionar entidades relacionadas (como propietarios en CRUD de inmuebles) debe hacerse con búsqueda vía ajax. No se deben traer todos los elementos.
- Algún uso de API con JWT. Si la aplicación no tiene un uso natural, se puede compartir una API vía postman o similar.

## Entregables

Se debe entregar enlace al repositorio, incluyendo:

- Archivo gitignore correspondiente al proyecto.
- Diagrama de entidad-relación o de clases.
- El archivo readme.md con la descripción del proyecto.
- Especificar donde está desarrollado cada punto de los requerimientos mínimos (solo si está terminado para promocionar).
- Base de datos (solo si está terminado para promocionar).
- Un usuario de cada rol (solo si está terminado para promocionar).
- Colección de postman o similar para probar la API (solo si está terminado para promocionar).

## Presentación

Luego del cierre de la tarea, se harán las presentaciones y defensas de los proyectos que estén listos para promocionar.
