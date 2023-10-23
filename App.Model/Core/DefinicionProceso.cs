using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Core
{
    [Table("CoreDefinicionProceso")]
    public class DefinicionProceso
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int DefinicionProcesoId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre proceso")]
        public string Nombre { get; set; }
   
        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Duración (horas)")]
        public int DuracionHoras { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Habilitado?")]
        public bool Habilitado { get; set; }

        public virtual ICollection<DefinicionWorkflow> DefinicionWorkflows { get; set; } = new HashSet<DefinicionWorkflow>();
        public virtual ICollection<Proceso> Procesos { get; set; } = new HashSet<Proceso>();

        [NotMapped]
        [Display(Name = "Participantes")]
        public string Grupos { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Información asociada")]
        public int EntidadId { get; set; }
        public virtual Entidad Entidad { get; set; }

        [Display(Name = "Ejecutar primera tarea inmediatamente?")]
        public bool EjecutarInmediatamente { get; set; }
    }
}