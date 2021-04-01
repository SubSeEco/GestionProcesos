using System.ComponentModel.DataAnnotations;

namespace App.Model.DTO
{
    public class DTOFolio
    {
        public string status { get; set; }
        public string error { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Año")]
        public string periodo { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Código tipo documento")]
        public string tipodocumento { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Solicitante")]
        public string solicitante { get; set; }

        public string folio { get; set; }
    }
}
