using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Core
{
    [Table("CoreUsuario")]
    public class Usuario
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Grupo")]
        public int GrupoId { get; set; }
        public virtual Grupo Grupo { get; set; }

        [Display(Name = "Nombre usuario")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        public bool Habilitado { get; set; } = true;
    }
}
