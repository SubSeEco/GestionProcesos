using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Pasajes
{
    [Table("DestinosPasajes")]
    public class DestinosPasajes
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Display(Name = "Destinos Pasajes Id")]
        public int DestinosPasajesId { get; set; }

        [ForeignKey("Pasaje")]
        [Display(Name = "PasajeId")]
        public int? PasajeId { get; set; }
        public virtual Pasaje Pasaje { get; set; }

        //[Display(Name = "Destino Nacional")]
        //public bool? TipoDestino { get; set; }

        #region ORIGEN DESTINOS
        /*ORIGEN*/
        [Display(Name = "Origen Comuna Id")]
        public string IdComunaOrigen { get; set; }
        [Display(Name = "Origen Comuna")]
        public string OrigenComunaDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Origen Región Id")]
        public string IdRegionOrigen { get; set; }
        [Display(Name = "Región de origen")]
        public string OrigenRegionDescripcion { get; set; }
        [Display(Name = "Origen País Id")]
        public string IdPaisOrigen { get; set; }
        [Display(Name = "Origen País")]
        public string OrigenPaisDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Origen Ciudad Id")]
        public string IdCiudadOrigen { get; set; }
        [Display(Name = "Origen Ciudad")]
        public string OrigenCiudadDescripcion { get; set; }

        [Required(ErrorMessage = "Se debe indicar la Fecha Origen")]
        [Display(Name = "Fecha de origen")]
        //[DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaOrigen { get; set; }

        //[Display(Name = "Hora Origen")]
        ////[DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Time)]
        //public DateTime HoraOrigen { get; set; }

        [Display(Name = "Observaciones hora llegada")]
        public string ObservacionesOrigen { get; set; }

        #endregion

        [Display(Name = "Comuna")]
        public string IdComuna { get; set; }
        [Display(Name = "Comuna")]
        public string ComunaDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Región")]
        public string IdRegion { get; set; }

        [Display(Name = "Región de retorno")]
        public string RegionDescripcion { get; set; }

        [Display(Name = "País")]
        public string IdPais { get; set; }
        [Display(Name = "País")]
        public string PaisDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Ciudad")]
        public string IdCiudad { get; set; }
        [Display(Name = "Ciudad")]
        public string CiudadDescripcion { get; set; }

        [Required(ErrorMessage = "Se debe indicar la Fecha Ida")]
        [Display(Name = "Fecha Ida")]
        //[DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaIda { get; set; }

        //[Display(Name = "Hora Ida")]
        ////[DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Time)]
        //public DateTime HoraIda { get; set; }

        [Required(ErrorMessage = "Se debe indicar la Fecha Vuelta")]
        [Display(Name = "Fecha de retorno")]
        //[DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaVuelta { get; set; }

        //[Display(Name = "Hora Salida")]
        ////[DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Time)]
        //public DateTime? HoraVuelta { get; set; }

        [Display(Name = "Observaciones hora salida")]
        public string ObservacionesDestinos { get; set; }

        [NotMapped]
        [Display(Name = "Workflow")]
        public int? WorkflowId { get; set; }
    }
}
