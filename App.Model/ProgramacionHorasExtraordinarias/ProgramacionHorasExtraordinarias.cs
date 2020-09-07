using App.Model.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ExpressiveAnnotations.Attributes;
using System.Web.Mvc;

namespace App.Model.ProgramacionHorasExtraordinarias
{
    [Table("ProgramacionHorasExtraordinarias")]
    public class ProgramacionHorasExtraordinarias : Core.BaseEntity
    {
        public ProgramacionHorasExtraordinarias()
        {
        }

        //[HiddenInput(DisplayValue = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int ProgramacionHorasExtraordinariasId { get; set; }

        //[Required(ErrorMessage = "Se debe indicar la fecha")]
        [Display(Name = "Fecha")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Display(Name = "Unidad Jefatura")]
        public int? Pl_UndCod { get; set; }

        [Display(Name = "Unidad Jefatura")]
        public string Pl_UndDes { get; set; }

        [Display(Name = "Email Jefatura")]
        public string Email { get; set; }

        [Display(Name = "Jefatura (Nombre)")]
        public string To { get; set; }

        public int? Pl_UndCodFunc1 { get; set; }

        [Display(Name = "Unidad Jefatura")]
        public string Pl_UndDesFunc1 { get; set; }

        [Display(Name = "Email Jefatura")]
        public string EmailFunc1 { get; set; }

        [Display(Name = "Jefatura (Nombre)")]
        public string ToFunc1 { get; set; }

        [Display(Name = "Firma y Timbre")]
        public string FirmaTimbre { get; set; }
    }
}
