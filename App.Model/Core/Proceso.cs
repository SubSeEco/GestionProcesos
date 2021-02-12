using App.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

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
        [Display(Name = "Tipo de proceso")]
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

        [Display(Name = "Email autor")]
        public string Email { get; set; }

        [Display(Name = "Nombre autor")]
        public string NombreFuncionario { get; set; }

        [Display(Name = "Unidad")]
        public int? Pl_UndCod { get; set; }

        [Display(Name = "Unidad")]
        public string Pl_UndDes { get; set; }


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

        [Display(Name = "Justificación anulación")]
        [DataType(DataType.MultilineText)]
        public string JustificacionAnulacion { get; set; }


        //deprecado
        //[Display(Name = "Estado")]
        //public bool Terminada { get; set; } = false;

        [Display(Name = "Anulada?")]
        public bool Anulada { get; set; } = false;

        [Display(Name = "Reservado?")]
        public bool Reservado { get; set; } = false;

        [NotMapped]
        [Display(Name = "Es autor?")]
        public bool EsAutor { get; set; } = false;


        [Display(Name = "Tags")]
        public string Tags { get; set; }

        public string GetTags()
        {
            StringBuilder tag = new StringBuilder();
            tag.Append(ProcesoId.ToString().TrimOrEmpty() + " ");
            tag.Append(Observacion.TrimOrEmpty() + " ");
            tag.Append(Email.TrimOrEmpty() + " ");
            tag.Append(NombreFuncionario.TrimOrEmpty() + " ");
            tag.Append(string.Join(" ", this.Documentos.Select(q => q.Texto)) + " ");
            tag.Append(string.Join(" ", this.Documentos.Select(q => q.Folio)) + " ");
            tag.Append(string.Join(" ", this.Documentos.Select(q => q.FileName)) + " ");
            tag.Append(string.Join(" ", this.Workflows.Select(q => q.Observacion)));
            tag.Append(string.Join(" ", this.Workflows.Select(q => q.Mensaje)));

            return tag.ToString();
        }

        public void CalcularFechaVencimiento(List<DateTime> festivos)
        {
            var fin = this.FechaCreacion;
            int days = 0;

            if (this.DefinicionProceso != null)
            {
                while (days <= this.DefinicionProceso.DuracionHoras)
                {
                    fin.AddDays(1);
                    if (fin.DayOfWeek != DayOfWeek.Saturday && fin.DayOfWeek != DayOfWeek.Sunday && !festivos.Any(q => q.Date == fin.Date))
                        days++;
                }

                this.FechaVencimiento = fin;
            }
        }
    }
}