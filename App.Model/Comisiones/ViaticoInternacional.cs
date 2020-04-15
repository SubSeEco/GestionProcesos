using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Comisiones
{
    [Table("ViaticoInternacional")]
    public class ViaticoInternacional
    {
        public ViaticoInternacional()
        { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Display(Name = "Viatico Internacional Id")]
        public int ViaticoInternacionalId { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un")]
        [Display(Name = "Pais Id")]
        public int? PaisId { get; set; }

        [Display(Name = "Pais Nombre")]
        public string PaisNombre { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor")]
        [Display(Name = "Ciuadad Id")]
        public int? CiudadId { get; set; }

        [Display(Name = "Ciuadad Nombre")]
        public string CiudadNombre { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el Año")]
        [Display(Name = "Año")]
        public int? Año { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el campo Activo")]
        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Costo de Vida")]
        public float CostoVida { get; set; }

        [Display(Name = "Factor")]
        public float Factor { get; set; }

        [Display(Name = "Porcentaje Rango 1")]
        public float? PorcentajeRango1 { get; set; }

        [Display(Name = "Porcentaje Rango 2")]
        public float? PorcentajeRango2 { get; set; }

        [Display(Name = "Porcentaje Rango 3")]
        public float? PorcentajeRango3 { get; set; }

        [Display(Name = "Porcentaje Rango 4")]
        public float? PorcentajeRango4 { get; set; }

        [Display(Name = "Porcentaje Rango 5")]
        public float? PorcentajeRango5 { get; set; }

        [Display(Name = "Porcentaje Rango 6")]
        public float? PorcentajeRango6 { get; set; }
    }
}
