using App.Model.Core;
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

        //[Display(Name = "Tipo ingreso")]
        //public int? GDTipoId { get; set; }
        //public virtual GDTipo GDTipo { get; set; }

        [Display(Name = "Fecha solicitud")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; } = DateTime.Now;

        //[Required]
        //[Display(Name = "Tipo documento")]
        //public string TipoDocumentoCodigo { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el dato Materia")]
        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string Materia { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar el dato Referencia")]
        [Display(Name = "Referencia")]
        [DataType(DataType.MultilineText)]
        public string Referencia { get; set; }

        [Display(Name = "Observación")]
        [DataType(DataType.MultilineText)]
        public string Observacion { get; set; }

        [Display(Name = "Autor")]
        public string Autor { get; set; }

        [Display(Name = "Grupo")]
        public int? GrupoId { get; set; }
        public virtual Grupo Grupo { get; set; }

        [Display(Name = "Unidad del firmante")]
        public int? Pl_UndCod { get; set; }

        [Display(Name = "Unidad del firmante")]
        public string Pl_UndDes { get; set; }

        [Display(Name = "Usuario firmante")]
        public string UsuarioDestino { get; set; }

        //[Display(Name = "Organización")]
        //public string OrganizacionId { get; set; }

        //[Display(Name = "Organización")]
        //public string RazonSocial { get; set; }

        [Display(Name = "Folio")]
        public string Folio { get; set; }

        [Display(Name = "Código de barra")]
        public byte[] BarCode { get; set; }


        [Display(Name = "Proceso")]
        public int ProcesoId { get; set; }
        public virtual Core.Proceso Proceso { get; set; }

        [Display(Name = "Workflow")]
        public int WorkflowId { get; set; }
        public virtual Core.Workflow Workflow { get; set; }

        [Display(Name = "Requiere firma electrónica?")]
        public bool RequiereFirmaElectronica { get; set; } = false;


        public void GetFolio()
        {
            var year = DateTime.Now.Year;
            var unit = this.Pl_UndCod.ToString().PadLeft(6, '0');
            //var organization = !string.IsNullOrWhiteSpace(this.OrganizacionId) ? this.OrganizacionId.ToString().PadLeft(5, '0') : "0".PadLeft(5, '0');
            //var filetype = this.GDTipoId.ToString().PadLeft(3, '0');
            var secuence = "0".PadLeft(7, '0');
            //this.Folio = string.Concat(year, unit, organization, filetype, secuence);
        }
    }
}
