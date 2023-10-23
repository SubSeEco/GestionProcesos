using ExpressiveAnnotations.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Comisiones
{
    [Table("DestinosComision")]
    public class DestinosComision
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Display(Name = "Destinos Comision Id")]
        public int DestinosComisionId { get; set; }

        [ForeignKey("Comisiones")]
        [Display(Name = "ComisionesId")]
        public int? ComisionesId { get; set; }
        public virtual Comisiones Comisiones { get; set; }

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

        [Display(Name = "Dias 100% (Pernoctar + Alimentacion)")]
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
        [DataType(DataType.Currency)]
        [AssertThat("Dias100Monto >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public decimal? Dias100Monto { get; set; }

        [Display(Name = "Días 60% Monto")]
        [DataType(DataType.Currency)]
        [AssertThat("Dias60Monto >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public decimal? Dias60Monto { get; set; }

        [Display(Name = "Días 40% Monto")]
        [DataType(DataType.Currency)]
        [AssertThat("Dias40Monto >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public decimal? Dias40Monto { get; set; }

        [Display(Name = "Días 50% Monto")]
        [DataType(DataType.Currency)]
        [AssertThat("Dias50Monto >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public decimal? Dias50Monto { get; set; }

        [Display(Name = "Días 0% Monto")]
        [DataType(DataType.Currency)]
        [AssertThat("Dias00Monto >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public decimal? Dias00Monto { get; set; }
                
        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Currency)]
        [AssertThat("Total >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public decimal? Total { get; set; }

        [Display(Name = "Total Viatico")]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Currency)]
        [AssertThat("TotalViatico >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public decimal? TotalViatico { get; set; }

        [Display(Name = "Ciudad")]
        public string IdCiudad { get; set; }
        [Display(Name = "Ciudad")]
        public string CiudadDescripcion { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Pais")]
        public string IdPais { get; set; }
        [Display(Name = "Pais")]
        public string PaisDescripcion { get; set; }

        [NotMapped]
        [Display(Name = "Workflow")]
        public int? WorkflowId { get; set; }

        [NotMapped]
        [Display(Name = "Total Viatico Palabras")]
        public string TotalViaticoPalabras { get; set; }
    }
}
