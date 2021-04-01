using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Web;

namespace App.Model.FirmaDocumentoGenerico
{
    [Table("FirmaDocumentoGenerico")]
    public class FirmaDocumentoGenerico
    {
        public FirmaDocumentoGenerico()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int FirmaDocumentoGenericoId { get; set; }

        [Display(Name = "OTP (One Time Password)")]
        public string OTP { get; set; }

        [Display(Name = "Fecha Creación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Archivo")]
        public byte[] Archivo { get; set; }        
        
        [Display(Name = "Archivo Firmado")]
        public byte[] ArchivoFirmado { get; set; }

        [Display(Name = "Archivo Firmado")]
        public byte[] ArchivoFirmado2 { get; set; }

        [Display(Name = "Run")]
        public string Run { get; set; }

        [Display(Name = "Nombre Funcionario")]
        public string Nombre { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Subsecretaria")]
        public string Subsecretaria { get; set; }

        [Display(Name = "Folio")]
        public string Folio { get; set; }

        [Display(Name = "Tipo Documento ")]
        public bool TipoDocumento { get; set; } = true;

        [Display(Name = "Tipo Firma")]
        public string TiposFirmas { get; set; }

        [Display(Name = "Permiso Especial")]
        public bool permisoEspecial { get; set; } = false;        
        
        [Display(Name = "Acceso Premium")]
        public string AccesoPremium { get; set; } 

        //[Required(ErrorMessage = "Es necesario especificar una región")]
        //[RequiredIf("Region", null, ErrorMessage = "Es necesario especificar una región")]
        [Display(Name = "Tipo Documento")]
        public string TipoDocumentos { get; set; }

        [Display(Name = "Código de barra")]
        public byte[] BarCode { get; set; }              

        public int DocumentoId { get; set; } 

        public string GetTags()
        {
            StringBuilder tag = new StringBuilder();

            //solo generar tags de procesos no reservados
            if (this.Proceso != null && !this.Proceso.Reservado)
            {
                if (!string.IsNullOrWhiteSpace(FirmaDocumentoGenericoId.ToString()))
                    tag.Append(FirmaDocumentoGenericoId + " ");

                if (!string.IsNullOrWhiteSpace(Nombre))
                    tag.Append(Nombre + " ");

                if (!string.IsNullOrWhiteSpace(Run))
                    tag.Append(Run + " ");

                if (!string.IsNullOrWhiteSpace(Email))
                    tag.Append(Email + " ");

                if (!string.IsNullOrWhiteSpace(Subsecretaria))
                    tag.Append(Subsecretaria + " ");

                if (!string.IsNullOrWhiteSpace(TipoDocumentos.ToString()))
                    tag.Append(TipoDocumentos + " ");
            }

            return tag.ToString();
        }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Archivo")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase[] File { get; set; }

        [Display(Name = "Proceso")]
        public int ProcesoId { get; set; }
        public virtual Core.Proceso Proceso { get; set; }

        [Display(Name = "Workflow")]
        public int WorkflowId { get; set; }
        public virtual Core.Workflow Workflow { get; set; }
    }
}
