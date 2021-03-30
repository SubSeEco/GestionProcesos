using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using App.Model.Core;
using App.Model.DTO;
//using App.Model.Shared;
using App.Core.Interfaces;
using App.Model.HorasExtras;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class GeneraResolucionController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;
        protected readonly IHSM _hsm;
        protected readonly IEmail _email;

        private static List<App.Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }

        public GeneraResolucionController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;


            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
        }

        public ActionResult GeneraResolucion(int WorkFlowId)
        {
            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new GeneracionResolucion()
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
            };
            

            List<SelectListItem> Meses = new List<SelectListItem>
            {
                new SelectListItem {Text = "Enero", Value = "Enero"},
                new SelectListItem {Text = "Febrero", Value = "Febrero"},
                new SelectListItem {Text = "Marzo", Value = "Marzo"},
                new SelectListItem {Text = "Abril", Value = "Abril"},
                new SelectListItem {Text = "Mayo", Value = "Mayo"},
                new SelectListItem {Text = "Junio", Value = "Junio"},
                new SelectListItem {Text = "Julio", Value = "Julio"},
                new SelectListItem {Text = "Agosto", Value = "Agosto"},
                new SelectListItem {Text = "Septiembre", Value = "Septiembre"},
                new SelectListItem {Text = "Octubre", Value = "Octubre"},
                new SelectListItem {Text = "Noviembre", Value = "Noviembre"},
                new SelectListItem {Text = "Diciembre", Value = "Diciembre"},
            };

            List<SelectListItem> Anno = new List<SelectListItem>();
            for (var i = DateTime.Now.Year - 5; i < DateTime.Now.Year + 5; i++)
                Anno.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            ViewBag.Mes = new SelectList(Meses, "Value", "Text");
            ViewBag.Annio = new SelectList(Anno, "Value", "Text");

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult /*FileResult*/ GeneraResolucion(string mes, string annio, GeneracionResolucion model)
        {
            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            int tipoDoc = 0;
            int idDoctoHoras = 0;
            string Name = string.Empty;
            if (!_repository.GetExists<GeneracionResolucion>(q => q.Mes == mes && q.Annio == annio))
            {
                var hrs = _repository.GetAll<HorasExtras>().Where(c => c.Mes == mes && c.Annio == annio);

                /*Se genera resolucuion de trabajos extraordinarios*/
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("ResolucionServicio", new { mes = hrs.FirstOrDefault().Mes, annio = hrs.FirstOrDefault().Annio }) { FileName = "ResolucionProgramacionServicio" + ".pdf", FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                data = _file.BynaryToText(pdf);
                tipoDoc = 12;
                Name = "Resolución Programación Trabajos Extraordinarios mes" + " " + hrs.FirstOrDefault().Mes.ToString() + ".pdf";

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                //var docto = _repository.GetAll<Documento>().Where(d => d.ProcesoId == hrs.FirstOrDefault().ProcesoId);
                var docto = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
                if (docto != null)
                {
                    foreach (var res in docto)
                    {
                        if (res.TipoDocumentoId == 12)
                            idDoctoHoras = res.DocumentoId;
                    }
                }
                var docOld = new Documento();

                if (idDoctoHoras == 0)
                {
                    var email = UserExtended.Email(User);
                    var doc = new Documento();
                    doc.Fecha = DateTime.Now;
                    doc.Email = email;
                    doc.FileName = Name;
                    doc.File = pdf;
                    doc.ProcesoId = model.ProcesoId.Value;
                    doc.WorkflowId = model.WorkflowId.Value;
                    doc.Signed = false;
                    doc.Texto = data.Text;
                    doc.Metadata = data.Metadata;
                    doc.Type = data.Type;
                    doc.TipoPrivacidadId = 1;
                    doc.TipoDocumentoId = tipoDoc;

                    _repository.Create(doc);
                    _repository.Save();

                    /*Se genera registro de la generacin de la resolucion*/
                    var genera = new GeneracionResolucion();
                    genera.FechaCreacion = DateTime.Now;
                    genera.Annio = annio;
                    genera.Mes = mes;
                    genera.ProcesoId = hrs.FirstOrDefault().ProcesoId.Value;
                    genera.WorkflowId = hrs.FirstOrDefault().WorkflowId.Value;
                    _repository.Create(genera);
                    _repository.Save();
                }
                else
                {
                    docOld = _repository.GetById<Documento>(idDoctoHoras);
                    docOld.File = pdf;
                    docOld.Signed = false;
                    docOld.Texto = data.Text;
                    docOld.Metadata = data.Metadata;
                    docOld.Type = data.Type;
                    _repository.Update(docOld);
                    _repository.Save();
                }
            }
            else
                TempData["Warning"] = "Ya se ha generado una resolucion para el periodo señalado.";



            List<SelectListItem> Meses = new List<SelectListItem>
            {
                new SelectListItem {Text = "Enero", Value = "Enero"},
                new SelectListItem {Text = "Febrero", Value = "Febrero"},
                new SelectListItem {Text = "Marzo", Value = "Marzo"},
                new SelectListItem {Text = "Abril", Value = "Abril"},
                new SelectListItem {Text = "Mayo", Value = "Mayo"},
                new SelectListItem {Text = "Junio", Value = "Junio"},
                new SelectListItem {Text = "Julio", Value = "Julio"},
                new SelectListItem {Text = "Agosto", Value = "Agosto"},
                new SelectListItem {Text = "Septiembre", Value = "Septiembre"},
                new SelectListItem {Text = "Octubre", Value = "Octubre"},
                new SelectListItem {Text = "Noviembre", Value = "Noviembre"},
                new SelectListItem {Text = "Diciembre", Value = "Diciembre"},
            };

            List<SelectListItem> Anno = new List<SelectListItem>();
            for (var i = DateTime.Now.Year - 5; i < DateTime.Now.Year + 5; i++)
                Anno.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            ViewBag.Mes = new SelectList(Meses, "Value", "Text");
            ViewBag.Annio = new SelectList(Anno, "Value", "Text");

            return View(model);
            //return Redirect(Request.UrlReferrer.PathAndQuery);
            //return File(docOld.File, "application/pdf");
        }

        [AllowAnonymous]
        public ActionResult ResolucionServicio(string mes, string annio)
        {
            var model = _repository.GetAll<HorasExtras>().Where(c => c.Mes == mes && c.Annio == annio);
            return View(model);
        }
    }
}