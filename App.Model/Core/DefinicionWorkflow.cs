using ExpressiveAnnotations.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Core
{
    [Table("CoreDefinicionWorkflow")]
    public class DefinicionWorkflow
    {
        public DefinicionWorkflow()
        {
            Workflows = new HashSet<Workflow>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int DefinicionWorkflowId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Definición proceso")]
        public int DefinicionProcesoId { get; set; }
        public virtual DefinicionProceso DefinicionProceso { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Acción a ejecutar")]
        public int AccionId { get; set; }
        public virtual Accion Accion { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Información asociada a la tarea")]
        public int EntidadId { get; set; }
        public virtual Entidad Entidad { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Quien ejecuta la tarea")]
        public int TipoEjecucionId { get; set; }
        public virtual TipoEjecucion TipoEjecucion { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Secuencia")]
        [Range(1, long.MaxValue, ErrorMessage = "Debe ingresar un valor mayor a 0")]
        public int Secuencia { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Duración (dias)")]
        public int DuracionDias { get; set; }

        [Display(Name = "Grupo")]
        public int? GrupoId { get; set; }
        public virtual Grupo Grupo { get; set; }

        [Display(Name = "Unidad")]
        public int? Pl_UndCod { get; set; }

        [Display(Name = "Unidad")]
        public string Pl_UndDes { get; set; }

        [RequiredIf("TipoEjecucionId == 6", ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Ejecutado por")]
        public string Email { get; set; }

        [Display(Name = "Nombre usuario")]
        public string NombreUsuario { get; set; }

        [Display(Name = "En caso de rechazo ejecutar tarea")]
        [ForeignKey("DefinicionWorkflowRechazo")]
        public int? DefinicionWorkflowRechazoId { get; set; }

        [Display(Name = "Depende de tarea (opcional)")]
        public int? DefinicionWorkflowDependeDeId { get; set; }

        [Display(Name = "Requiere aprobación de todos los usuarios para continuar?")]
        public bool RequiereAprobacionGrupal { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Tarea habilitada?")]
        public bool Habilitado { get; set; } = true;



        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Notificar cambios de estado al autor por correo electrónico?")]
        public bool NotificarAlAutor { get; set; } = false;

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Notificar asignaciones de tareas de estado al autor por correo electrónico?")]
        public bool NotificarAsignacion { get; set; } = false;




        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Mostrar detalles del proceso?")]
        public bool PermitirVerDetalles { get; set; } = false;

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Permitir ver documentos adjuntos?")]
        public bool PermitirVerDocumentos { get; set; } = false;


        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Permitir adjuntar documentos simples?")]
        public bool PermitirAdjuntarDocumentos { get; set; } = false;

        [Display(Name = "Permitir adjuntar documentos para FEA?")]
        public bool PermitirAdjuntarDocumentosConFirmaElectronica { get; set; } = false;



        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Requiere documentos adjuntos?")]
        public bool RequireDocumentacion { get; set; } = false;

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Permitir generar documentos a partir de información ingresada?")]
        public bool PermitirGenerarDocumentos { get; set; } = false;



        [Display(Name = "Permitir reasignar tarea a otra unidad o funcionario?")]
        public bool PermitirReenvioUnidad { get; set; } = false;

        [Display(Name = "Permitir reasignar tarea an grupo especial?")]
        public bool PermitirReenvioGrupoEspecial { get; set; } = false;

        [Display(Name = "Permitir archivar tarea?")]
        public bool PermitirArchivarTarea { get; set; } = false;

        //[Display(Name = "Permitir anular proceso?")]
        //public bool PermitirAnularProceso { get; set; } = false;


        [Display(Name = "Permitir envío de tarea?")]
        public bool PermitirTerminar { get; set; } = false;

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Requiere aprobación al momento de enviar tarea?")]
        public bool RequiereAprobacionAlEnviar { get; set; } = false;

        [Display(Name = "Permitir seleccionar unidad y funcionario de destino?")]
        public bool PermitirSeleccionarUnidadDestino { get; set; } = false;

        [Display(Name = "Permitir seleccionar grupo especial de destino?")]
        public bool PermitirSeleccionarGrupoEspecialDestino { get; set; } = false;

        [Display(Name = "Permitir multiples evaluaciones de la misma tarea?")]
        public bool PermitirMultipleEvaluacion { get; set; } = false;

        public virtual DefinicionWorkflow DefinicionWorkflowRechazo { get; set; }
        public virtual ICollection<Workflow> Workflows { get; set; }

        [Display(Name = "Instrucciones")]
        public string Instrucciones { get; set; }

        [Display(Name = "Seleccionar solo personas de la misma unidad?")]
        public bool PermitirSeleccionarPersonasMismaUnidad { get; set; } = false;

        [Display(Name = "Permitir finalizar proceso?")]
        public bool PermitirFinalizarProceso { get; set; } = false;

        [Display(Name = "Desactivar destino en el rechazo?")]
        public bool DesactivarDestinoEnRechazo { get; set; } = false;
    }
}