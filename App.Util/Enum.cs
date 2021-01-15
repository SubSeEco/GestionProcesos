using System;
using System.Collections.Generic;

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
            GDInterno,
            GDExterno,
            SIACSolicitud,
            RadioTaxi,
            Cometido,
            Pasaje,
            Comision,
            CometidoPasaje,
            FirmaDocumento,
            Memorandum,
            ProgramacionHorasExtraordinarias,
            HorasExtras,
            GeneraResolucion,
            FirmaDocumentoGenerico,
        }

        public enum Grupo
        {
            Administrador,
            Operador,
            Consultor,
            Cometido,
            Remuneraciones
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
            OrdenHEProg = 16,
            FirmanteHEProg = 17,
            CargoFirmanteHEProg = 18,
            DistribucionHEProg = 19,
            VistosHEProg = 20,
            OrdenHEPag = 21,
            FirmanteHEPag = 22,
            CargoFirmanteHEPag = 23,
            DistribucionHEPag = 24,
            VistosHEPag = 25, 
            OrdenHECom = 26,
            FirmanteHECom = 27,
            CargoFirmanteHECom = 28,
            DistribucionHECom = 29,
            VistosHECom = 30,
            InicialesRHPagadas = 31,
            InicialesRHCompensadas = 32,
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
            EjecutaSecretariaUnidadOrigen = 10,
            EjecutaSecretariaUnidadDestino = 11

        }
        public enum Configuracion
        {
            url_tramites_en_linea,
            plantilla_nuevo_proceso,
            plantilla_fin_proceso,
            plantilla_anulacion_proceso,
            autoridades,

            PlantillaNuevaTarea = 1,
            AsuntoCorreoNotificacion = 2,
            HSMUser = 4,
            HSMPassword = 5,
            PlantillaCorreoArchivoTarea = 7,
            PlantillaCorreoCambioEstado = 10,
            PlantillaFirmaResolucion = 11,
            PlantillaNotificacionPago = 12,


            /*PLANTILLAS DE CORREO --> COMETIDOS*/
            PlantillaEnvíoSolicitudCometido = 13,
            AsuntoSolicitudCometido_Solicitante_QuienViaja = 14,
            PlantillaEnvíoSolicitudCometidoJefatura = 15,
            AsuntoSolicitudCometido_Jefatura = 16,
            PlantillaAprobaciónRechazoCometidoJefatura_Solicitante_QuienViaja = 17,
            PlantillaAprobaciónRechazoCometidoJefatura_Jefatura = 18,
            PlantillaAprobaciónRechazoCometidoJefatura_GP = 19,
            PlantillaRechazoCometidoJefatura_Solicitante_QuienViaja = 20,
            PlantillaRechazoCometidoJefatura_Jefatura = 21,
            PlantillaGeneraciónDocumento = 22,
            PlantillaReasignacionSolicitud = 23,
            PlantillaEncargadoGP_AnalistaPpto = 24,
            PlantillaEncargadoGP_AnalistaGP = 25,
            PlantillaAnalistaPppto_JefePpto = 26,
            PlantillaAnalistaPppto_AnalistaGP = 27,
            PlantillaEncargadoPPto_JefaturaAdmin = 28,
            PlantillaEncargadoPPto_AnalistaPpto = 29,
            PlantillaEncargadoDeptoAdmin_OfPartes = 30,
            PlantillaEncargadoDeptoAdmin_Solicitante_QuienViaja = 31,
            PlantillaEncargadoDeptoAdmin_AnalistaConta = 32,
            PlantillaEncargadoDeptoAdmin_JefePpto = 33,
            PlantillaEncargadoDeptoAdmin_AnalistaGP = 34,
            PlantillaAnalistaConta_JefeConta = 35,
            PlantillaAnalistaConta_AnalistaGP = 36,
            PlantillaAnalistaConta_EncargadoConta = 37,
            PlantillaEncargadoConta_AnalistaTesoreria = 38,
            PlantillaAnalistaTesoreria_JefeTesoreria = 39,
            PlantillaAnalistaTesoreria_AnalistaGP = 40,
            PlantillaAnalistaTesoreria_EncargadoTesoreria = 41,
            PlantillaEncargadoTesoreria_EncargadoFinanzas = 42,
            PlantillaEncargadoTesoreria_EncargadoFinanzas2 = 43,
            PlantillaFinanzas_Solicitante_QuienViaja = 44,
            PlantillaFinanzas_Solicitante_QuienViaja2 = 45,
            UrlSistema = 46,
            PlantillaFinanzasRechazo_EncargadoTesoreria = 51,
            PlantillaEnvíoSolicitudCometidoPasaje = 52,
            PlantillaEnvioSolicitudAnalistaAbastecimiento = 53,
            PlantillaSeleccionPasaje_Jefatura = 54,
            PlantillaSeleccionPasaje_Solicitante = 55,
            PlantillaAprobacionPasaje_Jefatura = 56,
            PlantillaAprobacionPasaje_JefaturaAbastecimiento = 57,
            PlantillaRechazoPasaje_AnalistaAbastecimiento = 58,
            PlantillaCompraPasajes_AnalistaGP = 59,
            PlantillaCompraPasajes_Solicitante_QuienViaja = 60,
            PlantillaRechazoPasaje_Solicitante_QuienViaja = 61,
            Plantilla_Memo_Firmado = 63,
            PlantillaAnulacionCometido = 62,
            JefeGabineteMinistro = 64,

            /*Plantillas de correo --> horas extras*/
            PlantillaHorasExtras = 65,
            CorreoOfPartes = 66,
        }

        public enum DefinicionProceso
        {
            SIACIngreso = 7,
            SolicitudPasaje = 11,
            SolicitudCometidoPasaje = 13,
            Memorandum = 15,
            SolicitudCometido = 10,
            InformeHSA = 1,
            ProgramacionHorasExtraordinarias = 16,
            HorasExtras = 19,
            GeneraResolucionHE = 20,
        }

     
        public enum Cometidos
        {
            DiasAnticipacionIngreso = 7,
        }

        public enum HorasExtras
        {
            ConstateDias = 190,
            //HorasDiurnas = 1.25,
            //HorasNocturnas = 1.5,
        }

        public enum EstadoProceso
        {
            EnProceso = 1,
            Anulado = 2,
            Terminado = 3,
        }

        public enum Privacidad
        {
            Publico = 1,
            Privado = 2,
        }
    }
}