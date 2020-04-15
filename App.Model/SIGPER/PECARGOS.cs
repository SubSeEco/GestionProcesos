using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.SIGPER
{
    [Table("PECARGOS")]
    public partial class PECARGOS
    {
        [Key]
        public int Pl_CodCar  { get; set; }

        public string Pl_DesCar { get; set; }

    }
}
