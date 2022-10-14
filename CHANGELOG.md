# Todos los cambios realizados al proyecto se deben documentar en este archivo

este archivo esta basado en el formato Keep a Changelog(https://keepachangelog.com/es-ES/1.0.0/)

## [2.1.1 - Cometidos - 14-09-2022]

### Changed

- Se ajusto la grilla de las resoluciones para que quedaran en el punto 1.
- Se ajusto el salto de linea generado cuando hay mas de 3 destinos y el cometido es con atraso, lo que generaba una 3ra hoja.
- Se cambio la vista de Detalles Finanzas ajustando los modulos VII y VIII junto con el modulo de Destinos para ajustar la visualizacion de los detalles.
- Se modifico el boton de reinicio de proceso, para que este solo sea visible para Gestión de Personas y Administradores.

### Added

- Se agrego Historial en la vista de Finanzas.

## [2.1 - Cometidos - 12-09-2022]

### Changed

- Se modifico la fecha en la sección Detalles de los Destinos en la vista de Gestion de Personas.
- Se modifico la vista de los documentos en Finanzas.
- Se modifico en area de presupuestos, la visualizacion del modulo Destinos.

### Added

- Se agrego boton switch para poder ocultar o mostrar los documentos del modulo de documentos adjuntos.
- Se agrego validación de atraso en la tarea de Aprobacion y Generación Documento Gestión Personas.

## [2.0.1.1 - Cometidos - 30-09-2022]

### Changed

- Se modifico salto de linea aplicado a en las resoluciones de cometidos para personal a contrata.
- Se habilito smtp para envio de correos en Gestion Documental.

### Added

- Se agrego validacion para falta de documento en la tarea Encargado de Presupuestos

## [2.0.1 - Cometidos - 29-09-2022]

### Changed

- Se cambio el comportamiento al anular el cometido el cual era anulado pero seguia siendo visualizable en la Bandeja de Tareas.
- Se descomento la query de busqueda en el Proceso Consultor.

## [1.3.6 - Cometidos - 26-06-2022]

### Changed

- Se corrige error de fecha al momento de agregar un pasaje en el primer paso.
- Se corrige comportamiento del boton "Eliminar Documento" cuando este esta firmado.

## [1.3.5 - Cometidos - 09-09-2022]

### Changed

- Se modifico el flujo de Aprobacion de jefatura en el caso del ministro y la subsecretaria cuando el cometido viene con atraso. La visación ahora se autoriza al momento de aprobar la jefatura.
- se modifico la validacion de firma de documento para el Encargado de Presupuesto.
- Cambios visuales del boton de eliminar y Generar documento en Refrendacion.

## [1.3.4 - Cometidos - 30-08-2022]

### Added

- Se creo nueva tarea al proceso de cometidos, la numero 24 Visación de Subsecretaria, es una tarea exclusiva de aprobacion para el Ministro en caso de atraso.
- Se agrego modal de detalles en EditSigfe y EditSigfeTesoreria.

## [1.3.4 - Cometidos - 25-08-2022]

### Changed

- Se modifico control de errores en caso de no llenar el la justificacion de atraso cuando es Ministro/a o Subsecretaria/o.

## [1.3.4 - Cometidos - 23-08-2022]

### Added

- Se agrego nuevo flujo, Visación Ministro.
- Se creo nueva vista y nuevo ActionResult en el controlador de Cometido

### Changed

- Se cambiaron vistas en las tablas que se desbordaban, EditGP, EditAbast donde se reemplazo la grilla por un modal.

## [1.3.3 - Cometidos - 18-08-2022]

### Changed

- Se ajustaron las vistas de Tesoreria y Contabilidad para ajustarse al estilo elegido para Cometidos.
- Se modifico flujo erroneo desde Visación si este tenia pasajes.
- Se modifico Resolucion y Orden para que ahora muestren los parrafos que corresponden según bool de Atraso.

## [1.3.2 - Cometidos - 10-08-2022]

### Added

- Se agregaron filtros para los viaticos ajustando el sistema al Decreto 90.

## [1.3.1 - Cometidos - 09-08-2022]

### Changed

- Se modifico comportamiento al momento de crear un Cometido que luego de llenar los datos y guardarlos, borraba la justificacion de atraso y dejaba como False el campo Atrasado, cambiando los flujos en adelante.
- Se cambio la validación de atraso al momento de Guardar el cometido en el primer paso.

## [1.3.0 - Cometidos - 09-08-2022]

### Added

- Se creo nuevo flujo según BMPN de cometidos, ahora independiente de si es con o sin pasaje el cometido pasa por jefatura, si viene con atraso por aprobación de Gabinete y luego a Gestión de Personas o Abastecimiento si es con pasaje.
- Se agrego nueva vista de aprobacion de GAbinete.
- Se agrego vista parcial para un modal, el cual hasta el momento no funciona, pero se trabajo.

### Changed

- Se modificaron los botones en las vistas de pasaje de abasticimiento para que estuvieran a la par con Materialize.
- Se modifico la vista del Collapsable, ahora todas ocupan Materialize y no jQuery Accordion.

## [1.2.0 - Cometidos] - 16-06-2022

### Changed

- Se modifico la consulta realizada para obtener el documento de la cotización.
- Se modifico la vista de los Documentos, ahora siempre que un perfil de analista ingrese un documento, aparecera el boton "Eliminar".

### Added

- Se agrego control de anulación, para que no se pueda anular un proceso una vez pase de el primero paso del flujo (Ingreso de Solicitud).
- Se agrego formulario de justificacion junto con el dato JustificacionAtraso en la DB y el Modelo.
- Se agrego función para validar que no falte ingresar la justificacion del atraso, en caso de estar nulo, cuando corresponde, avita que la tarea avance.
- Se agrego SolicitudCometido al enum CometidoSecuencia.
- Se creo funcion para validar si un cometido viene o no con atraso de 20 dias de ingreso.
- Se agrego en las correspondientes vistas de los flujos de cometidos, el modulo de Justificacion de Atraso.

## [1.2.0 - Cometidos] - 10-06-2022

### Changed

- Se modifico la vista de Pasajes en el cometido para que esta no colapsara al mostrar la falta de fecha.
- Se modifico manejo de error cuando se elimina el pasaje.

### Added

- Se agrego error al enviar la tarea al siguiente flujo cuando no se agrego un pasaje al cometido.

## [1.2.0 - Cometidos] - 08-06-2022

### Changed

- Se modifico el funcionamiento y el color del boton Eliminar Documento.
- Se modifico la falla ortografica en los detalles de Cometido, cuando se debe ingresar el ID del Compromiso.
- Se modifico la vista EditSigfe y de EditSigfeTesoreria para que mostrara el Historial.
- Se modifico el comportamiento de el envio de las tareas para que estas pudieran rechazar o aprobar y en caso de fallar, mostrar el mensaje correspondiente.

### Added

- Se agrego la barra de tareas en la vista se firma de documento.
- Se agrego una variable de proceso en el controlador de Workflow en el metodo Send.
- Se agrego mensaje de error en la creación de la cotización.
- Se agrego un nuevo enum, CometidoSecuencia.

## [1.2.0 - Cometidos] - 16-05-2022

### Changed

- Se modifica en la vista Documentos, un boton para "eliminar" el documento, siempre y cuando la tarea sea de Gestion de Personas.
- Se agrega ActionResult DeleteDocumento al controlador de Documentos.
- Se agrega en la vista EditGP regiones para poder visualizar mejor las secciones en codigo.

## [1.2.0 - Cometidos] - 04-05-2022

### Added

- Se crea vista ProcessState.cshtml en la carpeta de Cometidos.

### Changed

- Se agrega metodo ProcessState al controlador de Cometido.
- Se modifica referencia para incluir como partial View al archivo ProcessState.cshtml.
- Se modifica el script de jquery del Edit.cshtml de Cometido para incluir verificacion de fechas origen y retorno. Aun no funcional.

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
