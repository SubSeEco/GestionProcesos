//using App.Model.Shared;
using App.Core.Interfaces;
using App.Core.UseCases;
using App.Model.Cometido;
using App.Model.Comisiones;
using App.Model.Core;
using App.Model.DTO;
using App.Model.Pasajes;
using App.Model.Shared;
using App.Model.Sigper;
using App.Util;
using Newtonsoft.Json;
using OfficeOpenXml;
using Rotativa.Core.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Enum = App.Util.Enum;
//using com.sun.corba.se.spi.ior;
//using System.Net.Mail;
//using com.sun.codemodel.@internal;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    //[NoDirectAccess]
    public class CometidoController : Controller
    {
        public class DTOFilterWorkflow
        {
            public DTOFilterWorkflow()
            {
                Select = new HashSet<DTOSelect>();
                Result = new HashSet<Workflow>();
            }

            [Display(Name = "Desde")]
            [DataType(DataType.Date)]
            public DateTime? Desde { get; set; }

            [Display(Name = "Hasta")]
            [DataType(DataType.Date)]
            public DateTime? Hasta { get; set; }

            [Display(Name = "DefinicionWorkflowId")]
            public int DefinicionWorkflowId { get; set; }

            public IEnumerable<DTOSelect> Select { get; set; }
            public IEnumerable<Workflow> Result { get; set; }
        }
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
            public DateTime? Desde { get; set; }

            [Display(Name = "Hasta")]
            [DataType(DataType.Date)]
            public DateTime? Hasta { get; set; }

            [Display(Name = "ID Cometido")]
            public int ID { get; set; }

            [Display(Name = "Funcionario")]
            public string Ejecutor { get; set; }

            [Display(Name = "Fecha Inicio")]
            [DataType(DataType.Date)]
            public DateTime? FechaInicio { get; set; }

            [Display(Name = "Fecha Término")]
            [DataType(DataType.Date)]
            public DateTime? FechaTermino { get; set; }

            [Display(Name = "Fecha de solicitud")]
            [DataType(DataType.Date)]
            public DateTime? FechaSolicitud { get; set; }

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

            [Display(Name = "Admin")]
            public bool Admin { get; set; }

            [Display(Name = "Activo")]
            public bool Activo { get; set; }

            [Display(Name = "Id Sigfe Tesoreria")]
            public string IdSigfeTesoreria { get; set; }

            public IEnumerable<DTOSelect> Select { get; set; }
            public IEnumerable<Cometido> Result { get; set; }
        }

        //public class Chart
        //{
        //    public Chart()
        //    {
        //        datasets = new List<Datasets>();
        //    }
        //    public string[] labels { get; set; }
        //    public List<Datasets> datasets { get; set; }
        //}

        //public class Datasets
        //{
        //    public string label { get; set; }
        //    public string[] backgroundColor { get; set; }
        //    public string[] borderColor { get; set; }
        //    public string borderWidth { get; set; }
        //    public double[] data { get; set; }
        //    public bool fill { get; set; }
        //}

        //public class DataColumn
        //{
        //    public DataColumn(string label, int valor)
        //    {
        //        this.label = label;
        //        this.valor = valor;
        //    }

        //    //public string label { get; set; }
        //    //public int valor { get; set; }
        //}

        private readonly IGestionProcesos _repository;
        private readonly ISigper _sigper;
        private readonly IFile _file;
        private readonly IHsm _hsm;
        private readonly IFolio _folio;
        private static List<DTODomainUser> ActiveDirectoryUsers { get; set; }
        private static List<Destinos> ListDestino = new List<Destinos>();

        public CometidoController(IGestionProcesos repository, ISigper sigper, IHsm hsm, IFile file, IFolio folio)
        {
            _repository = repository;
            _sigper = sigper;
            _hsm = hsm;
            _file = file;
            _folio = folio;

            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
        }

        public JsonResult GetUnidades()
        {
            var unidades = _sigper.GetUnidades();
            return Json(unidades, JsonRequestBehavior.AllowGet);
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

        //public JsonResult GetPatente(int TipoVehiculoId, int Rut)
        //{
        //    var per = _sigper.GetUserByRut(Rut);
        //    var vehiculo = _repository.GetById<SIGPERTipoVehiculo>(TipoVehiculoId);

        //    var patente = _repository.Get<PatenteVehiculo>().Where(q => q.SIGPERTipoVehiculoId == vehiculo.SIGPERTipoVehiculoId && q.RegionId == per.Contrato.ReContraSed);

        //    return Json(new
        //    {
        //        Vehiculo = vehiculo,
        //        Patente = patente.First().PlacaPatente,
        //    }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetUsuario(int Rut)
        {
            var correo = _sigper.GetUserByRut(Rut).Funcionario.Rh_Mail.Trim();
            //var per = _sigper.GetUserByEmail(correo.Trim());
            var per = _sigper.NewGetUserByEmail(correo.Trim());

            //var IdCargo = per.DatosLaborales.RhConCar.Value;
            //var cargo = string.IsNullOrEmpty(per.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == per.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            //var IdCalidad = per.DatosLaborales.RH_ContCod;
            //var calidad = string.IsNullOrEmpty(per.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == per.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGrado = string.IsNullOrEmpty(per.DatosLaborales.RhConGra.Trim()) ? "0" : per.DatosLaborales.RhConGra.Trim();
            //var grado = string.IsNullOrEmpty(per.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : per.DatosLaborales.RhConGra.Trim();
            //var estamento = per.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == per.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var IdEscalafon = int.Parse(per.DatosLaborales.RhConEsc.Trim());
            //var Escalafon = per.DatosLaborales.RhConEsc == "0" ? "S/A" : _sigper.GetGESCALAFONEs().Where(c => c.Pl_CodEsc == per.DatosLaborales.RhConEsc).FirstOrDefault().Pl_DesEsc.Trim();
            //var ProgId = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).OrderByDescending(c => c.ReContraLabCor).FirstOrDefault().Re_ConPyt;// == 0 ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte && c.Re_ConIni.Year == DateTime.Now.Year).FirstOrDefault().Re_ConPyt;
            //var Programa = ProgId != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgId).FirstOrDefault().RePytDes : "S/A";
            //var conglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte).ReContraSed;
            //var jefatura = per.Jefatura != null ? per.Jefatura.PeDatPerChq : "Sin jefatura definida" ;
            //var Unidad = per.Unidad.Pl_UndDes.Trim();
            //var IdUnidad = per.Unidad.Pl_UndCod;

            /*se toman los datos laborales de los funcionarios desde la tabla ReContra */
            var IdCargo = per.Contrato.Re_ConCar;
            var cargo = string.IsNullOrEmpty(per.Contrato.Re_ConCar.ToString().Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == per.Contrato.Re_ConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidad = per.Contrato.RH_ContCod;
            var calidad = string.IsNullOrEmpty(per.Contrato.RH_ContCod.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == per.Contrato.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGrado = string.IsNullOrEmpty(per.Contrato.Re_ConGra.Trim()) ? "0" : per.Contrato.Re_ConGra.Trim();
            var grado = string.IsNullOrEmpty(per.Contrato.Re_ConGra.Trim()) ? "Sin Grado" : per.Contrato.Re_ConGra.Trim();
            var estamento = per.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == per.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var IdEscalafon = int.Parse(per.Contrato.Re_ConEsc.Trim());
            var Escalafon = per.Contrato.Re_ConEsc == "0" ? "S/A" : _sigper.GetGESCALAFONEs().Where(c => c.Pl_CodEsc == per.Contrato.Re_ConEsc).FirstOrDefault().Pl_DesEsc.Trim();
            //var ProgId = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).OrderByDescending(c => c.ReContraLabCor).FirstOrDefault().Re_ConPyt;// == 0 ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte && c.Re_ConIni.Year == DateTime.Now.Year).FirstOrDefault().Re_ConPyt;
            var ProgId = per.Contrato.Re_ConPyt != 0 ? per.Contrato.Re_ConPyt : 1;
            var Programa = ProgId != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgId).FirstOrDefault().RePytDes : "S/A";
            var conglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte).ReContraSed;
            var jefatura = per.Jefatura != null ? per.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var IdUnidad = per.Contrato.Re_ConUni.ToString().Trim();
            var Unidad = _sigper.GetUnidad(per.Contrato.Re_ConUni).Pl_UndDes.Trim();



            string rut;
            if (per.Funcionario.RH_NumInte.ToString().Length < 8)
            {
                string t = per.Funcionario.RH_NumInte.ToString();
                rut = string.Concat("0", t);
            }
            else
            {
                rut = per.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                Rut = rut,
                DV = per.Funcionario.RH_DvNuInt,
                IdCargo,
                Cargo = cargo,
                IdCalidad,
                CalidadJuridica = calidad,
                IdGrado,
                Grado = grado,
                Estamento = estamento,
                Programa = Programa.Trim(),
                Conglomerado = conglomerado,
                Unidad,
                IdUnidad,
                Jefatura = jefatura,
                IdEscalafon,
                Escalafon,
                IdPrograma = ProgId
            }, JsonRequestBehavior.AllowGet);
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
            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo");
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");

            var proceso = _repository.GetById<Proceso>(id);
            var model = _repository.GetFirst<Cometido>(q => q.ProcesoId == id);
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            var mail = UserExtended.Email(User);

            var user = new HomeController.DTOUser()
            {
                IsCometido = _repository.GetExists<Usuario>(q => q.Habilitado && q.Email == mail && q.Grupo.Nombre.Contains(Enum.Grupo.Cometido.ToString())),
                /*IsAdmin = _repository.GetExists<Usuario>(q => q.Habilitado && q.Email == mail && q.Grupo.Nombre.Contains(Enum.Grupo.Administrador.ToString()))*/
            };

            if (user.IsAdmin || user.IsCometido)
            {
                ViewBag.User = user;
            }

            ViewBag.Pasajes = model.Pasajes;
            ViewBag.DestinosPasajes = _repository.Get<DestinosPasajes>(q => q.Pasaje.ProcesoId == model.ProcesoId);
            if (model == null)
            {
                //model.Proceso = proceso;
                return RedirectToAction("Details", "Proceso", new { id });
            }
            return View(model);
        }

        public ActionResult ViewDocWord(int id)
        {
            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo");
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");

            //var proceso = _repository.GetById<Proceso>(id);
            //var model = _repository.GetFirst<Cometido>(q => q.ProcesoId == id);
            //if (model == null)
            //{
            //    model.Proceso = proceso;
            //    return RedirectToAction("Details", "Proceso", new { id });
            //}

            var model = _repository.GetFirst<Cometido>(q => q.CometidoId == id);
            var pro = _repository.GetById<Proceso>(model.ProcesoId);
            var doc = _repository.Get<Documento>(c => c.ProcesoId == pro.ProcesoId).ToList();
            pro.Documentos = doc;
            model.Proceso = pro;

            /*ver en word*/
            //Read the saved Html File.
            var docto = _repository.Get<Documento>(c => c.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 1).FirstOrDefault();
            string wordHTML = System.IO.File.ReadAllText(docto.Texto);

            //Loop and replace the Image Path.
            foreach (Match match in Regex.Matches(wordHTML, "<v:imagedata.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase))
            {
                wordHTML = Regex.Replace(wordHTML, match.Groups[1].Value, "Temp/" + match.Groups[1].Value);
            }

            //Delete the Uploaded Word File.
            //System.IO.File.Delete(fileSavePath.ToString());

            ViewBag.WordHtml = wordHTML;




            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ShowWord(int id)
        {
            var model = _repository.GetById<Documento>(id);
            var doc = _repository.Get<Documento>(c => c.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 1).FirstOrDefault();

            ////Read the saved Html File.
            //string wordHTML = System.IO.File.ReadAllText(doc.Texto);

            ////Loop and replace the Image Path.
            //foreach (Match match in Regex.Matches(wordHTML, "<v:imagedata.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase))
            //{
            //    wordHTML = Regex.Replace(wordHTML, match.Groups[1].Value, "Temp/" + match.Groups[1].Value);
            //}

            ////Delete the Uploaded Word File.
            ////System.IO.File.Delete(fileSavePath.ToString());

            //ViewBag.WordHtml = wordHTML;
            return View();




            //if (string.IsNullOrWhiteSpace(model.Type))
            //{
            //    string pdfFile = @"D:\Soporte\Código de barras(0001103878152)1_20150310_115529.PDF";
            //    string docxFile = string.Empty;
            //    MemoryStream docxStream = new MemoryStream();
            //    // Convert PDF to word in memory
            //    SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();

            //    // Assume that we already have a PDF document as stream.
            //    using (FileStream pdfStream = new FileStream(pdfFile, FileMode.Open, FileAccess.Read))
            //    {
            //        f.OpenPdf(pdfStream);

            //        if (f.PageCount > 0)
            //        {
            //            int res = f.ToWord(docxStream);

            //            // Save docxStream to a file for demonstration purposes.
            //            if (res == 0)
            //            {
            //                docxFile = Path.ChangeExtension(pdfFile, ".docx");
            //                //File.WriteAllBytes(docxFile, docxStream.ToArray());
            //                //System.Diagnostics.Process.Start(docxFile);
            //            }
            //        }
            //    }

            //    //return File(docxStream, "application/msword");
            //    //return File(model.File, System.Net.Mime.MediaTypeNames.Application.Octet, docxFile);
            //    return File(docxStream.ToArray(), "application/msword", docxFile);
            //}
            //else
            //{
            //    return File(model.File, model.Type, model.FileName);
            //    //return File(model.File, "application/pdf");
            //}
        }

        public ActionResult Reiniciar(int? CometidoId)
        {
            var cometido = _repository.GetById<Cometido>(CometidoId);
            var pro = _repository.GetById<Proceso>(cometido.ProcesoId);
            var work = _repository.GetFirst<Workflow>(c => c.WorkflowId == cometido.WorkflowId);
            var definicionworkflowlist = _repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == 13).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId) ?? null;

            /*se activo cometido con opcion de resolucion revocatoria*/
            cometido.Activo = true;
            cometido.ResolucionRevocatoria = true;

            /*se debe activar proceso nuevamnete*/
            pro.EstadoProcesoId = (int)Util.Enum.EstadoProceso.EnProceso;


            /*se debe crear una nueva tarea de workflow, q inicie en la tarea de analista gestion personas*/
            var workflow = new Workflow();
            workflow.FechaCreacion = DateTime.Now;
            workflow.TipoAprobacionId = (int)Util.Enum.TipoAprobacion.SinAprobacion;
            workflow.Terminada = false;
            workflow.DefinicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 6); //work.DefinicionWorkflow.Secuencia;
            workflow.ProcesoId = work.ProcesoId;
            workflow.Mensaje = work.Observacion;
            workflow.TareaPersonal = false;
            workflow.Asunto = !string.IsNullOrEmpty(work.Asunto) ? work.Asunto : work.DefinicionWorkflow.DefinicionProceso.Nombre + " Nro: " + _repository.Get<Cometido>(c => c.ProcesoId == workflow.ProcesoId).FirstOrDefault().CometidoId;

            var persona = new Sigper();
            persona = _sigper.GetUserByEmail(definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 6).Email);
            if (persona == null)
                throw new Exception("No se encontró el usuario en Sigper.");

            workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
            workflow.Pl_UndDes = persona.Unidad.Pl_UndDes.Trim();
            workflow.Email = persona.Funcionario.Rh_Mail.Trim();
            workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
            workflow.TareaPersonal = true;

            _repository.Update(cometido);
            _repository.Update(pro);
            _repository.Create(workflow);
            _repository.Save();

            return RedirectToAction(workflow.DefinicionWorkflow.Accion.Codigo, workflow.DefinicionWorkflow.Entidad.Codigo, new { workflow.WorkflowId });

            //return View("Index", "Workflow");
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
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            var proceso = _repository.GetById<Proceso>(model.ProcesoId);

            model.Workflow = workflow;
            model.Proceso = proceso;

            if (model.GeneracionCDP.Count > 0)
            {
                var cdp = _repository.GetById<GeneracionCDP>(model.GeneracionCDP.FirstOrDefault().GeneracionCDPId);
                model.GeneracionCDP.Add(cdp);
            }



            return View(model);
        }

        public ActionResult ValidaSubseAlMinistro(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            var proceso = _repository.GetById<Proceso>(model.ProcesoId);

            model.Workflow = workflow;
            model.Proceso = proceso;

            return View(model);
        }

        public ActionResult ValidaMinistro(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            var proceso = _repository.GetById<Proceso>(model.ProcesoId);

            model.Workflow = workflow;
            model.Proceso = proceso;

            return View(model);
        }

        public ActionResult ValidaSubsecretario(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            var proceso = _repository.GetById<Proceso>(model.ProcesoId);

            model.Workflow = workflow;
            model.Proceso = proceso;



            return View(model);
        }

        public ActionResult DetailsDocto(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            return View(model);
        }

        public ActionResult DetailsFinanzas(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            var proceso = _repository.GetById<Proceso>(model.ProcesoId);
            model.Proceso = proceso;
            model.Workflow = workflow;
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DetailsFinanzas(Cometido model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _hsm, _file, _folio, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);
                var doc = _repository.GetFirst<Documento>(c => c.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 5);
                var user = User.Email();
                //var _UseCaseResponseMessage = _useCaseInteractor.DocumentoSign(doc, user,null);

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
            //else
            //{
            //    var errors = ModelState.Select(x => x.Value.Errors)
            //        .Where(y => y.Count > 0)
            //        .ToList();
            //}

            var modelo = _repository.GetFirst<Cometido>(c => c.CometidoId == model.CometidoId);

            return View(modelo);
            //return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            return View(model);
        }

        public ActionResult SignResolucion(Documento model, int? DocumentoId)
        {
            /*Se debe volver a generar el documento si corresponde a cometido para agregar los campos que se han actualizado*/
            /*Se verifica que corresponde a un proceso de cometido*/
            //var doc = _repository.GetById<Documento>(model.DocumentoId);
            //var DefinicionProceso = _repository.GetAll<Proceso>().Where(p => p.ProcesoId == doc.ProcesoId).FirstOrDefault().DefinicionProcesoId;
            //if(DefinicionProceso == 10)
            //{
            //    var IdCom = _repository.GetAll<Cometido>().Where(c => c.ProcesoId == doc.ProcesoId).FirstOrDefault().CometidoId;
            //    GeneraDocumento(IdCom);
            //}
            var ProcesoDocto = _repository.Get<Documento>(d => d.DocumentoId == model.DocumentoId).FirstOrDefault().ProcesoId;
            var CometidoId = _repository.Get<Cometido>(c => c.ProcesoId == ProcesoDocto).FirstOrDefault().CometidoId;
            var email = UserExtended.Email(User);

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _hsm, _file, _folio, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.DocumentoSign(model, email, CometidoId);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    //return Redirect(Request.UrlReferrer.PathAndQuery);
                    return RedirectToAction("Sign", "Cometido", new { id = CometidoId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
                //TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            //return View(model);
            return RedirectToAction("Sign", "Cometido", new { id = CometidoId });
        }

        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            var cdp = _repository.GetAll<GeneracionCDP>().Where(c => c.CometidoId == model.CometidoId).ToList();
            if (cdp != null)
            {
                model.GeneracionCDP.Add(cdp.FirstOrDefault());

                /*Validar si existe un documento asociado y si se encuentra firmado*/
                var doc = _repository.Get<Documento>(c => c.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 1).FirstOrDefault();
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

        public ActionResult SignOther()
        {
            //var model = _repository.GetAll<Cometido>().Where(c => c.CometidoId == id);
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SignOther(int? DocumentoId)
        {
            //IdProceso = 2423;
            //var model = _repository.GetAll<Cometido>().Where(c => c.ProcesoId == IdProceso.Value).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _hsm, _file, _folio, _sigper);
                var doc = _repository.Get<Documento>(c => c.DocumentoId == DocumentoId).FirstOrDefault();//.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 4).FirstOrDefault();
                var user = User.Email();
                var _UseCaseResponseMessage = _useCaseInteractor.DocumentoSign(doc, user, null);

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
            //else
            //{
            //    var errors = ModelState.Select(x => x.Value.Errors)
            //        .Where(y => y.Count > 0)
            //        .ToList();
            //}
            //return View(model);
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Create(int? WorkFlowId, int? ProcesoId)
        {
            //var persona = _sigper.GetUserByEmail(User.Email());
            var persona = _sigper.NewGetUserByEmail(User.Email());

            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().Where(q => q.Activo).OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo");
            //ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(201110), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreId = new SelectList(_sigper.GetAllUsers().Where(c =>c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreId = new SelectList(_sigper.GetAllUsersForCometido().Where(c => !c.Rh_Mail.ToLower().Contains("subturismo")), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreId = new SelectList(_sigper.GetAllUsersForCometido().Where(c => c.Rh_Mail.ToLower().Contains("economia")), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidadForFirma(10352548).Where(c => c.Rh_Mail.ToLower().Contains("economia")), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(100510), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreId = new SelectList(_sigper.GetUserByEmail('ppfuente'), "RH_NumInte", "PeDatPerChq");


            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new Cometido
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
                Tarea = workflow.DefinicionWorkflow.Nombre
            };

            if (persona.Funcionario == null)
                ModelState.AddModelError(string.Empty, "No se encontró información del funcionario en Sigper");
            if (persona.Unidad == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la unidad del funcionario en Sigper");
            if (persona.Jefatura == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la jefatura del funcionario en Sigper");
            if (persona.Contrato == null)
                ModelState.AddModelError(string.Empty, "No se encontró información asociada a los datos laborales del funcionario en Sigper");

            if (ModelState.IsValid)
            {
                /*Datos personales*/
                model.Rut = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte) : persona.Funcionario.RH_NumInte;
                model.DV = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreId = persona.Funcionario.RH_NumInte;
                model.Nombre = persona.Funcionario.PeDatPerChq.Trim();
                /*Datos laborales*/
                //model.IdUnidad = persona.Unidad.Pl_UndCod;
                //model.UnidadDescripcion = persona.Unidad.Pl_UndDes.Trim();                
                //model.IdCargo = persona.DatosLaborales.RhConCar.Value;
                //model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidad = persona.DatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGrado = string.IsNullOrEmpty(persona.DatosLaborales.RhConGra.Trim()) ? "0" : persona.DatosLaborales.RhConGra.Trim();
                //model.GradoDescripcion = string.IsNullOrEmpty(persona.DatosLaborales.RhConGra.Trim()) ? "0" : persona.DatosLaborales.RhConGra;
                //model.IdEstamento = persona.DatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcion = persona.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcion = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdPrograma = Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).OrderByDescending(c => c.ReContraLabCor).FirstOrDefault().Re_ConPyt); //_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).OrderByDescending(c =>c.RE_ConCor).FirstOrDefault().Re_ConPyt);
                ////model.ProgramaDescripcion = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt.ToString();
                //model.ProgramaDescripcion = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                //model.Jefatura = persona.Jefatura.PeDatPerChq;
                //model.IdEscalafon = int.Parse(persona.DatosLaborales.RhConEsc.Trim());
                //model.EscalafonDescripcion = persona.DatosLaborales.RhConEsc == "0" ? "S/A" : _sigper.GetGESCALAFONEs().Where(c => c.Pl_CodEsc == persona.DatosLaborales.RhConEsc).FirstOrDefault().Pl_DesEsc.Trim();
                model.IdUnidad = persona.Contrato.Re_ConUni;
                model.UnidadDescripcion = _sigper.GetUnidad(persona.Contrato.Re_ConUni).Pl_UndDes.Trim();
                model.IdCargo = persona.Contrato.Re_ConCar;
                model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.Contrato.Re_ConCar).FirstOrDefault().Pl_DesCar.Trim();
                model.IdCalidad = persona.Contrato.RH_ContCod;/*id calidad juridica*/
                model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.Contrato.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                model.IdGrado = string.IsNullOrEmpty(persona.Contrato.Re_ConGra.Trim()) ? "0" : persona.Contrato.Re_ConGra.Trim();
                model.GradoDescripcion = string.IsNullOrEmpty(persona.Contrato.Re_ConGra.Trim()) ? "0" : persona.Contrato.Re_ConGra.Trim();
                model.IdEstamento = persona.DatosLaborales.PeDatLabEst;
                model.EstamentoDescripcion = persona.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                model.IdConglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                model.ConglomeradoDescripcion = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                model.IdPrograma = persona.Contrato.Re_ConPyt != 0 ? Convert.ToInt32(persona.Contrato.Re_ConPyt) : 1; //Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).OrderByDescending(c => c.ReContraLabCor).FirstOrDefault().Re_ConPyt); //_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).OrderByDescending(c =>c.RE_ConCor).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcion = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt.ToString();
                model.ProgramaDescripcion = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.Jefatura = persona.Jefatura != null ? persona.Jefatura.PeDatPerChq : "Sin jefatura definida";
                model.IdEscalafon = int.Parse(persona.Contrato.Re_ConEsc.Trim());
                model.EscalafonDescripcion = persona.DatosLaborales.RhConEsc == "0" ? "S/A" : _sigper.GetGESCALAFONEs().Where(c => c.Pl_CodEsc == persona.Contrato.Re_ConEsc).FirstOrDefault().Pl_DesEsc.Trim();
                /*datos del cometido*/
                model.FechaSolicitud = DateTime.Now;
                model.FinanciaOrganismo = false;
                model.Vehiculo = false;
                model.SolicitaReembolso = true;


                model.Destinos = ListDestino;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cometido model)
        {
            //var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetAllUsersForCometido().Where(c => c.Rh_Mail.ToLower().Contains("economia")), "RH_NumInte", "PeDatPerChq");
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
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            var proceso = _repository.GetById<Proceso>(model.ProcesoId);
            model.Workflow = workflow;
            model.Proceso = proceso;


            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.IdFuncionarioPagadorTesoreria = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdTipoPagoTesoreria = new SelectList(_repository.Get<TipoPagoSIGFE>(q => q.TipoActivo == true), "TipoPagoSIGFEId", "DescripcionTipoPago");
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
                //else
                //{
                //    var errors = ModelState.Select(x => x.Value.Errors)
                //        .Where(y => y.Count > 0)
                //        .ToList();
                //}
            }
            else
            {
                foreach (var item in resp.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            //List<SelectListItem> tipoPagoTesoreria = new List<SelectListItem>
            //{
            //new SelectListItem {Text = "Pago", Value = "1"},
            //new SelectListItem {Text = "Pago con Observaciones", Value = "2"},
            //new SelectListItem {Text = "No Pago", Value = "3"},
            //};

            model = _repository.GetById<Cometido>(model.CometidoId);
            var proceso = _repository.GetById<Proceso>(model.ProcesoId);
            model.Proceso = proceso;

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.IdFuncionarioPagadorTesoreria = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdTipoPagoTesoreria = new SelectList(_repository.Get<TipoPagoSIGFE>(q => q.TipoActivo == true), "TipoPagoSIGFEId", "DescripcionTipoPago");
            return View(model);
        }

        public ActionResult EditSigfe(int id)
        {
            //var model = _repository.GetById<Cometido>(id);
            var model = _repository.Get<Cometido>(c => c.CometidoId == id).FirstOrDefault();
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            var proceso = _repository.GetById<Proceso>(model.ProcesoId);

            model.Workflow = workflow;
            model.Proceso = proceso;

            //List<SelectListItem> tipoPago = new List<SelectListItem>
            //{
            //new SelectListItem {Text = "Devengo", Value = "1"},
            //new SelectListItem {Text = "Devengo con Observaciones", Value = "2"},
            //new SelectListItem {Text = "No Devengo", Value = "3"},
            //};

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.IdFuncionarioPagador = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdTipoPago = new SelectList(_repository.Get<TipoPagoSIGFE>(q => q.TipoActivo == true), "TipoPagoSIGFEId", "DescripcionTipoPagoContabilidad");
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

            if (string.IsNullOrEmpty(model.IdSigfe))
                resp.Errors.Add("Debe ingresar Folio SIGFE.");

            if (!model.FechaPagoSigfe.HasValue)
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
                //else
                //{
                //    var errors = ModelState.Select(x => x.Value.Errors)
                //        .Where(y => y.Count > 0)
                //        .ToList();
                //}
            }
            else
            {
                foreach (var item in resp.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            //List<SelectListItem> tipoPago = new List<SelectListItem>
            //{
            //new SelectListItem {Text = "Devengo", Value = "1"},
            //new SelectListItem {Text = "Devengo con Observaciones", Value = "2"},
            //new SelectListItem {Text = "No Devengo", Value = "3"},
            //};

            model = _repository.GetById<Cometido>(model.CometidoId);
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            var proceso = _repository.GetById<Proceso>(model.ProcesoId);
            model.Workflow = workflow;
            model.Proceso = proceso;



            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.IdFuncionarioPagador = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdTipoPago = new SelectList(_repository.Get<TipoPagoSIGFE>(q => q.TipoActivo == true), "TipoPagoSIGFEId", "DescripcionTipoPagoContabilidad");
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

            //var destinosPasajes = _repository.Get<DestinosPasajes>(q => q.Pasaje.ProcesoId == model.ProcesoId).ToList();

            /* var pasajes = _repository.Get<Pasaje>(q => q.ProcesoId == model.ProcesoId).ToList();
             var destinosPasajes = new List<DestinosPasajes>();
             var help = new DestinosPasajes();

             foreach (var pasaje in pasajes)
             {
                 help = _repository.GetFirst<DestinosPasajes>(q => q.PasajeId == pasaje.PasajeId);
                 destinosPasajes.Add(help);
             }

             model.DestinosPasajes = destinosPasajes;*/

            /*En primera instancia, se debe dejar en false hasta que se ingrese el destino para realizar el calculo de fechas.*/
            model.Atrasado = false;

            //variable para contar fechas.
            var helper = new List<int>();

            for (int i = 0; i < model.Destinos.Count; i++)
            {
                var fecha = model.FechaSolicitud.Date.Subtract(model.Destinos[i].FechaInicio.Date).Days;
                var fechahelp = model.Destinos[i].FechaInicio.Date.Subtract(model.FechaSolicitud.Date).Days;

                if (fechahelp < 7)
                {
                    helper.Add(fechahelp);
                }
                else
                {
                    model.Atrasado = false;
                }

                /*if (model.IdGrado == "C" || model.IdGrado == "B")
                {
                    model.Atrasado = false;
                }*/

                if (helper.Any())
                {
                    model.Atrasado = true;
                }
            }

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
                    if (model.ReqPasajeAereo)
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
                            _destino.FechaOrigen = DesPasajes.FechaOrigen;
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
                    if (model.ReqPasajeAereo)
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

            }
            else
            {
                var help = _repository.Get<Destinos>(q => q.CometidoId == model.CometidoId);
                if (help != null)
                {
                    foreach (var dest in help)
                    {
                        model.Destinos.Add(dest);
                    }
                }
            }


            ViewBag.Pasajes = model.Pasajes;
            ViewBag.DestinosPasajes = _repository.Get<DestinosPasajes>(q => q.Pasaje.ProcesoId == model.ProcesoId);


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
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            model.Workflow = workflow;

            var persona = _sigper.GetUserByEmail(User.Email());

            var helper = new List<int>();

            for (int i = 0; i < model.Destinos.Count; i++)
            {
                var fecha = model.FechaSolicitud.Date.Subtract(model.Destinos[i].FechaInicio.Date).Days;
                var fechahelp = model.Destinos[i].FechaInicio.Date.Subtract(model.FechaSolicitud.Date).Days;

                if (fechahelp < 7)
                {
                    helper.Add(fechahelp);
                }
                else
                {
                    model.Atrasado = false;
                }

                /*if (model.IdGrado == "C" || model.IdGrado == "B")
                {
                    model.Atrasado = false;
                }*/

                if (helper.Any())
                {
                    model.Atrasado = true;
                }
            }

            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            ViewBag.FinanciaOrganismo = model.FinanciaOrganismo;


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGP(Cometido model)
        {
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            model.Workflow = workflow;

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
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            model.Workflow = workflow;

            //model.Destinos = ListDestino;
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            /*ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);*/
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
                var cometido = _repository.GetById<Cometido>(model.CometidoId);
                var Ggp = _repository.GetById<GeneracionCDP>(model.GeneracionCDP.FirstOrDefault(q => q.CometidoId == model.CometidoId));
                /*model.TipoVehiculoId = cometido.TipoVehiculoId;*/
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);

                var workflow = _repository.GetById<Workflow>(model.WorkflowId);

                model.Workflow = workflow;

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

        [AllowAnonymous]
        public ActionResult GeneraDocumento(int id)
        {
            //Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            //foreach (var key in Request.Cookies.AllKeys)
            //{
            //    cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            //}

            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            int tipoDoc = 0;
            int IdDocto = 0;
            string Name = string.Empty;
            var model = _repository.GetById<Cometido>(id);
            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
            if ((Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)) /*genera CDP, por la etapa en la que se encuentra*/
            {
                /*Se genera certificado de viatico*/
                //Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("CDPViatico", new { id = model.CometidoId }) { FileName = "CDP_Viatico" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("CDPViatico", new { id = model.CometidoId }) { FileName = "CDP_Viatico" + ".pdf" };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);
                tipoDoc = 2;
                Name = "CDP Viatico Cometido nro" + " " + model.CometidoId + ".pdf";
                int idDoctoViatico = 0;

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var cdpViatico = _repository.Get<Documento>(d => d.ProcesoId == model.ProcesoId);
                if (cdpViatico != null)
                {
                    foreach (var res in cdpViatico)
                    {
                        if (res.TipoDocumentoId == 2)
                            idDoctoViatico = res.DocumentoId;
                    }
                }

                if (idDoctoViatico == 0)
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
                    docOld.Fecha = DateTime.Now;
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
                if (model.IdEscalafon == 1 && model.IdEscalafon != null) /*Autoridad de Gobierno*/
                {
                    //Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Resolucion", new { id = model.CometidoId }) { FileName = "Resolucion Ministerial Exenta" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                    Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Resolucion", new { id = model.CometidoId }) { FileName = "Resolucion Ministerial Exenta" + ".pdf" };
                    resultPdf.PageMargins = new Rotativa.Options.Margins(15, 15, 15, 15);
                    resultPdf.PageSize = (Rotativa.Options.Size?)Size.Executive;
                    pdf = resultPdf.BuildFile(ControllerContext);
                    //data = GetBynary(pdf);
                    data = _file.BynaryToText(pdf);
                    tipoDoc = 1;
                    Name = "Resolucion Ministerial Exenta nro" + " " + model.CometidoId + ".pdf";
                }
                else
                {
                    if (model.CalidadDescripcion.Contains("HONORARIOS"))/*valida si es contrata u honorario*/
                    {
                        //if (model.IdGrado != "0" && model.GradoDescripcion != "0")
                        //{
                        //Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Orden", new { id = model.CometidoId }) { FileName = "Orden_Pago" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                        Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Orden", new { id = model.CometidoId }) { FileName = "Orden_Pago" + ".pdf" };
                        resultPdf.PageMargins = new Rotativa.Options.Margins(15, 15, 15, 15);
                        resultPdf.PageSize = (Rotativa.Options.Size?)Size.Executive;
                        pdf = resultPdf.BuildFile(ControllerContext);
                        //data = GetBynary(pdf);
                        data = _file.BynaryToText(pdf);

                        tipoDoc = 1;
                        Name = "Orden de Pago Cometido nro" + " " + model.CometidoId + ".pdf";
                        //}
                        //else
                        //{
                        //    //TempData["Error"] = "No existen antecedentes del grado del funcionario";
                        //    TempData["Success"] = "No existen antecedentes del grado del funcionario.";
                        //    return Redirect(Request.UrlReferrer.PathAndQuery);
                        //}
                    }
                    //else if (model.CalidadDescripcion.Contains("TITULAR"))/*valida si es autoridad*/
                    //{
                    //    Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Resolucion", new { id = model.CometidoId }) { FileName = "Resolucion Ministerial Exenta" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                    //    pdf = resultPdf.BuildFile(ControllerContext);
                    //    //data = GetBynary(pdf);
                    //    data = _file.BynaryToText(pdf);
                    //    tipoDoc = 1;
                    //    Name = "Resolucion Ministerial Exenta nro" + " " + model.CometidoId.ToString() + ".pdf";
                    //}
                    else
                    {
                        //Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.CometidoId }) { FileName = "Resolucion" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                        Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.CometidoId }) { FileName = "Resolucion" + ".pdf" };
                        resultPdf.PageMargins = new Rotativa.Options.Margins(15, 15, 15, 15);
                        resultPdf.PageSize = (Rotativa.Options.Size?)Size.Executive;
                        pdf = resultPdf.BuildFile(ControllerContext);
                        //data = GetBynary(pdf);
                        data = _file.BynaryToText(pdf);

                        tipoDoc = 1;
                        Name = "Resolucion Cometido nro" + " " + model.CometidoId + ".pdf";
                    }
                }

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                //var resolucion = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId && d.TipoDocumentoId == 1);
                var resolucion = _repository.Get<Documento>(d => d.ProcesoId == model.ProcesoId && d.TipoDocumentoId == 1).FirstOrDefault();
                if (resolucion != null)
                {
                    IdDocto = resolucion.DocumentoId;


                    //foreach (var res in resolucion)
                    //{
                    //    if (res.TipoDocumentoId == 1)
                    //        IdDocto = res.DocumentoId;
                    //}
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

                    if (!docOld.Activo)
                    {
                        docOld.Activo = true;
                        _repository.Update(docOld);
                        _repository.Save();
                    }
                    else
                    {
                        if (docOld.Signed != true)
                        {
                            docOld.Fecha = DateTime.Now;
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


        public ActionResult ProcessState(int WorkflowId)
        {
            int sec = 1;
            List<Workflow> _Workflow = new List<Workflow>();
            List<DefinicionWorkflow> _Definicionworkflow = new List<DefinicionWorkflow>();
            var _workflow = _repository.GetById<Workflow>(WorkflowId);
            if (_workflow != null)
            {
                switch (_workflow.Entity)
                {
                    case "Cometido":
                        var _cometido = _repository.GetFirst<Cometido>(c => c.WorkflowId.Value == WorkflowId);
                        if (_cometido != null)
                        {
                            _cometido.Workflow = _workflow;
                            sec = !string.IsNullOrEmpty(_cometido.Workflow.DefinicionWorkflow.Secuencia.ToString()) ? _cometido.Workflow.DefinicionWorkflow.Secuencia : 1;
                            _Workflow = _repository.Get<Workflow>(c => c.ProcesoId == _cometido.ProcesoId).ToList();
                        }
                        else
                        {
                            _Workflow = _repository.Get<Workflow>(c => c.WorkflowId == WorkflowId).ToList();
                        }

                        _Definicionworkflow = _repository.Get<DefinicionWorkflow>(c => c.DefinicionProcesoId == (int)Enum.DefinicionProceso.SolicitudCometidoPasaje && c.Habilitado).OrderBy(c => c.Secuencia).ToList();

                        break;
                }
            }

            var Total = _Definicionworkflow.Count(c => c.Habilitado).ToString();

            var por = (float.Parse(sec.ToString()) / float.Parse(Total)) * 100;

            ViewBag.Secuencia = sec;
            ViewBag.Total = Total;
            ViewBag.Porcentaje = Math.Round((Convert.ToDouble(por)), 0);

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

        [AllowAnonymous]
        public ActionResult Pdf(int id) /*Funcionarios Planta y Contrata*/
        {
            var model = _repository.GetById<Cometido>(id);

            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
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
                model.DiasPlural = "(s)";
                model.Tiempo = model.Destinos.FirstOrDefault().FechaInicio < DateTime.Now ? "Pasado" : "Futuro";
                model.Anno = DateTime.Now.Year.ToString();
                model.Subscretaria = model.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIO DE TURISMO" : "SUBSECRETARÍA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";
                model.FechaResolucion = DateTime.Now;
                model.Firma = false;
                model.NumeroResolucion = model.CometidoId;
                model.Destinos.FirstOrDefault().TotalViaticoPalabras = ExtensionesString.enletras(model.Destinos.FirstOrDefault().TotalViatico.ToString());

                /*se traen los datos de la tabla parrafos*/
                var parrafos = _repository.GetAll<Parrafos>();
                switch (model.IdGrado)
                {
                    case "B":/*Firma Subsecretario*/
                        model.Orden = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.OrdenSubse).FirstOrDefault().ParrafoTexto;
                        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.FirmanteSubse).FirstOrDefault().ParrafoTexto;
                        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.CargoSubse).FirstOrDefault().ParrafoTexto;
                        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
                        break;
                    case "C": /*firma ministro*/
                        model.Orden = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.OrdenMinistro).FirstOrDefault().ParrafoTexto;
                        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.FirmanteMinistro).FirstOrDefault().ParrafoTexto;
                        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.CargoMinistro).FirstOrDefault().ParrafoTexto;
                        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
                        break;
                    default:
                        model.Orden = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.Orden).FirstOrDefault().ParrafoTexto;
                        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.Firmante).FirstOrDefault().ParrafoTexto;
                        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.CargoFirmante).FirstOrDefault().ParrafoTexto;
                        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.VistoRAE).FirstOrDefault().ParrafoTexto;
                        var vit = _repository.Get<Parrafos>(c => c.ParrafosId == (int)Util.Enum.Firmas.ViaticodeVuelta && c.ParrafoActivo).ToList();
                        if (vit.Count > 0)
                            model.ViaticodeVuelta = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.ViaticodeVuelta).FirstOrDefault().ParrafoTexto;
                        else
                            model.ViaticodeVuelta = string.Empty;

                        break;
                }

                #region SE BUSCA FOLIO PARA RESOLUCION  --> SE ELIMINA POR LA NUEVA FIRMA

                /*Se valida que se encuentre en la tarea de Firma electronica para agregar folio y fecha de resolucion*/
                //var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId) ?? null;
                //if (workflowActual.DefinicionWorkflow.Secuencia == 13 || (workflowActual.DefinicionWorkflow.Secuencia == 13 && workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje))
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
                //        var obj = JsonConvert.DeserializeObject<App.Model.DTO.DTOFolio>(result);

                //        //verificar resultado
                //        if (obj.status == "OK")
                //        {
                //            model.Folio = obj.folio;
                //            model.FechaResolucion = DateTime.Now;
                //            model.Firma = true;
                //            model.TipoActoAdministrativo = "Resolución Administrativa Exenta";

                //            _repository.Update(model);
                //            _repository.Save();
                //        }
                //        if (obj.status == "ERROR")
                //        {
                //            TempData["Error"] = obj.error;
                //            //return View(DTOFolio);
                //        }
                //        #endregion
                //    }
                //}

                #endregion

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
            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
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
            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
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
        [AllowAnonymous]
        public ActionResult Orden(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            model.Dias = (model.Destinos.LastOrDefault().FechaHasta.Date - model.Destinos.FirstOrDefault().FechaInicio.Date).Days + 1;
            model.DiasPlural = "(s)";
            model.Tiempo = model.Destinos.FirstOrDefault().FechaInicio < DateTime.Now ? "Pasado" : "Futuro";
            model.Anno = DateTime.Now.Year.ToString();
            model.Subscretaria = model.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIO DE TURISMO" : "SUBSECRETARÍA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";
            model.FechaResolucion = DateTime.Now;
            model.Firma = false;
            model.NumeroResolucion = model.CometidoId;
            model.Destinos.FirstOrDefault().TotalViaticoPalabras = ExtensionesString.enletras(model.TotalViatico.ToString());//.Destinos.FirstOrDefault().Total.ToString());            

            /*se traen los datos de la tabla parrafos*/
            var parrafos = _repository.GetAll<Parrafos>();
            model.Orden = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.Orden).FirstOrDefault().ParrafoTexto;
            model.Firmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.Firmante).FirstOrDefault().ParrafoTexto;
            model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.CargoFirmante).FirstOrDefault().ParrafoTexto;
            model.Vistos = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.VistoOP).FirstOrDefault().ParrafoTexto;

            foreach (var p in parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.DejaseConstancia))
            {
                if (p.ParrafoActivo)
                    model.DejaseConstancia = parrafos.Where(xp => xp.ParrafosId == (int)Util.Enum.Firmas.DejaseConstancia).FirstOrDefault().ParrafoTexto;
                else
                    model.DejaseConstancia = string.Empty;
            }



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

            #region SE BUSCA FOLIO PARA RESOLUCION  --> SE ELIMINA POR LA NUEVA FIRMA


            ///*Se valida que se encuentre en la tarea de Firma electronica para agregar folio y fecha de resolucion*/
            //var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId) ?? null;
            //if (workflowActual.DefinicionWorkflow.Secuencia == 13)
            //{
            //    if (model.Folio == null)
            //    {
            //        #region Folio
            //        /*se va a buscar el folio de testing*/
            //        DTOFolio folio = new DTOFolio();
            //        folio.periodo = DateTime.Now.Year.ToString();
            //        folio.solicitante = "Gestion Procesos - Cometidos";/*Sistema que solicita el numero de Folio*/
            //        folio.tipodocumento = "OP";
            //        //if (model.IdCalidad == 10)
            //        //{
            //        //    folio.tipodocumento = "RAEX";/*"ORPA";*/
            //        //}
            //        //else
            //        //{
            //        //    switch (model.IdGrado)
            //        //    {
            //        //        case "B":/*Resolución Ministerial Exenta*/
            //        //            folio.tipodocumento = "RMEX";
            //        //            break;
            //        //        case "C": /*Resolución Ministerial Exenta*/
            //        //            folio.tipodocumento = "RMEX";
            //        //            break;
            //        //        default:
            //        //            /*Resolución Administrativa Exenta*/
            //        //            break;
            //        //    }
            //        //}

            //        //definir url
            //        var url = "http://wsfolio.test.economia.cl/api/folio/";

            //        //definir cliente http
            //        var clientehttp = new WebClient();
            //        clientehttp.Headers[HttpRequestHeader.ContentType] = "application/json";

            //        //invocar metodo remoto
            //        string result = clientehttp.UploadString(url, "POST", JsonConvert.SerializeObject(folio));

            //        //convertir resultado en objeto 
            //        var obj = JsonConvert.DeserializeObject<App.Model.DTO.DTOFolio>(result);

            //        //verificar resultado
            //        if (obj.status == "OK")
            //        {
            //            model.Folio = obj.folio;
            //            model.FechaResolucion = DateTime.Now;
            //            model.Firma = true;
            //            model.TipoActoAdministrativo = "Orden de Pago"; 

            //            _repository.Update(model);
            //            _repository.Save();
            //        }
            //        if (obj.status == "ERROR")
            //        {
            //            TempData["Error"] = obj.error;
            //            //return View(DTOFolio);
            //        }
            //        #endregion
            //    }
            //}

            #endregion


            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Resolucion(int id) /*Autoridades de Gobierno*/
        {
            var model = _repository.GetById<Cometido>(id);

            model.Dias = (model.Destinos.LastOrDefault().FechaHasta.Date - model.Destinos.FirstOrDefault().FechaInicio.Date).Days + 1;
            model.DiasPlural = "(s)";
            model.Tiempo = model.Destinos.FirstOrDefault().FechaInicio < DateTime.Now ? "Pasado" : "Futuro";
            model.Anno = DateTime.Now.Year.ToString();
            model.Subscretaria = model.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIO DE TURISMO" : "SUBSECRETARÍA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";
            model.FechaResolucion = DateTime.Now;
            model.Firma = false;
            model.NumeroResolucion = model.CometidoId;
            model.Destinos.FirstOrDefault().TotalViaticoPalabras = ExtensionesString.enletras(model.Destinos.FirstOrDefault().TotalViatico.ToString());
            /*se agrega el texto adicionale la seccin resuelvo*/
            //model.ParrafoResolucion = string.Empty;

            /*se traen los datos de la tabla parrafos*/
            var parrafos = _repository.GetAll<Parrafos>();
            switch (model.IdGrado)
            {
                case "B":/*Firma Subsecretario*/
                    model.Orden = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.OrdenSubse).FirstOrDefault().ParrafoTexto;
                    model.Firmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.FirmanteSubse).FirstOrDefault().ParrafoTexto;
                    model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.CargoSubse).FirstOrDefault().ParrafoTexto;
                    model.Vistos = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
                    break;
                case "C": /*firma ministro*/
                    model.Orden = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.OrdenMinistro).FirstOrDefault().ParrafoTexto;
                    model.Firmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.FirmanteMinistro).FirstOrDefault().ParrafoTexto;
                    model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.CargoMinistro).FirstOrDefault().ParrafoTexto;
                    model.Vistos = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
                    break;
                default:
                    model.Orden = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.Orden).FirstOrDefault().ParrafoTexto;
                    model.Firmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.Firmante).FirstOrDefault().ParrafoTexto;
                    model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.CargoFirmante).FirstOrDefault().ParrafoTexto;
                    model.Vistos = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.VistoRAE).FirstOrDefault().ParrafoTexto;
                    var vit = _repository.Get<Parrafos>(c => c.ParrafosId == (int)Util.Enum.Firmas.ViaticodeVuelta && c.ParrafoActivo);
                    if (vit != null)
                        model.ViaticodeVuelta = parrafos.Where(p => p.ParrafosId == (int)Util.Enum.Firmas.ViaticodeVuelta).FirstOrDefault().ParrafoTexto;
                    else
                        model.ViaticodeVuelta = string.Empty;
                    break;
            }

            #region SE BUSCA FOLIO PARA RESOLUCION  --> SE ELIMINA POR LA NUEVA FIRMA


            ///*Se valida que se encuentre en la tarea de Firma electronica para agregar folio y fecha de resolucion*/
            //var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId) ?? null;
            ////if (workflowActual.DefinicionWorkflow.Secuencia == 8)
            ////{
            ////    if (model.Folio == null)
            ////    {
            ////        #region Folio
            ////        /*se va a buscar el folio de testing*/
            ////        DTOFolio folio = new DTOFolio();
            ////        folio.periodo = DateTime.Now.Year.ToString();
            ////        folio.solicitante = "Gestion Procesos - Cometidos";/*Sistema que solicita el numero de Folio*/
            ////        if (model.IdCalidad == 10)
            ////        {
            ////            folio.tipodocumento = "RAEX";/*"ORPA";*/
            ////        }
            ////        else
            ////        {
            ////            switch (model.IdGrado)
            ////            {
            ////                case "B":/*Resolución Ministerial Exenta*/
            ////                    folio.tipodocumento = "RMEX";
            ////                    break;
            ////                case "C": /*Resolución Ministerial Exenta*/
            ////                    folio.tipodocumento = "RMEX";
            ////                    break;
            ////                default:
            ////                    folio.tipodocumento = "RAEX";/*Resolución Administrativa Exenta*/
            ////                    break;
            ////            }
            ////        }


            ////        //definir url
            ////        var url = "http://wsfolio.test.economia.cl/api/folio/";

            ////        //definir cliente http
            ////        var clientehttp = new WebClient();
            ////        clientehttp.Headers[HttpRequestHeader.ContentType] = "application/json";

            ////        //invocar metodo remoto
            ////        string result = clientehttp.UploadString(url, "POST", JsonConvert.SerializeObject(folio));

            ////        //convertir resultado en objeto 
            ////        var obj = JsonConvert.DeserializeObject<App.App.Model.DTO.DTOFolio>(result);

            ////        //verificar resultado
            ////        if (obj.status == "OK")
            ////        {
            ////            model.Folio = obj.folio;
            ////            model.FechaResolucion = DateTime.Now;
            ////            model.Firma = true;

            ////            _repository.Update(model);
            ////            _repository.Save();
            ////        }
            ////        if (obj.status == "ERROR")
            ////        {
            ////            TempData["Error"] = obj.error;
            ////        }
            ////        #endregion
            ////    }
            ////}

            //if (workflowActual.DefinicionWorkflow.Secuencia == 13 || workflowActual.DefinicionWorkflow.Secuencia == 14 || workflowActual.DefinicionWorkflow.Secuencia == 15 && workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
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
            //        var obj = JsonConvert.DeserializeObject<App.Model.DTO.DTOFolio>(result);

            //        //verificar resultado
            //        if (obj.status == "OK")
            //        {
            //            model.Folio = obj.folio;
            //            model.FechaResolucion = DateTime.Now;
            //            model.Firma = true;
            //            model.TipoActoAdministrativo = "Resolución Ministerial Exenta";

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


            #endregion

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
            var persona = _sigper.GetUserByEmail(User.Email());
            var model = new DTOFilterCometido();

            List<SelectListItem> Estado = new List<SelectListItem>
            {
            new SelectListItem {Text = "En Curso", Value = "1"},
            new SelectListItem {Text = "Anulado", Value = "2"},
            new SelectListItem {Text = "Terminado", Value = "3"},
            };

            /*Se busca rol de funcionario y unidad para mostrar el dropdwon de unidades en la busqueda del reporte*/
            var GrupoId = _repository.Get<Usuario>(c => c.Email == persona.Funcionario.Rh_Mail.Trim() && c.Habilitado);
            if (GrupoId.Any())
            {
                foreach (var usr in GrupoId)
                {
                    if (usr.GrupoId == 1)
                    {
                        ViewBag.IdUnidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes").Where(c => c.Text.Contains("ECONOMIA"));
                        model.Admin = true;
                    }
                    else
                    {
                        ViewBag.IdUnidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", persona.Unidad.Pl_UndCod);
                        model.Admin = false;
                    }
                }
            }
            else
            {
                ViewBag.IdUnidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", persona.Unidad.Pl_UndCod);
                model.Admin = false;
            }

            ViewBag.Estado = new SelectList(Estado, "Value", "Text");
            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.IdUnidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes").Where(c => c.Text.Contains("ECONOMIA"));
            ViewBag.Destino = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            return View(model);
        }

        [HttpPost]
        public ActionResult SeguimientoGP(DTOFilterCometido model)
        {
            var predicate = PredicateBuilder.True<Cometido>();

            if (ModelState.IsValid)
            {
                if (model.ID != 0)
                    predicate = predicate.And(q => q.CometidoId == model.ID);

                if (model.Estado.HasValue)
                {
                    //if (model.Estado == 1)
                    //    predicate = predicate.And(q => q.Proceso.Terminada == false && q.Proceso.Anulada == false);
                    //if (model.Estado == 2)
                    //    predicate = predicate.And(q => q.Proceso.Anulada == true);
                    //if (model.Estado == 3)
                    //    predicate = predicate.And(q => q.Proceso.Terminada == true);

                    if (model.Estado == 1)
                        predicate = predicate.And(q => q.Proceso.EstadoProcesoId != (int)Util.Enum.EstadoProceso.Terminado && q.Proceso.EstadoProcesoId != (int)Util.Enum.EstadoProceso.Anulado);
                    if (model.Estado == 2)
                        predicate = predicate.And(q => q.Proceso.EstadoProcesoId == (int)Util.Enum.EstadoProceso.Anulado);
                    if (model.Estado == 3)
                        predicate = predicate.And(q => q.Proceso.EstadoProcesoId == (int)Util.Enum.EstadoProceso.Terminado);
                }

                if (model.NombreId.HasValue)
                    predicate = predicate.And(q => q.NombreId == model.NombreId);

                if (model.IdUnidad.HasValue)
                    predicate = predicate.And(q => q.IdUnidad == model.IdUnidad);

                if (!string.IsNullOrEmpty(model.IdSigfeTesoreria))
                    predicate = predicate.And(q => q.IdSigfeTesoreria == model.IdSigfeTesoreria);

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

                predicate = predicate.And(q => q.Proceso.ProcesoId == q.ProcesoId);

                var CometidoId = model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
                if (CometidoId.Any())
                {
                    predicate = predicate.And(q => CometidoId.Contains(q.CometidoId));
                }

                model.Result = _repository.Get(predicate);

                foreach (var p in model.Result.ToList())
                {
                    model.Result.FirstOrDefault().Proceso = _repository.GetById<Proceso>(p.ProcesoId);
                }
                //model.Result.FirstOrDefault().Proceso = _repository.GetById<Proceso>(model.Result.FirstOrDefault().ProcesoId);

            }

            List<SelectListItem> Estado = new List<SelectListItem>
            {
                new SelectListItem {Text = "En Curso", Value = "1"},
                new SelectListItem {Text = "Anulado", Value = "2"},
                new SelectListItem {Text = "Terminado", Value = "3"},
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
                    //worksheet.Cells[fila, 1].Value = cometido.Proceso.Terminada == false && cometido.Proceso.Anulada == false ? "En Curso" : cometido.Workflow.Terminada == true ? "Terminada" : "Anulada";

                    if (cometido.Proceso.EstadoProcesoId == (int)Util.Enum.EstadoProceso.EnProceso)
                        worksheet.Cells[fila, 1].Value = "En Curso";
                    else if (cometido.Proceso.EstadoProcesoId == (int)Util.Enum.EstadoProceso.Terminado)
                        worksheet.Cells[fila, 1].Value = "Terminada";
                    else if (cometido.Proceso.EstadoProcesoId == (int)Util.Enum.EstadoProceso.Anulado)
                        worksheet.Cells[fila, 1].Value = "Anulada";

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

        public FileResult Caigg(DTOFilterCometido model)
        {
            var predicate = PredicateBuilder.True<Cometido>();

            if (model.Desde.HasValue)
                predicate = predicate.And(q =>
                    q.FechaSolicitud.Year >= model.Desde.Value.Year &&
                    q.FechaSolicitud.Month >= model.Desde.Value.Month &&
                    q.FechaSolicitud.Day >= model.Desde.Value.Day);

            if (model.Hasta.HasValue)
                predicate = predicate.And(q =>
                    q.FechaSolicitud.Year <= model.Hasta.Value.Year &&
                    q.FechaSolicitud.Month <= model.Hasta.Value.Month &&
                    q.FechaSolicitud.Day <= model.Hasta.Value.Day);

            var CometidoId = model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
            if (CometidoId.Any())
                predicate = predicate.And(q => CometidoId.Contains(q.CometidoId));

            model.Result = _repository.Get(predicate);


            var cometido = model.Result;// _repository.Get<Cometido>(c => c.FechaSolicitud >= model.Desde && c.FechaSolicitud <= model.Hasta);
            //var cometido = _repository.GetAll<Cometido>().Where(c => c.CometidoId == 364).ToList();

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\CAIGG.xlsx");
            var fileInfo = new FileInfo(file);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackageCaigg = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackageCaigg.Workbook.Worksheets[0];
            foreach (var com in cometido.OrderByDescending(r => r.CometidoId).ToList())
            {
                var workflow = _repository.Get<Workflow>(w => w.ProcesoId == com.ProcesoId);
                var pasaje = _repository.Get<Pasaje>(p => p.ProcesoId == com.ProcesoId).ToList();

                if (pasaje.Count > 0)
                {
                    /*se extraen los datos asociados al pasaje*/
                    for (var pas = 0; pas < pasaje.Count + 1; pas++)//foreach (var pas in pasaje)
                    {
                        fila++;

                        /*se extraen los datos asociados a las cotizaciones*/
                        var cotizacion = _repository.Get<Cotizacion>(p => p.PasajeId == pasaje[pas].PasajeId && p.CotizacionDocumento.FirstOrDefault().Selected == true).ToList();
                        if (cotizacion.Count > 0)
                        {
                            worksheet.Cells[fila, 16].Value = cotizacion.Count() >= 2 ? "SI" : "NO";
                            worksheet.Cells[fila, 18].Value = cotizacion.FirstOrDefault().FechaVuelo.ToShortDateString(); /*fecha del vuelo*/
                            worksheet.Cells[fila, 19].Value = cotizacion.FirstOrDefault().NumeroOrdenCompra; /*Id orden de compra*/
                            worksheet.Cells[fila, 12].Value = cotizacion.FirstOrDefault().ClasePasaje; /*clase de pasaje*/
                            worksheet.Cells[fila, 15].Value = cotizacion.FirstOrDefault().FormaAdquisicion; /*forma de adquision del pasaje*/
                            worksheet.Cells[fila, 17].Value = cotizacion.FirstOrDefault().FechaAdquisicion != null ? cotizacion.FirstOrDefault().FechaAdquisicion.ToShortDateString() : "S/A";/*fecha adquisicion*/

                            if ((pas % 2) == 0)
                                worksheet.Cells[fila, 20].Value = cotizacion.FirstOrDefault().ValorPasaje.ToString(); /*valor total pasaje*/
                            //else
                            //    worksheet.Cells[fila, 20].Value = "0";                            
                        }
                        else
                        {
                            worksheet.Cells[fila, 16].Value = "S/A";
                            worksheet.Cells[fila, 20].Value = "0";
                        }

                        /*se extraen los datos asociados a los destinos*/
                        var desPasaje = _repository.Get<DestinosPasajes>().Where(c => c.PasajeId == pasaje.FirstOrDefault().PasajeId).ToList();
                        if (desPasaje.Count > 0)
                        {
                            foreach (var p in desPasaje)
                            {
                                if ((pas % 2) == 0)
                                {
                                    worksheet.Cells[fila, 8].Value = p.RegionDescripcion.Trim(); //com.Destinos.Any() ? com.Destinos.FirstOrDefault().ComunaDescripcion : "S/A";                                    
                                    worksheet.Cells[fila, 22].Value = p.FechaIda.ToShortDateString(); //com.Destinos.Any() ? com.Destinos.FirstOrDefault().FechaInicio.ToString() : "S/A";/*fecha ida*/
                                    worksheet.Cells[fila, 23].Value = p.FechaVuelta.ToShortDateString();// com.Destinos.Any() ? com.Destinos.LastOrDefault().FechaHasta.ToString() : "S/A"; /*fecha vuelta*/
                                    worksheet.Cells[fila, 25].Value = ((p.FechaIda - com.FechaSolicitud).Days).ToString(); /*dias de antelacion*/
                                }
                                else
                                {
                                    worksheet.Cells[fila, 8].Value = p.OrigenRegionDescripcion.Trim();
                                    worksheet.Cells[fila, 22].Value = string.Empty; //p.FechaIda.ToShortDateString();/*fecha ida*/
                                    worksheet.Cells[fila, 23].Value = string.Empty; //p.FechaVuelta.ToShortDateString();/*fecha vuelta*/
                                    worksheet.Cells[fila, 25].Value = "0"; /*dias de antelacion*/
                                }
                            }
                        }

                        worksheet.Cells[fila, 1].Value = com.UnidadDescripcion.Contains("Turismo") ? "Turismo" : "Economía";
                        worksheet.Cells[fila, 2].Value = workflow.FirstOrDefault().Proceso.DefinicionProceso.Nombre;
                        worksheet.Cells[fila, 3].Value = com.UnidadDescripcion.Contains("Sere") ? com.UnidadDescripcion : "Nivel Central";
                        worksheet.Cells[fila, 4].Value = com.TipoActoAdministrativo != null ? com.TipoActoAdministrativo : "S/A"; /*Tipo Acto Administrativo*/
                        worksheet.Cells[fila, 5].Value = com.Folio != null ? com.Folio : "S/A"; /*Nro Acto Administrativo*/
                        worksheet.Cells[fila, 6].Value = com.CometidoId.ToString();
                        worksheet.Cells[fila, 7].Value = com.Nombre;

                        worksheet.Cells[fila, 9].Value = com.CargoDescripcion.Trim() == "Ministro" || com.CargoDescripcion.Trim() == "Subsecretario" ? com.CargoDescripcion.Trim() : "Otro";
                        worksheet.Cells[fila, 10].Value = com.CargoDescripcion;
                        worksheet.Cells[fila, 11].Value = com.ReqPasajeAereo ? "Nacional" : "N/A";
                        worksheet.Cells[fila, 13].Value = "N/A";
                        worksheet.Cells[fila, 14].Value = "N/A";

                        worksheet.Cells[fila, 21].Value = com.TotalViatico != null ? com.TotalViatico.ToString() : "0";
                        worksheet.Cells[fila, 24].Value = com.FechaSolicitud.ToShortDateString();
                        worksheet.Cells[fila, 26].Value = com.CometidoDescripcion;

                        var tarea = workflow.LastOrDefault().DefinicionWorkflow;
                        if (tarea.Secuencia < 9)
                            worksheet.Cells[fila, 27].Value = "Solicitado";
                        else if (tarea.Secuencia >= 9 && tarea.Secuencia < 17)
                            worksheet.Cells[fila, 27].Value = "Comprometido";
                        else if (tarea.Secuencia >= 17 && tarea.Secuencia < 20)
                            worksheet.Cells[fila, 27].Value = "Devengado";
                        else if (tarea.Secuencia >= 19)
                            worksheet.Cells[fila, 27].Value = "Pagado";
                    }
                }
                else
                {
                    fila++;
                    worksheet.Cells[fila, 1].Value = com.UnidadDescripcion.Contains("Turismo") ? "Turismo" : "Economía";
                    worksheet.Cells[fila, 2].Value = workflow.FirstOrDefault().Proceso.DefinicionProceso.Nombre;
                    worksheet.Cells[fila, 3].Value = com.UnidadDescripcion.Contains("Sere") ? com.UnidadDescripcion : "Nivel Central";
                    worksheet.Cells[fila, 4].Value = com.TipoActoAdministrativo != null ? com.TipoActoAdministrativo : "S/A"; /*Tipo Acto Administrativo*/
                    worksheet.Cells[fila, 5].Value = com.Folio != null ? com.Folio : "S/A"; /*Nro Acto Administrativo*/
                    worksheet.Cells[fila, 6].Value = com.CometidoId.ToString();
                    worksheet.Cells[fila, 7].Value = com.Nombre;

                    worksheet.Cells[fila, 8].Value = "S/A";

                    worksheet.Cells[fila, 9].Value = com.CargoDescripcion.Trim() == "Ministro" || com.CargoDescripcion.Trim() == "Subsecretario" ? com.CargoDescripcion.Trim() : "Otro";
                    worksheet.Cells[fila, 10].Value = com.CargoDescripcion;
                    worksheet.Cells[fila, 11].Value = com.ReqPasajeAereo ? "Nacional" : "N/A";
                    //worksheet.Cells[fila, 12].Value = "N/A"; /*clase de pasaje*/
                    worksheet.Cells[fila, 13].Value = "N/A";
                    worksheet.Cells[fila, 14].Value = "N/A";

                    worksheet.Cells[fila, 15].Value = "N/A";
                    worksheet.Cells[fila, 16].Value = "N/A";
                    //worksheet.Cells[fila, 17].Value = "N/A";/*fecha adquisicion*/
                    //worksheet.Cells[fila, 18].Value = "N/A";/*fecha del vuelo*/
                    //worksheet.Cells[fila, 19].Value = "N/A";/*Id orden de compra*/

                    worksheet.Cells[fila, 20].Value = "0";
                    worksheet.Cells[fila, 21].Value = com.TotalViatico != null ? com.TotalViatico.ToString() : "0";
                    worksheet.Cells[fila, 22].Value = "N/A";/*fecha ida*/
                    worksheet.Cells[fila, 23].Value = "N/A";/*fecha vuelta*/
                    worksheet.Cells[fila, 24].Value = com.FechaSolicitud.ToShortDateString();
                    worksheet.Cells[fila, 25].Value = "0"; /*dias de antelacion*/
                    worksheet.Cells[fila, 26].Value = com.CometidoDescripcion;

                    var tarea = workflow.LastOrDefault().DefinicionWorkflow;
                    if (tarea.Secuencia < 9)
                        worksheet.Cells[fila, 27].Value = "Solicitado";
                    else if (tarea.Secuencia >= 9 && tarea.Secuencia < 17)
                        worksheet.Cells[fila, 27].Value = "Comprometido";
                    else if (tarea.Secuencia >= 17 && tarea.Secuencia < 20)
                        worksheet.Cells[fila, 27].Value = "Devengado";
                    else if (tarea.Secuencia >= 19)
                        worksheet.Cells[fila, 27].Value = "Pagado";
                }
            }

            return File(excelPackageCaigg.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "rptCaigg_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
        }

        public ActionResult SeguimientoUnidades()
        {
            var model = new DTOFilterCometido();
            //ViewBag.Ejecutor = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.Ejecutor = new SelectList(_repository.GetAll<DefinicionWorkflow>().Where(c => c.DefinicionProcesoId == 13 && c.NombreUsuario != null).ToList(), "NombreUsuario", "NombreUsuario");
            ViewBag.Ejecutor = new SelectList(_repository.Get<Workflow>(c => c.Proceso.DefinicionProcesoId == 13 && c.Email != null).GroupBy(c => c.Email).Select(x => x.First()).ToList(), "Email", "Email");
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

                foreach (var pro in model.Result)
                {
                    pro.Proceso = _repository.GetById<Proceso>(pro.ProcesoId);
                }
            }

            ViewBag.Ejecutor = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            return View(model);
        }

        public FileResult DownloadSeguimiento(DTOFilterCometido model)
        {
            var predicateCometido = PredicateBuilder.True<Cometido>();

            if (model.Desde.HasValue)
            {
                predicateCometido = predicateCometido.And(q =>
                q.FechaSolicitud.Year >= model.Desde.Value.Year &&
                q.FechaSolicitud.Month >= model.Desde.Value.Month &&
                q.FechaSolicitud.Day >= model.Desde.Value.Day);
            }

            if (model.Hasta.HasValue)
            {
                predicateCometido = predicateCometido.And(q =>
                q.FechaSolicitud.Year <= model.Hasta.Value.Year &&
                q.FechaSolicitud.Month <= model.Hasta.Value.Month &&
                q.FechaSolicitud.Day <= model.Hasta.Value.Day);
            }

            var result = _repository.Get(predicateCometido);

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\SeguimientoUnidades.xlsx");
            var fileInfo = new FileInfo(file);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackageSeguimientoUnidades = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackageSeguimientoUnidades.Workbook.Worksheets[0];
            foreach (var cometido in result.OrderBy(c => c.FechaSolicitud))
            {
                var workflow = _repository.Get<Workflow>(w => w.ProcesoId == cometido.ProcesoId);
                var destino = _repository.Get<Destinos>(d => d.CometidoId == cometido.CometidoId).ToList();

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
                        worksheet.Cells[fila, 9].Value = w.FechaTermino.Value - w.FechaCreacion;
                    }
                    else
                    {
                        var hoy = DateTime.Now;
                        worksheet.Cells[fila, 9].Value = hoy.Day - w.FechaCreacion.Day;
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
            //var predicate = PredicateBuilder.True<Cometido>();


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
            foreach (var cometido in result.ToList().OrderByDescending(c => c.CometidoId))
            {
                //var workflow = _repository.GetAll<Workflow>().Where(w => w.ProcesoId == cometido.ProcesoId);
                var destino = _repository.GetAll<Destinos>().Where(d => d.CometidoId == cometido.CometidoId).ToList();

                if (destino.Count > 0)
                {
                    fila++;
                    worksheet.Cells[fila, 1].Value = cometido.Rut + "-" + cometido.DV;
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
                var workflow = _repository.Get<Workflow>(w => w.ProcesoId == cometido.ProcesoId);
                var destino = _repository.Get<Destinos>(d => d.CometidoId == cometido.CometidoId).ToList();

                if (destino.Count > 0)
                {
                    fila++;
                    worksheet.Cells[fila, 1].Value = cometido.UnidadDescripcion.Contains("Turismo") ? "Turismo" : "Economía";
                    worksheet.Cells[fila, 2].Value = cometido.CometidoId.ToString();
                    worksheet.Cells[fila, 3].Value = cometido.NombreCometido;
                    worksheet.Cells[fila, 4].Value = cometido.FechaSolicitud.ToShortDateString();
                    worksheet.Cells[fila, 5].Value = cometido.Nombre != null ? cometido.Nombre.Trim() : "S/A";
                    //worksheet.Cells[fila, 6].Value = cometido.Proceso.Terminada == false && cometido.Proceso.Anulada == false ? "En Proceso" : cometido.Workflow.Terminada == true ? "Terminada" : "Anulada";

                    if (cometido.Proceso.EstadoProcesoId == (int)Util.Enum.EstadoProceso.EnProceso)
                        worksheet.Cells[fila, 6].Value = "En Curso";
                    else if (cometido.Proceso.EstadoProcesoId == (int)Util.Enum.EstadoProceso.Terminado)
                        worksheet.Cells[fila, 6].Value = "Terminada";
                    else if (cometido.Proceso.EstadoProcesoId == (int)Util.Enum.EstadoProceso.Anulado)
                        worksheet.Cells[fila, 6].Value = "Anulada";

                    worksheet.Cells[fila, 7].Value = cometido.Folio != null ? cometido.Folio : "S/A";
                    worksheet.Cells[fila, 8].Value = cometido.TipoActoAdministrativo != null ? cometido.TipoActoAdministrativo : "S/A";
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

            return File(excelPackageTransparencia.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "rptSolicitudTransparencia_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
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
                var workflow = _repository.GetFirst<Workflow>(w => w.ProcesoId == cometido.ProcesoId && w.DefinicionWorkflow.Secuencia == 9);
                var cdp = _repository.GetFirst<GeneracionCDP>(w => w.CometidoId == cometido.CometidoId);
                var destino = _repository.GetAll<Destinos>().Where(d => d.CometidoId == cometido.CometidoId).ToList();

                if (cdp != null)
                {
                    fila++;
                    worksheet.Cells[fila, 1].Value = cometido.UnidadDescripcion.Contains("Turismo") ? "Turismo" : "Economía";
                    worksheet.Cells[fila, 2].Value = cometido.CometidoId.ToString();
                    worksheet.Cells[fila, 3].Value = cdp.VtcIdCompromiso != null ? cdp.VtcIdCompromiso : "S/A";
                    worksheet.Cells[fila, 4].Value = cdp.VtcTipoSubTituloId.HasValue && cdp.VtcTipoItemId.HasValue ? cdp.TipoSubTitulo.TstNombre + "." + cdp.TipoItem.TitNombre : "S/A";
                    worksheet.Cells[fila, 5].Value = workflow != null && workflow.FechaTermino.HasValue ? workflow.FechaTermino.Value.ToShortDateString() : "S/A";
                    worksheet.Cells[fila, 6].Value = cometido.Rut + "-" + cometido.DV;
                    worksheet.Cells[fila, 7].Value = cometido.Nombre != null ? cometido.Nombre : "S/A";
                    if (cometido.UnidadDescripcion.Trim() == "SECRETARÍA REGIONAL MINISTERIAL DE ARICA Y PARINACOTA")
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

                    if (destino.Count > 0)
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

            return File(excelPackagePresupuesto.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "rptPresupuesto_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
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

                predicate = predicate.And(q => q.Activo == true);/*Selecciona los cometidos q esten activos y que no hayan sido anulados*/

                predicate = predicate.And(q => q.Proceso.EstadoProcesoId == (int)Util.Enum.EstadoProceso.Terminado); /*selecciona los cometidos que se encuentre terminados*/

                model.Result = _repository.Get(predicate);
            }

            var xdoc = new XDocument(new XElement("LISTADOCUMENTOS", model.Result.Select(w => new XElement("DOCUMENTO",
                                                new XElement("RUN", w.Rut),
                                                new XElement("DIGITO_VERIFICADOR", w.DV),
                                                new XElement("TIPO_DOCUMENTO", w.TipoActoAdministrativo != null ? w.TipoActoAdministrativo.Contains("Ministerial") ? "DECRETO EXENTO" : "RESOLUCION EXENTA" : " "),
                                                new XElement("NUMERO_DOCUMENTO", w.CometidoId),
                                                new XElement("FECHA_DOCUMENTO", w.FechaSolicitud.ToString("dd/MM/yyyy").Replace("-", "/")),//.ToShortDateString("dd/mm/yyyy")),
                                                new XElement("SERVICIO_EMISOR", "119246"),
                                                new XElement("DEPENDENCIA_EMISORA", "119247"),
                                                new XElement("SERVICIO_DESTINO", "119247"),
                                                new XElement("DEPENDENCIA_DESTINO", "119247"),
                                                new XElement("REGION_DESTINO", w.Destinos.Any() ? RegionComunaContraloria.Where(r => r.REGIÓN.Contains(w.Destinos.FirstOrDefault().RegionDescripcion.Trim())).FirstOrDefault().CODIGOREGION.ToString() : "S/A"),
                                                new XElement("COMUNA_DESTINO", w.Destinos.Any() ? RegionComunaContraloria.Where(r => r.COMUNA.Contains(w.Destinos.FirstOrDefault().ComunaDescripcion.Trim())).FirstOrDefault().CODIGOCOMUNA.ToString() : "S/A"),
                                                new XElement("FECHA_DESDE", w.Destinos.Count > 0 ? w.Destinos.FirstOrDefault().FechaInicio.ToString("dd/MM/yyyy").Replace("-", "/") : "S/A"),
                                                new XElement("FECHA_HASTA", w.Destinos.Count > 0 ? w.Destinos.FirstOrDefault().FechaHasta.ToString("dd/MM/yyyy").Replace("-", "/") : "S/A"),
                                                new XElement("MOTIVO_COMETIDO_FUNCIONARIO", "Reunión fuera del servicio" /*w.NombreCometido != null ? w.NombreCometido.Trim() : "S/A"*/),
                                                new XElement("TIENE_BENEFICIOS",
                                                                new XElement("SELECCIONE_BENEFICIOS",
                                                                new XElement("PASAJE", w.ReqPasajeAereo ? "SI" : "NO"),
                                                                new XElement("VIATICO", w.SolicitaViatico ? "SI" : "NO"),
                                                                new XElement("ALOJAMIENTO", w.Alojamiento ? "SI" : "NO"))
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
            var writer = XmlWriter.Create(stream, settings);
            writer.WriteStartDocument(true);
            writer.WriteRaw(xmldoc.InnerXml);
            writer.Close();
            stream.Position = 0;
            var fileStreamResult = File(stream, "application/xml", "ReporteContraloria - " + DateTime.Now.ToString("dd-MM-yyyy HH:mm") + ".xml");
            return fileStreamResult;
        }

        public FileResult SeguimientoFinanzas()
        {
            var result = _repository.GetAll<Cometido>();

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\SeguimientoFinanzas.xlsx");
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
                worksheet.Cells[fila, 1].Value = cometido.CometidoId.ToString();
                worksheet.Cells[fila, 2].Value = cometido.Nombre;
                worksheet.Cells[fila, 3].Value = cometido.UnidadDescripcion;
                worksheet.Cells[fila, 4].Value = cometido.FechaSolicitud.ToShortDateString();
                worksheet.Cells[fila, 5].Value = cometido.TotalViatico.HasValue ? cometido.TotalViatico.ToString() : "S/A";
                /*Datos desde destinos*/
                if (destino.Count > 0)
                {
                    worksheet.Cells[fila, 6].Value = destino != null ? destino.LastOrDefault().Dias40Aprobados.ToString() : "S/A";
                    worksheet.Cells[fila, 7].Value = destino != null ? destino.LastOrDefault().Dias60Aprobados.ToString() : "S/A";
                    worksheet.Cells[fila, 8].Value = destino != null ? destino.LastOrDefault().Dias100Aprobados.ToString() : "S/A";
                    worksheet.Cells[fila, 9].Value = destino != null ? destino.LastOrDefault().Dias50Aprobados.ToString() : "S/A";
                    worksheet.Cells[fila, 10].Value = destino != null ? destino.LastOrDefault().Dias00Aprobados.ToString() : "S/A";
                    //fila++;
                }

                worksheet.Cells[fila, 12].Value = !string.IsNullOrEmpty(cometido.NombreFuncionarioPagador) ? cometido.NombreFuncionarioPagador : "S/A";
                worksheet.Cells[fila, 13].Value = !string.IsNullOrEmpty(cometido.IdSigfe) ? cometido.IdSigfe : "S/A";
                worksheet.Cells[fila, 14].Value = cometido.FechaPagoSigfe.HasValue ? cometido.FechaPagoSigfe.Value.ToShortDateString() : "S/A";

                worksheet.Cells[fila, 16].Value = !string.IsNullOrEmpty(cometido.NombreFuncionarioPagadorTesoreria) ? cometido.NombreFuncionarioPagadorTesoreria : "S/A";
                worksheet.Cells[fila, 17].Value = !string.IsNullOrEmpty(cometido.IdSigfeTesoreria) ? cometido.IdSigfeTesoreria : "S/A";
                worksheet.Cells[fila, 18].Value = cometido.FechaPagoSigfeTesoreria.HasValue ? cometido.FechaPagoSigfeTesoreria.Value.ToShortDateString() : "S/A";

                /*se busca la fecha de creacion de las tareas*/
                foreach (var w in workflow)
                {
                    if (w.DefinicionWorkflowId == 82)
                        worksheet.Cells[fila, 11].Value = w.FechaCreacion != null ? w.FechaCreacion.ToShortDateString() : "S/A"; /*fecha ingreso a analista de contabilidad*/

                    if (w.DefinicionWorkflowId == 84)
                        worksheet.Cells[fila, 15].Value = w.FechaCreacion != null ? w.FechaCreacion.ToShortDateString() : "S/A"; /*fecha ingreso a analista de tesorerria*/
                }
            }


            return File(excelPackageSeguimientoUnidades.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "rptSeguimientoFinanzas_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
        }

        public FileResult CDCFinanzas(DTOFilterWorkflow model, DTOFilterCometido _model)
        {
            /*se buscan las tareas asociadas a la tareas de analista de contabilidad, segun la fecha establecida*/
            var predicateWorkflow = PredicateBuilder.True<Workflow>();
            if (model.Desde.HasValue)
                predicateWorkflow = predicateWorkflow.And(q =>
                    q.FechaCreacion.Year >= model.Desde.Value.Year &&
                    q.FechaCreacion.Month >= model.Desde.Value.Month &&
                    q.FechaCreacion.Day >= model.Desde.Value.Day);

            if (model.Hasta.HasValue)
                predicateWorkflow = predicateWorkflow.And(q =>
                    q.FechaCreacion.Year <= model.Hasta.Value.Year &&
                    q.FechaCreacion.Month <= model.Hasta.Value.Month &&
                    q.FechaCreacion.Day <= model.Hasta.Value.Day);

            predicateWorkflow = predicateWorkflow.And(q => q.DefinicionWorkflowId == 84
            /*|| q.DefinicionWorkflowId == 84 || q.DefinicionWorkflowId == 86*/
            && q.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada && !q.Anulada); /*se valida que el flujo tenga la tarae de aprobacion analista contabilidad y quer esta este aprobada*/

            var WorkflowId = model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
            if (WorkflowId.Any())
                predicateWorkflow = predicateWorkflow.And(q => WorkflowId.Contains(q.WorkflowId));

            model.Result = _repository.Get(predicateWorkflow);


            /*Se buscan los cometidos asociados a las tareas antes encontradas.*/
            //var predicate = PredicateBuilder.True<Cometido>();

            ////if (_model.Desde.HasValue)
            ////    predicate = predicate.And(q =>
            ////        q.FechaSolicitud.Year >= _model.Desde.Value.Year &&
            ////        q.FechaSolicitud.Month >= _model.Desde.Value.Month &&
            ////        q.FechaSolicitud.Day >= _model.Desde.Value.Day);

            ////if (_model.Hasta.HasValue)
            ////    predicate = predicate.And(q =>
            ////        q.FechaSolicitud.Year <= _model.Hasta.Value.Year &&
            ////        q.FechaSolicitud.Month <= _model.Hasta.Value.Month &&
            ////        q.FechaSolicitud.Day <= _model.Hasta.Value.Day);


            //var CometidoId = _model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
            //if (CometidoId.Any())
            //    predicate = predicate.And(q =>  CometidoId.Contains(q.ProcesoId.Value));

            //_model.Result = _repository.Get(predicate);


            List<Cometido> ListCom = new List<Cometido>();
            foreach (var _com in model.Result.ToList())
            {
                var _cometido = _repository.GetFirst<Cometido>(q => q.ProcesoId == _com.ProcesoId);
                ListCom.Add(_cometido);
            }
            var result = ListCom;

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\CDCFinanzas.xlsx");
            var fileInfo = new FileInfo(file);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackageSeguimientoUnidades = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackageSeguimientoUnidades.Workbook.Worksheets[0];
            foreach (var cometido in result.ToList())//.Where(c => c.Proceso.EstadoProcesoId == (int)Util.Enum.EstadoProceso.Terminado)) //.OrderByDescending(c => c.CometidoId))
            {
                var pr = _repository.GetById<Proceso>(cometido.ProcesoId);
                if (pr.EstadoProcesoId == (int)Util.Enum.EstadoProceso.Terminado)
                {
                    //var halp = model.Result.ToList().FindAll(q => q.ProcesoId == cometido.ProcesoId);
                    //var help = model.Result.Where(q => q.ProcesoId == pr.ProcesoId);
                    var work = pr.Workflows.Where(q => q.DefinicionWorkflowId == 82 || q.DefinicionWorkflowId == 84 || q.DefinicionWorkflowId == 86);
                    work = work.Where(q => !q.Anulada);

                    /*var workflow = _repository.GetAll<Workflow>().Where(w => w.ProcesoId == cometido.ProcesoId);*/
                    DateTime? inicio = null;
                    DateTime? fin = null;

                    fila++;
                    worksheet.Cells[fila, 1].Value = cometido.CometidoId.ToString();
                    worksheet.Cells[fila, 2].Value = cometido.Nombre;
                    worksheet.Cells[fila, 3].Value = cometido.FechaSolicitud.ToShortDateString();
                    worksheet.Cells[fila, 4].Value = cometido.SolicitaViatico.ToString();// cometido.SolicitaViatico != null ? cometido.SolicitaViatico.ToString() : "S/A"; /*solicita viatico*/
                    worksheet.Cells[fila, 5].Value = cometido.TotalViatico.HasValue ? cometido.TotalViatico.ToString() : "S/A";
                    worksheet.Cells[fila, 7].Value = !string.IsNullOrEmpty(cometido.IdSigfe) ? cometido.IdSigfe : "S/A";
                    worksheet.Cells[fila, 8].Value = cometido.FechaPagoSigfe.HasValue ? cometido.FechaPagoSigfe.Value.ToShortDateString() : "S/A";

                    //worksheet.Cells[fila, 16].Value = !string.IsNullOrEmpty(cometido.NombreFuncionarioPagadorTesoreria) ? cometido.NombreFuncionarioPagadorTesoreria : "S/A";
                    worksheet.Cells[fila, 10].Value = !string.IsNullOrEmpty(cometido.IdSigfeTesoreria) ? cometido.IdSigfeTesoreria : "S/A";
                    worksheet.Cells[fila, 11].Value = cometido.FechaPagoSigfeTesoreria.HasValue ? cometido.FechaPagoSigfeTesoreria.Value.ToShortDateString() : "S/A";

                    /*se busca la fecha de creacion de las tareas*/
                    foreach (var w in work)
                    {
                        /*se solicita validacion q las tarea posterior a las 16.00 hrs, se cuenten para el dia sgte*/
                        //if (w.DefinicionWorkflowId == 82)
                        //{

                        //    if (w.FechaCreacion.Hour >= 16 && w.FechaCreacion.Minute >= 01)
                        //    {
                        //        worksheet.Cells[fila, 6].Value = w.FechaCreacion != null ? w.FechaCreacion.AddDays(1).ToString() : "S/A"; /*fecha ingreso a analista de contabilidad*/
                        //        inicio = w.FechaCreacion != null ? w.FechaCreacion.AddDays(1) : DateTime.Now;
                        //    }
                        //    else
                        //    {
                        //        worksheet.Cells[fila, 6].Value = w.FechaCreacion != null ? w.FechaCreacion.ToString() : "S/A"; /*fecha ingreso a analista de contabilidad*/
                        //        inicio = w.FechaCreacion != null ? w.FechaCreacion : DateTime.Now;
                        //    }

                        //}

                        if (w.DefinicionWorkflowId == 82)
                        {
                            worksheet.Cells[fila, 6].Value = w.FechaCreacion != null ? w.FechaCreacion.ToShortDateString() : "S/A"; /*fecha ingreso a analista de contabilidad*/
                            inicio = w.FechaCreacion != null ? w.FechaCreacion.Date.AddDays(1) : DateTime.Now;
                        }

                        if (w.DefinicionWorkflowId == 84)
                            worksheet.Cells[fila, 9].Value = w.FechaCreacion != null ? w.FechaCreacion.ToShortDateString() : "S/A"; /*fecha ingreso a analista de tesorerria*/

                        if (w.DefinicionWorkflowId == 86)
                        {
                            worksheet.Cells[fila, 12].Value = w.FechaTermino != null ? w.FechaTermino.Value.ToShortDateString() : "S/A"; /*fecha termino a analista de finznzas*/
                            fin = w.FechaTermino != null ? w.FechaTermino.Value.Date : DateTime.Now;
                        }

                        //worksheet.Cells[fila, 15].Value = w.WorkflowId;
                    }

                    if (inicio != null && fin != null)
                    {
                        int day = 0;
                        for (var i = inicio.Value; i <= fin.Value; i = i.AddDays(1))
                        {
                            if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday && !Util.Enum.Feriados.Any(q => q.Date == i.Date))
                            {
                                day++;
                            }
                        }

                        worksheet.Cells[fila, 13].Value = day.ToString(); /*dias transcurridos - habiles*/

                        //worksheet.Cells[fila, 14].Value = inicio.Value.Date;// pr.ProcesoId;
                        //worksheet.Cells[fila, 15].Value = fin.Value.Date;
                    }
                    else
                        worksheet.Cells[fila, 13].Value = "S/A"; /*dias transcurridos - habiles*/


                    //worksheet.Cells[fila, 14].Value = pr.ProcesoId;

                }
            }


            return File(excelPackageSeguimientoUnidades.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "rptCDCFinanzas_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
        }

        //public decimal? MinutosPermanencia
        //{
        //    get
        //    {
        //        if (!FechaTermino.HasValue)
        //            return null;

        //        int minutes = 0;
        //        for (var i = FechaCreacion; i <= FechaTermino.Value; i = i.AddMinutes(1))
        //            if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday && !feriados.Any(q => q.Date == i.Date))
        //                if (i.TimeOfDay.Hours >= 9 && i.TimeOfDay.Hours <= 18)
        //                    minutes++;

        //        return minutes;
        //    }
        //}

        //private List<DateTime> feriados = new List<DateTime>() {
        //new DateTime(2020,09,18),
        //new DateTime(2020,09,19),
        //new DateTime(2020,10,12),
        //new DateTime(2020,10,25),
        //new DateTime(2020,10,31),
        //new DateTime(2020,11,01),
        //new DateTime(2020,11,29),
        //new DateTime(2020,12,08),
        //new DateTime(2020,12,25),
        //new DateTime(2021,01,01),
        //};

        public ActionResult Finalizados()
        {
            var predicate = PredicateBuilder.True<Cometido>();
            var model = new DTOFilterCometido();

            if (ModelState.IsValid)
            {
                ////predicate = predicate.And(q => q.Proceso.Terminada == true && q.Proceso.EstadoProcesoId == (int)App.Util.Enum.EstadoProceso.Terminado);
                predicate = predicate.And(q => q.Proceso.EstadoProcesoId == (int)Util.Enum.EstadoProceso.Terminado);

                var CometidoId = model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
                if (CometidoId.Any())
                    predicate = predicate.And(q => CometidoId.Contains(q.CometidoId));

                model.Result = _repository.Get(predicate);
            }

            return View(model);
        }

        public FileResult FueraPlazo(DTOFilterCometido model)
        {
            var predicate = PredicateBuilder.True<Cometido>();

            if (model.Desde.HasValue)
                predicate = predicate.And(q =>
                    q.FechaSolicitud.Year >= model.Desde.Value.Year &&
                    q.FechaSolicitud.Month >= model.Desde.Value.Month &&
                    q.FechaSolicitud.Day >= model.Desde.Value.Day);

            if (model.Hasta.HasValue)
                predicate = predicate.And(q =>
                    q.FechaSolicitud.Year <= model.Hasta.Value.Year &&
                    q.FechaSolicitud.Month <= model.Hasta.Value.Month &&
                    q.FechaSolicitud.Day <= model.Hasta.Value.Day);

            var CometidoId = model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
            if (CometidoId.Any())
                predicate = predicate.And(q => CometidoId.Contains(q.CometidoId));

            model.Result = _repository.Get(predicate);



            var result = model.Result;// _repository.GetAll<Cometido>();

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\FueraPlazo.xlsx");
            var fileInfo = new FileInfo(file);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackageFueraPlazo = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackageFueraPlazo.Workbook.Worksheets[0];
            foreach (var cometido in result.ToList().OrderByDescending(c => c.CometidoId))
            {
                var destino = _repository.GetAll<Destinos>().Where(d => d.CometidoId == cometido.CometidoId).ToList();
                if (destino.Count > 0)
                {
                    if ((destino.FirstOrDefault().FechaInicio.Date - cometido.FechaSolicitud.Date).Days < 7)
                    {
                        fila++;
                        worksheet.Cells[fila, 1].Value = cometido.UnidadDescripcion;
                        worksheet.Cells[fila, 2].Value = cometido.FechaSolicitud.ToShortDateString();
                        worksheet.Cells[fila, 3].Value = cometido.SolicitaViatico.ToString();
                        worksheet.Cells[fila, 4].Value = (destino.FirstOrDefault().FechaInicio.Date - cometido.FechaSolicitud.Date).Days.ToString();
                        worksheet.Cells[fila, 5].Value = destino.FirstOrDefault().FechaInicio.ToShortDateString();
                    }
                }
            }

            return File(excelPackageFueraPlazo.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "rptFueraPlazo_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
        }

        public ActionResult Report()
        {
            var user = User.Email();
            var grupo = _repository.GetExists<Usuario>(c => c.Email == user && c.Habilitado) ? _repository.Get<Usuario>(c => c.Email == user && c.Habilitado).FirstOrDefault().GrupoId.ToString() : string.Empty;
            if (string.IsNullOrWhiteSpace(grupo))
                grupo = "0";

            ViewBag.User = User.Email();
            ViewBag.Grupo = grupo;

            return View();
        }

        public ActionResult DeleteDoc(int Id)
        {
            var model = _repository.GetById<Documento>(Id);
            var _useCaseInteractor = new UseCaseDocumento(_repository);// (_repository, _file, _folio);
            var _UseCaseResponseMessage = _useCaseInteractor.DeleteActivo(model.DocumentoId);

            if (_UseCaseResponseMessage.IsValid)
            {
                TempData["Success"] = "Operación terminada correctamente.";
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }
            else
            {
                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }
        }

        /*Graficos*/
        public ActionResult ColumnChart()
        {
            //Below code can be used to include dynamic data in Chart. Check view page and uncomment the line "dataPoints: @Html.Raw(ViewBag.DataPoints)"
            //ViewBag.DataPoints = JsonConvert.SerializeObject(DataService.GetRandomDataForCategoryAxis(20), _jsonSetting);
            List<DataPoint> _lis = new List<DataPoint>();
            List<DataPoint> _lisUnidades = new List<DataPoint>();
            List<DataPoint> _lisMeses = new List<DataPoint>();
            List<DataPoint> _lisDemora = new List<DataPoint>();
            List<DataPoint> _lisPendienteUnidades = new List<DataPoint>();
            List<DataPoint> _lisPendJuridica = new List<DataPoint>();

            /*CANTIDAD DE TAREAS ACTIVAS*/
            var tareas = _repository.Get<DefinicionWorkflow>(c => c.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje && c.Habilitado).OrderBy(c => c.Secuencia).ToList();
            foreach (var t in tareas)
            {
                var work = _repository.Get<Workflow>(c => c.DefinicionWorkflowId == t.DefinicionWorkflowId && c.Terminada == false && c.Anulada == false && c.FechaCreacion.Year == DateTime.Now.Year).Count();
                _lis.Add(new DataPoint(Convert.ToDouble(work), t.Nombre));
            }
            //ViewBag.DataPoints = JsonConvert.SerializeObject(DataService.GetRandomDataForDateTimeAxis(10), _jsonSetting);
            ViewBag.DataPoints = JsonConvert.SerializeObject(_lis, _jsonSetting);

            /*CANTIDAD DE SOLICITUDES POR UNIDADES*/
            var unidades = _sigper.GetUnidades();
            foreach (var u in unidades)
            {
                var sol = _repository.Get<Cometido>(c => c.Activo == true && c.IdUnidad.Value == u.Pl_UndCod && c.FechaSolicitud.Year == DateTime.Now.Year).Count();
                _lisUnidades.Add(new DataPoint(Convert.ToDouble(sol), u.Pl_UndDes.Trim()));
            }
            ViewBag.DataUnidades = JsonConvert.SerializeObject(_lisUnidades, _jsonSetting);

            /*CANTIDAD DE SOLICITUDES POR MES*/
            List<string> mes = System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.MonthNames.ToList();
            int m = 1;
            for (int i = 0; i < mes.Count() - 1; i++)
            {
                var solicitud = _repository.Get<Cometido>(c => c.Activo == true && c.FechaSolicitud.Month == m && c.FechaSolicitud.Year == DateTime.Now.Year).Count();
                _lisMeses.Add(new DataPoint(solicitud, mes[i]));
                m = m + 1;
            }

            ViewBag.DataMeses = JsonConvert.SerializeObject(_lisMeses, _jsonSetting);


            /*TIEMPO DE DEMORA DE EJECUCION TAREAS*/
            var demora = _repository.Get<DefinicionWorkflow>(c => c.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje && c.Habilitado).OrderBy(c => c.Secuencia).ToList();
            foreach (var t in demora)
            {
                int dif = 0;
                var work = _repository.Get<Workflow>(c => c.DefinicionWorkflowId == t.DefinicionWorkflowId && c.Terminada && c.Anulada == false).Count();
                var tar = _repository.Get<Workflow>(c => c.DefinicionWorkflowId == t.DefinicionWorkflowId && c.Terminada && c.Anulada == false).ToList();
                foreach (var d in tar)
                {
                    if (d.FechaTermino != null)
                    {
                        var res = (d.FechaTermino.Value.Date - d.FechaCreacion.Date);
                        dif += res.Days;
                    }
                }
                if (work == 0)
                    work = 1;

                var time = dif / work;
                _lisDemora.Add(new DataPoint(Convert.ToDouble(time), t.Nombre));
            }
            //ViewBag.DataPoints = JsonConvert.SerializeObject(DataService.GetRandomDataForDateTimeAxis(10), _jsonSetting);
            ViewBag.DataDemora = JsonConvert.SerializeObject(_lisDemora, _jsonSetting);

            /*TAREAS PENDIENTES POR UNIDADES*/
            var und = _sigper.GetUnidades();
            var PendientesUnidades = _repository.Get<Workflow>(c => c.Terminada == false && c.Pl_UndCod != null).OrderBy(c => c.Pl_UndCod);
            foreach (var u in und)
            {
                int cant = 0;
                foreach (var pendientes in PendientesUnidades)
                {
                    if (pendientes.Pl_UndCod == u.Pl_UndCod)
                        cant++; //cant + 1;                    
                }
                _lisPendienteUnidades.Add(new DataPoint(Convert.ToDouble(cant), u.Pl_UndDes));
            }
            ViewBag.PendientesUnidades = JsonConvert.SerializeObject(_lisPendienteUnidades, _jsonSetting);

            /*TAREAS PENDIENTES POR FUNCIONARIOS - UNIDAD JURIDICA*/
            //int IdUnd = 0;// Id.HasValue ? Id.Value : 0;// 200810 - 201910;
            //var userJuridica = _sigper.GetUserByUnidad(IdUnd);
            //var PendientesJuridica = _repository.Get<Workflow>(c => c.Terminada == false && c.Pl_UndCod == IdUnd);
            //foreach (var u in userJuridica)
            //{
            //    int c = 0;
            //    foreach (var t in PendientesJuridica)
            //    {
            //        if (t.NombreFuncionario == u.PeDatPerChq.Trim())
            //            c++;

            //    }
            //    _lisPendJuridica.Add(new DataPoint(Convert.ToDouble(c), u.PeDatPerChq.Trim()));
            //}
            //ViewBag.PendientesJuridica = JsonConvert.SerializeObject(_lisPendJuridica, _jsonSetting);

            return View();
        }

        public ActionResult ChartFuncionarios()
        {
            List<DataPoint> _lisPendJuridica = new List<DataPoint>();
            /*TAREAS PENDIENTES POR FUNCIONARIOS - UNIDAD JURIDICA*/

            int IdUnd = 0;// Id.HasValue ? Id.Value : 0;// 200810 - 201910;
            var userJuridica = _sigper.GetUserByUnidad(IdUnd);
            var PendientesJuridica = _repository.Get<Workflow>(c => c.Terminada == false && c.Pl_UndCod == IdUnd);
            foreach (var u in userJuridica)
            {
                int c = 0;
                foreach (var t in PendientesJuridica)
                {
                    if (t.NombreFuncionario == u.PeDatPerChq.Trim())
                        c++;

                }
                _lisPendJuridica.Add(new DataPoint(Convert.ToDouble(c), u.PeDatPerChq.Trim()));
            }
            ViewBag.PendientesJuridica = JsonConvert.SerializeObject(_lisPendJuridica, _jsonSetting);

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ChartFuncionarios(int? Id)
        {
            List<DataPoint> _lisPendJuridica = new List<DataPoint>();

            /*TAREAS PENDIENTES POR FUNCIONARIOS - UNIDAD JURIDICA*/

            int IdUnd = Id.HasValue ? Id.Value : 0;// 200810 - 201910;
            var userJuridica = _sigper.GetUserByUnidad(IdUnd);
            var PendientesJuridica = _repository.Get<Workflow>(c => c.Terminada == false && c.Pl_UndCod == IdUnd);
            foreach (var u in userJuridica)
            {
                int c = 0;
                foreach (var t in PendientesJuridica)
                {
                    if (t.NombreFuncionario == u.PeDatPerChq.Trim())
                        c++;

                }
                _lisPendJuridica.Add(new DataPoint(Convert.ToDouble(c), u.PeDatPerChq.Trim()));
            }

            ViewBag.PendientesJuridica = JsonConvert.SerializeObject(_lisPendJuridica, _jsonSetting);
            //JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            //return Content(JsonConvert.SerializeObject(_lisPendJuridica, _jsonSetting), "application/json");
            return View();
        }

        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
    }
}