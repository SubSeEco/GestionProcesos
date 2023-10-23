using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Core
{
    [Table("Consulta")]
    public class Consulta : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ConsultaId")]
        public int ConsultaId { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-dd-MM}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Rut")]
        public int Rut { get; set; }

        [Display(Name = "DV")]
        public string DV { get; set; }

        [Display(Name = "Unidad")]
        public string Unidad { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Desea que sus datos sean revelados o compartidos")]
        public bool? CampoPrivacidad { get; set; }

        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(Name = "Seleccione Tipo de Respuesta")]
        public string TipoRespuesta { get; set; }

        //Direccion
        [Display(Name = "Calle, Avenida o Pasaje")]
        public string Direccion { get; set; }

        //Numero
        [Display(Name = "Numero")]
        public string Numero { get; set; }

        //Depto/Oficina
        [Display(Name = "Depto/Oficina")]
        public string DeptoOficina { get; set; }

        //Comuna
        [Display(Name = "Comuna")]
        public string Comuna { get; set; }

        //Codigo Postal
        [Display(Name = "Codigo Postal")]
        public string CodigoPostal { get; set; }

        [Display(Name = "Ingrese su Consulta")]
        public string ConsultaIntegridad { get; set; }


        public bool? CorreoElectronico { get; set; }
        public bool? CorreoPostal { get; set; }
    }
}
