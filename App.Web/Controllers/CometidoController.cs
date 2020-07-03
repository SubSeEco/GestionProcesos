using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using App.Model.Cometido;
using App.Model.Pasajes;
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
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using jdk.nashorn.@internal.objects.annotations;
using com.mp4parser.streaming.extensions;
//using com.sun.corba.se.spi.ior;
//using System.Net.Mail;
//using com.sun.codemodel.@internal;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class CometidoController : Controller
    {
        public class DTOFilterCometido
        {
            public DTOFilterCometido()
            {
                TextSearch = string.Empty;
                Select = new HashSet<DTOSelect>();
                Result = new HashSet<Cometido>();
            }

            [Display(Name = "Texto de búsqueda")]
            public string TextSearch { get; set; }

            [Display(Name = "Desde")]
            [DataType(DataType.Date)]
            public System.DateTime? Desde { get; set; }

            [Display(Name = "Hasta")]
            [DataType(DataType.Date)]
            public System.DateTime? Hasta { get; set; }

            [Display(Name = "ID")]
            public int ID { get; set; }

            [Display(Name = "Funcionario")]
            public string Ejecutor { get; set; }

            [Display(Name = "Fecha Inicio")]
            [DataType(DataType.Date)]
            public System.DateTime? FechaInicio { get; set; }

            [Display(Name = "Fecha Término")]
            [DataType(DataType.Date)]
            public System.DateTime? FechaTermino { get; set; }

            [Display(Name = "Fecha de solicitud")]
            [DataType(DataType.Date)]
            public System.DateTime? FechaSolicitud { get; set; }

            [Display(Name = "Funcionario")]
            public int? NombreId { get; set; }

            [Display(Name = "Rut")]
            public int Rut { get; set; }

            [Display(Name = "Unidad Funcionario")]
            public int? IdUnidad { get; set; }

            [Display(Name = "Unidad")]
            public string NombreUnidad { get; set; }

            [Display(Name = "Estado")]
            public int? Estado { get; set; }

            [Display(Name = "Destino")]
            public int? Destino { get; set; }

            [Display(Name = "N° Dias")]
            public int DiasDiferencia { get; set; }

            public IEnumerable<DTOSelect> Select { get; set; }
            public IEnumerable<Cometido> Result { get; set; }
        }

        public class Chart
        {
            public Chart()
            {
                datasets = new List<Datasets>();
            }
            public string[] labels { get; set; }
            public List<Datasets> datasets { get; set; }
        }

        public class Datasets
        {
            public string label { get; set; }
            public string[] backgroundColor { get; set; }
            public string[] borderColor { get; set; }
            public string borderWidth { get; set; }
            public double[] data { get; set; }
            public bool fill { get; set; }
        }

        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IHSM _hsm;
        private static List<App.Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }
        public static List<Destinos> ListDestino = new List<Destinos>();

        public CometidoController(IGestionProcesos repository, ISIGPER sigper, IHSM hsm, IFile file)
        {
            _repository = repository;
            _sigper = sigper;
            _hsm = hsm;
            _file = file;

            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
        }

        public JsonResult GetUser(string term)
        {
            var result = ActiveDirectoryUsers
               .Where(q => (q.User != null && q.User.ToLower().Contains(term.ToLower())) || (q.Email != null && q.Email.ToLower().Contains(term.ToLower())))
               .Take(25)
               .Select(c => new { id = c.Email, value = string.Format("{0} ({1})", c.User, c.Email) })
               .OrderBy(q => q.value)
               .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuario(int Rut)
        {
            var correo = _sigper.GetUserByRut(Rut).Funcionario.Rh_Mail.Trim();
            var per = _sigper.GetUserByEmail(correo.Trim());
            var IdCargo = per.FunDatosLaborales.RhConCar.Value;
            var cargo = string.IsNullOrEmpty(per.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == per.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidad = per.FunDatosLaborales.RH_ContCod;
            var calidad = string.IsNullOrEmpty(per.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == per.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGrado = string.IsNullOrEmpty(per.FunDatosLaborales.RhConGra.Trim()) ? "0" : per.FunDatosLaborales.RhConGra.Trim();
            var grado = string.IsNullOrEmpty(per.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : per.FunDatosLaborales.RhConGra.Trim();
            var estamento = per.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == per.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();


            //var algo = _sigper.GetReContra().Where(c => /*c.RH_NumInte == per.Funcionario.RH_NumInte && */c.Re_ConIni >= Convert.ToDateTime("01-01-2020")).ToList(); 
            //var jhkj = algo.Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte);


            var ProgId = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte && c.Re_ConIni.Year == DateTime.Now.Year).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte && c.Re_ConIni.Year == DateTime.Now.Year) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte && c.Re_ConIni.Year == DateTime.Now.Year).FirstOrDefault().Re_ConPyt;
            var Programa = ProgId != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgId).FirstOrDefault().RePytDes : "S/A";
            var conglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte).ReContraSed;
            var jefatura = per.Jefatura != null ? per.Jefatura.PeDatPerChq : "Sin jefatura definida" ;

            string rut;
            if (per.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = per.Funcionario.RH_NumInte.ToString();
                rut = string.Concat("0", t);
            }
            else
            {
                rut = per.Funcionario.RH_NumInte.ToString();
            }

            return Json(new { Rut = rut,
                DV = per.Funcionario.RH_DvNuInt.ToString(),
                IdCargo = IdCargo,
                Cargo = cargo,
                IdCalidad = IdCalidad,
                CalidadJuridica = calidad,
                IdGrado = IdGrado,
                Grado = grado,
                Estamento = estamento,
                Programa = Programa.Trim(),
                Conglomerado = conglomerado,
                Unidad = per.Unidad.Pl_UndDes.Trim(),
                Jefatura = jefatura
            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Cometido>();
            return View(model);
        }

        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult View(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo");
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso");
            //ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");


            //var model = _repository.GetById<Cometido>(id);
            var model = _repository.GetFirst<Cometido>(q => q.ProcesoId == id);
            if (model == null)
                return RedirectToAction("Details", "Proceso", new { id });

            return View(model);
        }

        public ActionResult CreaDocto(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            if (model.GeneracionCDP.Count > 0)
            {
                var cdp = _repository.GetById<GeneracionCDP>(model.GeneracionCDP.FirstOrDefault().GeneracionCDPId);
                model.GeneracionCDP.Add(cdp);
            }

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            if (model.GeneracionCDP.Count > 0)
            {
                var cdp = _repository.GetById<GeneracionCDP>(model.GeneracionCDP.FirstOrDefault().GeneracionCDPId);
                model.GeneracionCDP.Add(cdp);
            }

            return View(model);
        }

        public ActionResult DetailsDocto(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            //if (model.GeneracionCDP.Count > 0)
            //{
            //    var cdp = _repository.GetById<GeneracionCDP>(model.GeneracionCDP.FirstOrDefault().GeneracionCDPId);
            //    model.GeneracionCDP.Add(cdp);
            //}

            return View(model);
        }

        public ActionResult DetailsFinanzas(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DetailsFinanzas(Cometido model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _hsm);
                //var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);
                var doc = _repository.Get<Documento>(c =>c.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 5).FirstOrDefault();
                var user = User.Email();
                var _UseCaseResponseMessage = _useCaseInteractor.DocumentoSign(doc, user);



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

            var modelo = _repository.Get<Cometido>(c => c.CometidoId == model.CometidoId).FirstOrDefault();
            
            return View(modelo);
            //return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            return View(model);
        }

        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            var cdp = _repository.GetAll<GeneracionCDP>().Where(c => c.CometidoId == model.CometidoId).ToList();
            if (cdp != null)
            {
                model.GeneracionCDP.Add(cdp.FirstOrDefault());

                /*Validar si existe un documento asociado y si se encuentra firmado*/
                var doc = _repository.GetAll<Documento>().Where(c => c.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 1).FirstOrDefault();
                if (doc != null)
                {
                    if (doc.Signed != true)
                        GeneraDocumento(model.CometidoId);
                    //else
                    //    TempData["Warning"] = "Documento se encuentra firmado electronicamente";
                }
            }
            
            return View(model);
        }

        public ActionResult Create(int? WorkFlowId, int? ProcesoId)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().Where(q => q.Activo == true).OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo");
            //ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers().Where(c =>c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");


            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new Cometido
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
                Tarea = workflow.DefinicionWorkflow.Nombre
            };

            if (persona.Funcionario == null)
                ModelState.AddModelError(string.Empty, "No se encontró información del funcionario en SIGPER");
            if (persona.Unidad == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la unidad del funcionario en SIGPER");
            if (persona.Jefatura == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la jefatura del funcionario en SIGPER");

            if (ModelState.IsValid)
            {
                model.FechaSolicitud = DateTime.Now;
                model.IdUnidad = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcion = persona.Unidad.Pl_UndDes.Trim();
                model.Rut = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DV = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreId = persona.Funcionario.RH_NumInte;
                model.Nombre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargo = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.IdCalidad = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                model.IdGrado = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                model.GradoDescripcion = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                model.IdEstamento = persona.FunDatosLaborales.PeDatLabEst;
                model.EstamentoDescripcion = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                model.FinanciaOrganismo = false;
                model.Vehiculo = false;
                model.SolicitaReembolso = true;
                model.IdConglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                model.ConglomeradoDescripcion = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                model.IdPrograma = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcion = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt.ToString();
                model.ProgramaDescripcion = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.Jefatura = persona.Jefatura.PeDatPerChq;

                model.Destinos = ListDestino;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cometido model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);

            model.FechaSolicitud = DateTime.Now;
            model.SolicitaReembolso = true;
            model.Destinos = ListDestino;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.CometidoInsert(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", "Cometido", new { model.WorkflowId, id = model.CometidoId });
                }
                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            return View(model);
        }

        public ActionResult EditSigfeTesoreria(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            List<SelectListItem> tipoPagoTesoreria = new List<SelectListItem>
            {
            new SelectListItem {Text = "Pago", Value = "1"},
            new SelectListItem {Text = "Pago con Observaciones", Value = "2"},
            new SelectListItem {Text = "No Pago", Value = "3"},
            };

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.IdFuncionarioPagadorTesoreria = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdTipoPagoTesoreria = new SelectList(_repository.GetAll<TipoPagoSIGFE>().Where(q => q.TipoActivo == true), "TipoPagoSIGFEId", "DescripcionTipoPago");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSigfeTesoreria(Cometido model)
        {
            var resp = new ResponseMessage();

            if (model.IdFuncionarioPagadorTesoreria != 0)
            {
                var nombre = _sigper.GetUserByRut(model.IdFuncionarioPagadorTesoreria).Funcionario.PeDatPerChq;
                model.NombreFuncionarioPagadorTesoreria = nombre.Trim();
            }

            if (!model.IdTipoPagoTesoreria.HasValue)
                resp.Errors.Add("Debe ingresar tipo de pago Tesoreria.");

            if (string.IsNullOrEmpty(model.IdSigfeTesoreria))
                resp.Errors.Add("Debe ingresar ID SIGFE Tesoreria.");

            if (!model.FechaPagoSigfeTesoreria.HasValue)
                resp.Errors.Add("Debe ingresar fecha pago sigfe Tesoreria");

            if (resp.Errors.Count == 0)
            {
                model.TesoreriaOk = true;

                if (ModelState.IsValid)
                {
                    var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                    var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);

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
            }
            else
            {
                foreach (var item in resp.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            List<SelectListItem> tipoPagoTesoreria = new List<SelectListItem>
            {
            new SelectListItem {Text = "Pago", Value = "1"},
            new SelectListItem {Text = "Pago con Observaciones", Value = "2"},
            new SelectListItem {Text = "No Pago", Value = "3"},
            };

            model = _repository.GetById<Cometido>(model.CometidoId);
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.IdFuncionarioPagadorTesoreria = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdTipoPagoTesoreria = new SelectList(_repository.GetAll<TipoPagoSIGFE>().Where(q => q.TipoActivo == true), "TipoPagoSIGFEId", "DescripcionTipoPago");
            return View(model);
        }

        public ActionResult EditSigfe(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            List<SelectListItem> tipoPago = new List<SelectListItem>
            {
            new SelectListItem {Text = "Pago", Value = "1"},
            new SelectListItem {Text = "Pago con Observaciones", Value = "2"},
            new SelectListItem {Text = "No Pago", Value = "3"},
            };

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.IdFuncionarioPagador = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdTipoPago = new SelectList(_repository.GetAll<TipoPagoSIGFE>().Where(q => q.TipoActivo == true), "TipoPagoSIGFEId", "DescripcionTipoPago");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSigfe(Cometido model)
        {
            var resp = new ResponseMessage();

            if (model.IdFuncionarioPagador.HasValue)
            {
                var nombre = _sigper.GetUserByRut(model.IdFuncionarioPagador.Value).Funcionario.PeDatPerChq;
                model.NombreFuncionarioPagador = nombre.Trim();
            }
            
            if (!model.IdTipoPago.HasValue)
                resp.Errors.Add("Debe ingresar tipo de pago.");

            if(string.IsNullOrEmpty(model.IdSigfe))
                resp.Errors.Add("Debe ingresar ID SIGFE.");

            if(!model.FechaPagoSigfe.HasValue)
                resp.Errors.Add("Debe ingresar fecha pago sigfe");

            if (resp.Errors.Count == 0)
            {
                model.ContabilidadOk = true;

                if (ModelState.IsValid)
                {
                    var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                    var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);

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
            }
            else
            {
                foreach (var item in resp.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            List<SelectListItem> tipoPago = new List<SelectListItem>
            {
            new SelectListItem {Text = "Pago", Value = "1"},
            new SelectListItem {Text = "Pago con Observaciones", Value = "2"},
            new SelectListItem {Text = "No Pago", Value = "3"},
            };

            model = _repository.GetById<Cometido>(model.CometidoId);
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.IdFuncionarioPagador = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdTipoPago = new SelectList(_repository.GetAll<TipoPagoSIGFE>().Where(q => q.TipoActivo == true), "TipoPagoSIGFEId", "DescripcionTipoPago");
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            //model.Destinos = ListDestino;
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso", model.TipoReembolsoId);
            ViewBag.FinanciaOrganismo = model.FinanciaOrganismo;
            ViewBag.Alimentacion = model.Alimentacion;
            ViewBag.Alojamiento = model.Alojamiento;
            ViewBag.Pasajes = model.Pasajes;

            ViewBag.DestinosPasajes = _repository.Get<DestinosPasajes>(q => q.Pasaje.ProcesoId == model.ProcesoId);


            ViewBag.Pasaje = new DestinosPasajes(); //_des; //_repository.Get<DestinosPasajes>(c => c.PasajeId == 2038).FirstOrDefault(); //_repository.Get<Dest>(c => c.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault().CometidoId;
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
            //ViewBag.FechaOrigen = DateTime.Now;
            //ViewBag.FechaVuelta = DateTime.Now;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cometido model, DestinosPasajes DesPasajes)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);
                var resp = new ResponseMessage();
                var res = new ResponseMessage();
                

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    /*Se ingresa informacion de pasajes*/
                    /*Se crean los pasajes, si se solicitan*/
                    if (model.ReqPasajeAereo == true)
                    {
                        if (DesPasajes != null && DesPasajes.IdRegion != null)
                        {
                            /*se crea encabezado de pasaje y destinos de pasajes*/
                            Pasaje _pasaje = new Pasaje();
                            _pasaje.FechaSolicitud = DateTime.Now;
                            _pasaje.Nombre = model.Nombre;
                            _pasaje.NombreId = model.NombreId;
                            _pasaje.Rut = model.Rut;
                            _pasaje.DV = model.DV;
                            _pasaje.IdCalidad = model.IdCalidad;
                            _pasaje.CalidadDescripcion = model.CalidadDescripcion;
                            _pasaje.PasajeDescripcion = model.CometidoDescripcion;
                            _pasaje.TipoDestino = true;
                            _pasaje.ProcesoId = model.ProcesoId;
                            _pasaje.WorkflowId = model.WorkflowId;
                            resp = _useCaseInteractor.PasajeInsert(_pasaje);
                            if (resp.Errors.Count > 0)
                                _UseCaseResponseMessage.Errors.Add(resp.Errors.FirstOrDefault());

                            /*genera resgistro en tabla destino pasaje, segun los destinos señalados en el cometido*/
                            //foreach (var com in DesPasajes)
                            //{
                            DestinosPasajes _destino = new DestinosPasajes();
                            _destino.PasajeId = resp.EntityId;
                            _destino.IdRegion = DesPasajes.IdRegion;
                            _destino.RegionDescripcion = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == DesPasajes.IdRegion).Pl_DesReg.Trim(); //DesPasajes.RegionDescripcion;
                            _destino.IdRegionOrigen = DesPasajes.IdRegionOrigen;
                            _destino.OrigenRegionDescripcion = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == DesPasajes.IdRegionOrigen).Pl_DesReg.Trim();
                            _destino.FechaIda = DesPasajes.FechaOrigen;
                            _destino.FechaVuelta = DesPasajes.FechaVuelta;
                            _destino.FechaOrigen = DateTime.Now;
                            _destino.ObservacionesOrigen = DesPasajes.ObservacionesOrigen;
                            _destino.ObservacionesDestinos = DesPasajes.ObservacionesDestinos;
                            res = _useCaseInteractor.DestinosPasajesInsert(_destino);
                            if (res.Errors.Count > 0)
                                _UseCaseResponseMessage.Errors.Add(res.Errors.FirstOrDefault());
                            //}
                        }
                        //else
                        //{
                        //    _UseCaseResponseMessage.Errors.Add("Debe agregar datos del pasaje");
                        //    //TempData["Errors"] = "Debe agregar datos del pasaje";
                        //    //return Redirect(Request.UrlReferrer.PathAndQuery);
                        //}
                    }

                    /*se valiada se si ha creado los pasajes, cuando se solicitan*/
                    if (model.ReqPasajeAereo == true)
                    {
                        var pasaje = _repository.Get<Pasaje>(p => p.ProcesoId == model.ProcesoId).ToList();
                        if (pasaje.Count <= 0)
                        {
                            _UseCaseResponseMessage.Errors.Add("Debe agregar datos del pasaje");
                            //TempData["Errors"] = "Debe agregar datos del pasaje";
                        }
                    }
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }

                //TempData["Success"] = "Operación terminada correctamente.";
                //return Redirect(Request.UrlReferrer.PathAndQuery);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }

            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso", model.TipoReembolsoId);
            ViewBag.Pasaje = new DestinosPasajes(); //_repository.Get<DestinosPasajes>(c => c.PasajeId == 2038).FirstOrDefault(); //_repository.Get<Dest>(c => c.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault().CometidoId;
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");

            return View(model);
        }

        public ActionResult EditGP(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            ViewBag.FinanciaOrganismo = model.FinanciaOrganismo;


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGP(Cometido model)
        {

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);

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

            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso", model.TipoReembolsoId);

            return View(model);
        }

        public ActionResult EditPpto(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            //model.Destinos = ListDestino;
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso", model.TipoReembolsoId);
            ViewBag.FinanciaOrganismo = model.FinanciaOrganismo;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPpto(Cometido model)
        {

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);

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

            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso", model.TipoReembolsoId);

            return View(model);
        }

        public ActionResult GeneraDocumento(int id)
        {
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }

            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            int tipoDoc = 0;
            int IdDocto = 0;
            string Name = string.Empty;
            var model = _repository.GetById<Cometido>(id);
            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();
            if ((Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)) /*genera CDP, por la etapa en la que se encuentra*/
            {
                /*Se genera certificado de viatico*/
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("CDPViatico", new { id = model.CometidoId }) { FileName = "CDP_Viatico" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);
                tipoDoc = 2;
                Name = "CDP Viatico Cometido nro" + " " + model.CometidoId.ToString() + ".pdf";
                int idDoctoViatico = 0;

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var cdpViatico = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
                if (cdpViatico != null)
                {
                    foreach (var res in cdpViatico)
                    {
                        if (res.TipoDocumentoId == 2)
                            idDoctoViatico = res.DocumentoId;
                    }
                }

                if(idDoctoViatico == 0)
                {
                    /*se guarda certificado de viatico*/
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
                }
                else
                {
                    var docOld = _repository.GetById<Documento>(idDoctoViatico);
                    docOld.File = pdf;
                    docOld.Signed = false;
                    docOld.Texto = data.Text;
                    docOld.Metadata = data.Metadata;
                    docOld.Type = data.Type;
                    _repository.Update(docOld);
                    _repository.Save();
                }

                

                /*Se genera certificado de pasaje si es que existe*/
                //if (model.ReqPasajeAereo == true)
                //{
                //    Rotativa.ActionAsPdf resultPdfPasaje = new Rotativa.ActionAsPdf("CDPPasajes", new { id = model.CometidoId }) { FileName = "CDP_Pasajes" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                //    pdf = resultPdfPasaje.BuildFile(ControllerContext);
                //    data = GetBynary(pdf);
                //    tipoDoc = 3;
                //    Name = "CDP Pasaje Cometido nro" + " " + model.CometidoId.ToString() + ".pdf";
                //    int idDoctoPasaje = 0;

                //    /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                //    var CdpPasaje = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
                //    if (CdpPasaje != null)
                //    {
                //        foreach (var res in CdpPasaje)
                //        {
                //            if (res.TipoDocumentoId == 3)
                //                idDoctoPasaje = res.DocumentoId;
                //        }
                //    }

                //    if(idDoctoPasaje == 0)
                //    {
                //        /*se guarda certificado de pasaje*/
                //        var email = UserExtended.Email(User);
                //        var docPasaje = new Documento();
                //        docPasaje.Fecha = DateTime.Now;
                //        docPasaje.Email = email;
                //        docPasaje.FileName = Name;
                //        docPasaje.File = pdf;
                //        docPasaje.ProcesoId = model.ProcesoId.Value;
                //        docPasaje.WorkflowId = model.WorkflowId.Value;
                //        docPasaje.Signed = false;
                //        docPasaje.Texto = data.Text;
                //        docPasaje.Metadata = data.Metadata;
                //        docPasaje.Type = data.Type;
                //        docPasaje.TipoPrivacidadId = 1;
                //        docPasaje.TipoDocumentoId = tipoDoc;

                //        _repository.Create(docPasaje);
                //        _repository.Save();
                //    }
                //    else
                //    {
                //        var docOld = _repository.GetById<Documento>(idDoctoPasaje);
                //        docOld.File = pdf;
                //        docOld.Signed = false;
                //        docOld.Texto = data.Text;
                //        docOld.Metadata = data.Metadata;
                //        docOld.Type = data.Type;
                //        _repository.Update(docOld);
                //        _repository.Save();
                //    }
                //}
            }
            else
            {
                if (model.CalidadDescripcion.Contains("HONORARIOS"))/*valida si es contrata u honorario*/
                {
                    //if (model.IdGrado != "0" && model.GradoDescripcion != "0")
                    //{
                        Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Orden", new { id = model.CometidoId }) { FileName = "Orden_Pago" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                        pdf = resultPdf.BuildFile(ControllerContext);
                        //data = GetBynary(pdf);
                        data = _file.BynaryToText(pdf);

                    tipoDoc = 1;
                        Name = "Orden de Pago Cometido nro" + " " + model.CometidoId.ToString() + ".pdf";
                    //}
                    //else
                    //{
                    //    //TempData["Error"] = "No existen antecedentes del grado del funcionario";
                    //    TempData["Success"] = "No existen antecedentes del grado del funcionario.";
                    //    return Redirect(Request.UrlReferrer.PathAndQuery);
                    //}
                }
                else if (model.CalidadDescripcion.Contains("TITULAR"))/*valida si es autoridad*/
                {
                    Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Resolucion", new { id = model.CometidoId }) { FileName = "Resolucion Ministerial Exenta" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                    pdf = resultPdf.BuildFile(ControllerContext);
                    //data = GetBynary(pdf);
                    data = _file.BynaryToText(pdf);
                    tipoDoc = 1;
                    Name = "Resolucion Ministerial Exenta nro" + " " + model.CometidoId.ToString() + ".pdf";
                }
                else
                {
                    Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.CometidoId }) { FileName = "Resolucion" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                    pdf = resultPdf.BuildFile(ControllerContext);
                    //data = GetBynary(pdf);
                    data = _file.BynaryToText(pdf);

                    tipoDoc = 1;
                    Name = "Resolucion Cometido nro" + " " + model.CometidoId.ToString() + ".pdf";
                }

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var resolucion = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
                if (resolucion != null)
                {
                    foreach (var res in resolucion)
                    {
                        if (res.TipoDocumentoId == 1)
                            IdDocto = res.DocumentoId;
                    }
                }

                /*se guarda el pdf generado como documento adjunto -- se valida si ya existe el documento para actualizar*/
                if (IdDocto == 0)
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
                }
                else
                {
                    var docOld = _repository.GetById<Documento>(IdDocto);
                    if (docOld.Signed != true)
                    {
                        docOld.File = pdf;
                        docOld.Signed = false;
                        docOld.Texto = data.Text;
                        docOld.Metadata = data.Metadata;
                        docOld.Type = data.Type;
                        _repository.Update(docOld);
                        _repository.Save();
                    }
                }

            }

            return Redirect(Request.UrlReferrer.PathAndQuery);            
        }
        
        //private DTOFileMetadata GetBynary(byte[] pdf)
        //{
        //    var data = new App.Model.DTO.DTOFileMetadata();
        //    var textExtractor = new TextExtractor();
        //    var extract = textExtractor.Extract(pdf);

        //    data.Text = !string.IsNullOrWhiteSpace(extract.Text) ? extract.Text.Trim() : null;
        //    data.Metadata = extract.Metadata.Any() ? string.Join(";", extract.Metadata) : null;
        //    data.Type = extract.ContentType;

        //    return data;
        //}

        [AllowAnonymous]
        public ActionResult Pdf(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();
            if (Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
            {
                model.GeneracionCDP.FirstOrDefault().FechaFirma = DateTime.Now;
                model.GeneracionCDP.FirstOrDefault().PsjFechaFirma = DateTime.Now;

                return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);
                //return View(model);
            }
            else
            {
                model.Dias = (model.Destinos.LastOrDefault().FechaHasta.Date - model.Destinos.FirstOrDefault().FechaInicio.Date).Days + 1;
                model.DiasPlural = "s";
                model.Tiempo = model.Destinos.FirstOrDefault().FechaInicio < DateTime.Now ? "Pasado" : "Futuro";
                model.Anno = DateTime.Now.Year.ToString();
                model.Subscretaria = model.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIO DE TURISMO" : "SUBSECRETARIA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";
                model.FechaResolucion = DateTime.Now;
                model.Firma = false;
                model.NumeroResolucion = model.CometidoId;
                model.Destinos.FirstOrDefault().TotalViaticoPalabras = ExtensionesString.enletras(model.Destinos.FirstOrDefault().TotalViatico.ToString());

                /*se traen los datos de la tabla parrafos*/
                var parrafos = _repository.GetAll<Parrafos>();
                switch (model.IdGrado)
                {
                    case "B":/*Firma Subsecretario*/
                        model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenSubse).FirstOrDefault().ParrafoTexto;
                        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteSubse).FirstOrDefault().ParrafoTexto;
                        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoSubse).FirstOrDefault().ParrafoTexto;
                        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
                        break;
                    case "C": /*firma ministro*/
                        model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenMinistro).FirstOrDefault().ParrafoTexto;
                        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteMinistro).FirstOrDefault().ParrafoTexto;
                        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoMinistro).FirstOrDefault().ParrafoTexto;
                        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
                        break;
                    default:
                        model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Orden).FirstOrDefault().ParrafoTexto;
                        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Firmante).FirstOrDefault().ParrafoTexto;
                        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoFirmante).FirstOrDefault().ParrafoTexto;
                        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistoRAE).FirstOrDefault().ParrafoTexto;
                        var vit = _repository.Get<Parrafos>(c => c.ParrafosId == (int)App.Util.Enum.Firmas.ViaticodeVuelta && c.ParrafoActivo == true).ToList();
                        if (vit.Count > 0)
                            model.ViaticodeVuelta = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.ViaticodeVuelta).FirstOrDefault().ParrafoTexto;
                        else
                            model.ViaticodeVuelta = string.Empty;

                        break;
                }
                                
                /*Se valida que se encuentre en la tarea de Firma electronica para agregar folio y fecha de resolucion*/
                var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId) ?? null;
                if (workflowActual.DefinicionWorkflow.Secuencia == 13 || (workflowActual.DefinicionWorkflow.Secuencia == 13 && workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje))
                {
                    if (model.Folio == null)
                    {
                        #region Folio
                        /*se va a buscar el folio de testing*/
                        DTOFolio folio = new DTOFolio();
                        folio.periodo = DateTime.Now.Year.ToString();
                        folio.solicitante = "Gestion Procesos - Cometidos";/*Sistema que solicita el numero de Folio*/
                        if (model.IdCalidad == 10)
                        {
                            folio.tipodocumento = "RAEX";/*"ORPA";*/
                        }
                        else
                        {
                            switch (model.IdGrado)
                            {
                                case "B":/*Resolución Ministerial Exenta*/
                                    folio.tipodocumento = "RMEX";
                                    break;
                                case "C": /*Resolución Ministerial Exenta*/
                                    folio.tipodocumento = "RMEX";
                                    break;
                                default:
                                    folio.tipodocumento = "RAEX";/*Resolución Administrativa Exenta*/
                                    break;
                            }
                        }


                        //definir url
                        var url = "http://wsfolio.test.economia.cl/api/folio/";

                        //definir cliente http
                        var clientehttp = new WebClient();
                        clientehttp.Headers[HttpRequestHeader.ContentType] = "application/json";

                        //invocar metodo remoto
                        string result = clientehttp.UploadString(url, "POST", JsonConvert.SerializeObject(folio));

                        //convertir resultado en objeto 
                        var obj = JsonConvert.DeserializeObject<App.Model.DTO.DTOFolio>(result);

                        //verificar resultado
                        if (obj.status == "OK")
                        {
                            model.Folio = obj.folio;
                            model.FechaResolucion = DateTime.Now;
                            model.Firma = true;
                            model.TipoActoAdministrativo = "Resolución Administrativa Exenta";

                            _repository.Update(model);
                            _repository.Save();
                        }
                        if (obj.status == "ERROR")
                        {
                            TempData["Error"] = obj.error;
                            //return View(DTOFolio);
                        }
                        #endregion
                    }
                }

                //if (model.CalidadDescripcion.Contains("honorario"))
                //if (model.IdGrado == "0")
                if (model.GradoDescripcion == "0")
                {
                    //return new Rotativa.MVC.ViewAsPdf("Orden", model);
                    return View(model);
                }
                else
                {
                    //return new Rotativa.MVC.ViewAsPdf("Pdf", model);
                    //return new ViewAsPdf("Resolucion", model);
                    return View(model);
                }
            }
        }

        public ActionResult CDPViatico(int Id)
        {
            var model = _repository.GetById<Cometido>(Id);
            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();
            if (Workflow.DefinicionWorkflow.Secuencia == 6 || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje))
            {
                model.GeneracionCDP.FirstOrDefault().FechaFirma = DateTime.Now;
                model.GeneracionCDP.FirstOrDefault().PsjFechaFirma = DateTime.Now;

                //return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);
                return View(model);
            }
            //return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);
            return null;
        }

        public ActionResult CDPPasajes(int Id)
        {
            var model = _repository.GetById<Cometido>(Id);
            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();
            if (Workflow.DefinicionWorkflow.Secuencia == 6 || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje))
            {
                model.GeneracionCDP.FirstOrDefault().FechaFirma = DateTime.Now;
                model.GeneracionCDP.FirstOrDefault().PsjFechaFirma = DateTime.Now;

                //return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);
                return View(model);
            }
            //return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);
            return null;
        }

        public ActionResult Orden(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            model.Dias = (model.Destinos.LastOrDefault().FechaHasta.Date - model.Destinos.FirstOrDefault().FechaInicio.Date).Days + 1;
            model.DiasPlural = "s";
            model.Tiempo = model.Destinos.FirstOrDefault().FechaInicio < DateTime.Now ? "Pasado" : "Futuro";
            model.Anno = DateTime.Now.Year.ToString();
            model.Subscretaria = model.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIO DE TURISMO" : "SUBSECRETARIA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";
            model.FechaResolucion = DateTime.Now;
            model.Firma = false;
            model.NumeroResolucion = model.CometidoId;
            model.Destinos.FirstOrDefault().TotalViaticoPalabras = ExtensionesString.enletras(model.Destinos.FirstOrDefault().TotalViatico.ToString());

            /*se traen los datos de la tabla parrafos*/
            var parrafos = _repository.GetAll<Parrafos>();
            model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Orden).FirstOrDefault().ParrafoTexto;
            model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Firmante).FirstOrDefault().ParrafoTexto;
            model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoFirmante).FirstOrDefault().ParrafoTexto;
            model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistoOP).FirstOrDefault().ParrafoTexto;
            model.DejaseConstancia = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.DejaseConstancia).FirstOrDefault().ParrafoTexto;

            //switch (model.IdGrado)
            //{
            //    case "B":/*Firma Subsecretario*/
            //        model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenSubse).FirstOrDefault().ParrafoTexto;
            //        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteSubse).FirstOrDefault().ParrafoTexto;
            //        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoSubse).FirstOrDefault().ParrafoTexto;
            //        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
            //        break;
            //    case "C": /*firma ministro*/
            //        model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenMinistro).FirstOrDefault().ParrafoTexto;
            //        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteMinistro).FirstOrDefault().ParrafoTexto;
            //        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoMinistro).FirstOrDefault().ParrafoTexto;
            //        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
            //        break;
            //    default:
            //        model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Orden).FirstOrDefault().ParrafoTexto;
            //        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Firmante).FirstOrDefault().ParrafoTexto;
            //        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoFirmante).FirstOrDefault().ParrafoTexto;                    
            //        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistoOP).FirstOrDefault().ParrafoTexto;
            //        model.DejaseConstancia = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.DejaseConstancia).FirstOrDefault().ParrafoTexto;
            //        break;
            //}

            /*Se valida que se encuentre en la tarea de Firma electronica para agregar folio y fecha de resolucion*/
            var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId) ?? null;
            if (workflowActual.DefinicionWorkflow.Secuencia == 13)
            {
                if (model.Folio == null)
                {
                    #region Folio
                    /*se va a buscar el folio de testing*/
                    DTOFolio folio = new DTOFolio();
                    folio.periodo = DateTime.Now.Year.ToString();
                    folio.solicitante = "Gestion Procesos - Cometidos";/*Sistema que solicita el numero de Folio*/
                    folio.tipodocumento = "OP";
                    //if (model.IdCalidad == 10)
                    //{
                    //    folio.tipodocumento = "RAEX";/*"ORPA";*/
                    //}
                    //else
                    //{
                    //    switch (model.IdGrado)
                    //    {
                    //        case "B":/*Resolución Ministerial Exenta*/
                    //            folio.tipodocumento = "RMEX";
                    //            break;
                    //        case "C": /*Resolución Ministerial Exenta*/
                    //            folio.tipodocumento = "RMEX";
                    //            break;
                    //        default:
                    //            /*Resolución Administrativa Exenta*/
                    //            break;
                    //    }
                    //}

                    //definir url
                    var url = "http://wsfolio.test.economia.cl/api/folio/";

                    //definir cliente http
                    var clientehttp = new WebClient();
                    clientehttp.Headers[HttpRequestHeader.ContentType] = "application/json";

                    //invocar metodo remoto
                    string result = clientehttp.UploadString(url, "POST", JsonConvert.SerializeObject(folio));

                    //convertir resultado en objeto 
                    var obj = JsonConvert.DeserializeObject<App.Model.DTO.DTOFolio>(result);

                    //verificar resultado
                    if (obj.status == "OK")
                    {
                        model.Folio = obj.folio;
                        model.FechaResolucion = DateTime.Now;
                        model.Firma = true;
                        model.TipoActoAdministrativo = "Orden de Pago"; 

                        _repository.Update(model);
                        _repository.Save();
                    }
                    if (obj.status == "ERROR")
                    {
                        TempData["Error"] = obj.error;
                        //return View(DTOFolio);
                    }
                    #endregion
                }
            }


            return View(model);
        }

        public ActionResult Resolucion(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            model.Dias = (model.Destinos.LastOrDefault().FechaHasta.Date - model.Destinos.FirstOrDefault().FechaInicio.Date).Days + 1;
            model.DiasPlural = "s";
            model.Tiempo = model.Destinos.FirstOrDefault().FechaInicio < DateTime.Now ? "Pasado" : "Futuro";
            model.Anno = DateTime.Now.Year.ToString();
            model.Subscretaria = model.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIO DE TURISMO" : "SUBSECRETARIA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";
            model.FechaResolucion = DateTime.Now;
            model.Firma = false;
            model.NumeroResolucion = model.CometidoId;
            model.Destinos.FirstOrDefault().TotalViaticoPalabras = ExtensionesString.enletras(model.Destinos.FirstOrDefault().TotalViatico.ToString());

            /*se traen los datos de la tabla parrafos*/
            var parrafos = _repository.GetAll<Parrafos>();
            switch (model.IdGrado)
            {
                case "B":/*Firma Subsecretario*/
                    model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenSubse).FirstOrDefault().ParrafoTexto;
                    model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteSubse).FirstOrDefault().ParrafoTexto;
                    model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoSubse).FirstOrDefault().ParrafoTexto;
                    model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
                    break;
                case "C": /*firma ministro*/
                    model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenMinistro).FirstOrDefault().ParrafoTexto;
                    model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteMinistro).FirstOrDefault().ParrafoTexto;
                    model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoMinistro).FirstOrDefault().ParrafoTexto;
                    model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
                    break;
                default:
                    model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Orden).FirstOrDefault().ParrafoTexto;
                    model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Firmante).FirstOrDefault().ParrafoTexto;
                    model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoFirmante).FirstOrDefault().ParrafoTexto;
                    model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistoRAE).FirstOrDefault().ParrafoTexto;
                    var vit = _repository.Get<Parrafos>(c => c.ParrafosId == (int)App.Util.Enum.Firmas.ViaticodeVuelta && c.ParrafoActivo == true);
                    if (vit != null)
                        model.ViaticodeVuelta = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.ViaticodeVuelta).FirstOrDefault().ParrafoTexto;
                    else
                        model.ViaticodeVuelta = string.Empty;
                    break;
            }

            /*Se valida que se encuentre en la tarea de Firma electronica para agregar folio y fecha de resolucion*/
            var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId) ?? null;
            //if (workflowActual.DefinicionWorkflow.Secuencia == 8)
            //{
            //    if (model.Folio == null)
            //    {
            //        #region Folio
            //        /*se va a buscar el folio de testing*/
            //        DTOFolio folio = new DTOFolio();
            //        folio.periodo = DateTime.Now.Year.ToString();
            //        folio.solicitante = "Gestion Procesos - Cometidos";/*Sistema que solicita el numero de Folio*/
            //        if (model.IdCalidad == 10)
            //        {
            //            folio.tipodocumento = "RAEX";/*"ORPA";*/
            //        }
            //        else
            //        {
            //            switch (model.IdGrado)
            //            {
            //                case "B":/*Resolución Ministerial Exenta*/
            //                    folio.tipodocumento = "RMEX";
            //                    break;
            //                case "C": /*Resolución Ministerial Exenta*/
            //                    folio.tipodocumento = "RMEX";
            //                    break;
            //                default:
            //                    folio.tipodocumento = "RAEX";/*Resolución Administrativa Exenta*/
            //                    break;
            //            }
            //        }


            //        //definir url
            //        var url = "http://wsfolio.test.economia.cl/api/folio/";

            //        //definir cliente http
            //        var clientehttp = new WebClient();
            //        clientehttp.Headers[HttpRequestHeader.ContentType] = "application/json";

            //        //invocar metodo remoto
            //        string result = clientehttp.UploadString(url, "POST", JsonConvert.SerializeObject(folio));

            //        //convertir resultado en objeto 
            //        var obj = JsonConvert.DeserializeObject<App.App.Model.DTO.DTOFolio>(result);

            //        //verificar resultado
            //        if (obj.status == "OK")
            //        {
            //            model.Folio = obj.folio;
            //            model.FechaResolucion = DateTime.Now;
            //            model.Firma = true;

            //            _repository.Update(model);
            //            _repository.Save();
            //        }
            //        if (obj.status == "ERROR")
            //        {
            //            TempData["Error"] = obj.error;
            //        }
            //        #endregion
            //    }
            //}

            if (workflowActual.DefinicionWorkflow.Secuencia == 13 || workflowActual.DefinicionWorkflow.Secuencia == 14 || workflowActual.DefinicionWorkflow.Secuencia == 15 && workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
            {
                if (model.Folio == null)
                {
                    #region Folio
                    /*se va a buscar el folio de testing*/
                    DTOFolio folio = new DTOFolio();
                    folio.periodo = DateTime.Now.Year.ToString();
                    folio.solicitante = "Gestion Procesos - Cometidos";/*Sistema que solicita el numero de Folio*/
                    if (model.IdCalidad == 10)
                    {
                        folio.tipodocumento = "RAEX";/*"ORPA";*/
                    }
                    else
                    {
                        switch (model.IdGrado)
                        {
                            case "B":/*Resolución Ministerial Exenta*/
                                folio.tipodocumento = "RMEX";
                                break;
                            case "C": /*Resolución Ministerial Exenta*/
                                folio.tipodocumento = "RMEX";
                                break;
                            default:
                                folio.tipodocumento = "RAEX";/*Resolución Administrativa Exenta*/
                                break;
                        }
                    }


                    //definir url
                    var url = "http://wsfolio.test.economia.cl/api/folio/";

                    //definir cliente http
                    var clientehttp = new WebClient();
                    clientehttp.Headers[HttpRequestHeader.ContentType] = "application/json";

                    //invocar metodo remoto
                    string result = clientehttp.UploadString(url, "POST", JsonConvert.SerializeObject(folio));

                    //convertir resultado en objeto 
                    var obj = JsonConvert.DeserializeObject<App.Model.DTO.DTOFolio>(result);

                    //verificar resultado
                    if (obj.status == "OK")
                    {
                        model.Folio = obj.folio;
                        model.FechaResolucion = DateTime.Now;
                        model.Firma = true;
                        model.TipoActoAdministrativo = "Resolución Ministerial Exenta";

                        _repository.Update(model);
                        _repository.Save();
                    }
                    if (obj.status == "ERROR")
                    {
                        TempData["Error"] = obj.error;
                    }
                    #endregion
                }
            }

            if (model.GradoDescripcion == "0")
            {
                return View(model);
            }
            else
            {
                return View(model);
            }            
    }

        public ActionResult Anular(int id)
        {
            var model = _repository.GetById<Workflow>(id);
            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Anular(Workflow model)
        //{
        //    model.Email = UserExtended.Email(User);

        //    if (ModelState.IsValid)
        //    {
        //        var _useCaseInteractor = new UseCaseInteractorCustom(_repository, _email, _sigper);
        //        ResponseMessage _UseCaseResponseMessageDelete = new ResponseMessage();

        //        var workflow = _repository.Get<Workflow>(c => c.WorkflowId == model.WorkflowId).FirstOrDefault();
        //        var Entity = _repository.Get<DefinicionWorkflow>(d => d.DefinicionWorkflowId == workflow.DefinicionWorkflowId).FirstOrDefault().EntidadId;
        //        if (Entity == 8)/*Se eliminan datos del destino, luego el cometido*/
        //        {
        //            var cometido = _repository.Get<Cometido>(c => c.CometidoId == workflow.EntityId.Value).FirstOrDefault();
        //            var destinos = _repository.Get<Destinos>(c => c.CometidoId == cometido.CometidoId);
        //            var cdp = _repository.Get<GeneracionCDP>(c => c.CometidoId == cometido.CometidoId);
        //            if (destinos != null)
        //            {
        //                foreach (var des in destinos)
        //                {
        //                    _UseCaseResponseMessageDelete = _useCaseInteractor.DestinosAnular(des.DestinoId);
        //                }
        //            }

        //            if (cdp != null)
        //            {
        //                foreach (var c in cdp)
        //                {
        //                    _UseCaseResponseMessageDelete = _useCaseInteractor.GeneracionCDPAnular(c.GeneracionCDPId);
        //                }
        //            }



        //            _UseCaseResponseMessageDelete = _useCaseInteractor.CometidoAnular(cometido.CometidoId);
        //        }

        //        /*se elimina datos del proceso*/
        //        //var _UseCaseResponseMessage = _useCaseInteractorCore.WorkflowArchive(model);
        //        //if (_UseCaseResponseMessage.IsValid && _UseCaseResponseMessageDelete.IsValid)
        //        //{
        //        //    TempData["Success"] = "Operación terminada correctamente.";
        //        //    return RedirectToAction("Index", "Workflow");
        //        //}
        //        //else
        //        //    TempData["Error"] = _UseCaseResponseMessage.Errors;
        //    }

        //    return View(model);
        //}

        public ActionResult Search()
        {
            var model = new DTOFilterCometido()
            {
                Select = _repository.GetAll<DefinicionProceso>().OrderBy(q => q.Nombre).ToList().Select(q => new DTOSelect() { Id = q.DefinicionProcesoId, Descripcion = q.Nombre, Selected = false }),
                Result = _repository.Get<Cometido>().ToList()
            };

            //foreach (var res in model.Result)//.Where(p => p.DefinicionProcesoId == 13))
            //{
            //    switch (res.CometidoId)
            //    {
            //        case 13:
            //            var com = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId);
            //            if (com.Count() > 0)
            //            {
            //                model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId.ToString();
            //            }
            //            break;
            //        case 10:
            //            var come = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId);
            //            if (come.Count() > 0)
            //            {
            //                model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId.ToString();
            //            }
            //            break;
            //    }
            //}

            return View(model);
        }

        public ActionResult ResultSearch(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            return View(model);
        }

        public FileResult Download()
        {
            var result = _repository.GetAll<Cometido>();

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\COMETIDOS.xlsx");
            var fileInfo = new FileInfo(file);
            var excelPackageCometidos = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackageCometidos.Workbook.Worksheets[1];
            foreach (var cometido in result.ToList())
            {
                fila++;
                worksheet.Cells[fila, 1].Value = cometido.Workflow.Terminada ? "Terminado" : cometido.Workflow.Anulada ? "Anulado" : "En Curso";
                worksheet.Cells[fila, 2].Value = cometido.UnidadDescripcion.Contains("Turismo") ? "Turismo" : "Economía";
                worksheet.Cells[fila, 3].Value = cometido.CometidoId.ToString();
                worksheet.Cells[fila, 4].Value = cometido.Nombre;
                worksheet.Cells[fila, 5].Value = cometido.UnidadDescripcion;
                worksheet.Cells[fila, 6].Value = cometido.Destinos.Any() ? "SI" : "NO";
                worksheet.Cells[fila, 7].Value = cometido.FechaSolicitud.ToString();
                worksheet.Cells[fila, 8].Value = cometido.Destinos.Any() ? cometido.Destinos.FirstOrDefault().FechaInicio.ToString() : "S/A";
                worksheet.Cells[fila, 9].Value = cometido.Destinos.Any() ? cometido.Destinos.LastOrDefault().FechaHasta.ToString() : "S/A";
                //worksheet.Cells[fila, 10].Value = cometido.Proceso.DefinicionProceso != null ? cometido.Proceso.DefinicionProceso.Nombre : string.Empty;

                var workflow = _repository.GetAll<Workflow>().Where(w => w.ProcesoId == cometido.ProcesoId);

                worksheet.Cells[fila, 10].Value = workflow.LastOrDefault().Email;
                worksheet.Cells[fila, 11].Value = workflow.LastOrDefault().FechaCreacion.ToString();
            }

            return File(excelPackageCometidos.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "RPTSegGP_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
        }

        public ActionResult SeguimientoGP()
        {
            var model = new DTOFilterCometido();

            List<SelectListItem> Estado = new List<SelectListItem>
            {
            new SelectListItem {Text = "En Curso", Value = "1"},
            new SelectListItem {Text = "Terminado", Value = "2"},
            new SelectListItem {Text = "Anulado", Value = "3"},
            };

            ViewBag.Estado = new SelectList(Estado, "Value", "Text");
            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdUnidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes").Where(c => c.Text.Contains("ECONOMIA"));
            ViewBag.Destino = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            return View(model);
        }

        [HttpPost]
        public ActionResult SeguimientoGP(DTOFilterCometido model)
        {
            var predicate = PredicateBuilder.True<Cometido>();

            if (ModelState.IsValid)
            {
                if (model.Estado.HasValue)
                {
                    if (model.Estado == 1)
                        predicate = predicate.And(q => q.Proceso.Terminada == false && q.Proceso.Anulada == false);
                    if (model.Estado == 2)
                        predicate = predicate.And(q => q.Proceso.Anulada == true);
                    if (model.Estado == 3)
                        predicate = predicate.And(q => q.Proceso.Terminada == true);
                }

                    if (model.NombreId.HasValue)
                    predicate = predicate.And(q => q.NombreId == model.NombreId);

                if (model.IdUnidad.HasValue)
                    predicate = predicate.And(q => q.IdUnidad == model.IdUnidad);

                if (model.Destino.HasValue)
                    predicate = predicate.And(q => q.Destinos.FirstOrDefault().IdRegion == model.Destino.Value.ToString());

                if (model.FechaInicio.HasValue)
                    predicate = predicate.And(q =>
                        q.Destinos.FirstOrDefault().FechaInicio.Year >= model.FechaInicio.Value.Year &&
                        q.Destinos.FirstOrDefault().FechaInicio.Month >= model.FechaInicio.Value.Month &&
                        q.Destinos.FirstOrDefault().FechaInicio.Day >= model.FechaInicio.Value.Day);

                if (model.FechaTermino.HasValue)
                    predicate = predicate.And(q =>
                        q.Destinos.LastOrDefault().FechaInicio.Year <= model.FechaTermino.Value.Year &&
                        q.Destinos.LastOrDefault().FechaInicio.Month <= model.FechaTermino.Value.Month &&
                        q.Destinos.LastOrDefault().FechaInicio.Day <= model.FechaTermino.Value.Day);

                var CometidoId = model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
                if (CometidoId.Any())
                    predicate = predicate.And(q => CometidoId.Contains(q.CometidoId));

                model.Result = _repository.Get(predicate);
            }

            List<SelectListItem> Estado = new List<SelectListItem>
            {
                new SelectListItem {Text = "En Curso", Value = "1"},
                new SelectListItem {Text = "Terminado", Value = "2"},
                new SelectListItem {Text = "Anulado", Value = "3"},
            };

            ViewBag.Estado = new SelectList(Estado, "Value", "Text");
            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdUnidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes").Where(c => c.Text.Contains("ECONOMIA"));
            ViewBag.Destino = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            return View(model);
        }

        public FileResult DownloadGP()
        {
            var result = _repository.GetAll<Cometido>();

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\SeguimientoGP.xlsx");
            var fileInfo = new FileInfo(file);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackageSeguimientoGP = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackageSeguimientoGP.Workbook.Worksheets[0];
            foreach (var cometido in result.ToList())
            {
                var workflow = _repository.GetAll<Workflow>().Where(w => w.ProcesoId == cometido.ProcesoId);
                var destino = _repository.GetAll<Destinos>().Where(d => d.CometidoId == cometido.CometidoId).ToList();

                if (destino.Count > 0)
                {
                    fila++;
                    worksheet.Cells[fila, 1].Value = cometido.Proceso.Terminada == false && cometido.Proceso.Anulada == false ? "En Curso" : cometido.Workflow.Terminada == true ? "Terminada" : "Anulada";
                    worksheet.Cells[fila, 2].Value = cometido.UnidadDescripcion.Contains("Turismo") ? "Turismo" : "Economía";
                    worksheet.Cells[fila, 3].Value = cometido.CometidoId.ToString();
                    worksheet.Cells[fila, 4].Value = cometido.Nombre != null ? cometido.Nombre : "S/A";
                    worksheet.Cells[fila, 5].Value = cometido.UnidadDescripcion;
                    worksheet.Cells[fila, 6].Value = destino.FirstOrDefault().RegionDescripcion != null ? destino.FirstOrDefault().RegionDescripcion : "S/A";
                    worksheet.Cells[fila, 7].Value = cometido.FechaSolicitud.ToShortDateString();
                    worksheet.Cells[fila, 8].Value = destino.Any() ? destino.FirstOrDefault().FechaInicio.ToShortDateString() : "S/A";
                    worksheet.Cells[fila, 9].Value = destino.Any() ? destino.LastOrDefault().FechaHasta.ToShortDateString() : "S/A";
                    worksheet.Cells[fila, 10].Value = workflow.LastOrDefault().Email;
                    worksheet.Cells[fila, 11].Value = workflow.LastOrDefault().FechaCreacion.ToShortDateString();
                }
            }

            return File(excelPackageSeguimientoGP.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "rptSeguimientoGP_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
        }

        public FileResult Caigg()
        {
            var cometido = _repository.GetAll<Cometido>();

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\CAIGG.xlsx");
            var fileInfo = new FileInfo(file);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackageCaigg = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackageCaigg.Workbook.Worksheets[0];
            foreach (var com in cometido.OrderByDescending(r => r.CometidoId).ToList())
            {
                var workflow = _repository.GetAll<Workflow>().Where(w => w.ProcesoId == com.ProcesoId);
                var pasaje = _repository.Get<Pasaje>().Where(p => p.ProcesoId == com.ProcesoId).ToList();
                var cotizacion = _repository.GetAll<Cotizacion>().Where(p =>p.PasajeId == pasaje.FirstOrDefault().PasajeId);

                if(pasaje.Count > 0)
                {
                    for (var pas = 0; pas < pasaje.Count + 1 ; pas++)//foreach (var pas in pasaje)
                    {
                        fila++;
                        worksheet.Cells[fila, 1].Value = com.UnidadDescripcion.Contains("Turismo") ? "Turismo" : "Economía";
                        worksheet.Cells[fila, 2].Value = workflow.FirstOrDefault().Proceso.DefinicionProceso.Nombre;
                        worksheet.Cells[fila, 3].Value = com.UnidadDescripcion.Contains("Sere") ? com.UnidadDescripcion : "Nivel Central";
                        worksheet.Cells[fila, 4].Value = com.TipoActoAdministrativo != null ? com.TipoActoAdministrativo.ToString() : "S/A"; /*Tipo Acto Administrativo*/
                        worksheet.Cells[fila, 5].Value = com.Folio != null ? com.Folio.ToString() : "S/A"; /*Nro Acto Administrativo*/
                        worksheet.Cells[fila, 6].Value = com.CometidoId.ToString();
                        worksheet.Cells[fila, 7].Value = com.Nombre;

                        var desPasaje = _repository.Get<DestinosPasajes>().Where(c => c.PasajeId == pasaje.FirstOrDefault().PasajeId).ToList();
                        if (desPasaje.Count > 0)
                        {
                            foreach(var p in desPasaje)
                            {
                                if ((pas % 2) == 0)
                                {
                                    worksheet.Cells[fila, 8].Value = p.RegionDescripcion.Trim(); //com.Destinos.Any() ? com.Destinos.FirstOrDefault().ComunaDescripcion : "S/A";
                                    worksheet.Cells[fila, 15].Value = p.FechaIda.ToShortDateString(); //com.Destinos.Any() ? com.Destinos.FirstOrDefault().FechaInicio.ToString() : "S/A";
                                    worksheet.Cells[fila, 16].Value = p.FechaVuelta.ToShortDateString();// com.Destinos.Any() ? com.Destinos.LastOrDefault().FechaHasta.ToString() : "S/A";
                                    worksheet.Cells[fila, 20].Value = ((com.FechaSolicitud - p.FechaIda).Days + 1).ToString();
                                }
                                else
                                {
                                    worksheet.Cells[fila, 8].Value = p.OrigenRegionDescripcion.Trim();
                                    worksheet.Cells[fila, 15].Value = p.FechaIda.ToShortDateString();
                                    worksheet.Cells[fila, 16].Value = p.FechaVuelta.ToShortDateString();
                                    worksheet.Cells[fila, 20].Value = "0";
                                }
                            }
                        }
                        
                        worksheet.Cells[fila, 9].Value = com.CargoDescripcion.Trim() == "Ministro" || com.CargoDescripcion.Trim() == "Subsecretario" ? com.CargoDescripcion.Trim() : "Otro";
                        worksheet.Cells[fila, 10].Value = com.CargoDescripcion;
                        worksheet.Cells[fila, 11].Value = com.ReqPasajeAereo == true ? "Nacional" : "N/A";
                        worksheet.Cells[fila, 12].Value = "N/A";
                        worksheet.Cells[fila, 13].Value = "N/A";
                        if (pasaje.Count() > 0)
                        {
                            worksheet.Cells[fila, 14].Value = pasaje.FirstOrDefault().Cotizacion.Count() >= 2 ? "SI" : "NO";
                            worksheet.Cells[fila, 17].Value = cotizacion.Count() > 0 ? cotizacion.FirstOrDefault().ValorPasaje.ToString() : "S/A";
                        }
                        else
                        {
                            worksheet.Cells[fila, 14].Value = "N/A";
                            worksheet.Cells[fila, 17].Value = "0";
                        }                        

                        worksheet.Cells[fila, 18].Value = com.TotalViatico != null ? com.TotalViatico.ToString() : "0";
                        worksheet.Cells[fila, 19].Value = com.FechaSolicitud.ToShortDateString();
                        worksheet.Cells[fila, 21].Value = com.CometidoDescripcion;

                        var tarea = workflow.LastOrDefault().DefinicionWorkflow;
                        if (tarea.Secuencia < 9)
                            worksheet.Cells[fila, 22].Value = "Solicitado";
                        else if (tarea.Secuencia >= 9 && tarea.Secuencia < 17)
                            worksheet.Cells[fila, 22].Value = "Comprometido";
                        else if (tarea.Secuencia >= 17 && tarea.Secuencia < 20)
                            worksheet.Cells[fila, 22].Value = "Devengado";
                        else if (tarea.Secuencia >= 19)
                            worksheet.Cells[fila, 22].Value = "Pagado";
                    }
                }
                else 
                {
                    fila++;
                    worksheet.Cells[fila, 1].Value = com.UnidadDescripcion.Contains("Turismo") ? "Turismo" : "Economía";
                    worksheet.Cells[fila, 2].Value = workflow.FirstOrDefault().Proceso.DefinicionProceso.Nombre;
                    worksheet.Cells[fila, 3].Value = com.UnidadDescripcion.Contains("Sere") ? com.UnidadDescripcion : "Nivel Central";
                    worksheet.Cells[fila, 4].Value = com.TipoActoAdministrativo != null ? com.TipoActoAdministrativo.ToString() : "S/A"; /*Tipo Acto Administrativo*/
                    worksheet.Cells[fila, 5].Value = com.Folio != null ? com.Folio.ToString() : "S/A"; /*Nro Acto Administrativo*/
                    worksheet.Cells[fila, 6].Value = com.CometidoId.ToString();
                    worksheet.Cells[fila, 7].Value = com.Nombre;
                    
                    worksheet.Cells[fila, 8].Value = "S/A";

                    worksheet.Cells[fila, 9].Value = com.CargoDescripcion.Trim() == "Ministro" || com.CargoDescripcion.Trim() == "Subsecretario" ? com.CargoDescripcion.Trim() : "Otro";
                    worksheet.Cells[fila, 10].Value = com.CargoDescripcion;
                    worksheet.Cells[fila, 11].Value = com.ReqPasajeAereo == true ? "Nacional" : "N/A";
                    worksheet.Cells[fila, 12].Value = "N/A";
                    worksheet.Cells[fila, 13].Value = "N/A";

                    worksheet.Cells[fila, 14].Value = "N/A";
                    worksheet.Cells[fila, 15].Value = "N/A";
                    worksheet.Cells[fila, 16].Value = "N/A";
                    worksheet.Cells[fila, 17].Value = "0";

                    worksheet.Cells[fila, 18].Value = com.TotalViatico != null ? com.TotalViatico.ToString() : "0";
                    worksheet.Cells[fila, 19].Value = com.FechaSolicitud.ToShortDateString();

                    worksheet.Cells[fila, 20].Value = "0";

                    var tarea = workflow.LastOrDefault().DefinicionWorkflow;
                    if (tarea.Secuencia < 9)
                        worksheet.Cells[fila, 22].Value = "Solicitado";
                    else if (tarea.Secuencia >= 9 && tarea.Secuencia < 17)
                        worksheet.Cells[fila, 22].Value = "Comprometido";
                    else if (tarea.Secuencia >= 17 && tarea.Secuencia < 20)
                        worksheet.Cells[fila, 22].Value = "Devengado";
                    else if (tarea.Secuencia >= 19)
                        worksheet.Cells[fila, 22].Value = "Pagado";
                }
            }

            return File(excelPackageCaigg.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "rptCaigg_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
        }

        public ActionResult SeguimientoUnidades()
        {
            var model = new DTOFilterCometido();
            //ViewBag.Ejecutor = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.Ejecutor = new SelectList(_repository.GetAll<DefinicionWorkflow>().Where(c => c.DefinicionProcesoId == 13 && c.NombreUsuario != null).ToList(), "NombreUsuario", "NombreUsuario");
            ViewBag.Ejecutor = new SelectList(_repository.GetAll<Workflow>().Where(c => c.Proceso.DefinicionProcesoId == 13 && c.Email != null).GroupBy(c => c.Email).Select(x =>x.First()).ToList(), "Email", "Email");
            return View(model);
        }

        [HttpPost]
        public ActionResult SeguimientoUnidades(DTOFilterCometido model)
        {
            var predicate = PredicateBuilder.True<Cometido>();

            if (ModelState.IsValid)
            {
                if (model.ID != 0)
                    predicate = predicate.And(q => q.CometidoId == model.ID);

                if (!string.IsNullOrWhiteSpace(model.Ejecutor))
                {
                    //int valor = model.Ejecutor.IndexOf("(",0);
                    //int valor2 = model.Ejecutor.IndexOf(")",0);
                    //var mail = model.Ejecutor.Substring(valor + 1, (valor2-valor) - 1);
                    var mail = model.Ejecutor;

                    //var mail = _sigper.GetUserByRut(int.Parse(model.Ejecutor)).Funcionario.Rh_Mail.Trim();
                    predicate = predicate.And(q => q.Proceso.Workflows.Any(p => p.Email == mail));
                }
                    

                if (model.FechaSolicitud.HasValue)
                    predicate = predicate.And(q =>
                        q.FechaSolicitud.Year == model.FechaSolicitud.Value.Year &&
                        q.FechaSolicitud.Month == model.FechaSolicitud.Value.Month &&
                        q.FechaSolicitud.Day == model.FechaSolicitud.Value.Day);

                if (model.FechaInicio.HasValue)
                    predicate = predicate.And(q =>
                        q.Destinos.FirstOrDefault().FechaInicio.Year == model.FechaInicio.Value.Year &&
                        q.Destinos.FirstOrDefault().FechaInicio.Month == model.FechaInicio.Value.Month &&
                        q.Destinos.FirstOrDefault().FechaInicio.Day == model.FechaInicio.Value.Day);

                if (model.FechaTermino.HasValue)
                    predicate = predicate.And(q =>
                        q.Destinos.LastOrDefault().FechaInicio.Year == model.FechaTermino.Value.Year &&
                        q.Destinos.LastOrDefault().FechaInicio.Month == model.FechaTermino.Value.Month &&
                        q.Destinos.LastOrDefault().FechaInicio.Day == model.FechaTermino.Value.Day);

                var CometidoId = model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
                if (CometidoId.Any())
                    predicate = predicate.And(q => CometidoId.Contains(q.CometidoId));

                model.Result = _repository.Get(predicate);
            }

            //foreach (var res in model.Result)//.Where(p => p.DefinicionProcesoId == 13))
            //{
            //    var work = _repository.Get<Workflow>(c => c.ProcesoId == model.Result.FirstOrDefault().ProcesoId.Value).ToList();
            //    if(work != null)
            //    {
            //        foreach(var w in work)
            //        {
            //            res.Workflow.Pl_UndDes = _sigper.GetUserByEmail(w.Email).Unidad.Pl_UndDes.Trim();
            //        }
            //    }
            //}

            


            ViewBag.Ejecutor = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            return View(model);
        }

        public FileResult DownloadSeguimiento()
        {
            var result = _repository.GetAll<Cometido>();

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\SeguimientoUnidades.xlsx");
            var fileInfo = new FileInfo(file);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackageSeguimientoUnidades = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackageSeguimientoUnidades.Workbook.Worksheets[0];
            foreach (var cometido in result.ToList().OrderByDescending(c => c.CometidoId))
            {
                var workflow = _repository.GetAll<Workflow>().Where(w => w.ProcesoId == cometido.ProcesoId);
                var destino = _repository.GetAll<Destinos>().Where(d => d.CometidoId == cometido.CometidoId).ToList();

                fila++;
                foreach (var w in workflow)
                {
                    worksheet.Cells[fila, 1].Value = cometido.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIA DE TURISMO" : "SUBSECRETARIA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";
                    worksheet.Cells[fila, 2].Value = cometido.CometidoId.ToString();
                    worksheet.Cells[fila, 3].Value = cometido.Nombre != null ? cometido.Nombre.Trim() : "S/A";
                    worksheet.Cells[fila, 4].Value = cometido.FechaSolicitud.ToShortDateString();
                    /*datos desde core workflow*/

                    worksheet.Cells[fila, 5].Value = w.Email != null ? w.Email.Trim() : "S/A";
                    worksheet.Cells[fila, 6].Value = w.Pl_UndDes != null ? w.Pl_UndDes.Trim() : "";
                    worksheet.Cells[fila, 7].Value = w.FechaCreacion.ToShortDateString().Trim();
                    worksheet.Cells[fila, 8].Value = w.FechaTermino.HasValue ? w.FechaTermino.Value.ToShortDateString().Trim() : "Pendiente";
                    if (w.FechaTermino.HasValue)
                    {
                        worksheet.Cells[fila, 9].Value = w.FechaCreacion.Day - w.FechaTermino.Value.Day;
                    }
                    else
                    {
                        var hoy = DateTime.Now;
                        worksheet.Cells[fila, 9].Value = w.FechaCreacion.Day - hoy.Day;
                    }
                    //fila++;
                }
            }
            

            return File(excelPackageSeguimientoUnidades.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "rptSeguimientoUnidades_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
        }

        public ActionResult SolicitudesTransparencia()
        {
            var model = new DTOFilterCometido();
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdUnidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes").Where(c => c.Text.Contains("ECONOMIA"));            
            return View(model);
        }

        [HttpPost]
        public ActionResult SolicitudesTransparencia(DTOFilterCometido model)
        {
            var predicate = PredicateBuilder.True<Cometido>();


            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdUnidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes").Where(c => c.Text.Contains("ECONOMIA"));
            return View(model);
        }

        public FileResult AjusteAsistencia()
        {
            var result = _repository.GetAll<Cometido>();

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\AjusteAsistencia.xlsx");
            var fileInfo = new FileInfo(file);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackageAjusteAsistencia = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackageAjusteAsistencia.Workbook.Worksheets[0];
            foreach (var cometido in result.ToList().OrderByDescending(c =>c.CometidoId))
            {
                //var workflow = _repository.GetAll<Workflow>().Where(w => w.ProcesoId == cometido.ProcesoId);
                var destino = _repository.GetAll<Destinos>().Where(d => d.CometidoId == cometido.CometidoId).ToList();

                if (destino.Count > 0)
                {
                    fila++;
                    worksheet.Cells[fila, 1].Value = cometido.Rut + "-" + cometido.DV.ToString();
                    worksheet.Cells[fila, 2].Value = destino.Any() ? ((destino.LastOrDefault().FechaHasta.Day - destino.FirstOrDefault().FechaInicio.Day) + 1).ToString() : "S/A";
                    worksheet.Cells[fila, 3].Value = destino.Any() ? destino.FirstOrDefault().FechaInicio.ToShortDateString() : "S/A";
                    worksheet.Cells[fila, 4].Value = destino.Any() ? destino.LastOrDefault().FechaHasta.ToShortDateString() : "S/A";
                    worksheet.Cells[fila, 5].Value = destino.Any() ? destino.FirstOrDefault().FechaInicio.Hour.ToString() : "S/A";
                    worksheet.Cells[fila, 6].Value = destino.Any() ? destino.LastOrDefault().FechaHasta.Hour.ToString() : "S/A";
                    worksheet.Cells[fila, 7].Value = cometido.Nombre != null ? cometido.Nombre.Trim() : "S/A";
                }
            }

            return File(excelPackageAjusteAsistencia.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "rptAjusteAsistencia_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
        }

        public FileResult SolicitudTransparencia()
        {
            var result = _repository.GetAll<Cometido>();

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\SolicitudTransparencia.xlsx");
            var fileInfo = new FileInfo(file);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackageTransparencia = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackageTransparencia.Workbook.Worksheets[0];
            fila++;
            foreach (var cometido in result.ToList().OrderByDescending(c => c.CometidoId))
            {
                var workflow = _repository.GetAll<Workflow>().Where(w => w.ProcesoId == cometido.ProcesoId);
                var destino = _repository.GetAll<Destinos>().Where(d => d.CometidoId == cometido.CometidoId).ToList();

                if (destino.Count > 0)
                {
                    fila++;
                    worksheet.Cells[fila, 1].Value = cometido.UnidadDescripcion.Contains("Turismo") ? "Turismo" : "Economía"; 
                    worksheet.Cells[fila, 2].Value = cometido.CometidoId.ToString();
                    worksheet.Cells[fila, 3].Value = cometido.NombreCometido;
                    worksheet.Cells[fila, 4].Value = cometido.FechaSolicitud.ToShortDateString();
                    worksheet.Cells[fila, 5].Value = cometido.Nombre != null ? cometido.Nombre.Trim() : "S/A";
                    worksheet.Cells[fila, 6].Value = cometido.Proceso.Terminada == false && cometido.Proceso.Anulada == false ? "En Proceso" : cometido.Workflow.Terminada == true ? "Terminada" : "Anulada";
                    worksheet.Cells[fila, 7].Value = cometido.Folio != null ? cometido.Folio.ToString() : "S/A";
                    worksheet.Cells[fila, 8].Value = cometido.TipoActoAdministrativo != null ? cometido.TipoActoAdministrativo.ToString() : "S/A";
                    worksheet.Cells[fila, 9].Value = cometido.Nombre != null ? cometido.Nombre.Trim() : "S/A";
                    worksheet.Cells[fila, 10].Value = cometido.EstamentoDescripcion;
                    worksheet.Cells[fila, 11].Value = cometido.Rut + "-" + cometido.DV;
                    worksheet.Cells[fila, 12].Value = cometido.UnidadDescripcion;
                    worksheet.Cells[fila, 13].Value = destino.FirstOrDefault().RegionDescripcion != null ? destino.FirstOrDefault().RegionDescripcion : "S/A";
                    worksheet.Cells[fila, 14].Value = destino.FirstOrDefault().ComunaDescripcion != null ? destino.FirstOrDefault().ComunaDescripcion : "S/A";
                    worksheet.Cells[fila, 15].Value = destino.FirstOrDefault().FechaInicio != null ? destino.FirstOrDefault().FechaInicio.ToString() : "S/A";
                    worksheet.Cells[fila, 16].Value = destino.LastOrDefault().FechaHasta != null ? destino.LastOrDefault().FechaHasta.ToString() : "S/A";
                    worksheet.Cells[fila, 17].Value = destino.Any() ? destino.FirstOrDefault().Dias40Aprobados.ToString() : "0";
                    worksheet.Cells[fila, 18].Value = destino.Any() ? destino.FirstOrDefault().Dias50Aprobados.ToString() : "0";
                    worksheet.Cells[fila, 19].Value = destino.Any() ? destino.FirstOrDefault().Dias60Aprobados.ToString() : "0";
                    worksheet.Cells[fila, 20].Value = destino.Any() ? destino.FirstOrDefault().Dias100Aprobados.ToString() : "0";
                    worksheet.Cells[fila, 21].Value = destino.Any() ? destino.FirstOrDefault().Dias40Monto.ToString() : "0";
                    worksheet.Cells[fila, 22].Value = destino.Any() ? destino.FirstOrDefault().Dias50Monto.ToString() : "0";
                    worksheet.Cells[fila, 23].Value = destino.Any() ? destino.FirstOrDefault().Dias60Monto.ToString() : "0";
                    worksheet.Cells[fila, 24].Value = destino.Any() ? destino.FirstOrDefault().Dias100Monto.ToString() : "0";
                    worksheet.Cells[fila, 25].Value = "0";
                    worksheet.Cells[fila, 26].Value = cometido.CometidoDescripcion;
                }
            }

            return File(excelPackageTransparencia.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "rptSolicitudTransparencia_" +  DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
        }

        public FileResult ReportePresupuesto()
        {
            var result = _repository.GetAll<Cometido>();

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\ReportePresupuesto.xlsx");
            var fileInfo = new FileInfo(file);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackagePresupuesto = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackagePresupuesto.Workbook.Worksheets[0];
            foreach (var cometido in result.ToList().OrderByDescending(c => c.CometidoId))
            {
                var workflow = _repository.GetAll<Workflow>().Where(w => w.ProcesoId == cometido.ProcesoId && w.DefinicionWorkflow.Secuencia == 9).FirstOrDefault();
                var cdp = _repository.GetAll<GeneracionCDP>().Where(w => w.CometidoId == cometido.CometidoId).FirstOrDefault();
                var destino = _repository.GetAll<Destinos>().Where(d => d.CometidoId == cometido.CometidoId).ToList();

                if (cdp != null)
                {
                    fila++;
                    worksheet.Cells[fila, 1].Value = cometido.UnidadDescripcion.Contains("Turismo") ? "Turismo" : "Economía";
                    worksheet.Cells[fila, 2].Value = cometido.CometidoId.ToString();
                    worksheet.Cells[fila, 3].Value = cdp.VtcIdCompromiso != null ? cdp.VtcIdCompromiso.ToString() : "S/A" ;
                    worksheet.Cells[fila, 4].Value = cdp.VtcTipoSubTituloId.HasValue && cdp.VtcTipoItemId.HasValue ? cdp.TipoSubTitulo.TstNombre + "." + cdp.TipoItem.TitNombre : "S/A";
                    worksheet.Cells[fila, 5].Value = workflow != null && workflow.FechaTermino.HasValue ? workflow.FechaTermino.Value.ToShortDateString() : "S/A";
                    worksheet.Cells[fila, 6].Value = cometido.Rut + "-" + cometido.DV.ToString();
                    worksheet.Cells[fila, 7].Value = cometido.Nombre != null ? cometido.Nombre : "S/A";
                    if(cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DE ARICA Y PARINACOTA")
                        worksheet.Cells[fila, 8].Value = "REGION DE ARICA Y PARINACOTA";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DE TARAPACA")
                        worksheet.Cells[fila, 8].Value = "REGION DE TARAPACA";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DE ANTOFAGASTA")
                        worksheet.Cells[fila, 8].Value = "REGION DE ANTOFAGASTA";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DE ATACAMA")
                        worksheet.Cells[fila, 8].Value = "REGION DE ATACAMA";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DE COQUIMBO")
                        worksheet.Cells[fila, 8].Value = "REGION DE COQUIMBO";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DE VALPARAISO")
                        worksheet.Cells[fila, 8].Value = "REGION DE VALPARAISO";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DEL LIB. BERNARDO O´HIGGINS")
                        worksheet.Cells[fila, 8].Value = "REGION DEL LIB. BERNARDO O´HIGGINS";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DEL MAULE")
                        worksheet.Cells[fila, 8].Value = "REGION DEL MAULE";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DEL BIO BIO")
                        worksheet.Cells[fila, 8].Value = "REGION DEL BIO BIO";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DE LA ARAUCANÍA")
                        worksheet.Cells[fila, 8].Value = "REGION DE LA ARAUCANÍA";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DE LOS RÍOS")
                        worksheet.Cells[fila, 8].Value = "REGION DE LOS RÍOS";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DE LOS LAGOS")
                        worksheet.Cells[fila, 8].Value = "REGION DE LOS LAGOS";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DE AYSÉN")
                        worksheet.Cells[fila, 8].Value = "REGION DE AYSÉN";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DE MAGALLANES Y LA ANTÁRTICA CHILENA")
                        worksheet.Cells[fila, 8].Value = "REGION DE MAGALLANES Y LA ANTÁRTICA CHILENA";
                    else if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DEL ÑUBLE")
                        worksheet.Cells[fila, 8].Value = "REGION DEL ÑUBLE";
                    else
                        worksheet.Cells[fila, 8].Value = "REGION METROPOLITANA";

                    worksheet.Cells[fila, 9].Value = cometido.GradoDescripcion;

                    if(destino.Count > 0)
                    {
                        worksheet.Cells[fila, 10].Value = destino != null ? destino.FirstOrDefault().ComunaDescripcion : "S/A";
                        worksheet.Cells[fila, 11].Value = destino != null ? destino.FirstOrDefault().FechaInicio.ToString("MMMM") : "S/A";
                        worksheet.Cells[fila, 12].Value = destino != null ? destino.FirstOrDefault().FechaInicio.ToString() : "S/A";
                        worksheet.Cells[fila, 13].Value = destino != null ? destino.LastOrDefault().FechaHasta.ToString() : "S/A";
                        worksheet.Cells[fila, 14].Value = destino != null ? destino.LastOrDefault().Dias100Aprobados.ToString() : "S/A";
                        worksheet.Cells[fila, 15].Value = destino != null ? destino.LastOrDefault().Dias60Aprobados.ToString() : "S/A";
                        worksheet.Cells[fila, 16].Value = destino != null ? destino.LastOrDefault().Dias40Aprobados.ToString() : "S/A";
                        worksheet.Cells[fila, 17].Value = destino != null ? destino.LastOrDefault().Dias50Aprobados.ToString() : "S/A";
                    }
                    
                    worksheet.Cells[fila, 18].Value = cometido.TotalViatico.HasValue ? cometido.TotalViatico.ToString() : "S/A";
                    worksheet.Cells[fila, 19].Value = cdp.VtcCompromisoAcumulado != null ? cdp.VtcCompromisoAcumulado : "S/A";
                    worksheet.Cells[fila, 20].Value = cdp.VtcSaldo != null ? cdp.VtcSaldo : "S/A";
                }
            }

            return File(excelPackagePresupuesto.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet,"rptPresupuesto_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
        }     
        public ActionResult ReporteContraloria()
        {
            var model = new DTOFilterCometido();
            return View(model);
        }
        [HttpPost]
        public FileStreamResult ReporteContraloria(DTOFilterCometido model)
        {
            var predicate = PredicateBuilder.True<Cometido>();
            var RegionComunaContraloria = _repository.Get<RegionComunaContraloria>().ToList();

            if (ModelState.IsValid)
            {
                if (model.FechaInicio.HasValue)
                    predicate = predicate.And(q =>
                        q.FechaSolicitud.Year >= model.FechaInicio.Value.Year &&
                        q.FechaSolicitud.Month >= model.FechaInicio.Value.Month &&
                        q.FechaSolicitud.Day >= model.FechaInicio.Value.Day);

                if (model.FechaTermino.HasValue)
                    predicate = predicate.And(q =>
                        q.FechaSolicitud.Year <= model.FechaTermino.Value.Year &&
                        q.FechaSolicitud.Month <= model.FechaTermino.Value.Month &&
                        q.FechaSolicitud.Day <= model.FechaTermino.Value.Day);

                model.Result = _repository.Get(predicate);
            }
            
            var xdoc = new XDocument(new XElement("LISTADOCUMENTOS", model.Result.Select(w => new XElement("DOCUMENTO",
                                                new XElement("RUN", w.Rut),
                                                new XElement("DIGITO_VERIFICADOR", w.DV),
                                                new XElement("TIPO_DOCUMENTO", w.TipoActoAdministrativo != null ? w.TipoActoAdministrativo.Contains("Ministerial") ? "DECRETO EXENTO" : "RESOLUCION EXENTA" : "S/A"),
                                                new XElement("NUMERO_DOCUMENTO", w.CometidoId),
                                                new XElement("FECHA_DOCUMENTO", w.FechaSolicitud.ToShortDateString()),
                                                new XElement("SERVICIO_EMISOR", "119246"),
                                                new XElement("DEPENDENCIA_EMISORA", "119247"),
                                                new XElement("SERVICIO_DESTINO", "119247"),
                                                new XElement("DEPENDENCIA_DESTINO", "119247"),
                                                new XElement("COMUNA_DESTINO", w.Destinos.Count > 0 ? RegionComunaContraloria.Where(r => r.COMUNA.Contains(w.Destinos.FirstOrDefault().ComunaDescripcion.Trim())).FirstOrDefault().CODIGOCOMUNA.ToString() : "S/A"),
                                                new XElement("REGION_DESTINO", w.Destinos.Count > 0 ? RegionComunaContraloria.Where(r => r.REGIÓN.Contains(w.Destinos.FirstOrDefault().RegionDescripcion.Trim())).FirstOrDefault().CODIGOREGION.ToString() : "S/A"),
                                                new XElement("FECHA_DESDE", w.Destinos.Count > 0 ? w.Destinos.FirstOrDefault().FechaInicio.ToString("dd/MM/yyyy") : "S/A"),
                                                new XElement("FECHA_HASTA", w.Destinos.Count > 0 ? w.Destinos.FirstOrDefault().FechaHasta.ToString("dd/MM/yyyy") : "S/A"),
                                                new XElement("MOTIVO_COMETIDO_FUNCIONARIO", "Reunión fuera del servicio" /*w.NombreCometido != null ? w.NombreCometido.Trim() : "S/A"*/),
                                                new XElement("TIENE_BENEFICIOS",
                                                                new XElement("SELECCIONE_BENEFICIOS",
                                                                new XElement("PASAJE", w.ReqPasajeAereo == true ? "SI" : "NO"),
                                                                new XElement("VIATICO", w.SolicitaViatico == true ? "SI" : "NO"),
                                                                new XElement("ALOJAMIENTO", w.Alojamiento == true ? "SI" : "NO"))
                                                                ),
                                                new XElement("MONTO", w.TotalViatico != null ? w.TotalViatico.Value.ToString() : "0")
                                                ))));

            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(xdoc.ToString());

            xmldoc.DocumentElement.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            settings.NewLineHandling = NewLineHandling.Replace;
            settings.NewLineChars = Environment.NewLine;
            settings.IndentChars = "\t";

            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream,settings);
            writer.WriteStartDocument(true);
            writer.WriteRaw(xmldoc.InnerXml);
            writer.Close();
            stream.Position = 0;
            var fileStreamResult = File(stream, "application/xml", "ReporteContraloria.xml");
            return fileStreamResult;
        }
    }
}