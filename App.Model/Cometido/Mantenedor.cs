using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Cometido
{
    [Table("Mantenedor")]
    public class Mantenedor: Core.BaseEntity
    {
        public Mantenedor()
        { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mantenedor Id")]
        public int MantenedorId { get; set; }

        [Display(Name = "Nombre Campo")]
        public string NombreCampo { get; set; }

        [Display(Name = "Valor Campo")]
        public string ValorCampo { get; set; }

        [Display(Name = "Numero Cometido")]
        public string IdCometido { get; set; }
    }
}
