
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace App.Infrastructure
{
    internal sealed class GestionProcesosConfiguration : DbMigrationsConfiguration<AppContext>
    {
        public GestionProcesosConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(AppContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}