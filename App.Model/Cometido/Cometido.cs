using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using App.Model.Core;
using App.Model.Shared;
using ExpressiveAnnotations.Attributes;

namespace App.Model.Cometido
{
    [Table("Cometido")]
    public class Cometido : BaseEntity
    {
        /*list de Generacion CDP*/
        [Display(Name = "Lista GeneracionCDP")]
        public virtual List<GeneracionCDP> GeneracionCDP { get; set; } = new List<GeneracionCDP>();

        /*list de destino*/
        [Display(Name = "Lista Destinos")]
        public virtual List<Destinos> Destinos { get; set; } = new List<Destinos>();


        /*Definicion de atributos*/
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Número Cometido")]
        public int CometidoId { get; set; }

        [Required(ErrorMessage = "Se debe indicar la fecha de la solicitud")]
        [Display(Name = "Fecha Solicitud")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaSolicitud { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "Nombre Id")]
        public int? NombreId { get; set; }
        
        [Required(ErrorMessage = "Se debe señalar el RUT del funcionario")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut")]
        public int Rut { get; set; }

        [Required(ErrorMessage = "Se debe señalar el DV del Rut")]
        [StringLength(1, ErrorMessage = "Excede el largo máximo (1)")]
        [Display(Name = "DV")]
        public string DV { get; set; }

        [Required(ErrorMessage = "Se debe señalar el Id de la Uhidad del funcionario")]
        [Display(Name = "Unidad")]
        public int? IdUnidad { get; set; }

        [Required(ErrorMessage = "Se debe señalar el nombre de la unidad de funcionario")]
        [Display(Name = "Unidad")]
        public string UnidadDescripcion { get; set; }

        [Required(ErrorMessage = "Se debe señalar el Id de la calidad jurídica")]
        [Display(Name = "Calidad Jurídica")]
        public int? IdCalidad { get; set; }

        [Required(ErrorMessage = "Se debe señalar la Calidad Jurídica del funcionario")]
        [Display(Name = "Calidad Jurídica")]
        public string CalidadDescripcion { get; set; }

        [Required(ErrorMessage = "Se debe señalar el id del grado (en caso de HSA se debe homologr a un grado de la E.U.S)")]
        [Display(Name = "Grado E.U.S")]
        public string IdGrado { get; set; }

        [Required(ErrorMessage = "Se debe señalar el Grado del funcionario")]
        [Display(Name = "Grado E.U.S.")]
        public string GradoDescripcion { get; set; }       

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la E.U.S)")]
        public string Grado { get; set; }

        [Required(ErrorMessage = "Se debe señalar el Id del cargo")]
        [Display(Name = "Cargo")]
        public int? IdCargo { get; set; }

        [Required(ErrorMessage = "Se debe señalar el Cargo del funcionario")]
        [Display(Name = "Cargo")]
        public string CargoDescripcion { get; set; }
                
        [Display(Name = "Estamento")]
        public int? IdEstamento { get; set; }
        
        [Display(Name = "Estamento")]
        public string EstamentoDescripcion { get; set; }
        
        [Display(Name = "Escalafon")]
        public int? IdEscalafon { get; set; }
        
        [Display(Name = "Escalafon")]
        public string EscalafonDescripcion { get; set; }

        [Display(Name = "Programa")]
        public int? IdPrograma { get; set; }

        [Display(Name = "Jefatura")]
        public string Jefatura { get; set; }

        [Display(Name = "Fuente de financiamiento")]
        public string ProgramaDescripcion { get; set; }

        [Display(Name = "Región")]
        public int? IdConglomerado { get; set; }

        [Display(Name = "Región")]
        public string ConglomeradoDescripcion { get; set; }


        /*Datos Solicitud*/
        [Display(Name = "¿El organismo invitante financia alojamiento, alimentación y/o pasajes?")]
        public bool? FinanciaOrganismo { get; set; }

        [Display(Name = "Alojamiento")]
        public bool Alojamiento { get; set; }

        [Display(Name = "Alimentación")]
        public bool Alimentacion { get; set; }

