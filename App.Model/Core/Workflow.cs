using ExpressiveAnnotations.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Core
{
    [Table("CoreWorkflow")]
    public class Workflow 
    {
        public Workflow()
        {
            Documentos = new HashSet<Documento>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int WorkflowId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Proceso Id")]
        public int ProcesoId { get; set; }
        public virtual Proceso Proceso { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Tarea")]
        public int DefinicionWorkflowId { get; set; }
        public virtual DefinicionWorkflow DefinicionWorkflow { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha creación")]
        public DateTime FechaCreacion { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha vencimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaVencimiento { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha término")]
        [DataType(DataType.Date)]
        public DateTime? FechaTermino { get; set; }

        [Display(Name = "Grupo")]
        public int? GrupoId { get; set; }
        public virtual Grupo Grupo { get; set; }

        [Display(Name = "Unidad")]
        public int? Pl_UndCod { get; set; }

        [Display(Name = "Unidad")]
        public string Pl_UndDes { get; set; }

        [Display(Name = "Email funcionario")]
        public string Email { get; set; }

        [Display(Name = "Funcionario")]
        public string To { get; set; }

        [Display(Name = "Observaciones de la tarea")]
        [DataType(DataType.MultilineText)]
        [RequiredIf("TipoAprobacionId == 3", ErrorMessage = "Se debe señalar el motivo del rechazo para la tarea")]
        public string Observacion { get; set; }

        [Display(Name = "Observaciones")]
        [DataType(DataType.MultilineText)]
        public string Mensaje { get; set; }

        [Display(Name = "Tipo aprobación")]
        public int? TipoAprobacionId { get; set; }
        public virtual TipoAprobacion TipoAprobacion { get; set; }

        [Display(Name = "Terminada?")]
        public bool Terminada { get; set; } = false;

        [Display(Name = "Anulada?")]
        public bool Anulada { get; set; } = false;

        [Display(Name = "Tarea es personal?")]
        public bool TareaPersonal { get; set; } = false;

        public int? WorkflowGrupoId { get; set; }
        public string Entity { get; set; }
        public int? EntityId { get; set; }

        public virtual ICollection<Documento> Documentos { get; set; }

        [Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(Name = "Firmante")]
        public string Firmante { get; set; }

        [NotMapped]
        [Display(Name = "Certificado electrónico")]
        public string SerialNumber { get; set; }

        [Display(Name = "Nombre Funcionario")]
        public string NombreFuncionario { get; set; }

        [NotMapped]
        [Display(Name = "Numero Proceso")]
        public string NumeroProceso { get; set; }




        [NotMapped]
        [Display(Name = "Tiempo ejecución")]
        public TimeSpan Span
        {
            get
            {
                return ((FechaTermino.HasValue ? FechaTermino.Value : DateTime.Now) - FechaCreacion);
            }
        }

    }
}
