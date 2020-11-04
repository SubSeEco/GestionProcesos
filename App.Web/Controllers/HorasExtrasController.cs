using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using App.Model.ProgramacionHorasExtraordinarias;
using App.Model.Core;
using App.Model.DTO;
//using App.Model.Shared;
using App.Core.Interfaces;
using App.Model.Shared;
using App.Util;
using Newtonsoft.Json;
using App.Core.UseCases;
using App.Model.Comisiones;
using System.ComponentModel.DataAnnotations;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using Rotativa;
using App.Model.HorasExtras;
using System.Globalization;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class HorasExtrasController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;
        protected readonly IHSM _hsm;
        protected readonly IEmail _email;
        private static List<App.Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }
        public static List<Colaborador> ListDestino = new List<Colaborador>();

        public HorasExtrasController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;

            //if (tipoDocumentoList == null)
            //    tipoDocumentoList = _folio.GetTipoDocumento();

            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
        }
        public ActionResult Index()
        {
            var model = _repository.GetAll<HorasExtras>();
            return View(model);
        }

        public ActionResult View(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            var model = _repository.GetById<HorasExtras>(id);

            return View(model);
        }

        public ActionResult Details(int id)
        {
            //var persona = _sigper.GetUserByEmail(User.Email());
            //var usuarios = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            var model = _repository.GetById<HorasExtras>(id);

            return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<HorasExtras>(id);
            return View(model);
        }

        public ActionResult Create(int WorkFlowId)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new HorasExtras()
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
            };

            if (persona.Funcionario == null)
                ModelState.AddModelError(string.Empty, "No se encontró información del funcionario en SIGPER");
            if (persona.Unidad == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la unidad del funcionario en SIGPER");
            if (persona.Jefatura == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la jefatura del funcionario en SIGPER");

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
            for (var i = DateTime.Now.Year - 15; i < DateTime.Now.Year + 15; i++)
                Anno.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            if (ModelState.IsValid)
            {
                model.FechaSolicitud = DateTime.Now;
                model.UnidadId = persona.Unidad.Pl_UndCod;
                model.Unidad = persona.Unidad.Pl_UndDes.Trim();
                model.jefaturaId = persona.Jefatura.RH_NumInte;
                model.JefaturaDV = persona.Jefatura.RH_DvNuInt.Trim();
                model.NombreJefatura = persona.Jefatura.RH_NomFunCap.Trim();
                model.Nombre = persona.Funcionario.RH_NomFunCap.Trim();
                model.NombreId = persona.Funcionario.RH_NumInte;
                model.DV = persona.Funcionario.RH_DvNuInt.Trim();
                model.Annio = DateTime.Now.Year.ToString();
                ViewBag.Mes = new SelectList(Meses, "Value", "Text");
                ViewBag.Annio = new SelectList(Anno, "Value", "Text");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HorasExtras model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseHorasExtras(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.HorasExtrasInsert(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Create", "Colaborador", new { id = model.HorasExtrasId });
                }
                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

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
            for (var i = DateTime.Now.Year - 15; i < DateTime.Now.Year + 15; i++)
                Anno.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            var usuarios = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            var model = _repository.GetById<HorasExtras>(id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HorasExtras model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseHorasExtras(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.HorasExtrasUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<HorasExtras>(id);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseHorasExtras(_repository);
            var model = _repository.GetById<HorasExtras>(id);
            var HorasId = model.HorasExtrasId;
            var _UseCaseResponseMessage = _useCaseInteractor.HorasExtrasDelete(id);


            if (_UseCaseResponseMessage.IsValid)
            {
                TempData["Success"] = "Operación terminada correctamente.";

                /*se redireccina a la vista que llamo al metodo de borrar*/
                var com = _repository.Get<HorasExtras>(c => c.HorasExtrasId == HorasId).FirstOrDefault();
                var pro = _repository.Get<Workflow>(p => p.ProcesoId == com.ProcesoId);//.Where(c => c.DefinicionWorkflow.Secuencia == 6);
                if (pro.Count() > 0)
                    return RedirectToAction("Edit", "HorasExtras", new { id = HorasId });
                else
                    return RedirectToAction("Edit", "HorasExtras", new { id = HorasId });
            }


            foreach (var item in _UseCaseResponseMessage.Errors)
            {
                ModelState.AddModelError(string.Empty, item);
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult GeneraDocumento(int id)
        {
            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            int tipoDoc = 0;
            int idDoctoHoras = 0;
            string Name = string.Empty;
            var hrs = _repository.GetById<HorasExtras>(id);

            /*Se genera resolucuion de trabajos extraordinarios*/
            Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("ResolucionProgramacion", new { id = hrs.HorasExtrasId }) { FileName = "ResolucionProgramacion" + ".pdf", FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
            pdf = resultPdf.BuildFile(ControllerContext);
            data = _file.BynaryToText(pdf);
            tipoDoc = 9;
            Name = "Resolución Programación Trabajos Extraordinarios nro" + " " + hrs.HorasExtrasId.ToString() + ".pdf";
            
            /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
            var docto = _repository.GetAll<Documento>().Where(d => d.ProcesoId == hrs.ProcesoId);
            if (docto != null)
            {
                foreach (var res in docto)
                {
                    if (res.TipoDocumentoId == 9)
                        idDoctoHoras = res.DocumentoId;
                }
            }

            if (idDoctoHoras == 0)
            {
                var email = UserExtended.Email(User);
                var doc = new Documento();
                doc.Fecha = DateTime.Now;
                doc.Email = email;
                doc.FileName = Name;
                doc.File = pdf;
                doc.ProcesoId = hrs.ProcesoId.Value;
                doc.WorkflowId = hrs.WorkflowId.Value;
                doc.Signed = false;
                doc.Texto = data.Text;
                doc.Metadata = data.Metadata;
                doc.Type = data.Type;
                doc.TipoPrivacidadId = 1;
                doc.TipoDocumentoId = tipoDoc;

                _repository.Create(doc);
                _repository.Save();
            }
            else
            {
                var docOld = _repository.GetById<Documento>(idDoctoHoras);
                docOld.File = pdf;
                docOld.Signed = false;
                docOld.Texto = data.Text;
                docOld.Metadata = data.Metadata;
                docOld.Type = data.Type;
                _repository.Update(docOld);
                _repository.Save();
            }

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        [AllowAnonymous]
        public ActionResult ResolucionProgramacion(int id)
        {
            var model = _repository.GetById<HorasExtras>(id);
            return View(model);
        }

        public ActionResult SignResolucion(int id)
        {
            var model = _repository.GetById<HorasExtras>(id);
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SignResolucion(int? DocumentoId)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseHorasExtras(_repository, _sigper, _file, _folio, _hsm, _email);
                var obj = _repository.Get<HorasExtras>(c => c.HorasExtrasId == DocumentoId).FirstOrDefault();
                var doc = _repository.Get<Documento>(c => c.ProcesoId == obj.ProcesoId && c.TipoDocumentoId == 9).FirstOrDefault();//.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 4).FirstOrDefault();
                var user = User.Email();
                var _UseCaseResponseMessage = _useCaseInteractor.SignReso(doc, user, null);

                //if (_UseCaseResponseMessage.Warnings.Count > 0)
                //    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }
            //return View(model);
            return Redirect(Request.UrlReferrer.ToString());
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
        public FileResult GeneraResolucion(string mes, string annio)
        {
            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            int tipoDoc = 0;
            int idDoctoHoras = 0; 
            string Name = string.Empty;
            var hrs = _repository.GetAll<HorasExtras>().Where(c =>c.Mes == mes && c.Annio == annio);

            /*Se genera resolucuion de trabajos extraordinarios*/
            Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("ResolucionServicio", new { mes = hrs.FirstOrDefault().Mes, annio = hrs.FirstOrDefault().Annio }) { FileName = "ResolucionProgramacionServicio" + ".pdf", FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
            pdf = resultPdf.BuildFile(ControllerContext);
            data = _file.BynaryToText(pdf);
            tipoDoc = 12;
            Name = "Resolución Programación Trabajos Extraordinarios mes" + " " + hrs.FirstOrDefault().Mes.ToString() + ".pdf";

            /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
            var docto = _repository.GetAll<Documento>().Where(d => d.ProcesoId == hrs.FirstOrDefault().ProcesoId);
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
                doc.ProcesoId = hrs.FirstOrDefault().ProcesoId.Value;
                doc.WorkflowId = hrs.FirstOrDefault().WorkflowId.Value;
                doc.Signed = false;
                doc.Texto = data.Text;
                doc.Metadata = data.Metadata;
                doc.Type = data.Type;
                doc.TipoPrivacidadId = 1;
                doc.TipoDocumentoId = tipoDoc;

                _repository.Create(doc);
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

            ///*Se genera registro de la generacin de la resolucion*/
            //var genera = new GeneracionResolucion();
            //genera.FechaCreacion = DateTime.Now;
            //genera.Annio = annio;
            //genera.Mes = mes;
            //genera.ProcesoId = hrs.FirstOrDefault().ProcesoId.Value;
            //genera.WorkflowId = hrs.FirstOrDefault().WorkflowId.Value;
            //_repository.Create(genera);
            //_repository.Save();

            return File(docOld.File, "application/pdf");            
        }

        [AllowAnonymous]
        public ActionResult ResolucionServicio(string mes, string annio)
        {
            var model = _repository.GetAll<HorasExtras>().Where(c => c.Mes == mes && c.Annio == annio);
            return View(model);
        }

        public ActionResult FirmaDocumento()
        {
            var model = _repository.GetAll<HorasExtras>();

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
        public ActionResult FirmaDocumento(string mes, string annio)
        {
            var model = _repository.Get<HorasExtras>(c => c.Mes == mes && c.Annio == annio);

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
    }
}