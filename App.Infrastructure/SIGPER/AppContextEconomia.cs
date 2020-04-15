using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using App.Model.Cometido;
using App.Model.Comisiones;
using App.Model.SIGPER;

namespace App.Infrastructure.SIGPER
{
    public partial class AppContextEconomia : DbContext
    {
        public AppContextEconomia(): base("name=SIGPEREconomia")
        {
        }
        public virtual DbSet<PEDATPER> PEDATPER { get; set; }
        public virtual DbSet<PeDatLab> PeDatLab { get; set; }
        public virtual DbSet<PLUNILAB> PLUNILAB { get; set; }
        public virtual DbSet<PEFERJEFAF> PEFERJEFAF { get; set; }
        public virtual DbSet<PEFERJEFAJ> PEFERJEFAJ { get; set; }
        public virtual DbSet<PECARGOS> PECARGOS { get; set; }
        public virtual DbSet<DGREGIONES> DGREGIONES { get; set; }
        public virtual DbSet<DGCOMUNAS> DGCOMUNAS { get; set; }
        public virtual DbSet<DGESCALAFONES> DGESCALAFONES { get; set; }
        public virtual DbSet<DGESTAMENTOS> DGESTAMENTOS { get; set; }
        public virtual DbSet<ReContra> ReContra { get; set; }
        public virtual DbSet<REPYT> REPYT { get; set; }
        public virtual DbSet<DGCONTRATOS> DGCONTRATOS { get; set; }
        public virtual DbSet<SIGPERTipoVehiculo> SIGPERTipoVehiculo { get; set; }
        public virtual DbSet<CentroCosto> CentroCosto { get; set; }
        public virtual DbSet<TipoAsignacion> TipoAsignacion { get; set; }
        public virtual DbSet<TipoCapitulo> TipoCapitulo { get; set; }
        public virtual DbSet<TipoItem> TipoItem { get; set; }
        public virtual DbSet<TipoSubAsignacion> TipoSubAsignacion { get; set; }
        public virtual DbSet<TipoSubTitulo> TipoSubTitulo { get; set; }

        //public virtual DbSet<PEFERJEF> PEFERJEF { get; set; }
        //public virtual DbSet<DGPAISES> DGPAISES { get; set; }
        //public virtual DbSet<SIGPERTipoReembolso> SIGPERTipoReembolso { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AppContextEconomia>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }
}
