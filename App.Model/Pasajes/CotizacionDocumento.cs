using App.Model.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Pasajes
{
    [Table("CotizacionDocumento")]
    public class CotizacionDocumento
    {
        public CotizacionDocumento()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CotizacionDocumentoId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Proceso")]
        public int CotizacionId { get; set; }
        public virtual Cotizacion Cotizacion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Proceso")]
        //public int WorkflowId { get; set; }
        //public virtual Workflow Workflow { get; set; }

        [Display(Name = "Tipo documento")]
        public int? TipoDocumentoId { get; set; }
        public virtual TipoDocumento TipoDocumento { get; set; }

        [Display(Name = "Privacidad")]
        public int? TipoPrivacidadId { get; set; } = 1;
        public virtual TipoPrivacidad TipoPrivacidad { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Display(Name = "Autor")]
        //public int? UsuarioId { get; set; }
        //public virtual Usuario Usuario { get; set; }
        public string Email { get; set; }

        [Display(Name = "Nombre Archivo")]
        public string FileName { get; set; }

        [Display(Name = "Extensión")]
        public string Extension { get; set; }

        [Display(Name = "URL")]
        public string URL { get; set; }

        [Display(Name = "Archivo")]
        //[DataType(DataType.Upload)]
        //[FileTypes("pdf")]
        public byte[] File { get; set; }

        [Display(Name = "Texto")]
        public string Texto { get; set; }

        [Display(Name = "Metadata")]
        public string Metadata { get; set; }

        [Display(Name = "Metadata")]
        public string Type { get; set; }

        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; }

        [Display(Name = "Firmado")]
        public bool Signed { get; set; } = false;

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Display(Name = "Código de barra")]
        public byte[] BarCode { get; set; }

        public bool Selected { get; set; } = false;

        [NotMapped]
        [Display(Name = "Certificado electrónico")]
        public string SerialNumber { get; set; }

        //[Display(Name = "Hash md5")]
        //public string Md5 { get; set; }
    }
}
