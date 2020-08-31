using ExpressiveAnnotations.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace App.Model.GestionDocumental
{
    public class DTOFileUploadFEA
    {
        public DTOFileUploadFEA()
        {
        }

        public int ProcesoId { get; set; }
        public int WorkflowId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Archivo")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase[] File { get; set; }

        [Display(Name = "Requiere firma electrónica?")]
        public bool RequiereFirmaElectronica { get; set; } = false;

        [Display(Name = "Es documento oficial?")]
        public bool EsOficial { get; set; } = false;

        [RequiredIf("RequiereFirmaElectronica == true", ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad del firmante")]
        public string Pl_UndCod { get; set; }

        [Display(Name = "Unidad del firmante")]
        public string Pl_UndDes { get; set; }

        [RequiredIf("RequiereFirmaElectronica == true", ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Usuario firmante")]
        public string UsuarioFirmante { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Tipo documento")]
        public string TipoDocumentoCodigo { get; set; }
    }
}
