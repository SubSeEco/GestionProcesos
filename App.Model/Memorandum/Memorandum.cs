using App.Model.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ExpressiveAnnotations.Attributes;

namespace App.Model.Memorandum
{
    [Table("Memorandum")]
    public class Memorandum : Core.BaseEntity
    {
        public Memorandum()
        {
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Numero Memorandum")]
        public int MemorandumId { get; set; }

        //[Required(ErrorMessage = "Se debe indicar la fecha")]
        [Display(Name = "Fecha Solicitud")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaSolicitud { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "Nombre Id")]
        public int? NombreId { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut")]
        public int Rut { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV")]
        public string DV { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad")]
        public int? IdUnidad { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad")]
        public string UnidadDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Juridica")]
        public int? IdCalidad { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Juridica")]
        public string CalidadDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "GradoUES")]
        public string IdGrado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "GradoUES")]
        public string GradoDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        public string Grado { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo")]
        public int? IdCargo { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo")]
        public string CargoDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estamento")]
        public int? IdEstamento { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estamento")]
        public string EstamentoDescripcion { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Programa")]
        public int? IdPrograma { get; set; }

        [Display(Name = "Programa")]
        public string ProgramaDescripcion { get; set; }

        [Display(Name = "Conglomerado")]
        public int? IdConglomerado { get; set; }

        [Display(Name = "Conglomerado")]
        public string ConglomeradoDescripcion { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Funcionario")]
        public string EmailRem { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Funcionario")]
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
        [Display(Name = "Nombre Dest")]
        public string NombreDest { get; set; }
        [Display(Name = "Nombre Id Dest")]
        public int? NombreIdDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut")]
        public int RutDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV")]
        public string DVDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad")]
        public int? IdUnidadDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad")]
        public string UnidadDescripcionDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Juridica")]
        public int? IdCalidadDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Juridica")]
        public string CalidadDescripcionDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "GradoUES")]
        public string IdGradoDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "GradoUES")]
        public string GradoDescripcionDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        public string GradoDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo")]
        public int? IdCargoDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo")]
        public string CargoDescripcionDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estamento")]
        public int? IdEstamentoDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estamento")]
        public string EstamentoDescripcionDest { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Programa")]
        public int? IdProgramaDest { get; set; }

        [Display(Name = "Programa")]
        public string ProgramaDescripcionDest { get; set; }

        [Display(Name = "Conglomerado")]
        public int? IdConglomeradoDest { get; set; }

        [Display(Name = "Conglomerado")]
        public string ConglomeradoDescripcionDest { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Funcionario")]
        public string EmailDest { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Funcionario")]
        public string NombreChqDest { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Dest")]
        public string NombreSecre { get; set; }
        [Display(Name = "Nombre Id Dest")]
        public int? NombreIdSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut")]
        public int RutSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV")]
        public string DVSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad")]
        public int? IdUnidadSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad")]
        public string UnidadDescripcionSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Juridica")]
        public int? IdCalidadSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Juridica")]
        public string CalidadDescripcionSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "GradoUES")]
        public string IdGradoSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "GradoUES")]
        public string GradoDescripcionSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        public string GradoSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo")]
        public int? IdCargoSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo")]
        public string CargoDescripcionSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estamento")]
        public int? IdEstamentoSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estamento")]
        public string EstamentoDescripcionSecre { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Programa")]
        public int? IdProgramaSecre { get; set; }

        [Display(Name = "Programa")]
        public string ProgramaDescripcionSecre { get; set; }

        [Display(Name = "Conglomerado")]
        public int? IdConglomeradoSecre { get; set; }

        [Display(Name = "Conglomerado")]
        public string ConglomeradoDescripcionSecre { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Funcionario")]
        public string EmailSecre { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Funcionario")]
        public string NombreChqSecre { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Dest")]
        public string NombreVisa1 { get; set; }
        [Display(Name = "Nombre Id Dest")]
        public int? NombreIdVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut")]
        public int RutVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV")]
        public string DVVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad")]
        public int? IdUnidadVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad")]
        public string UnidadDescripcionVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Juridica")]
        public int? IdCalidadVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Juridica")]
        public string CalidadDescripcionVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "GradoUES")]
        public string IdGradoVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "GradoUES")]
        public string GradoDescripcionVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        public string GradoVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo")]
        public int? IdCargoVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo")]
        public string CargoDescripcionVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estamento")]
        public int? IdEstamentoVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estamento")]
        public string EstamentoDescripcionVisa1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Programa")]
        public int? IdProgramaVisa1 { get; set; }

        [Display(Name = "Programa")]
        public string ProgramaDescripcionVisa1 { get; set; }

        [Display(Name = "Conglomerado")]
        public int? IdConglomeradoVisa1 { get; set; }

        [Display(Name = "Conglomerado")]
        public string ConglomeradoDescripcionVisa1 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Funcionario")]
        public string EmailVisa1 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Funcionario")]
        public string NombreChqVisa1 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Dest")]
        public string NombreVisa2 { get; set; }
        [Display(Name = "Nombre Id Dest")]
        public int? NombreIdVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut")]
        public int RutVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV")]
        public string DVVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad")]
        public int? IdUnidadVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad")]
        public string UnidadDescripcionVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Juridica")]
        public int? IdCalidadVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Calidad Juridica")]
        public string CalidadDescripcionVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "GradoUES")]
        public string IdGradoVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "GradoUES")]
        public string GradoDescripcionVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Grado de contratación (en caso de HSA se debe homologr a un grado de la EUS)")]
        public string GradoVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo")]
        public int? IdCargoVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo")]
        public string CargoDescripcionVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estamento")]
        public int? IdEstamentoVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estamento")]
        public string EstamentoDescripcionVisa2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Programa")]
        public int? IdProgramaVisa2 { get; set; }

        [Display(Name = "Programa")]
        public string ProgramaDescripcionVisa2 { get; set; }

        [Display(Name = "Conglomerado")]
        public int? IdConglomeradoVisa2 { get; set; }

        [Display(Name = "Conglomerado")]
        public string ConglomeradoDescripcionVisa2 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Funcionario")]
        public string EmailVisa2 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Funcionario")]
        public string NombreChqVisa2 { get; set; }

        [Display(Name = "Número de Documento")]
        public string NumDocumento { get; set; }

        [Display(Name = "Número Memo Referencia")]
        public string NumMemoRef { get; set; }

        [Display(Name = "Lista Distribución")]
        public string Distribucion { get; set; }

        [Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(Name = "Antecedentes")]
        public string Antecedentes { get; set; }

        [Display(Name = "Número de Documento")]
        public string Materia { get; set; }
    }
}
