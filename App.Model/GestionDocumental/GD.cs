using App.Model.Helper;
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

        [Display(Name = "Fecha creación")]
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



        [Display(Name = "Ingreso externo?")]
        public bool IngresoExterno { get; set; }



        [Display(Name = "Origen documentación")]
        [RequiredIf("IngresoExterno", true, ErrorMessage = "Es necesario especificar este dato")]
        public int GDOrigenId { get; set; }
        public virtual GDOrigen GDOrigen { get; set; }

        [Display(Name = "Número externo")]
        [RequiredIf("IngresoExterno", true, ErrorMessage = "Es necesario especificar este dato")]
        public string NumeroExterno { get; set; }

        [Display(Name = "Unidad destino")]
        [RequiredIf("IngresoExterno", true, ErrorMessage = "Es necesario especificar este dato")]
        public string DestinoUnidadCodigo { get; set; }

        //[Display(Name = "Unidad destino")]
        //[RequiredIf("IngresoExterno", true, ErrorMessage = "Es necesario especificar este dato")]
        //public string DestinoUnidadDescripcion { get; set; }

        [Display(Name = "Usuario destino")]
        [RequiredIf("IngresoExterno", true, ErrorMessage = "Es necesario especificar este dato")]
        public string DestinoFuncionarioEmail { get; set; }

        //[Display(Name = "Usuario destino")]
        //[RequiredIf("IngresoExterno", true, ErrorMessage = "Es necesario especificar este dato")]
        //public string DestinoFuncionarioNombre { get; set; }




        [Display(Name = "Proceso")]
        public int ProcesoId { get; set; }
        public virtual Core.Proceso Proceso { get; set; }

        [Display(Name = "Workflow")]
        public int WorkflowId { get; set; }
        public virtual Core.Workflow Workflow { get; set; }
    }
}
