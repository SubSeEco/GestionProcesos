using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Model.Core;

namespace App.Model.Cometido
{
    [Table("TipoPartida")]
    public class TipoPartida : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "TipoPartidaId")]
        public int TipoPartidaId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string TpaNombre { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Activo")]
        public bool TpaActivo { get; set; }
    }
}
