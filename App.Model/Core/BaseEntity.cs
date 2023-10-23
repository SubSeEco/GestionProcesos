using System.ComponentModel.DataAnnotations;

namespace App.Model.Core
{
    public class BaseEntity
    {
        [Display(Name = "Proceso")]
        public int? ProcesoId { get; set; }
        public Proceso Proceso { get; set; }

        [Display(Name = "Workflow")]
        public int? WorkflowId { get; set; }
        public Workflow Workflow { get; set; }

        public string Tarea { get; set; }
        public string Instrucciones { get; set; }
    }
}
