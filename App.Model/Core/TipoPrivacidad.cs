using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Core
{
    [Table("CoreTipoPrivacidad")]
    public class TipoPrivacidad
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int TipoPrivacidadId { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
    }
}
