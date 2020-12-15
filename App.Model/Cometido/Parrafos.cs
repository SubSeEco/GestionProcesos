using App.Model.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Cometido
{
    [Table("Parrafos")]
    public class Parrafos 
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

        [Display(Name = "Definicion Proceso Id")]
        public int DefinicionProcesoId { get; set; }
        public virtual DefinicionProceso DefinicionProceso { get; set; }

        [Display(Name = "Documento")]
        public string Documento { get; set; }

        [Display(Name = "Tipo Documento Id")]
        public int TipoDocumentoId { get; set; } = 0;
        public virtual TipoDocumento TipoDocumento { get; set; }
    }
}
