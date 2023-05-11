using App.Core.Interfaces;
using App.Core.UseCases;
using App.Model.Core;
using App.Model.DTO;
using App.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class DenunciaIntegridadController : Controller
    {

        private readonly IGestionProcesos _repository;
        private readonly ISigper _sigper;
        private readonly IFile _file;
        private readonly IFolio _folio;
        private readonly IHsm _hsm;
        private readonly IEmail _email;
        private static List<DTODomainUser> ActiveDirectoryUsers { get; set; }

        public DenunciaIntegridadController(IGestionProcesos repository, ISigper sigper, IFile file, IFolio folio, IHsm hsm, IEmail email)
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

        // GET: Denuncia
        public ActionResult Index()
        {
            return View();
        }

        // GET: Denuncia/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Denuncia/Create
        public ActionResult Create(int WorkFlowId)
        {

            var persona = _sigper.GetUserByEmail(User.Email());
            var IdCargo = persona.Contrato.Re_ConCar;
            var cargo = string.IsNullOrEmpty(persona.Contrato.Re_ConCar.ToString().Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == persona.Contrato.Re_ConCar).FirstOrDefault().Pl_DesCar.Trim();
            var workFlow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new Denuncia()
            {
                WorkflowId = workFlow.WorkflowId,
                ProcesoId = workFlow.ProcesoId,
                NombreVictima = persona.Funcionario.PeDatPerChq,
                DescripcionUnidadVictima = persona.Unidad.Pl_UndDes,
                CorreoVictima = persona.Funcionario.Rh_Mail,
                RutVictima = persona.Funcionario.RH_NumInte,
                DvVictima = persona.Funcionario.RH_DvNuInt.ToInt(),
                IdUnidadVictima = persona.Contrato.Re_ConUni,
            };
            /*List<SelectListItem> Regiones = new List<SelectListItem>
            {
            new SelectListItem {Text = "Tarapacá", Value = "Tarapacá"},
            new SelectListItem {Text = "Antofagasta", Value = "Antofagasta"},
            new SelectListItem {Text = "Atacama", Value = "Atacama"},
            new SelectListItem {Text = "Coquimbo", Value = "Coquimbo"},
            new SelectListItem {Text = "Valparaíso", Value = "Valparaíso"},
            new SelectListItem {Text = "Libertador General Bernado O'higgins", Value = "Libertador General Bernado O'higgins"},
            new SelectListItem {Text = "Maule", Value = "Maule"},
            new SelectListItem {Text = "Biobío", Value = "Biobío"},
            new SelectListItem {Text = "Araucanía", Value = "Araucanía"},
            new SelectListItem {Text = "Los Lagos", Value = "Los Lagos"},
            new SelectListItem {Text = "General Carlos Ibáñez del Campo", Value = "General Carlos Ibáñez del Campo"},
            new SelectListItem {Text = "Magallanes y Antárica Chilena", Value = "Magallanes y Antárica Chilena"},
            new SelectListItem {Text = "Metropolitana", Value = "Metropolitana"},
            new SelectListItem {Text = "Los Ríos", Value = "Los Ríos"},
            new SelectListItem {Text = "Arica y Parinacota", Value = "Arica y Parinacota"},
            new SelectListItem {Text = "Ñuble", Value = "Ñuble"},
            };
            ViewBag.Regiones = Regiones;*/
            ViewBag.Fecha = DateTime.Now.ToString("dd-MM-yyyy");


            ViewBag.IdUnidadDenunciado = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", "Seleccione...");
            //ViewBag.UnidadDenunciado = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", "Seleccione...");
            ViewBag.NombreDenunciado = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", "Seleccione");
            //ViewBag.NombreDenunciado = new SelectList(new List<PEDATPER>().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre").OrderBy(q => q.Text);

            /*ViewBag.IdUnidadDenunciante = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", "Seleccione...");
            ViewBag.UnidadDenunciante = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", "Seleccione...");
            ViewBag.NombreDenunciante = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", "Seleccione");*/


            return View(model);
        }

        // POST: Denuncia/Create
        [HttpPost]
        public ActionResult Create(Denuncia model)
        {
            var _useCaseInteractor = new UseCaseIntegridad(_repository, _sigper, _file, _folio, _hsm, _email);

            if (model.DescripcionUnidadDenunciado.IsNullOrWhiteSpace())
            {
                model.DescripcionUnidadDenunciado = _sigper.GetUnidad(model.IdUnidadDenunciado.Value).Pl_UndDes;
            }
            var _UseCaseResponseMessage = _useCaseInteractor.DenunciaInsert(model);
            if (_UseCaseResponseMessage.IsValid)
            {
                return RedirectToAction("Edit", "DenunciaIntegridad", new { id = model.DenunciaIntegridadId });
            }


            ViewBag.IdUnidadDenunciado = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", "Seleccione...");
            ViewBag.NombreDenunciado = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", "Seleccione");

            //ViewBag.UnidadDenunciante = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", "Seleccione...");
            //ViewBag.NombreDenunciante = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", "Seleccione");
            return View(model);
        }

        // GET: Denuncia/Edit/5
        public ActionResult Edit(int id)
        {

            var model = _repository.GetById<Denuncia>(id);
            //ViewBag.Fecha = DateTime.Now.ToString("dd-MM-yyyy");

            ViewBag.IdUnidadDenunciado = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", "Seleccione...");
            //ViewBag.NombreDenunciado = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", "Seleccione");


            //ViewBag.IdUnidadDenunciado = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.IdUnidadDenunciado);
            //ViewBag.UnidadDenunciado = new SelectList(_sigper.GetUnidades()/*.Where(q => q.Pl_UndCod.ToString() == model.UnidadDenunciado)*/, "Pl_UndCod", "Pl_UndDes", model.DescripcionUnidadDenunciado);
            ViewBag.NombreDenunciado = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Nombre", "Nombre", model.NombreDenunciado);

            //ViewBag.UnidadDenunciante = new SelectList(_sigper.GetUnidades()/*.Where(q => q.Pl_UndCod.ToString() == model.UnidadDenunciado)*/, "Pl_UndCod", "Pl_UndDes", model.UnidadDenunciante);
            //ViewBag.NombreDenunciante = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Nombre", "Nombre", model.NombreDenunciante);

            return View(model);
        }

        // POST: Denuncia/Edit/5
        [HttpPost]
        public ActionResult Edit(Denuncia model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseIntegridad(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.DenunciaUpdate(model);

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
            //Revisar
            ViewBag.IdUnidadDenunciado = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.IdUnidadDenunciado);
            ViewBag.UnidadDenunciado = new SelectList(_sigper.GetUnidades()/*.Where(q => q.Pl_UndCod.ToString() == model.UnidadDenunciado)*/, "Pl_UndCod", "Pl_UndDes", model.DescripcionUnidadDenunciado);
            ViewBag.NombreDenunciado = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Nombre", "Nombre", model.NombreDenunciado);
            var proceso = _repository.GetById<Proceso>(model.ProcesoId);
            model.Proceso = proceso;
            return View(model);
        }

        // GET: Denuncia/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Denuncia/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult View(int id)
        {
            var proceso = _repository.GetById<Proceso>(id);
            var model = _repository.GetFirst<Denuncia>(q => q.ProcesoId == proceso.ProcesoId);

            ViewBag.UnidadDenunciado = new SelectList(_sigper.GetUnidades()/*.Where(q => q.Pl_UndCod.ToString() == model.UnidadDenunciado)*/, "Pl_UndCod", "Pl_UndDes", model.DescripcionUnidadDenunciado);
            ViewBag.NombreDenunciado = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Nombre", "Nombre", model.NombreDenunciado);

            return View(model);
        }

        public ActionResult ValidaCoordinador(int id)
        {
            var model = _repository.GetById<Denuncia>(id);
            ViewBag.UnidadDenunciado = new SelectList(_sigper.GetUnidades()/*.Where(q => q.Pl_UndCod.ToString() == model.UnidadDenunciado)*/, "Pl_UndCod", "Pl_UndDes", model.DescripcionUnidadDenunciado);
            ViewBag.NombreDenunciado = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Nombre", "Nombre", model.NombreDenunciado);

            //ViewBag.UnidadDenunciante = new SelectList(_sigper.GetUnidades()/*.Where(q => q.Pl_UndCod.ToString() == model.UnidadDenunciado)*/, "Pl_UndCod", "Pl_UndDes", model.UnidadDenunciante);
            //ViewBag.NombreDenunciante = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Nombre", "Nombre", model.NombreDenunciante);

            return View(model);
        }

        public ActionResult ValidaJefatura(int id)
        {
            var model = _repository.GetById<Denuncia>(id);
            //Se carga un ddl solo con opcion de Division Juridica (Economia) para asignar un funcionario de esa unidad a la denuncia
            //var unidades = _sigper.GetUnidades().Where(q => q.Pl_UndDes == "DIVISIÓN JURÍDICA (ECONOMIA)").Select(q => q.Pl_UndDes);
            //var funcionarios = _sigper.GetUserByUnidad(200810).Select(q => q.PeDatPerChq);

            ViewBag.UnidadDenunciado = new SelectList(_sigper.GetUnidades()/*.Where(q => q.Pl_UndCod.ToString() == model.UnidadDenunciado)*/, "Pl_UndCod", "Pl_UndDes", model.DescripcionUnidadDenunciado);
            ViewBag.NombreDenunciado = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Nombre", "Nombre", model.NombreDenunciado);

            //ViewBag.UnidadDenunciante = new SelectList(_sigper.GetUnidades()/*.Where(q => q.Pl_UndCod.ToString() == model.UnidadDenunciado)*/, "Pl_UndCod", "Pl_UndDes", model.UnidadDenunciante);
            //ViewBag.NombreDenunciante = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Nombre", "Nombre", model.NombreDenunciante);


            //ViewBag.Regiones = unidades;
            //ViewBag.Funcionarios = funcionarios;
            return View(model);
        }

        public ActionResult ValidaAbogado(int id, int? DefinicionwWorkflowId)
        {
            var model = _repository.GetById<Denuncia>(id);
            ViewBag.UnidadDenunciado = new SelectList(_sigper.GetUnidades()/*.Where(q => q.Pl_UndCod.ToString() == model.UnidadDenunciado)*/, "Pl_UndCod", "Pl_UndDes", model.DescripcionUnidadDenunciado);
            ViewBag.NombreDenunciado = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Nombre", "Nombre", model.NombreDenunciado);

            //ViewBag.UnidadDenunciante = new SelectList(_sigper.GetUnidades()/*.Where(q => q.Pl_UndCod.ToString() == model.UnidadDenunciado)*/, "Pl_UndCod", "Pl_UndDes", model.UnidadDenunciante);
            //ViewBag.NombreDenunciante = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Nombre", "Nombre", model.NombreDenunciante);

            return View(model);
        }

        [HttpPost]
        public ActionResult ValidaAbogado(Denuncia model)
        {
            //var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            //var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId) ?? null;
            //var definicionworkflowlist = _repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == workflowActual.Proceso.DefinicionProcesoId).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId) ?? null;
            //if ((bool)model.esInvestigador)
            //{
            //    var _useCaseInteractor = new UseCaseCore(_repository, _email, _sigper);
            //    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(workflow);
            //}

            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            var defWorkFlow = _repository.GetById<DefinicionWorkflow>(workflow.DefinicionWorkflow.DefinicionWorkflowId);
            if ((bool)!model.EsInvestigador)
            {
                model.EsInvestigador = Convert.ToBoolean(0);
                defWorkFlow.PermitirSeleccionarUnidadDestino = false;
                var _useCaseInteractor = new UseCaseCore(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.DefinicionWorkflowUpdate(defWorkFlow);


            }
            else
            {
                model.EsInvestigador = Convert.ToBoolean(1);
                defWorkFlow.PermitirSeleccionarUnidadDestino = true;
                var _useCaseInteractor = new UseCaseCore(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.DefinicionWorkflowUpdate(defWorkFlow);
            }


            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseIntegridad(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.DenunciaUpdate(model);

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


        public ActionResult ValidaInvestigador(int id)
        {
            var model = _repository.GetById<Denuncia>(id);
            ViewBag.UnidadDenunciado = new SelectList(_sigper.GetUnidades()/*.Where(q => q.Pl_UndCod.ToString() == model.UnidadDenunciado)*/, "Pl_UndCod", "Pl_UndDes", model.DescripcionUnidadDenunciado);
            ViewBag.NombreDenunciado = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Nombre", "Nombre", model.NombreDenunciado);

            //ViewBag.UnidadDenunciante = new SelectList(_sigper.GetUnidades()/*.Where(q => q.Pl_UndCod.ToString() == model.UnidadDenunciado)*/, "Pl_UndCod", "Pl_UndDes", model.UnidadDenunciante);
            //ViewBag.NombreDenunciante = new SelectList(_sigper.GetAllUsers().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).OrderBy(q => q.Nombre).Distinct().ToList(), "Nombre", "Nombre", model.NombreDenunciante);

            return View(model);
        }
    }
}
