using System.ComponentModel.DataAnnotations;

namespace App.Model.FirmaDocumento 
{
    public class DTOTipoDocumento
    {
        public DTOTipoDocumento()
        {
        }

        [Display(Name = "ID")]
        public int TipoDocumentoId { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Display(Name = "Habilitado?")]
        public bool Habilitado { get; set; }
    }
}