using System.ComponentModel.DataAnnotations;

namespace App.Model.Sigper
{
    public class DGESTAMENTOS
    {
        [Key]
        [Display(Name = "DgEstCod")]
        public short DgEstCod { get; set; }

        [Display(Name = "DgEstDsc")]
        public string DgEstDsc { get; set; }
    }
}
