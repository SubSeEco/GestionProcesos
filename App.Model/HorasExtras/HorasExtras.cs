using App.Model.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ExpressiveAnnotations.Attributes;
using System.Web.Mvc;

namespace App.Model.HorasExtras
{
    [Table("HorasExtras")]
    public class HorasExtras : Core.BaseEntity
    {
        public HorasExtras()
        {
            Colaborador = new List<Colaborador>();
        }

        /*list de destino*/
        [Display(Name = "Lista Colaborador")]
        public virtual List<Colaborador> Colaborador { get; set; }

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
    }
}
