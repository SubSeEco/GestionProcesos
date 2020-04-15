using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using App.Model.SIGPER;

namespace App.Infrastructure.SIGPER
{ 
    public partial class AppContextTurismo : DbContext
    {
        public AppContextTurismo(): base("name=SIGPERTurismo")
        {
        }

        public virtual DbSet<PEDATPER> PEDATPER { get; set; }
        public virtual DbSet<PeDatLab> PeDatLab { get; set; }
        public virtual DbSet<PLUNILAB> PLUNILAB { get; set; }
        public virtual DbSet<ReContra> ReContra { get; set; }
        public virtual DbSet<PEFERJEFAF> PEFERJEFAF { get; set; }
        public virtual DbSet<PEFERJEFAJ> PEFERJEFAJ { get; set; }
        public virtual DbSet<PECARGOS> PECARGOS { get; set; }

        //public virtual DbSet<PEFERJEF> PEFERJEF { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AppContextTurismo>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            ////modelBuilder.Conventions.Remove<OneToOneConstraintIntroductionConvention>();

            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }
}
