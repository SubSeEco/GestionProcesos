using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ExpressiveAnnotations.Attributes;

namespace App.Model.HorasExtras
{
    [Table("HorasExtras")]
    public class HorasExtras : Core.BaseEntity
    {
        /*list de destino*/
        [Display(Name = "Lista Colaborador")]
        public virtual List<Colaborador> Colaborador { get; set; } = new List<Colaborador>();

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Horas Extras Id")]
        public int HorasExtrasId { get; set; }

        [Display(Name = "Fecha Solicitud")]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [Display(Name = "Año")]
        public string Annio { get; set; }

        [Display(Name = "Mes")]
        public string Mes { get; set; }
                
        [Display(Name = "Nombre Solicitante")]
        public string Nombre { get; set; }
        //[Required(ErrorMessage = "Es necesario especificar este dato")]

        [Display(Name = "NombreId")]
        public int? NombreId { get; set; }

        [Display(Name = "DV")]
        public string DV { get; set; }

        [Display(Name = "Nombre Jefatura")]
        public string NombreJefatura { get; set; }
        //[Required(ErrorMessage = "Es necesario especificar este dato")]

        [Display(Name = "JefaturaId")]
        public int? jefaturaId { get; set; }        
        [Display(Name = "JefaturaDV")]
        public string JefaturaDV { get; set; }

        [Display(Name = "Unidad")]
        public string Unidad { get; set; }
        //[Required(ErrorMessage = "Es necesario especificar este dato")]

        [Display(Name = "UnidadId")]
        public int? UnidadId { get; set; }

        [Display(Name = "Aprobado")]
        public bool Aprobado { get; set; }

        [Display(Name = "Valor Total Horas")]
        [AssertThat("ValorTotalHoras >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        [DataType(DataType.Currency)]
        public int? ValorTotalHoras { get; set; } = 0;

        [NotMapped]
        [Display(Name = "Valor Total Horas Palabras")]
        public string ValorTotalHorasPalabras { get; set; }
                
        [Display(Name = "Mes Base Calculo")]
        public int MesBaseCalculo { get; set; }

        /*Pie de Pagina Resoluciones Programacion - 15122020*/
        [NotMapped]
        [Display(Name = "Orden")]
        public string OrdenHEProg { get; set; }

        [NotMapped]
        [Display(Name = "Firmante")]
        public string FirmanteHEProg { get; set; }

        [NotMapped]
        [Display(Name = "Cargo Firmante")]
        public string CargoFirmanteHEProg { get; set; }

        [NotMapped]
        [Display(Name = "Distribucion")]
        public string DistribucionHEProg { get; set; }

        [NotMapped]
        [Display(Name = "Vistos")]
        public string VistosHEProg { get; set; }

        
        /*Pie de Pagina Resoluciones Confirmacion Pagadas - 16122020*/
        [NotMapped]
        [Display(Name = "Orden")]
        public string OrdenHEPag { get; set; }

        [NotMapped]
        [Display(Name = "Firmante")]
        public string FirmanteHEPag { get; set; }

        [NotMapped]
        [Display(Name = "Cargo Firmante")]
        public string CargoFirmanteHEPag { get; set; }

        [NotMapped]
        [Display(Name = "Distribucion")]
        public string DistribucionHEPag { get; set; }

        [NotMapped]
        [Display(Name = "Vistos")]
        public string VistosHEPag { get; set; }

        /*Pie de Pagina Resoluciones Confirmacion Compensadas - 16122020*/
        [NotMapped]
        [Display(Name = "Orden")]
        public string OrdenHECom { get; set; }

        [NotMapped]
        [Display(Name = "Firmante")]
        public string FirmanteHECom { get; set; }

        [NotMapped]
        [Display(Name = "Cargo Firmante")]
        public string CargoFirmanteHECom { get; set; }

        [NotMapped]
        [Display(Name = "Distribucion")]
        public string DistribucionHECom { get; set; }

        [NotMapped]
        [Display(Name = "Vistos")]
        public string VistosHECom { get; set; }

        [NotMapped]
        [Display(Name = "Iniciales")]
        public string Iniciales { get; set; }
    }
}
