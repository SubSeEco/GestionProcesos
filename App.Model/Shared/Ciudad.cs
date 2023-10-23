using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Shared
{
    [Table("SharedCiudad")]
    public class Ciudad
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Ciudad Id")]
        public int CiudadId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Ciudad Activo")]
        public bool CiudadActivo { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Ciudad Nombre")]
        public string CiudadNombre { get; set; }

        [Display(Name = "Pais")]
        public int PaisId { get; set; }
    }
}
