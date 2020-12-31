using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Display(Name = "Año")]
        public string Annio { get; set; }

        [Display(Name = "Mes")]
        public string Mes { get; set; }

        /*Datos funcionario*/
        [Display(Name = "Nombre Remitente")]
        public string Nombre { get; set; }
        //[Required(ErrorMessage = "Es necesario especificar este dato")]

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

        [NotMapped]
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo Remitente")]
        public int? IdCargo { get; set; }

        [NotMapped]
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Cargo Remitente")]
        public string CargoDescripcion { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Remitente")]
        public string EmailRem { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Remitente")]
        public string NombreChqRem { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Funcionario 1")]
        public string NombreFunc1 { get; set; }
        [Display(Name = "Nombre Funcionario 1 ")]
        public int? NombreIdFunc1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Funcionario 1")]
        public int RutFunc1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Funcionario 1")]
        public string DVFunc1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 1")]
        public int? IdUnidadFunc1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 1")]
        public string UnidadDescripcionFunc1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 1")]
        public int? IdCalidadFunc1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 1")]
        public string CalidadDescripcionFunc1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 1")]
        public string IdGradoFunc1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 1")]
        public string GradoDescripcionFunc1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 1")]
        public int? IdCargoFunc1 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 1")]
        public string CargoDescripcionFunc1 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Funcionario 1")]
        public string EmailFunc1 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Funcionario 1")]
        public string NombreChqFunc1 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Funcionario 2")]
        public string NombreFunc2 { get; set; }
        [Display(Name = "Nombre Funcionario 2 ")]
        public int? NombreIdFunc2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Funcionario 2")]
        public int RutFunc2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Funcionario 2")]
        public string DVFunc2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 2")]
        public int? IdUnidadFunc2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 1")]
        public string UnidadDescripcionFunc2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 2")]
        public int? IdCalidadFunc2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 2")]
        public string CalidadDescripcionFunc2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 2")]
        public string IdGradoFunc2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 2")]
        public string GradoDescripcionFunc2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 2")]
        public int? IdCargoFunc2 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 2")]
        public string CargoDescripcionFunc2 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Funcionario 2")]
        public string EmailFunc2 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Funcionario 2")]
        public string NombreChqFunc2 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Funcionario 3")]
        public string NombreFunc3 { get; set; }
        [Display(Name = "Nombre Funcionario 3 ")]
        public int? NombreIdFunc3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Funcionario 3")]
        public int RutFunc3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Funcionario 3")]
        public string DVFunc3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 3")]
        public int? IdUnidadFunc3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 3")]
        public string UnidadDescripcionFunc3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 3")]
        public int? IdCalidadFunc3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 3")]
        public string CalidadDescripcionFunc3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 3")]
        public string IdGradoFunc3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 3")]
        public string GradoDescripcionFunc3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 2")]
        public int? IdCargoFunc3 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 2")]
        public string CargoDescripcionFunc3 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Funcionario 2")]
        public string EmailFunc3 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Funcionario 3")]
        public string NombreChqFunc3 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Funcionario 4")]
        public string NombreFunc4 { get; set; }
        [Display(Name = "Nombre Funcionario 4 ")]
        public int? NombreIdFunc4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Funcionario 4")]
        public int RutFunc4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Funcionario 4")]
        public string DVFunc4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 4")]
        public int? IdUnidadFunc4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 4")]
        public string UnidadDescripcionFunc4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 4")]
        public int? IdCalidadFunc4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 4")]
        public string CalidadDescripcionFunc4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 4")]
        public string IdGradoFunc4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 4")]
        public string GradoDescripcionFunc4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 4")]
        public int? IdCargoFunc4 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 4")]
        public string CargoDescripcionFunc4 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Funcionario 4")]
        public string EmailFunc4 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Funcionario 4")]
        public string NombreChqFunc4 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Funcionario 5")]
        public string NombreFunc5 { get; set; }
        [Display(Name = "Nombre Funcionario 5 ")]
        public int? NombreIdFunc5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Funcionario 5")]
        public int RutFunc5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Funcionario 5")]
        public string DVFunc5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 5")]
        public int? IdUnidadFunc5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 5")]
        public string UnidadDescripcionFunc5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 5")]
        public int? IdCalidadFunc5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 5")]
        public string CalidadDescripcionFunc5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 5")]
        public string IdGradoFunc5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 5")]
        public string GradoDescripcionFunc5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 5")]
        public int? IdCargoFunc5 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 5")]
        public string CargoDescripcionFunc5 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Funcionario 5")]
        public string EmailFunc5 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Funcionario 5")]
        public string NombreChqFunc5 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Funcionario 6")]
        public string NombreFunc6 { get; set; }
        [Display(Name = "Nombre Funcionario 6 ")]
        public int? NombreIdFunc6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Funcionario 6")]
        public int RutFunc6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Funcionario 6")]
        public string DVFunc6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 6")]
        public int? IdUnidadFunc6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 6")]
        public string UnidadDescripcionFunc6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 6")]
        public int? IdCalidadFunc6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 6")]
        public string CalidadDescripcionFunc6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 6")]
        public string IdGradoFunc6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 6")]
        public string GradoDescripcionFunc6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 6")]
        public int? IdCargoFunc6 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 6")]
        public string CargoDescripcionFunc6 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Funcionario 6")]
        public string EmailFunc6 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Funcionario 6")]
        public string NombreChqFunc6 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Funcionario 7")]
        public string NombreFunc7 { get; set; }
        [Display(Name = "Nombre Funcionario 7 ")]
        public int? NombreIdFunc7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Funcionario 7")]
        public int RutFunc7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Funcionario 7")]
        public string DVFunc7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 7")]
        public int? IdUnidadFunc7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 7")]
        public string UnidadDescripcionFunc7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 7")]
        public int? IdCalidadFunc7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 7")]
        public string CalidadDescripcionFunc7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 7")]
        public string IdGradoFunc7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 7")]
        public string GradoDescripcionFunc7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 7")]
        public int? IdCargoFunc7 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 7")]
        public string CargoDescripcionFunc7 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Funcionario 7")]
        public string EmailFunc7 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Funcionario 7")]
        public string NombreChqFunc7 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Funcionario 8")]
        public string NombreFunc8 { get; set; }
        [Display(Name = "Nombre Funcionario 8 ")]
        public int? NombreIdFunc8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Funcionario 8")]
        public int RutFunc8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Funcionario 8")]
        public string DVFunc8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 8")]
        public int? IdUnidadFunc8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 8")]
        public string UnidadDescripcionFunc8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 8")]
        public int? IdCalidadFunc8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 8")]
        public string CalidadDescripcionFunc8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 8")]
        public string IdGradoFunc8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 8")]
        public string GradoDescripcionFunc8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 8")]
        public int? IdCargoFunc8 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 8")]
        public string CargoDescripcionFunc8 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Funcionario 8")]
        public string EmailFunc8 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Funcionario 8")]
        public string NombreChqFunc8 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Funcionario 9")]
        public string NombreFunc9 { get; set; }
        [Display(Name = "Nombre Funcionario 9 ")]
        public int? NombreIdFunc9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Funcionario 9")]
        public int RutFunc9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Funcionario 9")]
        public string DVFunc9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 9")]
        public int? IdUnidadFunc9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 9")]
        public string UnidadDescripcionFunc9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 9")]
        public int? IdCalidadFunc9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 9")]
        public string CalidadDescripcionFunc9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 9")]
        public string IdGradoFunc9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 9")]
        public string GradoDescripcionFunc9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 9")]
        public int? IdCargoFunc9 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 9")]
        public string CargoDescripcionFunc9 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Funcionario 9")]
        public string EmailFunc9 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Funcionario 9")]
        public string NombreChqFunc9 { get; set; }

        /*Datos funcionario*/
        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre Funcionario 10")]
        public string NombreFunc10 { get; set; }
        [Display(Name = "Nombre Funcionario 10 ")]
        public int? NombreIdFunc10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        //[RegularExpression(@"\d{8}", ErrorMessage = "Excede el largo maximo (8)")]
        [Display(Name = "Rut Funcionario 10")]
        public int RutFunc10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [StringLength(1, ErrorMessage = "Excede el largo maximo (1)")]
        [Display(Name = "DV Funcionario 10")]
        public string DVFunc10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 10")]
        public int? IdUnidadFunc10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Unidad Funcionario 10")]
        public string UnidadDescripcionFunc10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 10")]
        public int? IdCalidadFunc10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Calidad Juridica Funcionario 10")]
        public string CalidadDescripcionFunc10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 10")]
        public string IdGradoFunc10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "GradoUES Funcionario 10")]
        public string GradoDescripcionFunc10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 10")]
        public int? IdCargoFunc10 { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [NotMapped]
        [Display(Name = "Cargo Funcionario 10")]
        public string CargoDescripcionFunc10 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Email Funcionario 10")]
        public string EmailFunc10 { get; set; }

        //Línea Exclusiva Visadores
        [Display(Name = "Nombre Funcionario 10")]
        public string NombreChqFunc10 { get; set; }

        [Display(Name = "Firma y Timbre")]
        public string FirmaTimbre { get; set; }

        [Display(Name = "N° de horas")]
        public int? HorasFunc1 { get; set; } = 0;

        [Display(Name = "Número de horas")]
        public int? HorasFunc2 { get; set; } = 0;

        [Display(Name = "Número de horas")]
        public int? HorasFunc3 { get; set; } = 0;

        [Display(Name = "Número de horas")]
        public int? HorasFunc4 { get; set; } = 0;

        [Display(Name = "Número de horas")]
        public int? HorasFunc5 { get; set; } = 0;

        [Display(Name = "Número de horas")]
        public int? HorasFunc6 { get; set; } = 0;

        [Display(Name = "Número de horas")]
        public int? HorasFunc7 { get; set; } = 0;

        [Display(Name = "Número de horas")]
        public int? HorasFunc8 { get; set; } = 0;

        [Display(Name = "Número de horas")]
        public int? HorasFunc9 { get; set; } = 0;

        [Display(Name = "Número de horas")]
        public int? HorasFunc10 { get; set; } = 0;

        [Display(Name = "Tipo de Hora")]
        public string TipoHoraFunc1 { get; set; }

        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string TareaFunc1 { get; set; }

        [Display(Name = "Tipo de Hora")]
        public string TipoHoraFunc2 { get; set; }

        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string TareaFunc2 { get; set; }

        [Display(Name = "Tipo de Hora")]
        public string TipoHoraFunc3 { get; set; }

        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string TareaFunc3 { get; set; }

        [Display(Name = "Tipo de Hora")]
        public string TipoHoraFunc4 { get; set; }

        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string TareaFunc4 { get; set; }

        [Display(Name = "Tipo de Hora")]
        public string TipoHoraFunc5 { get; set; }

        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string TareaFunc5 { get; set; }

        [Display(Name = "Tipo de Hora")]
        public string TipoHoraFunc6 { get; set; }

        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string TareaFunc6 { get; set; }

        [Display(Name = "Tipo de Hora")]
        public string TipoHoraFunc7 { get; set; }

        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string TareaFunc7 { get; set; }

        [Display(Name = "Tipo de Hora")]
        public string TipoHoraFunc8 { get; set; }

        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string TareaFunc8 { get; set; }

        [Display(Name = "Tipo de Hora")]
        public string TipoHoraFunc9 { get; set; }

        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string TareaFunc9 { get; set; }

        [Display(Name = "Tipo de Hora")]
        public string TipoHoraFunc10 { get; set; }

        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string TareaFunc10 { get; set; }

        [Display(Name = "Firmante")]
        public string Firmante { get; set; }

        [Display(Name = "Documento a firmar")]
        public byte[] DocumentoSinFirma { get; set; }

        [Display(Name = "Documento a firmar")]
        public string DocumentoSinFirmaFilename { get; set; }


        [Display(Name = "Documento firmado")]
        public byte[] DocumentoConFirma { get; set; }

        [Display(Name = "Documento firmado")]
        public string DocumentoConFirmaFilename { get; set; }

        [Display(Name = "Folio")]
        public string Folio { get; set; }

        //[Required]
        [Display(Name = "Código tipo documento")]
        public string TipoDocumentoCodigo { get; set; }

        [Display(Name = "Autor")]
        public string Autor { get; set; }

        //[Display(Name = "Firmado?")]
        //public bool Firmado { get; set; }

        [Display(Name = "Fecha firma")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? FechaFirma { get; set; }

        public int? DocumentoId { get; set; }

        [Display(Name = "Firma?")]
        public string Firma { get; set; } 
    }
}
