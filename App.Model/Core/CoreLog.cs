using System;
using System.ComponentModel.DataAnnotations;

namespace App.Model.Core
{
    //[Table("CoreLog")]
    public class CoreLog 
    {
        [Display(Name = "Id")]
        public Guid LogId { get; set; }

        [Display(Name = "UserName")]
        public string LogUserName { get; set; }

        [Display(Name = "IP")]
        public string LogIpAddress { get; set; }

        [Display(Name = "Módulo")]
        public string LogAreaAccessed { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha/hora local")]
        public DateTime LogTimeLocal { get; set; }

        [Display(Name = "Fecha/hora UTC")]
        public DateTime LogTimeUtc { get; set; }

        [Display(Name = "Detalles")]
        public string LogDetails { get; set; }

        [Display(Name = "Agente")]
        public string LogAgent { get; set; }

        [Display(Name = "Método HTTP")]
        public string LogHttpMethod { get; set; }

        [Display(Name = "Header")]
        public string LogHeader { get; set; }

        [Display(Name = "Contenido")]
        public string LogContent { get; set; }
    }
}
