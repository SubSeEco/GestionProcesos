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

        [Required(ErrorMessage = "Es necesario especificar el dato Materia")]
        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string Materia { get; set; }

        [Display(Name = "Referencia")]
        [DataType(DataType.MultilineText)]
        public string Referencia { get; set; }

        [Display(Name = "Observación")]
        [DataType(DataType.MultilineText)]
        public string Observacion { get; set; }

        [Display(Name = "Autor")]
        public string Autor { get; set; }

        [Display(Name = "Unidad del firmante")]
        public int? Pl_UndCod { get; set; }

        [Display(Name = "Unidad del firmante")]
        public string Pl_UndDes { get; set; }

        [Display(Name = "Usuario firmante")]
        public string UsuarioFirmante { get; set; }

        [Display(Name = "Proceso")]
        public int ProcesoId { get; set; }
        public virtual Core.Proceso Proceso { get; set; }

        [Display(Name = "Workflow")]
        public int WorkflowId { get; set; }
        public virtual Core.Workflow Workflow { get; set; }

        [Display(Name = "Requiere firma electrónica?")]
        public bool RequiereFirmaElectronica { get; set; } = false;
    }
}
