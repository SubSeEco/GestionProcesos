//using ExpressiveAnnotations.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace App.Model.Pasajes
{
    [Table("Cotizacion")]
    public class Cotizacion :Core.BaseEntity
    {
        public Cotizacion()
        {
            CotizacionDocumento = new List<CotizacionDocumento>();
        }

        /*list de docto cotizaciones*/
        [Display(Name = "Lista Cotización Docto")]
        public virtual IList<CotizacionDocumento> CotizacionDocumento { get; set; }

        /*Definicion de atributos*/

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Cotización Pasaje")]
        public int? CotizacionId { get; set; }

        [ForeignKey("Pasaje")]
        [Display(Name = "Pasaje Id")]
        public int? PasajeId { get; set; }
        public virtual Pasaje Pasaje { get; set; }

        [Display(Name = "Tipo de Cambio USD")]
        public float? TipoCambio { get; set; }

        [Display(Name = "Fecha de Tipo Cambio USD")]
        public DateTime FechaTipoCambio { get; set; }

        [Display(Name = "Valor Pasaje (peso chileno)")]
        [DataType(DataType.Currency)]
        public float ValorPasaje { get; set; }

        [Display(Name = "Nombre Empresa")]
        public string NombreEmpresa { get; set; }

        [ForeignKey("EmpresaAerolinea")]
        [Display(Name = "Nombre Empresa")]
        public int? EmpresaAerolineaId { get; set; }
        public virtual EmpresaAerolinea EmpresaAerolinea { get; set; }

        [Display(Name = "Vencimiento Cotización")]
        public DateTime VencimientoCotizacion { get; set; }

        [Display(Name = "Seleccion Cotización")]
        public bool Seleccion { get; set; } = false;
    }
}
