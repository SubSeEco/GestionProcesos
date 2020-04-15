using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Cometido
{
    [Table("TipoSubTitulo")]
    public class TipoSubTitulo : Core.BaseEntity
    {
        public TipoSubTitulo()
        { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "TipoSubTituloId")]
        public int TipoSubTituloId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string TstNombre { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Activo")]
        public bool TstActivo { get; set; }
    }
}
