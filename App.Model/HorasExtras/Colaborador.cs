using App.Model.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ExpressiveAnnotations.Attributes;
using System.Web.Mvc;

namespace App.Model.HorasExtras
{
    [Table("Colaborador")]
    public class Colaborador 
    {
        public Colaborador()
        { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int ColaboradorId { get; set; }

        //[ForeignKey("HorasExtras")]
        [Display(Name = "HorasExtrasId")]
        public int? HorasExtrasId { get; set; }
        //public virtual HorasExtras HorasExtras { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "Nombre Id")]
        public int? NombreId { get; set; }

        [Display(Name = "DV")]
        public string DV { get; set; }

        [Display(Name = "Horas Diurnas a Pago")]
        public int HDPago { get; set; }

        [Display(Name = "Horas Diurnas a Compensar")]
        public int HDCompensar { get; set; }

        [Display(Name = "Horas Nocturnas a Pago")]
        public int HNPago { get; set; }

        [Display(Name = "Horas Nocturnas a Compensar")]
        public int HNCompensar { get; set; }

        [Required(ErrorMessage = "Se deben señalar las descripcion de las tareas")]
        [Display(Name = "Descripcion de tareas")]
        public string Descripcion { get; set; }
    }
}
