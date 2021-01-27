﻿using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model.SIGPER
{
    public class MARCACIONES
    {
        public MARCACIONES()
        { }

        
        [Display(Name = "IDENTIFICADOR")]
        public string IDENTIFICADOR { get; set; }

        [Display(Name = "SERIAL")]
        public string SERIAL { get; set; }

        [Display(Name = "FECHA")]
        public DateTime FECHA { get; set; }

        [Key]
        [Display(Name = "HORA")]
        public DateTime HORA { get; set; }

        [Display(Name = "IN_OUT")]
        public string IN_OUT { get; set; }
    }
}