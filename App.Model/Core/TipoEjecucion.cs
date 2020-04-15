using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Core
{
    [Table("CoreTipoEjecucion")]
    public class TipoEjecucion
    {
        public TipoEjecucion()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int TipoEjecucionId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Tipo ejecución")]
        public string Nombre { get; set; }

        public bool Activo { get; set; } = true;

        [Display(Name = "Orden")]
        public int Order { get; set; } = 0;
    }
}
