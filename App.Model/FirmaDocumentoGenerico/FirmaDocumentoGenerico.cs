using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Display(Name = "Observaciones")]
        [DataType(DataType.MultilineText)]
        public string Observaciones { get; set; }

        [Display(Name = "Autor")]
        public string Autor { get; set; }

        [Display(Name = "Fecha Creación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Firmante")]
        public string Firmante { get; set; }


        [Display(Name = "Fecha firma")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? FechaFirma { get; set; }

        [Display(Name = "Documento a Firmar")]
        public byte[] DocumentoSinFirma { get; set; }

        [Display(Name = "Documento a Firmar")]
        public string DocumentoSinFirmaFilename { get; set; }

        [Display(Name = "Documento Firmado")]
        public byte[] DocumentoConFirma { get; set; }

        [Display(Name = "Documento Firmado")]
        public string DocumentoConFirmaFilename { get; set; }

        [NotMapped]
        [Display(Name = "Folio")]
        public string Folio { get; set; }

        [Display(Name = "Código Tipo Documento")]
        public string TipoDocumentoCodigo { get; set; }

        [Display(Name = "Tipo Documento")]
        public string TipoDocumentoDescripcion { get; set; }

        [Display(Name = "Código de Barra")]
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
