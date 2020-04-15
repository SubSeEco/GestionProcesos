using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Comisiones
{
    [Table("ParametrosComisiones")]
    public class ParametrosComisiones
    {
        public ParametrosComisiones()
        { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Parametro Comision Id")]
        public int ParametrosComisionesId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Parametro Texto")]
        public string ParametroTexto { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Parametro Activo")]
        public bool ParametroActivo { get; set; }
    }
}
