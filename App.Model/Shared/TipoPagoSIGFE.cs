using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Model.Shared
{
    [Table("SharedTipoPagoSIGFE")]
    public class TipoPagoSIGFE
    {
        public TipoPagoSIGFE()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "TipoPagoId")]
        public int TipoPagoSIGFEId { get; set; }
                
        [Display(Name = "Descripcion Tipo Pago")]
        public string DescripcionTipoPago { get; set; }

        [Display(Name = "Descripcion Tipo Pago Contabilidad")]
        public string DescripcionTipoPagoContabilidad { get; set; }
                
        [Display(Name = "Estado Tipo Pago")]
        public bool TipoActivo { get; set; }
    }
}
