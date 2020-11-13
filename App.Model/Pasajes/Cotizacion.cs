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

        /*Nuevos requerimientos 27102020*/
        [Display(Name = "ID Orden de Compra")]
        public string NumeroOrdenCompra { get; set; }

        [Display(Name = "Clase de Pasaje")]
        public string ClasePasaje { get; set; }

        [Display(Name = "Otra clase pasaje")]
        public string OtroPasaje { get; set; }

        [Display(Name = "Forma de Adquisicion del Pasaje")]
        public string FormaAdquisicion { get; set; }

        [Display(Name = "Otro Mecanismo Compra")]
        public string OtroMecanismo { get; set; }

        [Display(Name = "Fecha de Adquisicion")]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        public DateTime FechaAdquisicion { get; set; } = DateTime.Now;

        [Display(Name = "Fecha del Vuelo")]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        public DateTime FechaVuelo { get; set; } = DateTime.Now;

        [Display(Name = "Monto Viatico")]
        //[DataType(DataType.Currency)]
        public int MontoViatico { get; set; } = 0;


    }
}
