﻿using App.Model.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.GestionDocumental
{
    [Table("GDIngreso")]
    public class GD : BaseEntity
    {
        public GD()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int GDId { get; set; }

        [Display(Name = "Tipo ingreso")]
        public int? GDTipoId { get; set; }
        public virtual GDTipo GDTipo { get; set; }

        [Display(Name = "Fecha solicitud")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Asunto")]
        [DataType(DataType.MultilineText)]
        public string Asunto { get; set; }

        [Display(Name = "Referencia")]
        [DataType(DataType.MultilineText)]
        public string Referencia { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Display(Name = "Autor")]
        public string Autor { get; set; }

        [Display(Name = "Grupo")]
        public int? GrupoId { get; set; }
        public virtual Grupo Grupo { get; set; }

        [Display(Name = "Unidad destino")]
        public int? Pl_UndCod { get; set; }

        [Display(Name = "Unidad destino")]
        public string Pl_UndDes { get; set; }

        [Display(Name = "Usuario destino")]
        public string UsuarioDestino { get; set; }

        [Display(Name = "Organización")]
        public string OrganizacionId { get; set; }

        [Display(Name = "Organización")]
        public string RazonSocial { get; set; }

        [Display(Name = "Folio")]
        public string Folio { get; set; }

        [Display(Name = "Código de barra")]
        public byte[] BarCode { get; set; }

        public void GetFolio()
        {
            var year = DateTime.Now.Year;
            var unit = this.Pl_UndCod.ToString().PadLeft(6, '0');
            var organization = !string.IsNullOrWhiteSpace(this.OrganizacionId) ? this.OrganizacionId.ToString().PadLeft(5, '0') : "0".PadLeft(5, '0');
            var filetype = this.GDTipoId.ToString().PadLeft(3, '0');
            var secuence = "0".PadLeft(7, '0');
            this.Folio = string.Concat(year, unit, organization, filetype, secuence);
        }
    }
}
