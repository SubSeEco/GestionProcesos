using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Comisiones
{
    [Table("SIGPERTipoVehiculo")]
    public class SIGPERTipoVehiculo
    {
        public SIGPERTipoVehiculo()
        { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Display(Name = "Id")]
        public int SIGPERTipoVehiculoId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string Vehiculo { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
