
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Sigper
{

    [Table("PLUNILAB")]
    public class PLUNILAB
    {
        [Key]
        public int Pl_UndCod { get; set; }
        public string Pl_UndDes { get; set; }
        public string Pl_UndNomSec { get; set; }
    }
}
