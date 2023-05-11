# Todos los cambios realizados al proyecto se deben documentar en este archivo

este archivo esta basado en el formato Keep a Changelog(https://keepachangelog.com/es-ES/1.0.0/)

## [2.6.2.1 - Cometidos - 11-05-2023]

### Deleted

- Se eliminan los cambios realizados en patentes de cometidos que se pasaron por error a produccion.

## [2.6.2 - Integridad - 10-05-2023]

### Changed

- Se modificaron los dropdownlist en Edit y Create.

## [2.6.2 - Integridad - 09-05-2023]

### Changed

- Se modifico la vista de Edit para que tome la fecha en Fecha Hecho.
- Se modifico el Modelo Denuncia.

### Added

- Se agrego validacion de Nivel Jerarquico y Tipo Atentado.

## [2.6.1 - Cometidos - 08-05-2023]

### Changed

- Se modifico el comportamiento de las vistas de EditGP y Edit para el modulo de patentes.

## [2.6.1 - Integridad - 05-05-2023]

### Changed

- Se modificaron las vistas del flujo de Denuncia Integridad.
- Se modificaron los checkbox y radiobuttons de l a vista create y edit de Denuncia

### Added

- Se agregaron nuevas columnas en la tabla Denuncia.
- Se agrego al web.config conexión a DB Local de Gestion de Procesos.

## [2.6.1 - Integridad - 27-04-2023]

### Changed

- Se realizan cambios visuales en Valida: Abogado, Coordinador, Jefatura y vista View del Proceso Consulta Integridad.

## [2.6.1 - Integridad - 26-04-2023]

### Changed

- Se realizan cambios visuales en Create de Consulta.
- Se modifica ingreso de Consultas y Denuncias para que estas sean siempre Procesos Reservados.

## [2.5.9 Beta - GP Completo - 14-03-2023]

### Added

- Se creo Reporte Génerico GD, el cual solo mostrara los procesos y tareas que sean Gestion Documental.
- Se crearon las vistas "View" en Consulta y Denuncia de Integridad ya que al inspeccionar el proceso en el buscador de procesos, arrojaba error ya que la vista no existia.

### Changed

- Se modifico el reporte Génerico, agregando la unidad como selector y cambio el nombre a "Reporte Procesos Génerico".
- Se cambiaron las referencias a variables no ocupadas en el flujo de las vistas de Denuncia Integridad.

## [2.5.9 Alpha - GP Core - 27-02-2023]

### Added

- Se agrego el proyecto de Integridad al core de GP.

### Changed

- Se cambio la ruta de las vistas parciales a carpetas individuales para que sea mas facil encontrarlas.

## [2.5.9 Alpha - Gestión Procesos - 10-03-2023]

### Changed

- Se realizaron cambios visuales con los colores de los badge y los circulos de los estados en GD.

## [2.5.9 Alpha - Gestion Procesos - 09-03-2023]

### Added

- Se creo filtro de tareas en el Home para Gestion Documental, tanto personal como grupal.
- Se creo boton de plazo para los documentos que requieren firma.

### Changed

- Se modifico, en GD Oficina de partes, el modulo para que si un documento que es oficial requiere plazo de tramitación.

## [2.5.9.1 Alpha - Cometidos - 07-03-2023]

### Changed

- Se realizo corrección al reporte CDCFinanzas, cambiando la llamada de las tareas por la lista de tareas dentro del proceso.

## [2.5.9 - GP - 21-02-2023]

### Added

- Se creo nuevo modulo de Reportes de procesos por unidad.
- Se creo reporte por unidad segun el usuario que esta conectado.
- Se creo reporte de procesos pendientes por unidad segun el usuario conectado.

## [2.5.9.1 - Cometidos - 17-02-2023]

### Added

- Se agregaron las localidades entregadas por Procesos en Excel a las cuales les corresponde viatico según Decreto 90.

## [2.5.9.1 Alpha - Cometidos - 22-02-2023]

### Added

- Se creo metodo SwitchLocalidad, con la finalidad de hacer mas modular la insercion de las localidades.

### Changed

- Se modifico y corrigio el bug del switch del DestinoInsert para que no agregara todos los destinos como adjacentes.
- Se modifico DestinoUpdate agregandose el nuevo metodo creado, SwitchLocalidad.

## [2.5.9 Alpha - Cometidos - 15-02-2023]

### Added

- Se agregaron al UseCase de cometidos lo metodos post para Edit, Create y Delete.

### Changed

- Se modificaron los modal de Create y Delete para que funcionen como vista modal desde el index.
- Por problemas configuración del DropdownList, se dejo el Edit como una vista externa y no como modal.
- Se agrego Codigo para poder hacer referencia del ID de Región.
- Se modifico la vista de Horas Extra para que los botones fueran con el mismo estilo que el resto de GP.

## [2.5.8 Alpha - Cometidos - 05-01-2023]

### Added

- Se creo vista del la configuracion de patentes.

### Changed

- Se modifico el controlador de la configuracion de las patentes. Ahora llama a la base de dato y carga los datos en la vista.
- Se cambio el nombre de la carpeta, y archivos de configuracion Patente por PatenteVehiculo.
- Se cambio la ruta de la configuracion de patentes.

## [2.5.8 Alpha - Cometidos - 03-01-2023]

### Added

- Se creo modulo de mantencion de patentes.
- Se creo consulta JSON para obtener las patentes y cargalas de manera "dinamica" en Create, Edit y EditGP.
- Se creo tabla PatenteVehiculo y sus respectivos modelos y llamadas.

## [2.5.8 - Cometidos-GP - 19-01-2023]

## Changed

- Se modifico DestinoController para que aceptara el grado 5R.
- Se modifico lista de tareas de GDExterno y GDInterno para que no desplegara las tareas anuladas.

## [2.5.7.2 - Cometidos - 18-01-2023]

## Changed

- Se modifico la query del Reporte de Finanzas, optimizando el tiempo de ejecución.
- Se modifico la ejecucion del reporte de contraloria, agregando el filtro al index de reportes.

## [2.5.7.1 - Cometidos y Gestión Procesos - 07-12-2022]

### Changed

- Se aumento el tamaño de los documentos adjuntos, tanto en GD interno, GD externo como en Cometidos.
- Se elimino la validacion de la patente.

## [2.5.7 - Cometidos y Gestion Procesos - 29-11-2022]

### Added

- Se creo validación de limite de tamaño para los documentos subidos los sistemas.

### Changed

- Se modifico web.config para que soporte como maximo 100MB.

## [2.5.6 - Cometidos - 25-11-2022]

### Changed

- Se modifica carga de la vista en Tareas Grupales

### Added

- Se agregaron patentes a las resoluciones faltantes dejandolas en Mayusculas.

## [2.2.5 - Gestion Procesos - 22-11-2022]

### Added

- Se agrego vista Informe HSA en Tareas Grupales.

### Changed

- Se modifico libreria a su version 6.0.0 ya que la anterior estaba deprecada.

## [2.2.4 - Gestion Procesos - 17-11-2022]

### Added

- Se crearon vistas parciales para los modulos del collapse del Home de la bandeja de tareas.
- Se crearon nuevos valores en Enum DefinicionProcesos.

### Changed

- Se cambio la vista del home de Workflow para que ahora sea mas modular y facil de mantener.

## [2.2.3 - Cometidos - 17-11-2022]

### Changed

- Se cambio link para crear certificado en presupuesto por un boton.
- Se corrigio el flujo desde jefa de servicio a gabinete en caso de que viaje la primera y el rechazo de este paso ya que se derivaba de forma erronea.

## [2.0.1 - Gestion Documental - 17-11-2022]

### Changed

- Se modifico la lsita de la tabla de workflow para que ahora se muestre, por defecto, de forma descendente por el ID.

## [2.2.2 - Cometidos - 10-11-2022]

### Added

- Se creo fix para que los cometidos que pertenecen a Paula Cattan, lleguen a la bandeja de la Jefa de Gabinete.

## [2.2.2 - Cometidos - 09-11-2022]

### Changed

- Se hizo un refactor del metodo de notificacion y avance de las tareas.
- Se actualizo el package "System.Drawing.Common" el cual presentaba problemas de vulnerabilidad.
- Se cambio la vista en Workflow/Sign.

### Added

- Se agregaron plantillas de notificacion en la base de datos y en el Enum.cs.

## [2.2.0 - Cometidos - 27-10-2022]

### Changed

- Se realizaron cambios visuales en la firma de documentos.
- Se cambio la vista del modulo Documentos en Tesoria y Contabilidad.

## [2.1.1 - Cometidos - 21-10-2022]

### Changed

- Se corrigio duplicidad de tareas en al momento de mandar los correos pese a que no encontraba a los usuarios para notificar, ahora en caso de fallar, no se crea la nueva tarea mostrando un mensaje de error.
- Se ajusto la vista de Tesoreria y Contabilidad para mostrar los documentos que los encargados deben firmar siendo solo estos los pdf.

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
