using App.Model.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ExpressiveAnnotations.Attributes;
using System.Web.Mvc;
using App.Model.Cometido;

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
        public int HDPago { get; set; } = 0;

        [Display(Name = "Horas Diurnas a Compensar")]
        public int HDCompensar { get; set; } = 0;

        [Display(Name = "Horas Nocturnas a Pago")]
        public int HNPago { get; set; } = 0;

        [Display(Name = "Horas Nocturnas a Compensar")]
        public int HNCompensar { get; set; } = 0;

        [Required(ErrorMessage = "Se deben señalar las descripcion de las tareas")]
        [Display(Name = "Descripcion de tareas")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        /*Se agraga atributo de aprobados, para edicion de GP (04112020)*/
        [Display(Name = "Horas Diurnas a Pago Aprobadas")]
        [AssertThat("HDPagoAprobados >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? HDPagoAprobados { get; set; } = 0;

        [Display(Name = "Horas Diurnas a Compensar Aprobadas")]
        [AssertThat("HDCompensarAprobados >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? HDCompensarAprobados { get; set; } = 0;

        [Display(Name = "Horas Nocturnas a Pago Aprobadas")]
        [AssertThat("HNPagoAprobados >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? HNPagoAprobados { get; set; } = 0;

        [Display(Name = "Horas Nocturnas a Compensar Aprobadas")]
        [AssertThat("HNCompensarAprobados >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        public int? HNCompensarAprobados { get; set; } = 0;
                
        [Display(Name = "Cumplimiento de tareas programadas")]
        [DataType(DataType.MultilineText)]
        public string ObservacionesConfirmacion { get; set; }

        [Display(Name = "Observaciones")]
        [DataType(DataType.MultilineText)]
        public string ObservacionesJefatura { get; set; }

        [Display(Name = "Valor Horas Diurnas")]
        [AssertThat("ValorHorasDiurnas >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        [DataType(DataType.Currency)]
        public int? ValorHorasDiurnas { get; set; } = 0;

        [Display(Name = "Valor Pagado Horas Diurnas")]
        [AssertThat("ValorHorasDiurnas >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        [DataType(DataType.Currency)]
        public int? ValorPagadoHD { get; set; } = 0;

        [Display(Name = "Valor Horas Nocturnas")]
        [AssertThat("ValorHorasNocturnas >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        [DataType(DataType.Currency)]
        public int? ValorHorasNocturnas { get; set; } = 0;

        [Display(Name = "Valor Pagado Horas Nocturnas")]
        [AssertThat("ValorHorasDiurnas >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        [DataType(DataType.Currency)]
        public int? ValorPagadoHN { get; set; } = 0;

        [Display(Name = "Valor Total Pago")]
        [AssertThat("ValorTotalPago >= 0", ErrorMessage = "El valor debe ser mayor o igual a 0")]
        [DataType(DataType.Currency)]
        public int? ValorTotalPago { get; set; } = 0;

        /*se agregan los datos necesarios para las imputaciones - 17122020*/
        [Display(Name = "SubTitulo")]
        public int? TipoSubTituloId { get; set; } = 0;
        public virtual TipoSubTitulo TipoSubTitulo { get; set; }

        [Display(Name = "Asignación")]
        public int? TipoAsignacionId { get; set; } = 0;
        public virtual TipoAsignacion TipoAsignacion { get; set; }

        [Display(Name = "Item")]
        public int? TipoItemId { get; set; } = 0;
        public virtual TipoItem TipoItem { get; set; }

    }
}
