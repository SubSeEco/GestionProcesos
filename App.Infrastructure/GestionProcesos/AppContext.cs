using App.Model.Cometido;
using App.Model.Comisiones;
using App.Model.Core;
using App.Model.FirmaDocumento;
using App.Model.GestionDocumental;
using App.Model.HorasExtras;
using App.Model.InformeHSA;
using App.Model.Pasajes;
using App.Model.ProgramacionHorasExtraordinarias;
using App.Model.Shared;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace App.Infrastructure.GestionProcesos
{
    public class AppContext : DbContext
    {
        // Your context has been configured to use a 'App.Context' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'App..Persistence.App.Context' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'App.Context' 
        // connection string in the application configuration file.

        public AppContext() : base("name=GestionProcesos")
        {
            System.Diagnostics.Debug.WriteLine("New AppContext...");
            this.Database.CommandTimeout = 180;
        }

        public virtual DbSet<Consulta> Consulta { get; set; }
        public virtual DbSet<Denuncia> Denuncia { get; set; }
        public virtual DbSet<Configuracion> Configuracion { get; set; }
        public virtual DbSet<DefinicionProceso> DefinicionProceso { get; set; }
        public virtual DbSet<DefinicionWorkflow> DefinicionWorkflow { get; set; }
        public virtual DbSet<Proceso> Proceso { get; set; }
        public virtual DbSet<Workflow> Workflow { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Genero> Genero { get; set; }
        public virtual DbSet<Subsecretaria> Subsecretaria { get; set; }
        public virtual DbSet<TipoPagoSIGFE> TipoPagoSIGFE { get; set; }
        public virtual DbSet<Programa> Programa { get; set; }
        public virtual DbSet<Institucion> Institucion { get; set; }
        public virtual DbSet<Documento> Documento { get; set; }
        public virtual DbSet<TipoDocumento> TipoDocumento { get; set; }
        public virtual DbSet<TipoAprobacion> TipoAprobacion { get; set; }
        public virtual DbSet<TipoEjecucion> TipoEjecucion { get; set; }
        public virtual DbSet<Accion> Accion { get; set; }
        public virtual DbSet<Entidad> Entidad { get; set; }
        public virtual DbSet<Rubrica> Rubrica { get; set; }
        public virtual DbSet<TipoPrivacidad> TipoPrivacidad { get; set; }
        public virtual DbSet<Cometido> Cometido { get; set; }
        public virtual DbSet<Destinos> Destinos { get; set; }
        public virtual DbSet<Viatico> Viatico { get; set; }
        public virtual DbSet<ViaticoHonorario> ViaticoHonorario { get; set; }
        public virtual DbSet<TipoAsignacion> TipoAsignacion { get; set; }
        public virtual DbSet<TipoCapitulo> TipoCapitulo { get; set; }
        public virtual DbSet<TipoItem> TipoItem { get; set; }
        public virtual DbSet<TipoPartida> TipoPartida { get; set; }
        public virtual DbSet<TipoSubAsignacion> TipoSubAsignacion { get; set; }
        public virtual DbSet<TipoSubTitulo> TipoSubTitulo { get; set; }
        public virtual DbSet<GeneracionCDP> GeneracionCDP { get; set; }
        public virtual DbSet<CentroCosto> CentroCosto { get; set; }
        public virtual DbSet<Parrafos> Parrafos { get; set; }
        public virtual DbSet<Pasaje> Pasaje { get; set; }
        public virtual DbSet<DestinosPasajes> DestinosPasajes { get; set; }
        public virtual DbSet<Mantenedor> Mantenedor { get; set; }
        public virtual DbSet<EmpresaAerolinea> EmpresaAerolinea { get; set; }
        public virtual DbSet<Cotizacion> Cotizacion { get; set; }
        public virtual DbSet<CotizacionDocumento> CotizacionDocumento { get; set; }
        public virtual DbSet<Pais> Pais { get; set; }
        public virtual DbSet<Ciudad> Ciudads { get; set; }
        public virtual DbSet<Localidad> Localidad { get; set; }
        public virtual DbSet<Comisiones> Comisiones { get; set; }
        public virtual DbSet<DestinosComision> DestinosComision { get; set; }
        public virtual DbSet<GeneracionCDPComision> GeneracionCDPComision { get; set; }
        public virtual DbSet<ParametrosComisiones> ParametrosComisiones { get; set; }
        public virtual DbSet<ParrafoComisiones> ParrafoComisiones { get; set; }
        public virtual DbSet<ViaticoInternacional> ViaticoInternacional { get; set; }
        public virtual DbSet<FirmaDocumento> FirmaDocumento { get; set; }
        public virtual DbSet<SIGPERTipoVehiculo> SIGPERTipoVehiculo { get; set; }
        public virtual DbSet<InformeHSA> InformeHSA { get; set; }
        public virtual DbSet<EstadoProceso> EstadoProceso { get; set; }
        //public virtual DbSet<Memorandum> Memorandum { get; set; }
        public virtual DbSet<RegionComunaContraloria> RegionComunaContraloria { get; set; }
        public virtual DbSet<GD> GD { get; set; }
        public virtual DbSet<GDOrigen> GDOrigen { get; set; }
        public virtual DbSet<ProgramacionHorasExtraordinarias> ProgramacionHorasExtraordinarias { get; set; }
        public virtual DbSet<HorasExtras> HorasExtras { get; set; }
        public virtual DbSet<GeneracionResolucion> GeneracionResolucion { get; set; }
        //public virtual DbSet<FirmaDocumentoGenerico> FirmaDocumentoGenerico { get; set; }
        public virtual DbSet<Festivo> Festivo { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AppContext>(null);
            base.OnModelCreating(modelBuilder);

            if (modelBuilder != null)
                modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
