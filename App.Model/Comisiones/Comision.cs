//using ExpressiveAnnotations.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace App.Model.Comisiones
{
    [Table("Comisiones")]
    public class Comisiones : Core.BaseEntity
    {
        /*list de Generacion CDP*/
        [Display(Name = "Lista GeneracionCDP")]
        public virtual List<GeneracionCDPComision> GeneracionCDPComision { get; set; } = new List<GeneracionCDPComision>();

        /*list de destino*/
        [Display(Name = "Lista Destinos")]
        public virtual List<DestinosComision> DestinosComision { get; set; } = new List<DestinosComision>();

        [Required(ErrorMessage = "Se debe indicar la fecha")]
        [Display(Name = "Fecha Solicitud")]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaSolicitud { get; set; }

        /*Definicion de Atributos*/
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Numero Comisiones")]
        public int ComisionesId { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "Nombre Id")]
        public int? NombreId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el Rut")]
        [Display(Name = "Rut")]
        public int Rut { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el DV")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV")]
        public string DV { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el Id de la Unidad")]
        [Display(Name = "Unidad")]
        public int? IdUnidad { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el nombre de la Unidad")]
        [Display(Name = "Unidad")]
        public string UnidadDescripcion { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el Id de la Calidad Juridica")]
        [Display(Name = "Calidad Juridica")]
        public int? IdCalidad { get; set; }

        [Required(ErrorMessage = "Es necesario especificar la Calidad Juridica")]
        [Display(Name = "Calidad Juridica")]
        public string CalidadDescripcion { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el Id del Grado")]
        [Display(Name = "GradoUES")]
        public string IdGrado { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el Grado")]
        [Display(Name = "GradoUES")]
        public string GradoDescripcion { get; set; }

        [Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        public string Grado { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el Id del Cargo")]
        [Display(Name = "Cargo")]
        public int? IdCargo { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el Cargo")]
        [Display(Name = "Cargo")]
        public string CargoDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estamento")]
        public int? IdEstamento { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estamento")]
        public string EstamentoDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Programa")]
        public int? IdPrograma { get; set; }

        [Display(Name = "Programa")]
        public string ProgramaDescripcion { get; set; }

        [Display(Name = "Conglomerado")]
        public int? IdConglomerado { get; set; }

        [Display(Name = "Conglomerado")]
        public string ConglomeradoDescripcion { get; set; }

        /*Datos Solicitud*/
        [Display(Name = "Financia Organismo Invitante")]
        public bool? FinanciaOrganismo { get; set; }

        [Display(Name = "Alojamiento")]
        public bool Alojamiento { get; set; }

        [Display(Name = "Alimentacion")]
        public bool Alimentacion { get; set; }

        [Display(Name = "Pasajes")]
        public bool Pasajes { get; set; }

        [Display(Name = "Viatico")]
        public bool SolicitaViatico { get; set; }

        [Display(Name = "Pasaje Aereo")]
        public bool ReqPasajeAereo { get; set; }

        [Display(Name = "Pasaje Terrestre")]
        public bool ReqPasajeTerrestre { get; set; }

        [Display(Name = "¿Vehiculo?")]
        public bool Vehiculo { get; set; }

        [Display(Name = "¿Solicita Reembolso?")]
        public bool SolicitaReembolso { get; set; }

        [Display(Name = "Tipo Vehiculo")]
        public int? TipoVehiculoId { get; set; }
        [Display(Name = "Tipo Vehiculo")]
        public string TipoVehiculoDescripcion { get; set; }
        [Display(Name = "Placa Vehiculo")]
        public string PlacaVehiculo { get; set; }

        [Display(Name = "Tipo Reembolso")]
        public int? TipoReembolsoId { get; set; }
        [Display(Name = "Tipo Reembolso")]
        public string TipoReembolsoDescripcion { get; set; }

        [Display(Name = "¿Solicita Fondos por Rendir?")]
        public int FondosporRendir { get; set; }

        /*Datos de la comision*/
        [Required(ErrorMessage = "Es necesario especificar la descripcion de la Comision")]
        [Display(Name = "Descripcion Comision")]
        public string ComisionDescripcion { get; set; }

        [Display(Name = "Folio")]
        public string Folio { get; set; }

        [NotMapped]
        [Display(Name = "Dias")]
        public int? Dias { get; set; }

        [NotMapped]
        [Display(Name = "DiasPlural")]
        public string DiasPlural { get; set; }

        [NotMapped]
        [Display(Name = "Tiempo")] /*Guarda valor si es tiempo pasado o presente*/
        public string Tiempo { get; set; }

        [NotMapped]
        [Display(Name = "Anno")]
        public string Anno { get; set; }

        [NotMapped]
        [Display(Name = "Subscretaria")]
        public string Subscretaria { get; set; }

        [NotMapped]
        [Display(Name = "Fecha Resolucion")]
        public DateTime FechaResolucion { get; set; }

        [NotMapped]
        [Display(Name = "Numero Resolucion")]
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
        [Display(Name = "Vistos")]
        public string Vistos { get; set; }

        [Display(Name = "Comisiones Ok")]
        public bool? ComisionesOk { get; set; }
    }
}
