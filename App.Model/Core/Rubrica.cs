using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Core
{
    [Table("CoreRubrica")]
    public class Rubrica
    {
        public Rubrica()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int RubricaId { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Correo electrónico inválido")]
        [Display(Name = "Correo electrónico usuario")]
        public string Email { get; set; }

        [Display(Name = "Identificador de firma electrónica")]
        public string IdentificadorFirma { get; set; }        
        
        [Display(Name = "Unidad organizacional")]
        public string UnidadOrganizacional { get; set; }

        [Display(Name = "Habilitado para firmar")]
        public bool HabilitadoFirma { get; set; }
    }
}