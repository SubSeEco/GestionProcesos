using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using App.Model.SIGPER;

namespace App.Infrastructure.SIGPER
{ 
    public partial class AppContextTurismo : DbContext
    {
        public AppContextTurismo(): base("name=SIGPERTurismo")
        {
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
        }
        public virtual DbSet<PEDATPER> PEDATPER { get; set; }
        public virtual DbSet<PeDatLab> PeDatLab { get; set; }
        public virtual DbSet<PLUNILAB> PLUNILAB { get; set; }
        public virtual DbSet<ReContra> ReContra { get; set; }
        public virtual DbSet<PEFERJEFAF> PEFERJEFAF { get; set; }
        public virtual DbSet<PEFERJEFAJ> PEFERJEFAJ { get; set; }
        public virtual DbSet<PECARGOS> PECARGOS { get; set; }
        public virtual DbSet<DGESCALAFONES> DGESCALAFONES { get; set; }
        public virtual DbSet<DGESTAMENTOS> DGESTAMENTOS { get; set; }        
        public virtual DbSet<REPYT> REPYT { get; set; }
        public virtual DbSet<DGCONTRATOS> DGCONTRATOS { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AppContextTurismo>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }
}
