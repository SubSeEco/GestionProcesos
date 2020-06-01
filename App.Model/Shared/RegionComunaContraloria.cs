using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model.Shared
{
    public class RegionComunaContraloria
    {
        public RegionComunaContraloria()
        { }

        [Key]
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
