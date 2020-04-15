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
    }
}