using App.Model.Shared;
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
        [Display(Name = "Nombre Remitente")]
        public string Nombre { get; set; }
        [Display(Name = "Nombre Remitente")]
        public int? NombreId { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Remitente")]
        public int Rut { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
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
        [Display(Name = "Calidad Juridica Remitente")]
        public int? IdCalidad { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Juridica Remitente")]
        public string CalidadDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "GradoUES Remitente")]
        public string IdGrado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
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

        [Display(Name = "Folio")]
        public string Folio { get; set; }

        [NotMapped]
        [Display(Name = "Firmante")]
        public string Firmante { get; set; }

        [NotMapped]
        [Display(Name = "Cargo Firmante")]
        public string CargoFirmante { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Destinatario")]
        public string NombreDest { get; set; }
        [Display(Name = "Nombre Destinatario")]
        public int? NombreIdDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Destinatario")]
        public int RutDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Destinatario")]
        public string DVDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Destinatario")]
        public int? IdUnidadDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Destinatario")]
        public string UnidadDescripcionDest { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Destinatario")]
        //public int? IdCalidadDest { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Destinatario")]
        //public string CalidadDescripcionDest { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Destinatario")]
        //public string IdGradoDest { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Destinatario")]
        //public string GradoDescripcionDest { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string GradoDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo Destinatario")]
        public int? IdCargoDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo Destinatario")]
        public string CargoDescripcionDest { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Destinatario")]
        //public int? IdEstamentoDest { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Destinatario")]
        //public string EstamentoDescripcionDest { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Destinatario")]
        //public int? IdProgramaDest { get; set; }

        //[Display(Name = "Programa Destinatario")]
        //public string ProgramaDescripcionDest { get; set; }

        //[Display(Name = "Conglomerado Destinatario")]
        //public int? IdConglomeradoDest { get; set; }

        //[Display(Name = "Conglomerado Destinatario")]
        //public string ConglomeradoDescripcionDest { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Destinatario")]
        public string EmailDest { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Destinatario")]
        public string NombreChqDest { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Secretaria")]
        public string NombreSecreDest { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Secretaria")]
        public string EmailSecreDest { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Secretaria")]
        public string NombreSecre { get; set; }
        [Display(Name = "Nombre Secretaria")]
        public int? NombreIdSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Secretaria")]
        public int RutSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Secretaria")]
        public string DVSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Secretaria")]
        public int? IdUnidadSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Secreataria")]
        public string UnidadDescripcionSecre { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Secretaria")]
        //public int? IdCalidadSecre { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Secretaria")]
        //public string CalidadDescripcionSecre { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Secretaria")]
        //public string IdGradoSecre { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Secretaria")]
        //public string GradoDescripcionSecre { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string GradoSecre { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Secretaria")]
        //public int? IdCargoSecre { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Secretaria")]
        //public string CargoDescripcionSecre { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Secretaria")]
        //public int? IdEstamentoSecre { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Secretaria")]
        //public string EstamentoDescripcionSecre { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Secretaria")]
        //public int? IdProgramaSecre { get; set; }

        //[Display(Name = "Programa Secretaria")]
        //public string ProgramaDescripcionSecre { get; set; }

        //[Display(Name = "Conglomerado Secretaria")]
        //public int? IdConglomeradoSecre { get; set; }

        //[Display(Name = "Conglomerado Secretaria")]
        //public string ConglomeradoDescripcionSecre { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Secretaria")]
        public string EmailSecre { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Secretaria")]
        public string NombreChqSecre { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Visador 1")]
        public string NombreVisa1 { get; set; }
        [Display(Name = "Nombre Visador 1")]
        public int? NombreIdVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Visador 1")]
        public int RutVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Visador 1")]
        public string DVVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 1")]
        public int? IdUnidadVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 1")]
        public string UnidadDescripcionVisa1 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 1")]
        //public int? IdCalidadVisa1 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 1")]
        //public string CalidadDescripcionVisa1 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 1")]
        //public string IdGradoVisa1 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 1")]
        //public string GradoDescripcionVisa1 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string GradoVisa1 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 1")]
        //public int? IdCargoVisa1 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 1")]
        //public string CargoDescripcionVisa1 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 1")]
        //public int? IdEstamentoVisa1 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 1")]
        //public string EstamentoDescripcionVisa1 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Visador 1")]
        //public int? IdProgramaVisa1 { get; set; }

        //[Display(Name = "Programa  Visador 1")]
        //public string ProgramaDescripcionVisa1 { get; set; }

        //[Display(Name = "Conglomerado Visador 1")]
        //public int? IdConglomeradoVisa1 { get; set; }

        //[Display(Name = "Conglomerado Visador 1")]
        //public string ConglomeradoDescripcionVisa1 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Visador 1")]
        public string EmailVisa1 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Visador 1")]
        public string NombreChqVisa1 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Visador 2")]
        public string NombreVisa2 { get; set; }
        [Display(Name = "Nombre Visador 2")]
        public int? NombreIdVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Visador 2")]
        public int RutVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Visador 2")]
        public string DVVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 2")]
        public int? IdUnidadVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 2")]
        public string UnidadDescripcionVisa2 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 2")]
        //public int? IdCalidadVisa2 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 2")]
        //public string CalidadDescripcionVisa2 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 2")]
        //public string IdGradoVisa2 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 2")]
        //public string GradoDescripcionVisa2 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string GradoVisa2 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 2")]
        //public int? IdCargoVisa2 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 2")]
        //public string CargoDescripcionVisa2 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 2")]
        //public int? IdEstamentoVisa2 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 2")]
        //public string EstamentoDescripcionVisa2 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Visador 2")]
        //public int? IdProgramaVisa2 { get; set; }

        //[Display(Name = "Programa Visador 2")]
        //public string ProgramaDescripcionVisa2 { get; set; }

        //[Display(Name = "Conglomerado Visador 2")]
        //public int? IdConglomeradoVisa2 { get; set; }

        //[Display(Name = "Conglomerado Visador 2")]
        //public string ConglomeradoDescripcionVisa2 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Visador 2")]
        public string EmailVisa2 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Visador 2")]
        public string NombreChqVisa2 { get; set; }

        [Display(Name = "Número de Documento")]
        public string NumDocumento { get; set; } = "0";

        [Display(Name = "Número Memo Referencia")]
        public string NumMemoRef { get; set; } = "0";

        [Display(Name = "Lista Distribución")]
        public string Distribucion { get; set; }

        [Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(Name = "Antecedentes")]
        public string Antecedentes { get; set; }

        [Display(Name = "Materia")]
        public string Materia { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Visador 3")]
        public string NombreVisa3 { get; set; }
        [Display(Name = "Nombre Visador 3")]
        public int? NombreIdVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Visador 3")]
        public int RutVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Visador 3")]
        public string DVVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 3")]
        public int? IdUnidadVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 3")]
        public string UnidadDescripcionVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 3")]
        //public int? IdCalidadVisa3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 3")]
        //public string CalidadDescripcionVisa3 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 3")]
        //public string IdGradoVisa3 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 3")]
        //public string GradoDescripcionVisa3 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string GradoVisa3 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 3")]
        //public int? IdCargoVisa3 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 3")]
        //public string CargoDescripcionVisa3 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 3")]
        //public int? IdEstamentoVisa3 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 3")]
        //public string EstamentoDescripcionVisa3 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Visador 3")]
        //public int? IdProgramaVisa3 { get; set; }

        //[Display(Name = "Programa Visador 3")]
        //public string ProgramaDescripcionVisa3 { get; set; }

        //[Display(Name = "Conglomerado Visador 3")]
        //public int? IdConglomeradoVisa3 { get; set; }

        //[Display(Name = "Conglomerado Visador 3")]
        //public string ConglomeradoDescripcionVisa3 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Visador 3")]
        public string EmailVisa3 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Visador 3")]
        public string NombreChqVisa3 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Visador 4")]
        public string NombreVisa4 { get; set; }
        [Display(Name = "Nombre Visador 4")]
        public int? NombreIdVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Visador 4")]
        public int RutVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Visador 4")]
        public string DVVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 4")]
        public int? IdUnidadVisa4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 4")]
        public string UnidadDescripcionVisa4 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 4")]
        //public int? IdCalidadVisa4 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 4")]
        //public string CalidadDescripcionVisa4 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 4")]
        //public string IdGradoVisa4 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 4")]
        //public string GradoDescripcionVisa4 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string GradoVisa4 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 4")]
        //public int? IdCargoVisa4 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 4")]
        //public string CargoDescripcionVisa4 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 4")]
        //public int? IdEstamentoVisa4 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 4")]
        //public string EstamentoDescripcionVisa4 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Visador 4")]
        //public int? IdProgramaVisa4 { get; set; }

        //[Display(Name = "Programa Visador 4")]
        //public string ProgramaDescripcionVisa4 { get; set; }

        //[Display(Name = "Conglomerado Visador 4")]
        //public int? IdConglomeradoVisa4 { get; set; }

        //[Display(Name = "Conglomerado Visador 4")]
        //public string ConglomeradoDescripcionVisa4 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Visador 4")]
        public string EmailVisa4 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Visador 4")]
        public string NombreChqVisa4 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Visador 5")]
        public string NombreVisa5 { get; set; }
        [Display(Name = "Nombre Visador 5")]
        public int? NombreIdVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Visador 5")]
        public int RutVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Visador 5")]
        public string DVVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 5")]
        public int? IdUnidadVisa5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 5")]
        public string UnidadDescripcionVisa5 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 5")]
        //public int? IdCalidadVisa5 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 5")]
        //public string CalidadDescripcionVisa5 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 5")]
        //public string IdGradoVisa5 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 5")]
        //public string GradoDescripcionVisa5 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string GradoVisa5 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 5")]
        //public int? IdCargoVisa5 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 5")]
        //public string CargoDescripcionVisa5 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 5")]
        //public int? IdEstamentoVisa5 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 5")]
        //public string EstamentoDescripcionVisa5 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Visador 5")]
        //public int? IdProgramaVisa5 { get; set; }

        //[Display(Name = "Programa Visador 5")]
        //public string ProgramaDescripcionVisa5 { get; set; }

        //[Display(Name = "Conglomerado Visador 5")]
        //public int? IdConglomeradoVisa5 { get; set; }

        //[Display(Name = "Conglomerado Visador 5")]
        //public string ConglomeradoDescripcionVisa5 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Visador 5")]
        public string EmailVisa5 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Visador 5")]
        public string NombreChqVisa5 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Visador 6")]
        public string NombreVisa6 { get; set; }
        [Display(Name = "Nombre Visador 6")]
        public int? NombreIdVisa6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Visador 6")]
        public int RutVisa6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Visador 6")]
        public string DVVisa6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 6")]
        public int? IdUnidadVisa6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 6")]
        public string UnidadDescripcionVisa6 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 6")]
        //public int? IdCalidadVisa6 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 6")]
        //public string CalidadDescripcionVisa6 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 6")]
        //public string IdGradoVisa6 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 6")]
        //public string GradoDescripcionVisa6 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string GradoVisa6 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 6")]
        //public int? IdCargoVisa6 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 6")]
        //public string CargoDescripcionVisa6 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 6")]
        //public int? IdEstamentoVisa6 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 6")]
        //public string EstamentoDescripcionVisa6 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Visador 6")]
        //public int? IdProgramaVisa6 { get; set; }

        //[Display(Name = "Programa Visador 6")]
        //public string ProgramaDescripcionVisa6 { get; set; }

        //[Display(Name = "Conglomerado Visador 6")]
        //public int? IdConglomeradoVisa6 { get; set; }

        //[Display(Name = "Conglomerado Visador 6")]
        //public string ConglomeradoDescripcionVisa6 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Visador 6")]
        public string EmailVisa6 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Visador 6")]
        public string NombreChqVisa6 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Visador 7")]
        public string NombreVisa7 { get; set; }
        [Display(Name = "Nombre Visador 7")]
        public int? NombreIdVisa7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Visador 7")]
        public int RutVisa7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Visador 7")]
        public string DVVisa7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 7")]
        public int? IdUnidadVisa7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 7")]
        public string UnidadDescripcionVisa7 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 7")]
        //public int? IdCalidadVisa7 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 7")]
        //public string CalidadDescripcionVisa7 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 7")]
        //public string IdGradoVisa7 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 7")]
        //public string GradoDescripcionVisa7 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string GradoVisa7 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 7")]
        //public int? IdCargoVisa7 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 7")]
        //public string CargoDescripcionVisa7 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 7")]
        //public int? IdEstamentoVisa7 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 7")]
        //public string EstamentoDescripcionVisa7 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Visador 7")]
        //public int? IdProgramaVisa7 { get; set; }

        //[Display(Name = "Programa Visador 7")]
        //public string ProgramaDescripcionVisa7 { get; set; }

        //[Display(Name = "Conglomerado Visador 7")]
        //public int? IdConglomeradoVisa7 { get; set; }

        //[Display(Name = "Conglomerado Visador 7")]
        //public string ConglomeradoDescripcionVisa7 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Visador 7")]
        public string EmailVisa7 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Visador 7")]
        public string NombreChqVisa7 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Visador 8")]
        public string NombreVisa8 { get; set; }
        [Display(Name = "Nombre Visador 8")]
        public int? NombreIdVisa8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Visador 8")]
        public int RutVisa8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Visador 8")]
        public string DVVisa8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 8")]
        public int? IdUnidadVisa8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 8")]
        public string UnidadDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 8")]
        //public int? IdCalidadVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 8")]
        //public string CalidadDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 8")]
        //public string IdGradoVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 8")]
        //public string GradoDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string GradoVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 8")]
        //public int? IdCargoVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 8")]
        //public string CargoDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 8")]
        //public int? IdEstamentoVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 8")]
        //public string EstamentoDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Visador 8")]
        //public int? IdProgramaVisa8 { get; set; }

        //[Display(Name = "Programa Visador 8")]
        //public string ProgramaDescripcionVisa8 { get; set; }

        //[Display(Name = "Conglomerado Visador 8")]
        //public int? IdConglomeradoVisa8 { get; set; }

        //[Display(Name = "Conglomerado Visador 8")]
        //public string ConglomeradoDescripcionVisa8 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Visador 8")]
        public string EmailVisa8 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Visador 8")]
        public string NombreChqVisa8 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Visador 9")]
        public string NombreVisa9 { get; set; }
        [Display(Name = "Nombre Visador 9")]
        public int? NombreIdVisa9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Visador 9")]
        public int RutVisa9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Visador 9")]
        public string DVVisa9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 9")]
        public int? IdUnidadVisa9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 9")]
        public string UnidadDescripcionVisa9 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 8")]
        //public int? IdCalidadVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 8")]
        //public string CalidadDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 8")]
        //public string IdGradoVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 8")]
        //public string GradoDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string GradoVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 8")]
        //public int? IdCargoVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 8")]
        //public string CargoDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 8")]
        //public int? IdEstamentoVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 8")]
        //public string EstamentoDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Visador 8")]
        //public int? IdProgramaVisa8 { get; set; }

        //[Display(Name = "Programa Visador 8")]
        //public string ProgramaDescripcionVisa8 { get; set; }

        //[Display(Name = "Conglomerado Visador 8")]
        //public int? IdConglomeradoVisa8 { get; set; }

        //[Display(Name = "Conglomerado Visador 8")]
        //public string ConglomeradoDescripcionVisa8 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Visador 9")]
        public string EmailVisa9 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Visador 9")]
        public string NombreChqVisa9 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Visador 10")]
        public string NombreVisa10 { get; set; }
        [Display(Name = "Nombre Visador 10")]
        public int? NombreIdVisa10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Visador 10")]
        public int RutVisa10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Visador 10")]
        public string DVVisa10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 10")]
        public int? IdUnidadVisa10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Visador 10")]
        public string UnidadDescripcionVisa10 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 8")]
        //public int? IdCalidadVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Calidad Juridica Visador 8")]
        //public string CalidadDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 8")]
        //public string IdGradoVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "GradoUES Visador 8")]
        //public string GradoDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        //public string GradoVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 8")]
        //public int? IdCargoVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Cargo Visador 8")]
        //public string CargoDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 8")]
        //public int? IdEstamentoVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Estamento Visador 8")]
        //public string EstamentoDescripcionVisa8 { get; set; }

        ////[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[Display(Name = "Programa Visador 8")]
        //public int? IdProgramaVisa8 { get; set; }

        //[Display(Name = "Programa Visador 8")]
        //public string ProgramaDescripcionVisa8 { get; set; }

        //[Display(Name = "Conglomerado Visador 8")]
        //public int? IdConglomeradoVisa8 { get; set; }

        //[Display(Name = "Conglomerado Visador 8")]
        //public string ConglomeradoDescripcionVisa8 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Visador 10")]
        public string EmailVisa10 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Visador 10")]
        public string NombreChqVisa10 { get; set; }

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
        public string ListaChk1 { get; set; } = "NULL";

        [Display(Name = "Auditoría Ministerial")]
        public string ListaChk2 { get; set; } = "NULL";

        [Display(Name = "Comisión Nacional de la Productividad")]
        public string ListaChk3 { get; set; } = "NULL";

        [Display(Name = "Departamento Administrativo")]
        public string ListaChk4 { get; set; } = "NULL";

        [Display(Name = "Departamento de Cooperativas")]
        public string ListaChk5 { get; set; } = "NULL";

        [Display(Name = "División de Empresas de Menor Tamaño")]
        public string ListaChk6 { get; set; } = "NULL";

        [Display(Name = "División Jurídica")]
        public string ListaChk7 { get; set; } = "NULL";

        [Display(Name = "División Política Comercial e Industrial")]
        public string ListaChk8 { get; set; } = "NULL";


        [Display(Name = "Escritorio de Empresas")]
        public string ListaChk9 { get; set; } = "NULL";


        [Display(Name = "Gabinete de Ministro")]
        public string ListaChk10 { get; set; } = "NULL";


        [Display(Name = "Gabinete de Subsecretario")]
        public string ListaChk11 { get; set; } = "NULL";

        [Display(Name = "Oficina de Gestión de Proyectos Sustentables")]
        public string ListaChk12 { get; set; } = "NULL";

        [Display(Name = "Oficina de Partes")]
        public string ListaChk13 { get; set; } = "NULL";

        [Display(Name = "Oficina de Productividad y Emprendimiento Nacional")]
        public string ListaChk14 { get; set; } = "NULL";

        [Display(Name = "Pymes Digitales")]
        public string ListaChk15 { get; set; } = "NULL";

        [Display(Name = "Secretaría Regional Ministerial de Antofagasta")]
        public string ListaChk16 { get; set; } = "NULL";

        [Display(Name = "Secretaría Regional Ministerial de Arica y Parinacota")]
        public string ListaChk17 { get; set; } = "NULL";

        [Display(Name = "Secretaría Regional Ministerial de Atacama")]
        public string ListaChk18 { get; set; } = "NULL";


        [Display(Name = "Secretaría Regional Ministerial de Aysén")]
        public string ListaChk19 { get; set; } = "NULL";


        [Display(Name = "Secretaría Regional Ministerial de Coquimbo")]
        public string ListaChk20 { get; set; } = "NULL";


        [Display(Name = "Secretaría Regional Ministerial de la Araucanía")]
        public string ListaChk21 { get; set; } = "NULL";

        [Display(Name = "Secretaría Regional Ministerial de los Lagos")]
        public string ListaChk22 { get; set; } = "NULL";

        [Display(Name = "Secretaría Regional Ministerial de los Ríos")]
        public string ListaChk23 { get; set; } = "NULL";

        [Display(Name = "Secretaría Regional Ministerial de Magallanes y la Antártica Chilena")]
        public string ListaChk24 { get; set; } = "NULL";

        [Display(Name = "Secretaría Regional Ministerial de Tarapacá")]
        public string ListaChk25 { get; set; } = "NULL";

        [Display(Name = "Secretaría Regional Ministerial de Valparaiso")]
        public string ListaChk26 { get; set; } = "NULL";

        [Display(Name = "Secretaría Regional Ministerial del Bío Bío")]
        public string ListaChk27 { get; set; } = "NULL";

        [Display(Name = "Secretaría Regional Ministerial del Libertador Bernardo O'Higgins")]
        public string ListaChk28 { get; set; } = "NULL";


        [Display(Name = "Secretaría Regional Ministerial del Maule")]
        public string ListaChk29 { get; set; } = "NULL";


        [Display(Name = "Secretaría Regional Ministerial del Ñuble")]
        public string ListaChk30 { get; set; } = "NULL";


        [Display(Name = "Secretaría Regional Ministerial Metropolitana")]
        public string ListaChk31 { get; set; } = "NULL";

        [Display(Name = "Sistema Integral de Atención a la Ciudadanía")]
        public string ListaChk32 { get; set; } = "NULL";

        [Display(Name = "Sistema Unificado de Permisos")]
        public string ListaChk33 { get; set; } = "NULL";

        [Display(Name = "Tribunal Arbitral de Propiedad Industrial")]
        public string ListaChk34 { get; set; } = "NULL";

        [Display(Name = "Unidad Comsionados en Turismo")]
        public string ListaChk35 { get; set; } = "NULL";

        [Display(Name = "Unidad de Abastecimientos")]
        public string ListaChk36 { get; set; } = "NULL";

        [Display(Name = "Unidad de Comunicaciones")]
        public string ListaChk37 { get; set; } = "NULL";

        [Display(Name = "Unidad de Control de Gestión")]
        public string ListaChk38 { get; set; } = "NULL";


        [Display(Name = "Unidad de Control y Rendiciones")]
        public string ListaChk39 { get; set; } = "NULL";


        [Display(Name = "Unidad de Finanzas")]
        public string ListaChk40 { get; set; } = "NULL";


        [Display(Name = "Unidad de Presupuestos")]
        public string ListaChk41 { get; set; } = "NULL";

        [Display(Name = "Unidad de Procesos")]
        public string ListaChk42 { get; set; } = "NULL";

        [Display(Name = "Unidad Gestión y Desarrollo de Personas")]
        public string ListaChk43 { get; set; } = "NULL";

        [Display(Name = "Unidad Registro de Empresas y Sociedades")]
        public string ListaChk44 { get; set; } = "NULL";

        [Display(Name = "Unidad Servicios Generales")]
        public string ListaChk45 { get; set; } = "NULL";

        [Display(Name = "Unidad Tecnologías de Información y Comunicaciones")]
        public string ListaChk46 { get; set; } = "NULL";

        [Display(Name = "Subsecretaría de Turismo")]
        public string ListaChk47 { get; set; } = "NULL";
    }
}
