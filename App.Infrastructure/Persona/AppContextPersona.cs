using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace App.Infrastructure.GestionProcesos
{
    public class AppContextPersona : DbContext
    {
        // Your context has been configured to use a 'App.Context' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'App..Persistence.App.Context' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'App.Context' 
        // connection string in the application configuration file.

        public AppContextPersona() : base("name=Prueba")
        {
            System.Diagnostics.Debug.WriteLine("New AppContext...");
        }

        public virtual DbSet<App.Model.Persona.Persona> Persona { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AppContextPersona>(null);
            base.OnModelCreating(modelBuilder);

            if (modelBuilder != null)
                modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
