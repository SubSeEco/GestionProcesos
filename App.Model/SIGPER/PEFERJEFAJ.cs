using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Sigper
{

    [Table("PEFERJEFAJ")]
    public class PEFERJEFAJ
    {
        [Key]
        public decimal PeFerJerCod { get; set; }
        public int FyPFunARut { get; set; }
        public int PeFerJerAutEst { get; set; }

    }
}
