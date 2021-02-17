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
using System.Text;
using App.Model.Cometido;
using Enum = App.Util.Enum;

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

        public class DtoHoras
        {
            public double? ValorPagadoHD { get; set; } = 0;
            public double? ValorPagadoHN { get; set; } = 0;
            public double? TotalValorPagado { get; set; } = 0;
        }

        public DtoHoras CalculoHorasPagadas(int ValorHD, int HorasDiurnas, int ValorHN, int HorasNocturnas)
        {
            DtoHoras total = new DtoHoras();
            total.ValorPagadoHD = HorasDiurnas * ValorHD;
            total.ValorPagadoHN = HorasNocturnas * ValorHN;
            total.TotalValorPagado = total.ValorPagadoHD + total.ValorPagadoHN;

            return total;
        }
        public ActionResult Index()
        {
            var model = _repository.GetAll<HorasExtras>();
            return View(model);
        }

        public ActionResult procesState(int WorkflowId)
        {
            int sec = 1;
            List<Workflow> _Workflow = new List<Workflow>();
            List<DefinicionWorkflow> _Definicionworkflow = new List<DefinicionWorkflow>();
            var _workflow = _repository.GetById<Workflow>(WorkflowId);            
            if (_workflow != null)
            {
                switch (_workflow.Entity)
                {
                    case "HorasExtras":
                        var hrs = _repository.Get<HorasExtras>(c => c.WorkflowId.Value == WorkflowId).FirstOrDefault();
                        if (hrs != null)
                        {
                            hrs.Workflow = _workflow;
                            sec = !string.IsNullOrEmpty(hrs.Workflow.DefinicionWorkflow.Secuencia.ToString()) ? hrs.Workflow.DefinicionWorkflow.Secuencia : 1;
                            _Workflow = _repository.Get<Workflow>(c => c.ProcesoId == hrs.ProcesoId).ToList();
                        }
                        else
                            _Workflow = _repository.Get<Workflow>(c => c.WorkflowId == WorkflowId).ToList();

                        _Definicionworkflow = _repository.Get<DefinicionWorkflow>(c => c.DefinicionProcesoId == (int)Enum.DefinicionProceso.ProgramacionHorasExtraordinarias && c.Habilitado == true).OrderBy(c => c.Secuencia).ToList();

                        break;
                    case "Cometido":
                        var _cometido = _repository.Get<Cometido>(c => c.WorkflowId.Value == WorkflowId).FirstOrDefault();
                        if (_cometido != null)
                        {
                            _cometido.Workflow = _workflow;
                            sec = !string.IsNullOrEmpty(_cometido.Workflow.DefinicionWorkflow.Secuencia.ToString()) ? _cometido.Workflow.DefinicionWorkflow.Secuencia : 1;
                            _Workflow = _repository.Get<Workflow>(c => c.ProcesoId == _cometido.ProcesoId).ToList();
                        }
                        else
                            _Workflow = _repository.Get<Workflow>(c => c.WorkflowId == WorkflowId).ToList();

                        _Definicionworkflow = _repository.Get<DefinicionWorkflow>(c => c.DefinicionProcesoId == (int)Enum.DefinicionProceso.SolicitudCometidoPasaje && c.Habilitado == true).OrderBy(c => c.Secuencia).ToList();

                        break;
                }
            }

            var Total = _Definicionworkflow.Count(c => c.Habilitado == true).ToString();

            var por = (float.Parse(sec.ToString()) / float.Parse(Total))*100;
            
            ViewBag.Secuencia = sec;
            ViewBag.Total = Total;
            ViewBag.Porcentaje = Math.Round((Convert.ToDouble(por)),0);

            var model = new DTOStateProces()
            {
                Tarea = 1,
                Total = int.Parse(Total),
                Porcentaje = por,
                Secuencia = sec,
                CantTareasRealizadas = _Workflow,
                DefWorkflow = _Definicionworkflow,
            };

            return View(model);
        }

        public ActionResult View(int id)
        {
            //var persona = _sigper.GetUserByEmail(User.Email());
            var model = _repository.GetFirst<HorasExtras>(c => c.ProcesoId.Value == id);
            if (model == null)
                return RedirectToAction("Details", "Proceso", new { id });

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
                model.FechaSolicitud = DateTime.Today.Date;
                model.UnidadId = persona.Unidad.Pl_UndCod;
                model.Unidad = persona.Unidad.Pl_UndDes.Trim();
                model.jefaturaId = persona.Jefatura.RH_NumInte;
                model.JefaturaDV = persona.Jefatura.RH_DvNuInt.Trim();
                model.NombreJefatura = persona.Jefatura.RH_NomFunCap.Trim();
                model.Nombre = persona.Funcionario.RH_NomFunCap.Trim();
                model.NombreId = persona.Funcionario.RH_NumInte;
                model.DV = persona.Funcionario.RH_DvNuInt.Trim();
                model.Annio = DateTime.Now.Year.ToString();
                model.MesBaseCalculo = DateTime.Now.Month;
                model.Mes = DateTime.Now.ToString("MMMM").ToUpper();
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
                switch(model.Mes)
                {
                    case "Enero": model.MesBaseCalculo = 01;break;
                    case "Febrero": model.MesBaseCalculo = 02;break;
                    case "Marzo": model.MesBaseCalculo = 03;break;
                    case "Abril": model.MesBaseCalculo = 04;break;
                    case "Mayo": model.MesBaseCalculo = 05;break;
                    case "Junio": model.MesBaseCalculo = 06;break;
                    case "Julio": model.MesBaseCalculo = 07;break;
                    case "Agosto": model.MesBaseCalculo = 08;break;
                    case "Septiembre": model.MesBaseCalculo = 09;break;
                    case "Octubre": model.MesBaseCalculo = 10;break;
                    case "Noviembre": model.MesBaseCalculo = 11;break;
                    case "Diciembre": model.MesBaseCalculo = 12;break;
                }

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
            var parrafos = _repository.Get<Parrafos>(p => p.DefinicionProcesoId == (int)Enum.DefinicionProceso.ProgramacionHorasExtraordinarias);
            var model = _repository.GetById<HorasExtras>(id);
            model.OrdenHEProg = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenHEProg).FirstOrDefault().ParrafoTexto;
            model.FirmanteHEProg = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteHEProg).FirstOrDefault().ParrafoTexto;
            model.CargoFirmanteHEProg = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoFirmanteHEProg).FirstOrDefault().ParrafoTexto;
            model.DistribucionHEProg = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.DistribucionHEProg).FirstOrDefault().ParrafoTexto.Replace(",",Environment.NewLine);
            model.VistosHEProg = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosHEProg).FirstOrDefault().ParrafoTexto;
            
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
                var _UseCaseResponseMessage = _useCaseInteractor.SignReso(doc, user, obj.HorasExtrasId);

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
            var hrs = _repository.GetAll<HorasExtras>().Where(c => c.Mes == mes && c.Annio == annio);

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

        public ActionResult EditGP(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            var usuarios = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            var model = _repository.GetById<HorasExtras>(id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGP(HorasExtras model)
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

        public ActionResult EditConfirmacion(int id)
        {
            var model = _repository.GetById<HorasExtras>(id);

            foreach (var c in model.Colaborador)
            {
                /*Se toman la cantidad de horas extras desde persomatico, para validar la cantidad de horas extras*/
                if (c.HDSigper == 0)
                {
                    int mes = 0;
                    switch (model.Mes)
                    {
                        case "Enero": mes = 01; break;
                        case "Febrero": mes = 02; break;
                        case "Marzo": mes = 03; break;
                        case "Abril": mes = 04; break;
                        case "Mayo": mes = 05; break;
                        case "Junio": mes = 06; break;
                        case "Julio": mes = 07; break;
                        case "Agosto": mes = 08; break;
                        case "Septiembre": mes = 09; break;
                        case "Octubre": mes = 10; break;
                        case "Noviembre": mes = 11; break;
                        case "Diciembre": mes = 12; break;
                    }
                    var hrsExtrasDiurnas = _sigper.GetHorasTrabajadas(c.NombreId.ToString(), mes, int.Parse(model.Annio));
                    
                    c.HDSigper = Math.Round(Convert.ToDouble(hrsExtrasDiurnas) / 60,2);
                    c.HNSigper = 0;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirmacion(HorasExtras model, List<Colaborador> _colaborador)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseHorasExtras(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = new ResponseMessage();

                var work = _repository.GetById<Workflow>(model.WorkflowId);

                if (_colaborador.Exists(c => c.ObservacionesConfirmacion == null) && (work.DefinicionWorkflow.Secuencia != 9))
                {
                    _UseCaseResponseMessage.Errors.Add("Se deben llenar todos los campos de observaciones");
                }
                else
                {
                    /*se guardan las observaciones de la confirmacion*/
                    foreach (var c in _colaborador)
                    {
                        _UseCaseResponseMessage = _useCaseInteractor.ColaboradorUpdate(c);
                    }

                    if (_UseCaseResponseMessage.IsValid)
                    {
                        TempData["Success"] = "Operación terminada correctamente.";
                        return Redirect(Request.UrlReferrer.PathAndQuery);
                    }
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            var m = _repository.GetById<HorasExtras>(model.HorasExtrasId);
            return View(m);
        }

        public ActionResult GeneraResolucionConfirmacion(int id)
        {
            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            int tipoDoc = 0;
            int idDoctoHoras = 0;
            string Name = string.Empty;
            var hrs = _repository.GetById<HorasExtras>(id);

            /*Se genera resolucuion de trabajos extraordinarios*/
            Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("ResolucionConfirmacion", new { id = hrs.HorasExtrasId }) { FileName = "ResolucionConfirmacion" + ".pdf", FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
            pdf = resultPdf.BuildFile(ControllerContext);
            data = _file.BynaryToText(pdf);
            tipoDoc = 13;
            Name = "Resolución Confirmación Horas Extraordinarios Pagadas nro" + " " + hrs.HorasExtrasId.ToString() + ".pdf";

            /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
            var docto = _repository.GetAll<Documento>().Where(d => d.ProcesoId == hrs.ProcesoId);
            if (docto != null)
            {
                foreach (var res in docto)
                {
                    if (res.TipoDocumentoId == 13)
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
        public ActionResult ResolucionConfirmacion(int id)
        {
            var parrafos = _repository.Get<Parrafos>(p => p.DefinicionProcesoId == (int)Enum.DefinicionProceso.ProgramacionHorasExtraordinarias);
            var model = _repository.GetById<HorasExtras>(id);
            model.Colaborador.FirstOrDefault().ValorTotalPago = 4521;
            model.ValorTotalHorasPalabras = ExtensionesString.enletras(model.ValorTotalHoras.ToString());
            model.OrdenHEPag = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenHEPag).FirstOrDefault().ParrafoTexto;
            model.FirmanteHEPag = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteHEPag).FirstOrDefault().ParrafoTexto;
            model.CargoFirmanteHEPag = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoFirmanteHEPag).FirstOrDefault().ParrafoTexto;
            model.DistribucionHEPag = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.DistribucionHEPag).FirstOrDefault().ParrafoTexto.Replace(",", Environment.NewLine);
            model.VistosHEPag = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosHEPag).FirstOrDefault().ParrafoTexto;
            model.Iniciales = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.InicialesRHPagadas).FirstOrDefault().ParrafoTexto;

            return View(model);
        }

        public ActionResult SignConfirmacion(int id)
        {
            var model = _repository.GetById<HorasExtras>(id);
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SignConfirmacion(int? HorasId)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseHorasExtras(_repository, _sigper, _file, _folio, _hsm, _email);
                //var obj = _repository.Get<HorasExtras>(c => c.HorasExtrasId == HorasId).FirstOrDefault();
                //var doc = _repository.Get<Documento>(c => c.ProcesoId == obj.ProcesoId && c.TipoDocumentoId == 13).FirstOrDefault();
                var doc = _repository.GetById<Documento>(HorasId);
                var user = User.Email();
                var _UseCaseResponseMessage = _useCaseInteractor.SignReso(doc, user, null);

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
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }
            //return View(model);
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult EditRemuneraciones(int id)
        {
            var model = _repository.GetById<HorasExtras>(id);

            List<SelectListItem> Meses = new List<SelectListItem>
            {
                //new SelectListItem {Text = "..Seleccione", Value = "0"},
                new SelectListItem {Text = "Enero", Value = "1"},
                new SelectListItem {Text = "Febrero", Value = "2"},
                new SelectListItem {Text = "Marzo", Value = "3"},
                new SelectListItem {Text = "Abril", Value = "4"},
                new SelectListItem {Text = "Mayo", Value = "5"},
                new SelectListItem {Text = "Junio", Value = "6"},
                new SelectListItem {Text = "Julio", Value = "7"},
                new SelectListItem {Text = "Agosto", Value = "8"},
                new SelectListItem {Text = "Septiembre", Value = "9"},
                new SelectListItem {Text = "Octubre", Value = "10"},
                new SelectListItem {Text = "Noviembre", Value = "11"},
                new SelectListItem {Text = "Diciembre", Value = "12"},
            };

             ViewBag.MesBaseCalculo = new SelectList(Meses, "Value", "Text");

            foreach (var c in model.Colaborador)
            {
                /*Se toma base de calculo para valor horas extras*/
                var CalidadJurid = _sigper.GetUserByRut(c.NombreId.Value).Contrato.RH_ContCod;
                var anno = int.Parse(model.Annio); //Convert.ToInt32(DateTime.Now.Year);
                var mes = Convert.ToInt32(DateTime.Now.Month) - 1;
                if (model.MesBaseCalculo != 0)
                    mes = model.MesBaseCalculo;

                Int32 monto = _sigper.GetBaseCalculoHorasExtras(c.NombreId.Value, mes, anno, CalidadJurid);
                Int32 baseCalculo = monto / (int)Enum.HorasExtras.ConstateDias; //190;//constante

                c.ValorHorasDiurnas = Convert.ToInt32(baseCalculo * 1.25); /*horas diurnas*/
                c.ValorHorasNocturnas = Convert.ToInt32(baseCalculo * 1.5); /*horas nocturnas*/
            }


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRemuneraciones(HorasExtras model, List<Colaborador> _colaborador)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseHorasExtras(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = new ResponseMessage();

                foreach (var c in _colaborador)
                {
                    c.ValorPagadoHD = c.HDPagoAprobados * c.ValorHorasDiurnas.Value;
                    c.ValorPagadoHN = c.HNPagoAprobados * c.ValorHorasNocturnas.Value;
                    c.ValorTotalPago = c.ValorPagadoHD + c.ValorPagadoHN;
                    model.ValorTotalHoras += c.ValorTotalPago;

                    _useCaseInteractor.ColaboradorUpdate(c);
                }

                _useCaseInteractor.HorasExtrasUpdate(model);


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

        public ActionResult GeneraResolucionDescanso(int id)
        {
            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            int tipoDoc = 0;
            int idDoctoHoras = 0;
            string Name = string.Empty;
            var hrs = _repository.GetById<HorasExtras>(id);

            /*Se genera resolucuion de trabajos extraordinarios*/
            Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("ResolucionDescanso", new { id = hrs.HorasExtrasId }) { FileName = "ResolucionDescanso" + ".pdf", FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
            pdf = resultPdf.BuildFile(ControllerContext);
            data = _file.BynaryToText(pdf);
            tipoDoc = 14;
            Name = "Resolución Confirmación Horas Extraordinarios Compensadas nro" + " " + hrs.HorasExtrasId.ToString() + ".pdf";

            /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
            var docto = _repository.GetAll<Documento>().Where(d => d.ProcesoId == hrs.ProcesoId);
            if (docto != null)
            {
                foreach (var res in docto)
                {
                    if (res.TipoDocumentoId == 14)
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
        public ActionResult ResolucionDescanso(int id)
        {
            var model = _repository.GetById<HorasExtras>(id);
            var parrafos = _repository.Get<Parrafos>(p => p.DefinicionProcesoId == (int)Enum.DefinicionProceso.ProgramacionHorasExtraordinarias);
            model.OrdenHECom = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenHECom).FirstOrDefault().ParrafoTexto;
            model.FirmanteHECom = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteHECom).FirstOrDefault().ParrafoTexto;
            model.CargoFirmanteHECom = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoFirmanteHECom).FirstOrDefault().ParrafoTexto;
            model.DistribucionHECom = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.DistribucionHECom).FirstOrDefault().ParrafoTexto.Replace(",", Environment.NewLine);
            model.VistosHECom = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosHECom).FirstOrDefault().ParrafoTexto;
            model.Iniciales = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.InicialesRHCompensadas).FirstOrDefault().ParrafoTexto;

            return View(model);
        }

    }
}