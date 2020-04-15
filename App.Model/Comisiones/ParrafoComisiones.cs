using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Comisiones
{
    [Table("ParrafoComisiones")]
    public class ParrafoComisiones
    {
        public ParrafoComisiones()
        { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Parrafo Comision Id")]
        public int ParrafoComisionesId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Parrafo Texto")]
        public string ParrafoTexto { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Parrafo Activo")]
        public bool ParrafoActivo { get; set; }
    }
}
