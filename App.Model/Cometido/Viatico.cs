using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Cometido
{
    [Table("Viatico")]
    public class Viatico
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Display(Name = "ViaticoId")]
        public int ViaticoId { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el Rango 1")]
        [Display(Name = "Rango1")]
        public int? Rango1 { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el Rango 2")]
        [Display(Name = "Rango2")]
        public int? Rango2 { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el Rango 3")]
        [Display(Name = "Rango3")]
        public int? Rango3 { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el Rango 4")]
        [Display(Name = "Rango4")]
        public int? Rango4 { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el Rango 5")]
        [Display(Name = "Rango5")]
        public int? Rango5 { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el Año")]
        [Display(Name = "Año")]
        public int? Año { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un valor para el campo Activo")]
        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
