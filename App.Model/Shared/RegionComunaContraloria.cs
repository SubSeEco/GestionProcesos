using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Shared
{
    public class RegionComunaContraloria
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ID { get; set; }
                
        [Display(Name = "CODIGO REGION")]
        public int CODIGOREGION { get; set; }

        [Display(Name = "REGIÓN")]
        public string REGIÓN { get; set; }

        [Display(Name = "CODIGO COMUNA")]
        public int CODIGOCOMUNA { get; set; }

        [Display(Name = "COMUNA")]
        public string COMUNA { get; set; }
    }
}