        [Display(Name = "Pasajes")]
        public bool Pasajes { get; set; }
                
        [Display(Name = "El cometido incluye pago de viatico?")]        
        public bool SolicitaViatico { get; set; }

        [Display(Name = "El cometido incluye pago de pasaje aéreo?")]
        public bool ReqPasajeAereo { get; set; }

        [Display(Name = "El cometido incluye pago de pasaje terrestre?")]
        public bool ReqPasajeTerrestre { get; set; }

        [Display(Name = "El cometido incluye reembolso por traslado (vehículo)?")]
        public bool Vehiculo { get; set; }

        [Display(Name = "Reembolso")]
        public bool SolicitaReembolso { get; set; }

        //[Display(Name = "Tipo de vehículo")]
        public int? TipoVehiculoId { get; set; }
        [Display(Name = "Tipo de vehículo")]
        public string TipoVehiculoDescripcion { get; set; }

        [StringLength(6,ErrorMessage="Se acepta un maximo de 6 caracteres", MinimumLength = 1)]
        [Display(Name = "Placa Vehículo")]
        public string PlacaVehiculo { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Observaciones Vehiculo")]
        public string VehiculoObservaciones { get; set; }

        [Display(Name = "Tipo Reembolso")]
        public int? TipoReembolsoId { get; set; }
        [Display(Name = "Tipo Reembolso")]
        public string TipoReembolsoDescripcion { get; set; }
        
        [Required(ErrorMessage = "Se debe señalar el motivo del viaje")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Detalle y motivo del viaje")]
        public string CometidoDescripcion { get; set; }

        [Required(ErrorMessage = "Se debe señalar el nombre del cometido")]
        [StringLength(100, ErrorMessage = "Se acepta un máximo de 100 caracteres", MinimumLength = 1)]
        [Display(Name = "Objetivo del cometido")]
        public string NombreCometido { get; set; }

        [Display(Name = "Folio")]
        public string Folio { get; set; }

        [NotMapped]
        [Display(Name = "DÍas")]
        public int? Dias { get; set; }

        [NotMapped]
        [Display(Name = "DÍasPlural")]
        public string DiasPlural { get; set; }

        [NotMapped]
        [Display(Name = "Tiempo")] /*Guarda valor si es tiempo pasado o presente*/
        public string Tiempo { get; set; }

        [NotMapped]
        [Display(Name = "Anno")] 
        public string Anno { get; set; }

        [NotMapped]
        [Display(Name = "Subscretaría")]
        public string Subscretaria { get; set; }

        //[NotMapped]
        [Display(Name = "Fecha Resolución")]
        public DateTime? FechaResolucion { get; set; }

        [NotMapped]
        [Display(Name = "Numero Resolución")]
        public int? NumeroResolucion { get; set; }

        /*Pie de Pagina Resoluciones*/
        [NotMapped]
        [Display(Name = "Orden")]
        public string Orden { get; set; }

        [NotMapped]
        [Display(Name = "Firmante")]
        public string Firmante { get; set; }

        [NotMapped]
        [Display(Name = "Cargo Firmante")]
        public string CargoFirmante { get; set; }

        [NotMapped]
        [Display(Name = "ViaticodeVuelta")]
        public string ViaticodeVuelta { get; set; }

        [NotMapped]
        [Display(Name = "Vistos")]
        public string Vistos { get; set; }

        [NotMapped]
        [Display(Name = "DejaseConstancia")]
        public string DejaseConstancia { get; set; }

        [Display(Name = "Cometido Ok")]
        public bool? CometidoOk { get; set; } = false;

        [Display(Name = "Contabilidad Ok")]
        public bool? ContabilidadOk { get; set; } = false;

        [Display(Name = "Tesorería Ok")]
        public bool? TesoreriaOk { get; set; } = false;

        [NotMapped]
        [Display(Name = "Firma")]
        public bool? Firma { get; set; }

        [Display(Name = "Total Viático")]
        [AssertThat("TotalViatico >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? TotalViatico { get; set; }

        [Display(Name = "Cometido Activo")]
        public bool? Activo { get; set; } = true;

        /*Propiedades registro SIGFE Contabilidad*/
        //[Required(ErrorMessage = "Se debe ingresar el Id de SIGFE")]
        [Display(Name = "Folio Sigfe")]
        public string IdSigfe { get; set; }

        //[Required(ErrorMessage = "Se debe señalar el tipo de pago")]
        [ForeignKey("TipoPagoSIGFE"), Column(Order = 0)]
        [Display(Name = "Tipo de Devengo")]
        public int? IdTipoPago { get; set; }
        public virtual TipoPagoSIGFE TipoPagoSIGFE { get; set; }

        //[Required(ErrorMessage = "Se debe señalar el nombre del funcionario que realiza el pago")]
        [Display(Name = "Funcionario que realiza el devengo")]
        public int? IdFuncionarioPagador { get; set; }

        //[Required(ErrorMessage = "Se debe señalar el nombre del funcionario que realiza el pago")]
        [Display(Name = "Nombre Funcionario pagador")]
        public string NombreFuncionarioPagador { get; set; }

        //[Required(ErrorMessage = "Se debe ingresar la fecha del pago")]
        [Display(Name = "Fecha de Devengo SIGFE")]
        public DateTime? FechaPagoSigfe { get; set; }

        //[Required(ErrorMessage = "Se deben agregar observaciones")]
        [Display(Name = "Observaciones Devengo SIGFE")]
        public string ObservacionesPagoSigfe { get; set; }

        /*Propiedades registro SIGFE Tesoreria*/
        //[Required(ErrorMessage = "Se debe ingresar el Id de Tesoreria")]
        [Display(Name = "Folio Sigfe Tesorería")]
        public string IdSigfeTesoreria { get; set; }

        //[Required(ErrorMessage = "Se debe señalar el tipo de pago en tesoreria")]
        [ForeignKey("TipoPagoSIGFETesoreria"), Column(Order = 1)]
        [Display(Name = "Tipo de Pago Tesorería")]
        public int? IdTipoPagoTesoreria { get; set; }
        public virtual TipoPagoSIGFE TipoPagoSIGFETesoreria { get; set; }

        //[Required(ErrorMessage = "Se debe señalar el nombre del funcionario que realiza el pago")]
        [Display(Name = "Funcionario que realiza el pago Tesorería")]
        public int IdFuncionarioPagadorTesoreria { get; set; }

        [Display(Name = "Nombre Funcionario pagador Tesorería")]
        public string NombreFuncionarioPagadorTesoreria { get; set; }

        //[Required(ErrorMessage = "Se debe señalar la fecha del pago en tesoreria")]
        [Display(Name = "Fecha de Pago SIGFE Tesorería")]
        public DateTime? FechaPagoSigfeTesoreria { get; set; }

        //[Required(ErrorMessage = "Se deben agregar observaciones del pago en tesoreria")]
        [Display(Name = "Observaciones Pago SIGFE Tesorería")]
        public string ObservacionesPagoSigfeTesoreria { get; set; }

        [NotMapped]
        [Display(Name = "DocumnetoRefrendacion")]
        public bool? DocumentoRefrendacion { get; set; } = false;

        /*Datos Acto Administrativo*/
        [Display(Name = "Tipo Acto Administrativo")]
        public string TipoActoAdministrativo { get; set; }

        [Display(Name = "Resolucion Revocatoria")]
        public bool ResolucionRevocatoria { get; set; }

        public string GetTags()
        {
            var tag = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(CometidoId.ToString()))
                tag.Append(CometidoId + " ");
            if (!string.IsNullOrWhiteSpace(NombreCometido))
                tag.Append(NombreCometido + " ");
            if (string.IsNullOrWhiteSpace(Nombre))
                tag.Append(Nombre + " ");
            if (string.IsNullOrWhiteSpace(Destinos.FirstOrDefault()?.ComunaDescripcion))
                tag.Append(Destinos.FirstOrDefault()?.ComunaDescripcion + " ");
            if (!string.IsNullOrWhiteSpace(UnidadDescripcion))
                tag.Append(UnidadDescripcion);

            return tag.ToString();
        }
    }
}