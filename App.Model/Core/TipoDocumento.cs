using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Core
{
    [Table("CoreTipoDocumento")]
    public class TipoDocumento
    {
        public TipoDocumento()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int TipoDocumentoId { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
    }
}
