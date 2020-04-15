using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Model.Cometido;

namespace App.Model.Comisiones
{
    [Table("GeneracionCDPComision")]
    public class GeneracionCDPComision : Core.BaseEntity
    {
        public GeneracionCDPComision()
        { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "GeneracionCDPComisionId")]
        public int GeneracionCDPComisionId { get; set; }

        [ForeignKey("Comisiones")]
        [Display(Name = "ComisionesId")]
        public int? ComisionesId { get; set; }
        public virtual Comisiones Comisiones { get; set; }

        /*Viaticos*/

        [ForeignKey("TipoPartida")]
        [Display(Name = "Partida")]
        public int? VtcTipoPartidaId { get; set; }
        public virtual TipoPartida TipoPartida { get; set; }

        [ForeignKey("TipoCapitulo")]
        [Display(Name = "Capitulo")]
        public int? VtcTipoCapituloId { get; set; }
        public virtual TipoCapitulo TipoCapitulo { get; set; }

        [ForeignKey("CentroCosto")]
        [Display(Name = "Programa")]
        public int? VtcCentroCostoId { get; set; }
        public virtual CentroCosto CentroCosto { get; set; }

        [ForeignKey("TipoSubTitulo")]
        [Display(Name = "SubTitulo")]
        public int? VtcTipoSubTituloId { get; set; }
        public virtual TipoSubTitulo TipoSubTitulo { get; set; }

        [ForeignKey("TipoItem")]
        [Display(Name = "Item")]
        public int? VtcTipoItemId { get; set; }
        public virtual TipoItem TipoItem { get; set; }

        [ForeignKey("TipoAsignacion")]
        [Display(Name = "Asignacion")]
        public int? VtcTipoAsignacionId { get; set; }
        public virtual TipoAsignacion TipoAsignacion { get; set; }

        [ForeignKey("TipoSubAsignacion")]
        [Display(Name = "SubAsignacion")]
        public int? VtcTipoSubAsignacionId { get; set; }
        public virtual TipoSubAsignacion TipoSubAsignacion { get; set; }

        [Display(Name = "IdCompromiso")]
        public string VtcIdCompromiso { get; set; }

        [Display(Name = "Codigo Compromiso")]
        public string VtcCodCompromiso { get; set; }

        [Display(Name = "Presupuesto Total")]
        public string VtcPptoTotal { get; set; }

        [Display(Name = "Compromiso Acumulado")]
        public string VtcCompromisoAcumulado { get; set; }

        [Display(Name = "Obligacion Actual")]
        public string VtcObligacionActual { get; set; }

        [Display(Name = "Saldo")]
        public string VtcSaldo { get; set; }

        [Display(Name = "Valor Viatico")]
        public string VtcValorViatico { get; set; }

        /*Pasajes*/

        [ForeignKey("PsjTipoPartida")]
        [Display(Name = "Partida")]
        public int? PsjTipoPartidaId { get; set; }
        public virtual TipoPartida PsjTipoPartida { get; set; }

        [ForeignKey("PsjTipoCapitulo")]
        [Display(Name = "Capitulo")]
        public int? PsjVtcTipoCapituloId { get; set; }
        public virtual TipoCapitulo PsjTipoCapitulo { get; set; }

        [ForeignKey("PsjCentroCosto")]
        [Display(Name = "Programa")]
        public int? PsjCentroCostoId { get; set; }
        public virtual CentroCosto PsjCentroCosto { get; set; }

        [ForeignKey("PsjTipoSubTitulo")]
        [Display(Name = "TipoSubTitulo")]
        public int? PsjTipoSubTituloId { get; set; }
        public virtual TipoSubTitulo PsjTipoSubTitulo { get; set; }

        [ForeignKey("PsjTipoItem")]
        [Display(Name = "Item")]
        public int? PsjTipoItemId { get; set; }
        public virtual TipoItem PsjTipoItem { get; set; }

        [ForeignKey("PsjTipoAsignacion")]
        [Display(Name = "Asignacion")]
        public int? PsjTipoAsignacionId { get; set; }
        public virtual TipoAsignacion PsjTipoAsignacion { get; set; }

        [ForeignKey("PsjTipoSubAsignacion")]
        [Display(Name = "SubAsignacion")]
        public int? PsjTipoSubAsignacionId { get; set; }
        public virtual TipoSubAsignacion PsjTipoSubAsignacion { get; set; }

        [Display(Name = "Compromiso")]
        public string PsjIdCompromiso { get; set; }

        [Display(Name = "Codigo Compromiso")]
        public string PsjCodCompromiso { get; set; }

        [Display(Name = "Presupuesto Total")]
        public string PsjPptoTotal { get; set; }

        [Display(Name = "Compromiso Acumulado")]
        public string PsjCompromisoAcumulado { get; set; }

        [Display(Name = "Obligacion Actual")]
        public string PsjObligacionActual { get; set; }

        [Display(Name = "Saldo")]
        public string PsjSaldo { get; set; }

        [Display(Name = "Valor Viatico")]
        public string PsjValorViatico { get; set; }

        [NotMapped]
        [Display(Name = "Fecha Firma")]
        public DateTime FechaFirma { get; set; }

        [NotMapped]
        [Display(Name = "Fecha Firma")]
        public DateTime PsjFechaFirma { get; set; }
    }
}
