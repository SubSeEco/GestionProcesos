using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Pasajes
{
    [Table("EmpresaAerolinea")]
    public class EmpresaAerolinea : Core.BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "EmpresaAerolineaId")]
        public int EmpresaAerolineaId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Empresa")]
        public string NombreEmpresa { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Activo")]
        public bool ActivoEmpresa { get; set; }
    }
}
