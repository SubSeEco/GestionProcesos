using System;
using System.ComponentModel.DataAnnotations;

namespace App.Model.SIGPER
{
    public class DGPAISES
    {
        public DGPAISES()
        { }

        [Key]
        [Display(Name = "Pl_CodPai")]
        public Int16 Pl_CodPai { get; set; }

        //[Key]
        [Display(Name = "pl_DesPai")]
        public string pl_DesPai { get; set; }

        //[Display(Name = "Pl_FacPai")]
        //public float? Pl_FacPai { get; set; }
    }
}
