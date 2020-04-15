
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.SIGPER
{

    [Table("PLUNILAB")]
    public partial class PLUNILAB
    {
        [Key]
        public int Pl_UndCod { get; set; }
        public string Pl_UndDes { get; set; }
        public string Pl_UndNomSec { get; set; }
    }
}
