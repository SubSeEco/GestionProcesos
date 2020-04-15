using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Comisiones
{
    [Table("SIGPERTipoReembolso")]
    public class SIGPERTipoReembolso
    {
        public SIGPERTipoReembolso()
        { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Display(Name = "Id")]
        public int SIGPERTipoReembolsoId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string Reembolso { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
