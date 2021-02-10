using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.FirmaDocumentoGenerico
{
    [Table("FirmaDocumentoGenerico")]
    public class FirmaDocumentoGenerico : Core.BaseEntity
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
    }
}
