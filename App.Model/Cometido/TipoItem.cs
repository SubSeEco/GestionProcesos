using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Model.Core;

namespace App.Model.Cometido
{
    [Table("TipoItem")]
    public class TipoItem : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "TipoItemId")]
        public int TipoItemId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string TitNombre { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Activo")]
        public bool TitActivo { get; set; }
    }
}
