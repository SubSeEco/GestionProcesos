using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.GestionDocumental
{
    [Table("GDTipoIngreso")]
    public class GDTipo
    {
        public GDTipo()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int GDTipoId { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
    }
}
