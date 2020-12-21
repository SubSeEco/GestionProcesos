using System.ComponentModel.DataAnnotations;


namespace App.Model.DTO
{
    public class DTOImputacion
    {
        [Display(Name = "Subtitulo")]
        public int Subtitulo { get; set; }

        [Display(Name = "Item")]
        public int Item { get; set; }

        [Display(Name = "Asignacion")]
        public int Asignacion { get; set; }
    }
}
