using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Core
{
    [Table("Denuncia")]
    public class Denuncia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "DenunciaIntegridadId")]
        public int DenunciaIntegridadId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha del Hecho")]
        [DataType(DataType.Date)]
        public DateTime? FechaHecho { get; set; } = DateTime.Now;

        [Display(Name = "Proceso")]
        public int? ProcesoId { get; set; }
        public Proceso Proceso { get; set; }

        [Display(Name = "Workflow")]
        public int? WorkflowId { get; set; }
        public Workflow Workflow { get; set; }

        [Display(Name = "Amerita Investigación?")]
        public bool EsInvestigador { get; set; }

        [Display(Name = "Maltrato Laboral")]
        public bool MaltratoLaboral { get; set; }

        [Display(Name = "Acoso Laboral")]
        public bool AcosoLaboral { get; set; }

        [Display(Name = "Acoso Sexual")]
        public bool AcosoSexual { get; set; }

        [Display(Name = "Falta a la Probidad en la función publica y/o conflictos de interes")]
        public bool FaltaProbidad { get; set; }

        [Display(Name = "Delitos Financieros, lavado de activos y/o financiamiento al terrorismo")]
        public bool DelitoFinanciero { get; set; }

        #region Victima
        [Display(Name = "Nombre Victima")]
        public string NombreVictima { get; set; }

        [Display(Name = "Rut Victima")]
        public int RutVictima { get; set; }

        [Display(Name = "DV Victima")]
        public int? DvVictima { get; set; }

        [Display(Name = "Correo Victima")]
        public string CorreoVictima { get; set; }

        [Display(Name = "Id Unidad Victima")]
        public int? IdUnidadVictima { get; set; }

        [Display(Name = "Unidad Victima")]
        public string DescripcionUnidadVictima { get; set; }
        #endregion

        #region Denunciado
        [Display(Name = "Nombre Denunciado")]
        public string NombreDenunciado { get; set; }

        [Display(Name = "Id Unidad Denunciado")]
        public int? IdUnidadDenunciado { get; set; }

        [Display(Name = "Unidad Denunciado")]
        public string DescripcionUnidadDenunciado { get; set; }
        #endregion

        #region Denuncia/Hechos
        [Display(Name = "Nivel Jerarquico")]
        public string NivelJerarquico { get; set; }

        [Display(Name = "Es Jefatura?")]
        public bool EsJefatura { get; set; }

        [Display(Name = "Trabaja Directo?")]
        public bool TrabajaDirecto { get; set; }

        [Display(Name = "Es de Conocimiento?")]
        public bool EsDeConocimiento { get; set; }

        [Display(Name = "Hechos")]
        public string Hechos { get; set; }

        [Display(Name = "Observacion")]
        public string Observacion { get; set; }
        #endregion
    }
}
