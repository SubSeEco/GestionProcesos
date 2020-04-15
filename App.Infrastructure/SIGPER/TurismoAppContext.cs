using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using App.Model.SIGPER;

namespace App.Infrastructure
{ 
    public partial class TurismoAppContext : DbContext
    {
        public TurismoAppContext(): base("name=SIGPERTurismo")
        {
        }

        public virtual DbSet<PEDATPER> PEDATPER { get; set; }
        public virtual DbSet<PeDatLab> PeDatLab { get; set; }
        public virtual DbSet<PLUNILAB> PLUNILAB { get; set; }
        public virtual DbSet<ReContra> ReContra { get; set; }

        public virtual DbSet<PEFERJEF> PEFERJEF { get; set; }
        public virtual DbSet<PEFERJEFAF> PEFERJEFAF { get; set; }
        public virtual DbSet<PEFERJEFAJ> PEFERJEFAJ { get; set; }
        public virtual DbSet<PECARGOS> PECARGOS { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AppContext>(null);

            modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(18, 6));


            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            ////modelBuilder.Conventions.Remove<OneToOneConstraintIntroductionConvention>();

            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_InstPub)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_DvNuInt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_PaterFun)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_MaterFun)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_NombFun)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_DirCal)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_DirNum)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_DirDep)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_DirBlo)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Pl_CodPla)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Pl_CodCom)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_Telefono)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_LugNac)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_SexoCod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_EstCivi)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_IncMil)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_EstLab)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_Foto)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_TarjCAs)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_ObsMen)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_FunMed)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_FunEnf)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_FunAle)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_FunEme)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_FunEst)
                .HasPrecision(10, 4);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_Bienest)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_TipSocB)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_BieJub)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_BieEstL)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_BieSuel)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RE_Ac1EspCod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RE_Ac2EspCod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_BcoCod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_BcoAbo)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_CtaCrr)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_BcoEst)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_ClaIne)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_Mail)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_TelOfi)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_FotoTyp)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_Celular)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_ViPoCo)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_SitCant)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Pl_CodReg)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_IdeFun)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_Indica)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_PassWeb)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_KeyWeb)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_CalInd)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_MailPer)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_AntUti)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_BieBlq)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_BieBlqLeyA)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_BieBlqLeyB)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_AsiMarca)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_BieSegSal)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_NomFunCap)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_NomLDAP)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_IndConAct)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RhH_TarjTip)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_BieComUna)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_BieComDes)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_MarcaAC)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.PeDatPerAutEle)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.PeDatPerAutEst)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.PeDatPerIndExt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.PeDatPerChq)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_Alias)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.DgAgrCod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Gde_DatPerBlob)
                .HasPrecision(15, 0);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_IdSd)
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RhRutExt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_ObsSeg)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Gde_DatPerFir)
                .HasPrecision(15, 0);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_TipSocPas)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RhFunCod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_MonSocPas)
                .HasPrecision(10, 4);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_NumInsPasDv)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_NumInsPas)
                .HasPrecision(13, 0);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.RH_NumInteExt)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_UbiFun)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_SumPun)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_FacCom)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_AseTip)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_TipTraDisCod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_ConImpCod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_ResTraCod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_ConDisCod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_TipTraCod)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.UbiFunIns)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PEDATPER>()
                .Property(e => e.Rh_NroPriRes)
                .IsFixedLength()
                .IsUnicode(false);
        }
    }
}
