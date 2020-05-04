namespace App.Util
{
    public static class Enum
    {
        public enum Entidad
        {
            CDP,
            HSA,
            CuentaRed,
            Contrato,
            InformeHSA,
            GDIngreso,
            SIACSolicitud,
            RadioTaxi,
            Cometido,
            Pasaje,
            Comision,
            CometidoPasaje,
            FirmaDocumento,
        }

        public enum Grupo
        {
            Administrador,
            Operador,
            Consultor,
            Cometido,
        }

        public enum TipoWorkflow
        {
            Create = 1,
            Edit = 2,
            Details = 3,
            Sign = 4,
            Validate = 5,
            Evaluate = 6,
            Finish = 7,
            AskArchive = 8
        }

        public enum Firmas
        {
            Orden = 1,
            Firmante = 2,
            CargoFirmante = 3,
            OrdenSubse = 4,
            FirmanteSubse = 5,
            CargoSubse = 6,
            OrdenMinistro = 8,
            FirmanteMinistro = 9,
            CargoMinistro = 10,
            VistosRME = 11,
            VistoRAE = 12,
            VistoOP = 13,
            DejaseConstancia = 14,
            ViaticodeVuelta = 15,
        }
        public enum TipoAprobacion
        {
            SinAprobacion = 1,
            Aprobada = 2,
            Rechazada = 3,
        }
        public enum TipoEjecucion
        {
            CualquierPersonaGrupo = 1,
            EjecutaQuienIniciaElProceso = 2,
            EjecutaPorJefaturaDeQuienIniciaProceso = 3,
            EjecutaDestinoInicial = 4,
            EjecutaGrupoEspecifico = 5,
            EjecutaUsuarioEspecifico = 6,
            EjecutaPorJefaturaDeQuienEjecutoTareaAnterior = 7,
//            EjecutaDestinoGD = 8,
            EjecutaJefaturaDeFuncionarioQueViaja = 9,

        }
        public enum Configuracion
        {
            PlantillaCorreoNotificacionTarea = 1,
            AsuntoCorreoNotificacionTarea = 2,
            HSMUser = 4,
            HSMPassword = 5,
            PlantillaCorreoArchivoTarea = 7,
            PlantillaCorreoNuevoProceso = 8,
            PlantillaCorreoProcesoAnulado = 9,
            PlantillaCorreoCambioEstado = 10,
            PlantillaFirmaResolucion = 11,
            PlantillaNotificacionPago = 12,
            /*PLANTILLAS DE CORREOP --> COMETIDOS*/
            PlantillaEnvíoSolicitudCometido = 13,
            AsuntoSolicitudCometido_Solicitante_QuienViaja = 14,
            PlantillaEnvíoSolicitudCometidoJefatura = 15,
            AsuntoSolicitudCometido_Jefatura = 17,
            PlantillaAprobaciónRechazoCometidoJefatura_Solicitante_QuienViaja = 19,
            PlantillaAprobaciónRechazoCometidoJefatura_Jefatura = 20,
            PlantillaAprobaciónRechazoCometidoJefatura_GP = 22,
            PlantillaRechazoCometidoJefatura_Solicitante_QuienViaja = 23,
            PlantillaRechazoCometidoJefatura_Jefatura = 24,
            PlantillaGeneraciónDocumento = 25,
            PlantillaReasignacionSolicitud = 26,
            PlantillaEncargadoGP_AnalistaPpto = 27,
            PlantillaEncargadoGP_AnalistaGP = 28,
            PlantillaAnalistaPppto_JefePpto = 29,
            PlantillaAnalistaPppto_AnalistaGP = 30,
            PlantillaEncargadoPPto_JefaturaAdmin = 31,
            PlantillaEncargadoPPto_AnalistaPpto = 32,
            PlantillaEncargadoDeptoAdmin_OfPartes = 33,
            PlantillaEncargadoDeptoAdmin_Solicitante_QuienViaja = 34,
            PlantillaEncargadoDeptoAdmin_AnalistaConta = 35,
            PlantillaEncargadoDeptoAdmin_JefePpto = 36,
            PlantillaEncargadoDeptoAdmin_AnalistaGP = 37,
            PlantillaAnalistaConta_JefeConta = 38,
            PlantillaAnalistaConta_AnalistaGP = 39,
            PlantillaAnalistaConta_EncargadoConta = 40,
            PlantillaEncargadoConta_AnalistaTesoreria = 41,
            PlantillaAnalistaTesoreria_JefeTesoreria = 42,
            PlantillaAnalistaTesoreria_AnalistaGP = 43,
            PlantillaAnalistaTesoreria_EncargadoTesoreria = 44,
            PlantillaEncargadoTesoreria_EncargadoFinanzas = 45,
            PlantillaEncargadoTesoreria_EncargadoFinanzas2 = 46,
            PlantillaFinanzas_Solicitante_QuienViaja = 47,
            PlantillaFinanzas_Solicitante_QuienViaja2 = 48,
            UrlSistema = 50,
        }

        public enum DefinicionProceso
        {
            SIACIngreso = 7,
            SolicitudPasaje = 11,
            SolicitudCometidoPasaje = 13,
        }

        public enum Estadoorganizacion
        {
            EnConstitucion = 1,
            Vigente = 2,
            Disuelta = 3,
            Inexistente = 4,
            Cancelada = 5,
            RolAsignado = 6
        }
        public enum TipoOrganizacion
        {
            Cooperativa = 1,
            AsociacionGremial = 2,
            AsociacionConsumidores = 3,
            AunNoDefinida = 4
        }

        public enum Cometidos
        {
            DiasAnticipacionIngreso = 7,
        }

        public enum EstadoProceso
        {
            EnProceso = 1,
            Anulado = 2,
            Terminado = 3,
        }
    }
}