//using ExpressiveAnnotations.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace App.Model.Pasajes
{
    [Table("Pasaje")]
    public class Pasaje: Core.BaseEntity
    {
        public Pasaje()
        {
            Cotizacion = new List<Cotizacion>();
            DestinosPasajes = new List<DestinosPasajes>();
        }

        /*list de cotizaciones*/
        [Display(Name = "Lista Cotización")]
        public virtual IList<Cotizacion> Cotizacion { get; set; }

        /*list de destinos*/
        [Display(Name = "Lista Cotización")]
        public virtual IList<DestinosPasajes> DestinosPasajes { get; set; }

        /*Definicion de atributos*/
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Numero Pasaje")]
        public int PasajeId { get; set; }

        [Required(ErrorMessage = "Se debe indicar la fecha")]
        [Display(Name = "Fecha Solicitud")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaSolicitud { get; set; }

        //[ForeignKey("Cometido")]
        //[Display(Name = "CometidoId")]
        //public int? CometidoId { get; set; }
        //public virtual Cometido Cometido { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "Nombre Id")]
        public int? NombreId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el RUT")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut")]
        public int Rut { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el DV")]
        [StringLength(1, ErrorMessage = "Excede el largo máximo (1)")]
        [Display(Name = "DV")]
        public string DV { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Jurídica")]
        public int? IdCalidad { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Jurídica")]
        public string CalidadDescripcion { get; set; }

        [Required(ErrorMessage = "Es necesario especificar las descripción del pasaje")]
        [Display(Name = "Descripción Pasaje")]
        public string PasajeDescripcion { get; set; }

        [Display(Name = "Pasaje Ok")]
        public bool? PasajeOk { get; set; }

        //[Display(Name = "Destino Nacional")]
        //public bool? TipoDestino { get; set; }

        //[Display(Name = "Comuna")]
        //public string IdComuna { get; set; }
        //[Display(Name = "Comuna")]
        //public string ComunaDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Region")]
        //public string IdRegion { get; set; }
        //[Display(Name = "Region")]
        //public string RegionDescripcion { get; set; }

        // [Display(Name = "Pais")]
        //public string IdPais { get; set; }
        //[Display(Name = "Pais")]
        //public string PaisDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Ciudad")]
        //public string IdCiudad { get; set; }
        //[Display(Name = "Ciudad")]
        //public string CiudadDescripcion { get; set; }

        //[Required(ErrorMessage = "Se debe indicar la Fecha Ida")]
        //[Display(Name = "Fecha Ida")]
        ////[DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        //public DateTime FechaIda { get; set; }

        //[Display(Name = "Hora Ida")]
        ////[DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Time)]
        //public DateTime HoraIda { get; set; }

        //[Required(ErrorMessage = "Se debe indicar la Fecha Vuelta")]
        //[Display(Name = "Fecha Vuelta")]
        ////[DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        //public DateTime FechaVuelta { get; set; }

        //[Display(Name = "Hora Vuelta")]
        ////[DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Time)]
        //public DateTime? HoraVuelta { get; set; }  


        [Display(Name = "Destino Nacional")]
        public bool? TipoDestino { get; set; }

        #region ORIGEN DESTINOS

        ///*ORIGEN*/
        //[Display(Name = "Origen Comuna Id")]
        //public string IdComunaOrigen { get; set; }
        //[Display(Name = "Origen Comuna")]
        //public string OrigenComunaDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Origen Region Id")]
        //public string IdRegionOrigen { get; set; }
        //[Display(Name = "Origen Region")]
        //public string OrigenRegionDescripcion { get; set; }

        //[Display(Name = "Origen Pais Id")]
        //public string IdPaisOrigen { get; set; }
        //[Display(Name = "Origen Pais")]
        //public string OrigenPaisDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Origen Ciudad Id")]
        //public string IdCiudadOrigen { get; set; }
        //[Display(Name = "Origen Ciudad")]
        //public string OrigenCiudadDescripcion { get; set; }

        //[Required(ErrorMessage = "Se debe indicar la Fecha Origen")]
        //[Display(Name = "Fecha Origen")]
        ////[DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        //public DateTime FechaOrigen { get; set; }

        //[Display(Name = "Hora Origen")]
        ////[DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Time)]
        //public DateTime HoraOrigen { get; set; }

        #endregion

        



    }
}
