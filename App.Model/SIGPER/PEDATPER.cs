using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.SIGPER
{
    [Table("PEDATPER")]
    public partial class PEDATPER
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(3)]
        public string Rh_InstPub { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RH_NumInte { get; set; }

        [StringLength(1)]
        public string RH_DvNuInt { get; set; }

        [StringLength(20)]
        public string RH_PaterFun { get; set; }

        [StringLength(20)]
        public string RH_MaterFun { get; set; }

        [StringLength(30)]
        public string RH_NombFun { get; set; }

        [StringLength(30)]
        public string RH_DirCal { get; set; }

        [StringLength(6)]
        public string RH_DirNum { get; set; }

        [StringLength(10)]
        public string RH_DirDep { get; set; }

        [StringLength(10)]
        public string RH_DirBlo { get; set; }

        [StringLength(12)]
        public string Pl_CodPla { get; set; }

        [StringLength(5)]
        public string Pl_CodCom { get; set; }

        [StringLength(15)]
        public string RH_Telefono { get; set; }

        //public DateTime? RH_FecNaci { get; set; }

        [StringLength(20)]
        public string RH_LugNac { get; set; }

        [StringLength(1)]
        public string RH_SexoCod { get; set; }

        [StringLength(1)]
        public string RH_EstCivi { get; set; }

        //public short? RH_SitMili { get; set; }

        [StringLength(15)]
        public string RH_IncMil { get; set; }

        [StringLength(1)]
        public string RH_EstLab { get; set; }

        [StringLength(230)]
        public string RH_Foto { get; set; }

        //public DateTime? RH_Finginst { get; set; }

        //public DateTime? RH_Fingadm { get; set; }

        [StringLength(10)]
        public string RH_TarjCAs { get; set; }

        //public DateTime? RH_FInBien { get; set; }

        //public short? RH_MoPaCod { get; set; }

        //public short? RH_TramoAF { get; set; }

        [StringLength(200)]
        public string RH_ObsMen { get; set; }

        //public DateTime? RH_FunPre { get; set; }

        [StringLength(200)]
        public string RH_FunMed { get; set; }

        [StringLength(200)]
        public string RH_FunEnf { get; set; }

        [StringLength(200)]
        public string RH_FunAle { get; set; }

        //public short? RH_FunRhs { get; set; }

        [StringLength(200)]
        public string RH_FunEme { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal? RH_FunEst { get; set; }

        //public short? RH_FunPes { get; set; }

        [StringLength(1)]
        public string RH_Bienest { get; set; }

        //public int? RH_NumAfiB { get; set; }

        [StringLength(1)]
        public string RH_TipSocB { get; set; }

        //public DateTime? RH_BloqBi { get; set; }

        //public short? RH_BieTipF { get; set; }

        [StringLength(1)]
        public string RH_BieJub { get; set; }

        [StringLength(1)]
        public string RH_BieEstL { get; set; }

        [Column(TypeName = "money")]
        public decimal? RH_BieSuel { get; set; }

        //public short? RH_BieEstF { get; set; }

        //public short? RH_ClaSolfp { get; set; }

        [StringLength(11)]
        public string RE_Ac1EspCod { get; set; }

        [StringLength(11)]
        public string RE_Ac2EspCod { get; set; }

        [StringLength(3)]
        public string RH_BcoCod { get; set; }

        [StringLength(1)]
        public string RH_BcoAbo { get; set; }

        [StringLength(15)]
        public string RH_CtaCrr { get; set; }

        //public short? Rh_Bienio { get; set; }

        //public DateTime? Rh_FecBie { get; set; }

        //public DateTime? Rh_ProBie { get; set; }

        //public DateTime? Rh_BcoFiI { get; set; }

        //public DateTime? Rh_BcoFiM { get; set; }

        //public DateTime? Rh_BcoFiE { get; set; }

        [StringLength(1)]
        public string Rh_BcoEst { get; set; }

        //public short? ReBcoOfi { get; set; }

        //public short? ReBcoAct { get; set; }

        //public int? ReBcoCom { get; set; }

        [StringLength(6)]
        public string Rh_ClaIne { get; set; }

        //public short? Rh_ApliCin { get; set; }

        [StringLength(80)]
        //public string Rh_Mail { get; set; }
        private string rh_Mail;
        public string Rh_Mail
        {
            get { return string.IsNullOrEmpty(rh_Mail) ? null : rh_Mail.Trim().ToLower(); }
            set { rh_Mail = value; }
        }

        [StringLength(15)]
        public string Rh_TelOfi { get; set; }

        [StringLength(4)]
        public string Rh_FotoTyp { get; set; }

        //[Column(TypeName = "image")]
        //public byte[] Rh_FotoDoc { get; set; }

        [StringLength(15)]
        public string Rh_Celular { get; set; }

        [StringLength(40)]
        public string Rh_ViPoCo { get; set; }

        [StringLength(20)]
        public string Rh_SitCant { get; set; }

        //public short? Rh_UltNroEsc { get; set; }

        //public short? Rh_BieSocSec { get; set; }

        //public short? Pl_CodPai { get; set; }

        [StringLength(2)]
        public string Pl_CodReg { get; set; }

        //public short? Rh_UtlNroCur { get; set; }

        //public short? ultkeysol { get; set; }

        [StringLength(10)]
        public string Rh_IdeFun { get; set; }

        [StringLength(1)]
        public string Rh_Indica { get; set; }

        [StringLength(32)]
        public string Rh_PassWeb { get; set; }

        [StringLength(32)]
        public string Rh_KeyWeb { get; set; }

        //public short? Rh_IntWeb { get; set; }

        //public DateTime? Rh_FecIniWeb { get; set; }

        //public short? Rh_CadWeb { get; set; }

        //public short? Rh_BoqWeb { get; set; }

        //public short? Rh_AnoTse { get; set; }

        //public short? Rh_MesTse { get; set; }

        //public short? Rh_DiaTse { get; set; }

        [StringLength(1)]
        public string Rh_CalInd { get; set; }

        [StringLength(80)]
        public string Rh_MailPer { get; set; }

        [StringLength(500)]
        public string Rh_AntUti { get; set; }

        //public short? Rh_UltNroPos { get; set; }

        //public DateTime? Rh_FecFall { get; set; }

        //public short? Rh_CodEsl { get; set; }

        //public DateTime? Rh_FecIngCon { get; set; }

        //public DateTime? Rh_FecIngPla { get; set; }

        //public short? Rh_CodBieP { get; set; }

        //public short? RhSegJur01 { get; set; }

        public int? RhSegUnd01 { get; set; }

        //public short? RhSegJur02 { get; set; }

        public int? RhSegUnd02 { get; set; }

        //public short? RhSegJur03 { get; set; }

        public int? RhSegUnd03 { get; set; }

        [StringLength(1)]
        public string Rh_BieBlq { get; set; }

        [StringLength(300)]
        public string Rh_BieBlqLeyA { get; set; }

        [StringLength(300)]
        public string Rh_BieBlqLeyB { get; set; }

        [StringLength(1)]
        public string Rh_AsiMarca { get; set; }

        //public DateTime? Rh_FIngAfiEpre { get; set; }

        [StringLength(1)]
        public string Rh_BieSegSal { get; set; }

        [StringLength(70)]
        public string RH_NomFunCap { get; set; }

        [StringLength(32)]
        public string Rh_NomLDAP { get; set; }

        [StringLength(1)]
        public string Rh_IndConAct { get; set; }

        //public short? RH_TipCta { get; set; }

        [StringLength(1)]
        public string RhH_TarjTip { get; set; }

        //public DateTime? Rh_PriFecAfiSP { get; set; }

        [StringLength(20)]
        public string Rh_BieComUna { get; set; }

        [StringLength(40)]
        public string Rh_BieComDes { get; set; }

        [StringLength(1)]
        public string Rh_MarcaAC { get; set; }

        [StringLength(1)]
        public string PeDatPerAutEle { get; set; }

        [StringLength(1)]
        public string PeDatPerAutEst { get; set; }

        [StringLength(1)]
        public string PeDatPerIndExt { get; set; }

        [StringLength(60)]
        public string PeDatPerChq { get; set; }

        //public short? RH_PerRolDoc { get; set; }

        [StringLength(100)]
        public string RH_Alias { get; set; }

        [StringLength(20)]
        public string DgAgrCod { get; set; }

        public decimal? Gde_DatPerBlob { get; set; }

        [StringLength(128)]
        public string Rh_IdSd { get; set; }

        [StringLength(1)]
        public string RhRutExt { get; set; }

        //public short? RhFunJur { get; set; }

        //public short? Rh_NacCod { get; set; }

        [StringLength(200)]
        public string RH_ObsSeg { get; set; }

        //public DateTime? RhFecProg { get; set; }

        public decimal? Gde_DatPerFir { get; set; }

        [StringLength(1)]
        public string Rh_TipSocPas { get; set; }

        [StringLength(25)]
        public string RhFunCod { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal? Rh_MonSocPas { get; set; }

        [StringLength(1)]
        public string Rh_NumInsPasDv { get; set; }

        public decimal? Rh_NumInsPas { get; set; }

        [StringLength(15)]
        public string RH_NumInteExt { get; set; }

        [StringLength(60)]
        public string Rh_UbiFun { get; set; }

        //public DateTime? Rh_FecIngCar { get; set; }

        [Column(TypeName = "money")]
        public decimal? Rh_SumPun { get; set; }

        [Column(TypeName = "money")]
        public decimal? Rh_FacCom { get; set; }

        //public int? PeCodDis { get; set; }

        //public DateTime? Rh_FecVenDoc { get; set; }

        //public int? Rh_AseCor { get; set; }

        [StringLength(3)]
        public string Rh_AseTip { get; set; }

        //public short? Rh_TipSisSalCod { get; set; }

        //[StringLength(2)]
        //public string Rh_TipTraDisCod { get; set; }

        //[StringLength(2)]
        //public string Rh_ConImpCod { get; set; }

        //[StringLength(2)]
        //public string Rh_ResTraCod { get; set; }

        //[StringLength(2)]
        //public string Rh_ConDisCod { get; set; }

        //[StringLength(1)]
        //public string Rh_TipTraCod { get; set; }

        //[StringLength(3)]
        //public string UbiFunIns { get; set; }

        //public short? UbiFunIde { get; set; }

        //public DateTime? Rh_FchRes { get; set; }

        //[StringLength(7)]
        //public string Rh_NroPriRes { get; set; }
    }
}
