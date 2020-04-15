
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.SIGPER
{

    [Table("PEFERJEF")]
    public partial class PEFERJEF
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(3)]
        public string PeFerJerInst { get; set; }
        public string PeFerJerCod { get; set; }
        public string PeFerJerNom { get; set; }
    }
}
