# Todos los cambios realizados al proyecto se deben documentar en este archivo

este archivo esta basado en el formato Keep a Changelog(https://keepachangelog.com/es-ES/1.0.0/)

## [1.2.0 - Cometidos] - 02-05-2022

### Added

- Changelog para mantener un control de versiones de Cometidos.
- Se incluyo en el proyecto las carpetas HorasExtra para que el proyecto no arrojara error en ciertos procesos.
- Se incluyo en el proyecto el controlador de Horas Extra.
- Se agrego la patente al pdf de la resolución.
- Se agrego nueva excepción en caso de no encontrar funcionario en SIGPER al momento de ejecutar tarea cuando es "Usuario Especifico".

### Changed

- Se modifico el archivo Resolucion.cshtml ajustandolo a las nuevas Resoluciones ministeriales y de subsecretaria.
- Se modifico el archivo Orden.cshtml ya que arrojaba error al generar la resolución al estar bien codificado el if statement.
- Se modifico el archivo EditGP.cshtml ya que aun no correspondia el cuarto div de Edición Resolución ya que aun se esta desarrollando y no corresponde a este flujo de momento.
