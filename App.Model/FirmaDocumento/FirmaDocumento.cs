using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.FirmaDocumento
{
    [Table("FirmaDocumento")]
    public class FirmaDocumento
    {
        public FirmaDocumento()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int FirmaDocumentoId { get; set; }

        [Display(Name = "Observaciones")]
        [DataType(DataType.MultilineText)]
        public string Observaciones { get; set; }

        [Display(Name = "Autor")]
        public string Autor { get; set; }

        [Display(Name = "Fecha creación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Firmante")]
        public string Firmante { get; set; }

        [Display(Name = "Fecha firma")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? FechaFirma { get; set; }


        //[NotMapped]
        //[Required(ErrorMessage = "Es necesario especificar el archivo a firmar")]
        //[Display(Name = "Documento a firmar")]
        //[DataType(DataType.Upload)]
        //public HttpPostedFileBase DocumentoSinFirmaFile { get; set; }

        [Display(Name = "Documento a firmar")]
        public byte[] DocumentoSinFirma { get; set; }

        [Display(Name = "Documento a firmar")]
        public string DocumentoSinFirmaFilename { get; set; }


        [Display(Name = "Documento firmado")]
        public byte[] DocumentoConFirma { get; set; }

        [Display(Name = "Documento firmado")]
        public string DocumentoConFirmaFilename { get; set; }



        [Display(Name = "Folio")]
        public string Folio { get; set; }

        //[Required]
        [Display(Name = "Código tipo documento")]
        public string TipoDocumentoCodigo { get; set; }


        [Display(Name = "Tipo documento")]
        public string TipoDocumentoDescripcion { get; set; }

        [Display(Name = "Código de barra")]
        public byte[] BarCode { get; set; }

        [Display(Name = "Firmado?")]
        public bool Firmado { get; set; }




        [Display(Name = "Proceso")]
        public int ProcesoId { get; set; }
        public virtual Core.Proceso Proceso { get; set; }

        [Display(Name = "Workflow")]
        public int WorkflowId { get; set; }
        public virtual Core.Workflow Workflow { get; set; }

        [NotMapped]
        public bool TieneFirma { get; set; }

        [Display(Name = "URL gestión documental")]
        [DataType(DataType.Url, ErrorMessage = "Debe indicar una URL válida")]
        public string URL { get; set; }
        public int? DocumentoId { get; set; }
    }
}
