﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ExpressiveAnnotations.Attributes;
using Enum = App.Util.Enum;

namespace App.Model.Core
{
    [Table("CoreWorkflow")]
    public class Workflow
    {
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

        //[RequiredIf("PermitirSeleccionarUnidadDestino && TipoAprobacionId != 3", ErrorMessage = "Es necesario especificar el dato Unidad destino")]
        [RequiredIf("PermitirSeleccionarUnidadDestino", ErrorMessage = "Es necesario especificar el dato Unidad destino")]
        [Display(Name = "Unidad")]
        public int? Pl_UndCod { get; set; }

        //[RequiredIf("PermitirSeleccionarUnidadDestino && TipoAprobacionId != 3", ErrorMessage = "Es necesario especificar el dato Funcionario destino")]
        [RequiredIf("Reservado", ErrorMessage = "Es necesario especificar el dato Funcionario Destino ya que es un proceso reservado")]
        [Display(Name = "Funcionario")]
        public string To { get; set; }

        [Display(Name = "Unidad")]
        public string Pl_UndDes { get; set; }

        [Display(Name = "Email funcionario")]
        public string Email { get; set; }

        [RequiredIf("TipoAprobacionId == 3", ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Observaciones de la tarea")]
        [DataType(DataType.MultilineText)]
        public string Observacion { get; set; }

        [Display(Name = "Observaciones")]
        [DataType(DataType.MultilineText)]
        public string Mensaje { get; set; }

        [RequiredIf("RequiereAprobacionAlEnviar", ErrorMessage = "Es necesario especificar el dato Aprobación")]
        [Display(Name = "Tipo aprobación")]
        public int? TipoAprobacionId { get; set; }
        public virtual TipoAprobacion TipoAprobacion { get; set; }

        [Display(Name = "Terminada?")]
        public bool Terminada { get; set; }

        [Display(Name = "Anulada?")]
        public bool Anulada { get; set; }

        [Display(Name = "Tarea es personal?")]
        public bool TareaPersonal { get; set; }

        [Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(Name = "Nombre Funcionario")]
        public string NombreFuncionario { get; set; }

        [Display(Name = "Entidad")]
        public string Entity { get; set; }

        [Display(Name = "Entidad Id")]
        public int? EntityId { get; set; }

        public int? ToPl_UndCod { get; set; }

        public virtual ICollection<Documento> Documentos { get; set; } = new HashSet<Documento>();


        [NotMapped]
        public bool RequiereAprobacionAlEnviar { get; set; }

        [NotMapped]
        public bool PermitirMultipleEvaluacion { get; set; }

        [NotMapped]
        public bool PermitirSeleccionarUnidadDestino { get; set; }

        [NotMapped]
        public bool PermitirSeleccionarPersonasMismaUnidad { get; set; }

        [NotMapped]
        public bool PermitirSeleccionarGrupoEspecialDestino { get; set; }

        [NotMapped]
        public bool PermitirFinalizarProceso { get; set; }

        [NotMapped]
        public bool PermitirTerminar { get; set; }

        [NotMapped]
        public bool Reservado { get; set; }

        [NotMapped]
        public bool EsAprobacionCometidoCompraPasaje =>
               DefinicionWorkflow != null
            && DefinicionWorkflow.DefinicionProcesoId == 13
            && DefinicionWorkflow.Secuencia == 4;

        [NotMapped]
        public bool EsFirmaDocumento =>
               DefinicionWorkflow != null
            && DefinicionWorkflow.DefinicionProceso != null
            && DefinicionWorkflow.DefinicionProceso.Entidad != null
            && DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == Enum.Entidad.FirmaDocumento.ToString();

        [NotMapped]
        [Display(Name = "Tiempo ejecución")]
        public TimeSpan Span => ((FechaTermino.HasValue ? FechaTermino.Value : DateTime.Now) - FechaCreacion);

        [NotMapped]
        public bool DesactivarDestinoEnRechazo { get; set; } 


        [NotMapped]
        public int? Unidad { get; set; }

        [NotMapped]
        public string Funcionario { get; set; }

        [NotMapped]
        [Display(Name = "Minutos permanencia")]
        public decimal? MinutosPermanencia
        {
            get
            {
                if (!FechaTermino.HasValue)
                    return null;

                var minutes = 0;
                for (var i = FechaCreacion; i <= FechaTermino.Value; i = i.AddMinutes(1))
                    if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday && !Enum.Feriados.Any(q => q.Date == i.Date))
                        if (i.TimeOfDay.Hours >= 9 && i.TimeOfDay.Hours <= 18)
                            minutes++;

                return minutes;
            }
        }

     

        public bool EsTareaCierre { get; set; } 
    }
}