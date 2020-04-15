using System;
using System.ComponentModel.DataAnnotations;

namespace App.Model.SIGPER
{
    public class REPYT //programa
    {
        public REPYT()
        { }

        [Key]
        [Display(Name = "RePytCod")]
        public Decimal RePytCod { get; set; }

        [Display(Name = "RePytDes")]
        public string RePytDes { get; set; }

        [Display(Name = "RePytEst")]
        public string RePytEst { get; set; }

    }
}
