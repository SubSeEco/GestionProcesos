using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Model.Core
{
    [Table("CoreProceso")]
    public class Proceso 
    {
        public Proceso()
        {
            Workflows = new HashSet<Workflow>();
            Documentos = new HashSet<Documento>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        [Display(Name = "Id")]
        public int ProcesoId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Definición proceso")]
        public int DefinicionProcesoId { get; set; }
        public virtual DefinicionProceso DefinicionProceso { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha creación")]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha vencimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaVencimiento { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha término")]
        [DataType(DataType.Date)]
        public DateTime? FechaTermino { get; set; }

        [Display(Name = "Observaciones")]
        [DataType(DataType.MultilineText)]
        public string Observacion { get; set; }


        [Display(Name = "Autor")]
        public string Email { get; set; }

        //[Display(Name = "RUT")]
        //public int? RUT { get; set; }

        [NotMapped]
        [Display(Name = "Tiempo ejecución")]
        public TimeSpan Span
        {
            get
            {
                return ((FechaTermino.HasValue ? FechaTermino.Value : DateTime.Now) - FechaCreacion);
            }
        }

        [NotMapped]
        [Display(Name = "Numero Solicitud")]
        public string NroSolicitud { get; set; }

        public virtual ICollection<Workflow> Workflows { get; set; }
        public virtual ICollection<Documento> Documentos { get; set; }


        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Estado del proceso")]
        public int EstadoProcesoId { get; set; }
        public virtual EstadoProceso EstadoProceso { get; set; }




        //deprecado

        [Display(Name = "Estado")]
        public bool Terminada { get; set; } = false;

        [Display(Name = "Anulada?")]
        public bool Anulada { get; set; } = false;


    }
}