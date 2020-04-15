using System.ComponentModel.DataAnnotations;

namespace App.Model.SIGPER
{
    public class DGESCALAFONES
    {
        public DGESCALAFONES()
        { }

        [Key]
        [Display(Name = "Pl_CodEsc")]
        public string Pl_CodEsc { get; set; }

        [Display(Name = "Pl_DesEsc")]
        public string Pl_DesEsc { get; set; }

    }
}
