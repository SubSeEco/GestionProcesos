using System.ComponentModel.DataAnnotations;

namespace App.Model.SIGPER
{
    public class DGREGIONES
    {
        public DGREGIONES()
        {
        }

        [Key]
        [Display(Name = "Pl_CodReg")]
        public string Pl_CodReg { get; set; }

        [Display(Name = "Pl_DesReg")]
        public string Pl_DesReg { get; set; }
    }
}
