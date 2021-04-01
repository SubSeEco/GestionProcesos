using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.HorasExtras
{
    [Table("GeneracionResolucion")]
    public class GeneracionResolucion : Core.BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Generacion ResolucionId")]
        public int GeneracionResolucionId { get; set; }

        [Display(Name = "Fecha Creacion")]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Año")]
        public string Annio { get; set; }

        [Display(Name = "Mes")]
        public string Mes { get; set; }
    }
}
