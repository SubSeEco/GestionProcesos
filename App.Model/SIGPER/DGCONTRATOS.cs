using System.ComponentModel.DataAnnotations;

namespace App.Model.SIGPER
{
    public class DGCONTRATOS
    {
        public DGCONTRATOS()
        { }

        [Key]
        [Display(Name = "RH_ContCod")]
        public short RH_ContCod { get; set; }

        [Display(Name = "RH_ContDes")]
        public string RH_ContDes { get; set; }
    }
}
