﻿using App.Model.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ExpressiveAnnotations.Attributes;
using System.Web.Mvc;

namespace App.Model.Memorandum
{
    [Table("Memorandum")]
    public class Memorandum : Core.BaseEntity
    {
        public Memorandum()
        {
        }

        //[HiddenInput(DisplayValue = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Numero Memorandum")]
        public int MemorandumId { get; set; }

        //[Required(ErrorMessage = "Se debe indicar la fecha")]
        [Display(Name = "Fecha Solicitud")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        //[Required(ErrorMessage = "Se debe indicar la fecha")]
        [Display(Name = "Fecha Firma")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaFirma { get; set; } = DateTime.Now;

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Nombre Remitente")]
        public string Nombre { get; set; }
        [NotMapped]
        [Display(Name = "Nombre Remitente")]
        public int? NombreId { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [NotMapped]
        [Display(Name = "Rut Remitente")]
        public int Rut { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Remitente")]
        public string DV { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public int? IdUnidad { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public string UnidadDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public int? IdCalidad { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public string CalidadDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string IdGrado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string GradoDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string Grado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo Remitente")]
        public int? IdCargo { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo Remitente")]
        public string CargoDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public int? IdEstamento { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public string EstamentoDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Remitente")]
        //public int? IdPrograma { get; set; }

        //[Display(Name = "Programa Remitente")]
        //public string ProgramaDescripcion { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public int? IdConglomerado { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public string ConglomeradoDescripcion { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Remitente")]
        public string EmailRem { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Remitente")]
        public string NombreChqRem { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Nombre Remitente")]
        public string NombreDest { get; set; }
        [NotMapped]
        [Display(Name = "Nombre Destinatario")]
        public int? NombreIdDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [NotMapped]
        [Display(Name = "Rut Remitente")]
        public int RutDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Remitente")]
        public string DVDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public int? IdUnidadDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public string UnidadDescripcionDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public int? IdCalidadDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public string CalidadDescripcionDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string IdGradoDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string GradoDescripcionDest { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string Grado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo Remitente")]
        public int? IdCargoDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo Remitente")]
        public string CargoDescripcionDest { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public int? IdEstamento { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public string EstamentoDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Remitente")]
        //public int? IdPrograma { get; set; }

        //[Display(Name = "Programa Remitente")]
        //public string ProgramaDescripcion { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public int? IdConglomerado { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public string ConglomeradoDescripcion { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Remitente")]
        public string EmailDest { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Remitente")]
        public string NombreChqDest { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Nombre Remitente")]
        public string NombreSecre { get; set; }
        [NotMapped]
        [Display(Name = "Nombre Secretaria")]
        public int? NombreIdSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [NotMapped]
        [Display(Name = "Rut Remitente")]
        public int RutSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Remitente")]
        public string DVSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public int? IdUnidadSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public string UnidadDescripcionSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public int? IdCalidadSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public string CalidadDescripcionSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string IdGradoSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string GradoDescripcionSecre { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string Grado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo Remitente")]
        public int? IdCargoSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo Remitente")]
        public string CargoDescripcionSecre { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public int? IdEstamento { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public string EstamentoDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Remitente")]
        //public int? IdPrograma { get; set; }

        //[Display(Name = "Programa Remitente")]
        //public string ProgramaDescripcion { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public int? IdConglomerado { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public string ConglomeradoDescripcion { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Remitente")]
        public string EmailSecre { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Remitente")]
        public string NombreChqSecre { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Nombre Remitente")]
        public string NombreAna { get; set; }
        [NotMapped]
        [Display(Name = "Nombre Secretaria")]
        public int? NombreIdAna { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [NotMapped]
        [Display(Name = "Rut Remitente")]
        public int RutAna { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Remitente")]
        public string DVAna { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public int? IdUnidadAna { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public string UnidadDescripcionAna { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public int? IdCalidadAna { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public string CalidadDescripcionAna { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string IdGradoAna { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string GradoDescripcionAna { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string Grado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Remitente")]
        public int? IdCargoAna { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Remitente")]
        public string CargoDescripcionAna { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public int? IdEstamento { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public string EstamentoDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Remitente")]
        //public int? IdPrograma { get; set; }

        //[Display(Name = "Programa Remitente")]
        //public string ProgramaDescripcion { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public int? IdConglomerado { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public string ConglomeradoDescripcion { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Remitente")]
        public string EmailAna { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Remitente")]
        public string NombreChqAna { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Nombre Remitente")]
        public string NombreVisa1 { get; set; }
        [NotMapped]
        [Display(Name = "Nombre Visador ")]
        public int? NombreIdVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [NotMapped]
        [Display(Name = "Rut Remitente")]
        public int RutVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Remitente")]
        public string DVVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public int? IdUnidadVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public string UnidadDescripcionVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public int? IdCalidadVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public string CalidadDescripcionVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string IdGradoVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string GradoDescripcionVisa1 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string Grado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Remitente")]
        public int? IdCargoVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Remitente")]
        public string CargoDescripcionVisa1 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public int? IdEstamento { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public string EstamentoDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Remitente")]
        //public int? IdPrograma { get; set; }

        //[Display(Name = "Programa Remitente")]
        //public string ProgramaDescripcion { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public int? IdConglomerado { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public string ConglomeradoDescripcion { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Remitente")]
        public string EmailVisa1 { get; set; } 

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Remitente")]
        public string NombreChqVisa1 { get; set; } 

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Nombre Remitente")]
        public string NombreVisa2 { get; set; }
        [NotMapped]
        [Display(Name = "Nombre Visador 2")]
        public int? NombreIdVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [NotMapped]
        [Display(Name = "Rut Remitente")]
        public int RutVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Remitente")]
        public string DVVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public int? IdUnidadVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public string UnidadDescripcionVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public int? IdCalidadVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public string CalidadDescripcionVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string IdGradoVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string GradoDescripcionVisa2 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string Grado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Remitente")]
        public int? IdCargoVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Remitente")]
        public string CargoDescripcionVisa2 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public int? IdEstamento { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public string EstamentoDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Remitente")]
        //public int? IdPrograma { get; set; }

        //[Display(Name = "Programa Remitente")]
        //public string ProgramaDescripcion { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public int? IdConglomerado { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public string ConglomeradoDescripcion { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Remitente")]
        public string EmailVisa2 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Remitente")]
        public string NombreChqVisa2 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Nombre Remitente")]
        public string NombreVisa3 { get; set; }
        [NotMapped]
        [Display(Name = "Nombre Visador 3")]
        public int? NombreIdVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [NotMapped]
        [Display(Name = "Rut Remitente")]
        public int RutVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Remitente")]
        public string DVVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public int? IdUnidadVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public string UnidadDescripcionVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public int? IdCalidadVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public string CalidadDescripcionVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string IdGradoVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string GradoDescripcionVisa3 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string Grado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Remitente")]
        public int? IdCargoVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Remitente")]
        public string CargoDescripcionVisa3 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public int? IdEstamento { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public string EstamentoDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Remitente")]
        //public int? IdPrograma { get; set; }

        //[Display(Name = "Programa Remitente")]
        //public string ProgramaDescripcion { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public int? IdConglomerado { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public string ConglomeradoDescripcion { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Remitente")]
        public string EmailVisa3 { get; set; } 

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Remitente")]
        public string NombreChqVisa3 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Nombre Remitente")]
        public string NombreVisa4 { get; set; } 
        [NotMapped]
        [Display(Name = "Nombre Visador 4")]
        public int? NombreIdVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [NotMapped]
        [Display(Name = "Rut Remitente")]
        public int RutVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Remitente")]
        public string DVVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public int? IdUnidadVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public string UnidadDescripcionVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public int? IdCalidadVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public string CalidadDescripcionVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string IdGradoVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string GradoDescripcionVisa4 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string Grado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Remitente")]
        public int? IdCargoVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Remitente")]
        public string CargoDescripcionVisa4 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public int? IdEstamento { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public string EstamentoDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Remitente")]
        //public int? IdPrograma { get; set; }

        //[Display(Name = "Programa Remitente")]
        //public string ProgramaDescripcion { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public int? IdConglomerado { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public string ConglomeradoDescripcion { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Remitente")]
        public string EmailVisa4 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Remitente")]
        public string NombreChqVisa4 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Nombre Remitente")]
        public string NombreVisa5 { get; set; }
        [NotMapped]
        [Display(Name = "Nombre Visador 5")]
        public int? NombreIdVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [NotMapped]
        [Display(Name = "Rut Remitente")]
        public int RutVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Remitente")]
        public string DVVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public int? IdUnidadVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Remitente")]
        public string UnidadDescripcionVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public int? IdCalidadVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Remitente")]
        public string CalidadDescripcionVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string IdGradoVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Remitente")]
        public string GradoDescripcionVisa5 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string Grado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Remitente")]
        public int? IdCargoVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Remitente")]
        public string CargoDescripcionVisa5 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public int? IdEstamento { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Remitente")]
        //public string EstamentoDescripcion { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Remitente")]
        //public int? IdPrograma { get; set; }

        //[Display(Name = "Programa Remitente")]
        //public string ProgramaDescripcion { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public int? IdConglomerado { get; set; }

        //[Display(Name = "Conglomerado Remitente")]
        //public string ConglomeradoDescripcion { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Remitente")]
        public string EmailVisa5 { get; set; } 

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Remitente")]
        public string NombreChqVisa5 { get; set; }

        [Display(Name = "Folio")]
        public string Folio { get; set; }

        [Display(Name = "Número Documento")]
        public string NumDocumento { get; set; } = "0";

        [Display(Name = "Número Memo Referencia")]
        public string NumMemoRef { get; set; } = "0";

        [Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(Name = "Materia")]
        public string Materia { get; set; }

        [Display(Name = "Antecedentes")]
        public string Antecedentes { get; set; }

        [Display(Name = "+ Visadores ?")]
        public bool VisaChk1 { get; set; }

        [Display(Name = "+ visadores?")]
        public bool VisaChk2 { get; set; }

        [Display(Name = "+ visadores?")]
        public bool VisaChk3 { get; set; }

        [Display(Name = "+ visadores?")]
        public bool VisaChk4 { get; set; }

        [Display(Name = "+ visadores?")]
        public bool VisaChk5 { get; set; }

        [Display(Name = "+ visadores?")]
        public bool VisaChk6 { get; set; }

        [Display(Name = "+ visadores?")]
        public bool VisaChk7 { get; set; }

        [Display(Name = "+ visadores?")]
        public bool VisaChk8 { get; set; }

        [Display(Name = "+ visadores?")]
        public bool VisaChk9 { get; set; }

        [Display(Name = "+ visadores?")]
        public bool VisaChk10 { get; set; }

        [Display(Name = "Auditoría Interna")]
        public string ListaChk1 { get; set; } = "";

        [Display(Name = "Auditoría Ministerial")]
        public string ListaChk2 { get; set; } = "";

        [Display(Name = "Comisión Nacional de la Productividad")]
        public string ListaChk3 { get; set; } = "";

        [Display(Name = "Departamento Administrativo")]
        public string ListaChk4 { get; set; } = "";

        [Display(Name = "Departamento de Cooperativas")]
        public string ListaChk5 { get; set; } = "";

        [Display(Name = "División de Empresas de Menor Tamaño")]
        public string ListaChk6 { get; set; } = "";

        [Display(Name = "División Jurídica")]
        public string ListaChk7 { get; set; } = "";

        [Display(Name = "División Política Comercial e Industrial")]
        public string ListaChk8 { get; set; } = "";


        [Display(Name = "Escritorio de Empresas")]
        public string ListaChk9 { get; set; } = "";


        [Display(Name = "Gabinete de Ministro")]
        public string ListaChk10 { get; set; } = "";


        [Display(Name = "Gabinete de Subsecretario")]
        public string ListaChk11 { get; set; } = "";

        [Display(Name = "Oficina de Gestión de Proyectos Sustentables")]
        public string ListaChk12 { get; set; } = "";

        [Display(Name = "Oficina de Partes")]
        public string ListaChk13 { get; set; } = "";

        [Display(Name = "Oficina de Productividad y Emprendimiento Nacional")]
        public string ListaChk14 { get; set; } = "";

        [Display(Name = "Pymes Digitales")]
        public string ListaChk15 { get; set; } = "";

        [Display(Name = "Secretaría Regional Ministerial de Antofagasta")]
        public string ListaChk16 { get; set; } = "";

        [Display(Name = "Secretaría Regional Ministerial de Arica y Parinacota")]
        public string ListaChk17 { get; set; } = "";

        [Display(Name = "Secretaría Regional Ministerial de Atacama")]
        public string ListaChk18 { get; set; } = "";


        [Display(Name = "Secretaría Regional Ministerial de Aysén")]
        public string ListaChk19 { get; set; } = "";


        [Display(Name = "Secretaría Regional Ministerial de Coquimbo")]
        public string ListaChk20 { get; set; } = "";


        [Display(Name = "Secretaría Regional Ministerial de la Araucanía")]
        public string ListaChk21 { get; set; } = "";

        [Display(Name = "Secretaría Regional Ministerial de los Lagos")]
        public string ListaChk22 { get; set; } = "";

        [Display(Name = "Secretaría Regional Ministerial de los Ríos")]
        public string ListaChk23 { get; set; } = "";

        [Display(Name = "Secretaría Regional Ministerial de Magallanes y la Antártica Chilena")]
        public string ListaChk24 { get; set; } = "";

        [Display(Name = "Secretaría Regional Ministerial de Tarapacá")]
        public string ListaChk25 { get; set; } = "";

        [Display(Name = "Secretaría Regional Ministerial de Valparaiso")]
        public string ListaChk26 { get; set; } = "";

        [Display(Name = "Secretaría Regional Ministerial del Bío Bío")]
        public string ListaChk27 { get; set; } = "";

        [Display(Name = "Secretaría Regional Ministerial del Libertador Bernardo O'Higgins")]
        public string ListaChk28 { get; set; } = "";


        [Display(Name = "Secretaría Regional Ministerial del Maule")]
        public string ListaChk29 { get; set; } = "";


        [Display(Name = "Secretaría Regional Ministerial del Ñuble")]
        public string ListaChk30 { get; set; } = "";


        [Display(Name = "Secretaría Regional Ministerial Metropolitana")]
        public string ListaChk31 { get; set; } = "";

        [Display(Name = "Sistema Integral de Atención a la Ciudadanía")]
        public string ListaChk32 { get; set; } = "";

        [Display(Name = "Sistema Unificado de Permisos")]
        public string ListaChk33 { get; set; } = "";

        [Display(Name = "Tribunal Arbitral de Propiedad Industrial")]
        public string ListaChk34 { get; set; } = "";

        [Display(Name = "Unidad Comsionados en Turismo")]
        public string ListaChk35 { get; set; } = "";

        [Display(Name = "Unidad de Abastecimientos")]
        public string ListaChk36 { get; set; } = "";

        [Display(Name = "Unidad de Comunicaciones")]
        public string ListaChk37 { get; set; } = "";

        [Display(Name = "Unidad de Control de Gestión")]
        public string ListaChk38 { get; set; } = "";


        [Display(Name = "Unidad de Control y Rendiciones")]
        public string ListaChk39 { get; set; } = "";


        [Display(Name = "Unidad de Finanzas")]
        public string ListaChk40 { get; set; } = "";


        [Display(Name = "Unidad de Presupuestos")]
        public string ListaChk41 { get; set; } = "";

        [Display(Name = "Unidad de Procesos")]
        public string ListaChk42 { get; set; } = "";

        [Display(Name = "Unidad Gestión y Desarrollo de Personas")]
        public string ListaChk43 { get; set; } = "";

        [Display(Name = "Unidad Registro de Empresas y Sociedades")]
        public string ListaChk44 { get; set; } = "";

        [Display(Name = "Unidad Servicios Generales")]
        public string ListaChk45 { get; set; } = "";

        [Display(Name = "Unidad Tecnologías de Información y Comunicaciones")]
        public string ListaChk46 { get; set; } = "";

        [Display(Name = "Subsecretaría de Turismo")]
        public string ListaChk47 { get; set; } = "";

        [Display(Name = "Firmante")]
        public string Firmante { get; set; }
    }
}
