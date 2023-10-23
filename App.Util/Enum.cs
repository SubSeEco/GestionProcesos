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
            DenunciaIntegridad,
            ConsultaIntegridad
        }

        public enum Grupo
        {
            Administrador,
            Operador,
            Consultor,
            Cometido,
            Remuneraciones
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
            ParrafoAtraso = 16,
            SegundoParrafoAtraso = 17,
            #region Desarrollo
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
            #endregion
        }
        public enum TipoAprobacion
        {
            SinAprobacion = 1,
            Aprobada = 2,
            Rechazada = 3,
        }

        public enum TipoDocumento
        {
            Resolucion = 1,
            CDPViatico = 2,
            CDPPasaje = 3,
            Contabilidad = 4,
            Tesoreria = 5,
            Otros = 6,
            RefrendacionPresupuesto = 7,
            Memorandum = 8,
            ProgramacionHorasExtraordinarias = 9,
            ResoluciónProgramaciónHorasExtraordinarias = 10,
            ComprobaciónHorasExtraordinarias = 11,
            ResoluciónComprobaciónHorasExtraordinarias = 12,
            ResoluciónConfirmacionHorasExtraordinariasPagadas = 13,
            ResoluciónConfirmacionHorasExtraordinariasCompensadas = 14,
            DocumentosGenéricos = 15,
            ResolucionRevocatoriaCometido = 16
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
            PlantillaAprobacionCometidoJustificacionAtraso = 67,
            PlantillaRechazoCometidoJustificacionAtraso = 68,
            PlantillaAprobacionJuridica = 69,
            PlantillaRechazoJuridica = 70,
            PlantillaAprobacionGabineteSubsecretario = 73,
            PlantillaRechazoGabineteSubsecretario = 74,
            PlantillaAprobacionGabineteMinistro = 75,
            PlantillaRechazoGabineteMinistro = 76,
            PlantillaAprobacionFirmaMinistro = 77,
            PlantillaRechazoFirmaMinistro = 78,
            PlantillaAprobacionFirmaSubsecretario = 78,
            PlantillaRechazoFirmaSubsecretario = 80,

            /*Plantillas de correo --> horas extras*/
            PlantillaHorasExtras = 65,
            CorreoOfPartes = 66,
        }

        public enum DefinicionWorkflow
        {
            //Workflows Cometido
            SolicitudCometido = 72,

            #region Workflows Integridad

            IngresoDenuncia = 261,
            ValidacionCoordinador = 262,
            ValidacionCoordinadorSup = 266,
            ValidacionJefeJuridica = 263,
            ValidacionJefeJuridicaSup = 265,
            ValidacionAbogado = 264,
            ValidacionInvertigador = 267,
            ValidaAbogadoPostInvestigador = 268,

            #endregion
        }

        public enum CometidoSecuencia
        {
            SolicitudCometido = 1,
            AprobacionJefatura = 2,
            IngresoCotizacion = 3,
            RevisaCotizacion = 4,
            CompraPasaje = 5,
            AprobacionDocGP = 6,
            AprobacionJefaturaGP = 7,
            AnalistaPresupuesto = 8,
            EncargadoPresupuesto = 9,
            AprobacionJuridica = 10,
            AprobacionSubse = 11,
            AprobacionMinistro = 12,
            FirmaActoAdministrativo = 13,
            FirmaMinistro = 14,
            FirmaSubsecretario = 15,
            AnalistaContabilidad = 16,
            EncargadoContabilidad = 17,
            AnalistaTesoreria = 18,
            EncargadoTesoreria = 19,
            EncargadoFinanzas = 20,
            IngresoPagoTesoreria = 21,
            VisacionSubsecretaria = 22,
            VisacionMinistro = 23,
            VisacionSubseMinistro = 24,
        }

        public enum DefinicionProceso
        {
            InformeHSA = 1,
            GDInterno = 4,
            GDOficParte = 5,
            SIACIngreso = 7,
            SolicitudCometido = 10,
            SolicitudPasaje = 11,
            SolicitudCometidoPasaje = 13,
            Memorandum = 15,
            ProgramacionHorasExtraordinarias = 16,
            HorasExtras = 19,
            GeneraResolucionHE = 20,
            #region Desarrollo
            SolicitudFirma = 14,
            ConsultaIntegridad = 36,
            DenunciaIntegridad = 37,
            #endregion
            //SolicitudFirma = 1013,
            //DenunciaIntegridad = 1016,
            //ConsultaIntegridad = 1017
        }


        //public enum Cometidos
        //{
        //    DiasAnticipacionIngreso = 7,
        //}

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

        public static List<DateTime> Feriados => new List<DateTime> {
            new DateTime(2020,09,18),
            new DateTime(2020,09,19),
            new DateTime(2020,10,12),
            new DateTime(2020,10,25),
            new DateTime(2020,10,31),
            new DateTime(2020,11,01),
            new DateTime(2020,11,29),
            new DateTime(2020,12,08),
            new DateTime(2020,12,25),
            new DateTime(2021,01,01),
            new DateTime(2021,04,02),
            new DateTime(2021,05,21),
            new DateTime(2021,06,21),
            new DateTime(2021,06,28),
            new DateTime(2021,07,16),
            new DateTime(2021,09,17),
            new DateTime(2021,09,18),
            new DateTime(2021,09,19),
            new DateTime(2021,10,11),
            new DateTime(2021,11,01),
            new DateTime(2021,12,08),
            new DateTime(2021,12,31),
        };
    }
}