using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpressiveAnnotations.Attributes;

namespace App.Model.Cometido
{
    [Table("Destinos")]
    public class Destinos
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Display(Name = "DestinosId")]
        public int DestinoId { get; set; }

        [ForeignKey("Cometido")]
        [Display(Name = "CometidoId")]
        public int? CometidoId { get; set; }
        public virtual Cometido Cometido { get; set; }

        [Required(ErrorMessage = "Se debe indicar la Fecha Inicio")]
        [Display(Name = "Fecha Inicio")]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Se debe indicar la Fecha Fin")]
        [Display(Name = "Fecha Hasta")]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaHasta { get; set; }

        [Display(Name = "Días 100% (Pernoctar + Alimentación)")]
        [AssertThat("Dias100 >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias100 { get; set; }

        [Display(Name = "Días 60% (Pernoctar)")]
        [AssertThat("Dias60 >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias60 { get; set; }

        [Display(Name = "Días 40% (Alimentación)")]
        [AssertThat("Dias40 >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias40 { get; set; }

        [Display(Name = "Días 50%")]
        [AssertThat("Dias50 >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias50 { get; set; }

        [Display(Name = "Días 0%")]
        [AssertThat("Dias00 >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias00 { get; set; } 

        [Display(Name = "Días 100% Aprobados")]
        [AssertThat("Dias100Aprobados >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias100Aprobados { get; set; }

        [Display(Name = "Días 60% Aprobados")]
        [AssertThat("Dias60Aprobados >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias60Aprobados { get; set; }

        [Display(Name = "Días 40% Aprobados")]
        [AssertThat("Dias40Aprobados >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias40Aprobados { get; set; }

        [Display(Name = "Días 50% Aprobados")]
        [AssertThat("Dias50Aprobados >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias50Aprobados { get; set; }

        [Display(Name = "Días 0% Aprobados")]
        [AssertThat("Dias00Aprobados >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias00Aprobados { get; set; }

        [Display(Name = "Días 100% Monto")]
        [AssertThat("Dias100Monto >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias100Monto { get; set; }

        [Display(Name = "Días 60% Monto")]
        [AssertThat("Dias60Monto >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias60Monto { get; set; }

        [Display(Name = "Días 40% Monto")]
        [AssertThat("Dias40Monto >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias40Monto { get; set; }

        [Display(Name = "Días 50% Monto")]
        [AssertThat("Dias50Monto >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias50Monto { get; set; }

        [Display(Name = "Días 0% Monto")]
        [AssertThat("Dias00Monto >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? Dias00Monto { get; set; }

        [Display(Name = "Total")]
        [AssertThat("Total >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        [DataType(DataType.Currency)]
        public int? Total { get; set; }

        [Display(Name = "Total Viático")]
        [AssertThat("TotalViatico >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        [DataType(DataType.Currency)]
        public int? TotalViatico { get; set; }

        [Display(Name = "Comuna")]
        public string IdComuna { get; set; }
        [Display(Name = "Comuna")]
        public string ComunaDescripcion { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato - IdRegion")]
        [Display(Name = "Región")]
        public string IdRegion { get; set; }
        [Display(Name = "Región")]
        public string RegionDescripcion { get; set; }

        [NotMapped]
        [Display(Name = "Workflow")]
        public int? WorkflowId { get; set; }

        [NotMapped]
        [Display(Name = "Total Viático Palabras")]
        public string TotalViaticoPalabras { get; set; }

        [NotMapped]
        [Display(Name = "Edicion GP")]
        public bool EditGP { get; set; }

        [Display(Name = "Destino Activo")]
        public bool? DestinoActivo { get; set; } = true;

        /*DATOS ORIGEN*/
        
        [Display(Name = "Region Origen")]
        public string IdOrigenRegion { get; set; }
        
        [Display(Name = "Region Origen")]
        public string OrigenRegion { get; set; }
        
        [Display(Name = "Fecha Origen")]
        public DateTime? FechaOrigen { get; set; }
        
        [Display(Name = "Observaciones Origen")]
        public string ObsOrigen { get; set; }
        
        [Display(Name = "Observaciones Destino")]
        public string ObsDestino { get; set; }

        /*Modificaciones solicitadas 17112020*/
        [Display(Name = "Observaciones por modificacion")]
        [DataType(DataType.MultilineText)]
        public string ObservacionesModificacion { get; set; }

        /*Modificaciones solicitadas 20082021*/
        [Required(ErrorMessage = "Es necesario especificar este dato - Localidad")]
        [Display(Name = "Id Localidad")]
        public int? LocalidadId { get; set; } = null;

        [Display(Name = "Localidad")]
        public string NombreLocalidad { get; set; }
    }
}
