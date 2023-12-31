﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Model.Core;

namespace App.Model.Cometido
{
    [Table("CentroCosto")]
    public class CentroCosto : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "CentroCostoId")]
        public int CentroCostoId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string CCNombre { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Activo")]
        public bool CCActivo { get; set; }
    }
}
