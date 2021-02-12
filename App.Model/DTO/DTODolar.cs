using System.ComponentModel.DataAnnotations;

namespace App.Model.DTO
{
    public class DTODolar
    {
        public DTODolar()
        {

        }
        public string status { get; set; }
        public string error { get; set; }
                
        [Display(Name = "Fecha")]
        public string Fecha { get; set; }

        [Display(Name = "codigo")]
        public string codigo { get; set; }

        [Display(Name = "nombre")]
        public string nombre { get; set; }

        [Display(Name = "unidad_medida")]
        public string unidad_medida { get; set; }

        public float ValorDolar { get; set; }
    }
}
