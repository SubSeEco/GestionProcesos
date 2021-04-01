using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Model.Core;

namespace App.Model.Cometido
{
    [Table("TipoSubAsignacion")]
    public class TipoSubAsignacion : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "TipoSubAsignacionId")]
        public int TipoSubAsignacionId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string TsaNombre { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Activo")]
        public bool TsaActivo { get; set; }
    }
}
