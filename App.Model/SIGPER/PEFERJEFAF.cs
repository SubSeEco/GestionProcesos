
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.SIGPER
{

    [Table("PEFERJEFAF")]
    public partial class PEFERJEFAF
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(3)]
        public string PeFerJerInst { get; set; }
        public System.Decimal PeFerJerCod { get; set; }
        public int FyPFunRut { get; set; }
    }
}
