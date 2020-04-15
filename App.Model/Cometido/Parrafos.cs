using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Cometido
{
    [Table("Parrafos")]
    public class Parrafos : Core.BaseEntity
    {
        public Parrafos()
        { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Parrafos Id")]
        public int ParrafosId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Texto")]
        public string ParrafoTexto { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Activo")]
        public bool ParrafoActivo { get; set; }
    }
}
