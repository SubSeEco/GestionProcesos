using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model.Persona
{
    [Table("Persona")]
   public class Persona
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int id { get; set; }

        [Required(ErrorMessage = "Se debe señalar el nombre de la persona")]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }


        [Required(ErrorMessage = "Se debe señalar el apeliido de la persona")]
        [Display(Name = "Apellido")]
        public string apellido { get; set; }
    }
}
