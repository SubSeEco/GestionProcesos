using App.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model.DTO
{
    public class DTOStateProces
    {
        public DTOStateProces()
        { }
        public int Tarea { get; set; }
        public int Total { get; set; }
        public float Porcentaje { get; set; }
        public int Secuencia { get; set; }
        public List<Workflow> CantTareasRealizadas { get; set; }
        public List<DefinicionWorkflow> DefWorkflow { get; set; }
    }
}
