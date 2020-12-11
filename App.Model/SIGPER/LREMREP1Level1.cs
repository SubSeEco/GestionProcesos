using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace App.Model.SIGPER
{
    public class LREMREP1Level1
    {
        public LREMREP1Level1()
        {}

        [Key]
        [Display(Name = "lrem_codrep")]
        public int lrem_codrep { get; set; }

        [Display(Name = "lrem_tipo")]
        public int lrem_tipo { get; set; }
        
        [Display(Name = "lrem_reforcod")]
        public string lrem_reforcod { get; set; }
    }
}
