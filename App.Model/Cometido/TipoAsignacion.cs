using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Cometido
{
    [Table("TipoAsignacion")]
    public class TipoAsignacion : Core.BaseEntity
    {
        public TipoAsignacion()
        { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "TipoAsignacionId")]
        public int TipoAsignacionId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string TasNombre { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Activo")]
        public bool TasActivo { get; set; }
    }
}
