using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Cometido
{
    [Table("ViaticoHonorario")]
    public class ViaticoHonorario
    {
        public ViaticoHonorario()
        { }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Display(Name = "ViaticoHonorarioId")]
        public int ViaticoHonorarioId { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el campo Año")]
        [Display(Name = "Año")]
        public int? Año { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el campo Desde")]
        [Display(Name = "Desde")]
        public int? Desde { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el campo Hasta")]
        [Display(Name = "Hasta")]
        public int? Hasta { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el campo Paorcentaje 100%")]
        [Display(Name = "Porcentaje 100")]
        public int? Porcentaje100 { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el campo Paorcentaje 60%")]
        [Display(Name = "Porcentaje 60%")]
        public int? Porcentaje60 { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el campo Paorcentaje 40%")]
        [Display(Name = "Porcentaje 40%")]
        public int? Porcentaje40 { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el campo Paorcentaje 50%")]
        [Display(Name = "Porcentaje 50")]
        public int? Porcentaje50 { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el campo Tramo")]
        [Display(Name = "Tramo")]
        public string Tramo { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el campo Activo")]
        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
