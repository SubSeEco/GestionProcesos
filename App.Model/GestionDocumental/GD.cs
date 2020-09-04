using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.GestionDocumental
{
    public class GD
    {
        public GD()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int GDId { get; set; }

        [Display(Name = "Fecha solicitud")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Fecha ingreso (requerido)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? FechaIngreso { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Es necesario especificar el dato Materia")]
        [Display(Name = "Materia (requerido)")]
        [DataType(DataType.MultilineText)]
        public string Materia { get; set; }

        [Display(Name = "Referencia (opcional)")]
        [DataType(DataType.MultilineText)]
        public string Referencia { get; set; }

        [Display(Name = "Observación (opcional)")]
        [DataType(DataType.MultilineText)]
        public string Observacion { get; set; }

        [Display(Name = "Es documentación reservada?")]
        public bool EsReservado { get; set; } = false;

        [Display(Name = "Proceso")]
        public int ProcesoId { get; set; }
        public virtual Core.Proceso Proceso { get; set; }

        [Display(Name = "Workflow")]
        public int WorkflowId { get; set; }
        public virtual Core.Workflow Workflow { get; set; }
    }
}
