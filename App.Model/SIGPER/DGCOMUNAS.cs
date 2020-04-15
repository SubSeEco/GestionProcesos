using System.ComponentModel.DataAnnotations;

namespace App.Model.SIGPER
{
    public class DGCOMUNAS
    {
        public DGCOMUNAS()
        { }

        //[Key]
        [Display(Name = "Pl_CodReg")]
        public string Pl_CodReg { get; set; }

        [Key]
        [Display(Name = "Pl_CodCom")]
        public string Pl_CodCom { get; set; }

        [Display(Name = "Pl_DesCom")]
        public string Pl_DesCom { get; set; }

}
}
