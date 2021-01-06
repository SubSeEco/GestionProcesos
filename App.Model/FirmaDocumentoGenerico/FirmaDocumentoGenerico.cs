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

        [Display(Name = "OTP")]
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
    }
}
