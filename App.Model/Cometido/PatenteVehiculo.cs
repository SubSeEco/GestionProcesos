using App.Model.Comisiones;
using App.Model.Shared;
using App.Model.Sigper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model.Cometido
{
    [Table("PatenteVehiculo")]
    public class PatenteVehiculo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Patente Vehiculo")]
        public int PatenteVehiculoId { get; set; }

        [Display(Name = "Placa Patente")]
        public string PlacaPatente { get; set; }

        [Display(Name = "Tipo Vehiculo")]
        public int? SIGPERTipoVehiculoId { get; set; }
        public virtual SIGPERTipoVehiculo TipoVehiculo { get; set; }

        /*public int TipoVehiculoId { get; set; }*/

        [Display(Name ="Región del Vehiculo")]
        public int? RegionId { get; set; }
        public virtual Region Region { get; set; }
        public string Codigo { get; set; }
    }
}
