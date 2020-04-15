using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.SIGPER
{

    [Table("PEFERJEFAJ")]
    public partial class PEFERJEFAJ
    {
        [Key]
        //[Column(Order = 0)]
        //[StringLength(3)]
        //public string PeFerJerInst { get; set; }
        public System.Decimal PeFerJerCod { get; set; }
        public int FyPFunARut { get; set; }
        public int PeFerJerAutEst { get; set; }

    }
}
