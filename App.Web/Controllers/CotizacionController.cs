using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using App.Model.Pasajes;
//using App.Model.Shared;
using App.Core.Interfaces;
using Newtonsoft.Json;
using App.Core.UseCases;
using App.Model.Core;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class CotizacionController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        private static List<App.Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }

        public CotizacionController(IGestionProcesos repository, ISIGPER sigper, IFile file)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;

            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
        }

        public class FileUpload
        {
            public FileUpload()
            {
            }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Archivo")]
            [DataType(DataType.Upload)]
            //public HttpPostedFileBase File { get; set; }
            public System.Web.HttpPostedFileBase[] File { get; set; }

            //public int ProcesoId { get; set; }
            //public int WorkflowId { get; set; }
        }

        private string GetDolar()
        {
            string apiUrl = "https://www.mindicador.cl/api";
            string jsonString = "{}";
            WebClient http = new WebClient();
            JavaScriptSerializer jss = new JavaScriptSerializer();

            http.Headers.Add(HttpRequestHeader.Accept, "application/json");
            jsonString = http.DownloadString(apiUrl);
            var indicatorsObject = jss.Deserialize<Dictionary<string, object>>(jsonString);

            Dictionary<string, Dictionary<string, string>> dailyIndicators = new Dictionary<string, Dictionary<string, string>>();

            int i = 0;
            foreach (var key in indicatorsObject.Keys.ToArray())
            {
                var item = indicatorsObject[key];

                if (item.GetType().FullName.Contains("System.Collections.Generic.Dictionary"))
                {
                    Dictionary<string, object> itemObject = (Dictionary<string, object>)item;
                    Dictionary<string, string> indicatorProp = new Dictionary<string, string>();

                    int j = 0;
                    foreach (var key2 in itemObject.Keys.ToArray())
                    {
                        indicatorProp.Add(key2, itemObject[key2].ToString());
                        j++;
                    }

                    dailyIndicators.Add(key, indicatorProp);
                }
                i++;
            }

            var monto = dailyIndicators["dolar"]["valor"];
            string salida = monto;
            return salida;
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Cotizacion>();
            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetById<Cotizacion>(id);
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Cotizacion>(id);
            return View(model);
        }
        
        public ActionResult Create(int id)
        {
            /*inicializacion de atributos*/
            var model = new Cotizacion();
            var pasaje = _repository.Get<Pasaje>(p => p.PasajeId == id).FirstOrDefault();
            ViewBag.EmpresaAerolineaId = new List<SelectListItem>();
            ViewBag.EmpresaAerolineaId = new SelectList(_repository.Get<EmpresaAerolinea>(), "EmpresaAerolineaId", "NombreEmpresa");// (_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");

            var valor = GetDolar();

            //DTODolar dolar = new DTODolar();
            //dolar.Fecha = DateTime.Now.ToShortDateString();
            ////dolar.codigo = "dolar";
            ////dolar.nombre = "Dólar observado";
            ////dolar.unidad_medida = "Pesos";

            ////definir url
            //var url = "https://mindicador.cl/api/dolar/" + dolar.Fecha;
            ////definir cliente http
            //var clientehttp = new WebClient();
            //clientehttp.Headers[HttpRequestHeader.ContentType] = "application/json";
            ////invocar metodo remoto
            ////string result = clientehttp.UploadString(url, "POST", JsonConvert.SerializeObject(dolar));
            //var result = clientehttp.UploadString(url, "POST", JsonConvert.SerializeObject(dolar));
            ////convertir resultado en objeto 
            //var obj = JsonConvert.DeserializeObject<App.App.Model.DTO.DTODolar>(result);
            ////verificar resultado
            //if (obj.status == "OK")
            //{
            //    model.TipoCambio = dolar.ValorDolar;
            //}
            //if (obj.status == "ERROR")
            //{
            //    TempData["Error"] = obj.error;
            //    //return View(DTOFolio);
            //}

            model.PasajeId = id;
            model.TipoCambio = float.Parse(valor);
            model.FechaTipoCambio = DateTime.Now.Date;
            model.VencimientoCotizacion = DateTime.Now.Date;
            model.Pasaje = pasaje;

            return View(model);
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cotizacion model) 
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.CotizacionInsert(model);
                //var _UseCaseResponseMessage2 = _useCaseInteractor.CotizacionDocumentoInsert(File);

                if (_UseCaseResponseMessage.Warnings.Count > 0 )
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    /*si la cotizacion se guarda ok, se guardan los archivos asociados*/
                    var email = UserExtended.Email(User);
                    if (Request.Files.Count == 0)
                        ModelState.AddModelError(string.Empty, "Debe adjuntar un archivo.");

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var files = Request.Files[i];
                        var target = new MemoryStream();
                        files.InputStream.CopyTo(target);

                        var data = _file.BynaryToText(target.ToArray());

                        var doc = new CotizacionDocumento();
                        doc.CotizacionId = model.CotizacionId.Value;
                        doc.Fecha = DateTime.Now;
                        doc.Email = email;
                        doc.FileName = files.FileName;
                        doc.File = target.ToArray();
                        //doc.ProcesoId = model.ProcesoId.Value;
                        //doc.WorkflowId = model.WorkflowId.Value;
                        doc.Signed = false;
                        doc.Texto = data.Text;
                        doc.Metadata = data.Metadata;
                        doc.Type = data.Type;
                        doc.TipoPrivacidadId = 1;

                        _repository.Create(doc);
                        _repository.Save();
                    }

                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("EditAbast", "Pasaje", new { model.WorkflowId, id = model.PasajeId });
                }
                else
                {
                    TempData["Error"] = _UseCaseResponseMessage.Errors;
                }
            }
            ViewBag.EmpresaAerolineaId = new SelectList(_repository.Get<EmpresaAerolinea>(), "EmpresaAerolineaId", "NombreEmpresa");

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            List<SelectListItem> ClasePasaje = new List<SelectListItem>
            {
                new SelectListItem {Text = "Ejecutiva", Value = "Ejecutiva"},
                new SelectListItem {Text = "Económica", Value = "Económica"},
                new SelectListItem {Text = "Turista", Value = "Turista"},
                new SelectListItem {Text = "Otro (detallar)", Value = "Otro"},
            };

            List<SelectListItem> MecanismoCompra = new List<SelectListItem>
            {
                new SelectListItem {Text = "Convenio marco", Value = "Convenio marco"},
                new SelectListItem {Text = "Licitación pública", Value = "Licitación pública"},
                new SelectListItem {Text = "Trato directo", Value = "Trato directo"},
                new SelectListItem {Text = "Otros gastos menores", Value = "Otros gastos menores"},
                new SelectListItem {Text = "Otro (detallar)", Value = "Otro"},
            };

            ViewBag.ClasePasaje = new SelectList(ClasePasaje, "Value", "Text");
            ViewBag.FormaAdquisicion = new SelectList(MecanismoCompra, "Value", "Text");


            var model = _repository.GetById<Cotizacion>(id);
            ViewBag.EmpresaAerolineaId = new List<SelectListItem>();
            ViewBag.EmpresaAerolineaId = new SelectList(_repository.Get<EmpresaAerolinea>(), "EmpresaAerolineaId", "NombreEmpresa");

            //string valor = model.TipoCambio.ToString();
            //valor.Replace(',', '.');
            //model.TipoCambio = float.Parse(valor);

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Cotizacion model)
        {
            ViewBag.EmpresaAerolineaId = new SelectList(_repository.Get<EmpresaAerolinea>(), "EmpresaAerolineaId", "NombreEmpresa");

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.CotizacionUpdate(model);
                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {                    
                    TempData["Success"] = "Operación terminada correctamente.";
                    /*se devuelve a la tarea que llamo el metodo*/
                    var pas = _repository.Get<Pasaje>(c => c.PasajeId == model.PasajeId).FirstOrDefault();
                    var pro = _repository.Get<Workflow>(p => p.ProcesoId == pas.ProcesoId).Where(c => c.DefinicionWorkflow.Secuencia == 5);
                    if(pro.Count() > 0)
                        return RedirectToAction("EditSeleccion", "Pasaje", new { model.WorkflowId, id = model.PasajeId });
                    else
                        return RedirectToAction("EditAbast", "Pasaje", new { model.WorkflowId, id = model.PasajeId });

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
            var model = _repository.GetById<Cotizacion>(id);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            var model = _repository.GetById<Cotizacion>(id);
            var pasajeId = model.Pasaje.PasajeId;
            var _UseCaseResponseMessage = _useCaseInteractor.CotizacionDelete(id);


            if (_UseCaseResponseMessage.IsValid)
            {
                TempData["Success"] = "Operación terminada correctamente.";
                return RedirectToAction("EditAbast", "Pasaje", new { id = pasajeId });
            }


            foreach (var item in _UseCaseResponseMessage.Errors)
            {
                ModelState.AddModelError(string.Empty, item);
            }
            return View(model);
        }
    }
}