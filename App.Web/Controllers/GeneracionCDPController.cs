using System.Linq;
using System.Web.Mvc;
using App.Model.Cometido;
using App.Core.Interfaces;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class GeneracionCDPController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;

        public GeneracionCDPController(IGestionProcesos repository,ISIGPER sigper)
        {
            _repository = repository;
            _sigper = sigper;
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<GeneracionCDP>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<GeneracionCDP>(id);
            return View(model);
        }

        public ActionResult Create(int CometidoId)
        {
            var com = _repository.GetById<Cometido>(CometidoId);
            var destinos = _repository.Get<Destinos>(c => c.CometidoId == com.CometidoId);

            /*Viaticos*/
            ViewBag.VtcTipoPartidaId = new SelectList(_repository.GetAll<TipoPartida>(), "TipoPartidaId", "TpaNombre");
            ViewBag.VtcTipoCapituloId = new SelectList(_repository.GetAll<TipoCapitulo>(), "TipoCapituloId", "TcaNombre");
            //ViewBag.VtcCentroCostoId = new SelectList(_repository.GetAll<CentroCosto>(), "CentroCostoId", "CCNombre");
            ViewBag.VtcCentroCostoId = new SelectList(_sigper.GetREPYTs(), "RePytCod", "RePytDes");  // programa
            ViewBag.VtcTipoSubTituloId = new SelectList(_repository.GetAll<TipoSubTitulo>(), "TipoSubTituloId", "TstNombre");
            ViewBag.VtcTipoItemId = new SelectList(_repository.GetAll<TipoItem>(), "TipoItemId", "TitNombre");
            ViewBag.VtcTipoAsignacionId = new SelectList(_repository.GetAll<TipoAsignacion>(), "TipoAsignacionId", "TasNombre");
            ViewBag.VtcTipoSubAsignacionId = new SelectList(_repository.GetAll<TipoSubAsignacion>(), "TipoSubAsignacionId", "TsaNombre");
            /*pasajes*/
            ViewBag.PsjTipoPartidaId = new SelectList(_repository.GetAll<TipoPartida>(), "TipoPartidaId", "TpaNombre");
            ViewBag.PsjVtcTipoCapituloId = new SelectList(_repository.GetAll<TipoCapitulo>(), "TipoCapituloId", "TcaNombre");
            //ViewBag.PsjCentroCostoId = new SelectList(_repository.GetAll<CentroCosto>(), "CentroCostoId", "CCNombre");
            ViewBag.PsjCentroCostoId = new SelectList(_sigper.GetREPYTs(), "RePytCod", "RePytDes");  // programa
            ViewBag.PsjTipoSubTituloId = new SelectList(_repository.GetAll<TipoSubTitulo>(), "TipoSubTituloId", "TstNombre");
            ViewBag.PsjTipoItemId = new SelectList(_repository.GetAll<TipoItem>(), "TipoItemId", "TitNombre");
            ViewBag.PsjTipoAsignacionId = new SelectList(_repository.GetAll<TipoAsignacion>(), "TipoAsignacionId", "TasNombre");
            ViewBag.PsjTipoSubAsignacionId = new SelectList(_repository.GetAll<TipoSubAsignacion>(), "TipoSubAsignacionId", "TsaNombre");

            /*Se obtiene los valores necesarios para generar los datos del cdp*/
            int programa = com.IdPrograma.Value;
            int Idsubtitulo = programa == 5 || programa == 7 || programa == 11 || programa == 12 || programa == 16 || programa == 17 || programa == 18 || programa == 19 || programa == 20 ? 3 : 1;
            int Iditem;
            int Idasignacion = 0;
            int IdsubAsignacion = 3;

            if (programa == 21)
            {
                Iditem = 2;
                Idasignacion = 2; //004
            }                
            else if (programa == 22)
            {
                Iditem = 1;
                Idasignacion = 2; //004
            }                
            else
                Iditem = 3;

            switch (programa)
            {
                case 1:
                    Idasignacion = 1;
                    break;
                case 2:
                    Idasignacion = 1;
                    break;
                case 3:
                    Idasignacion = 1;
                    break;
                case 4:
                    Idasignacion = 1;
                    break;
                case 5:
                    Idasignacion = 4;// 472;
                    break;
                case 7:
                    Idasignacion = 12;// 477;
                    break;
                case 11:
                    Idasignacion = 15; // 214.05.008;
                    break;
                case 12:
                    Idasignacion = 1;// 001;
                    break;
                case 16:
                    Idasignacion = 16;// 413;
                    break;
                case 17:
                    Idasignacion = 17;// 611;
                    break;
                case 18:
                    Idasignacion = 18;//612;
                    break;
                case 19:
                    Idasignacion = 19;// 613;
                    break;
                case 20:
                    Idasignacion = 20;// 614;
                    break;
                case 21:
                    Idasignacion = 2;
                    break;
                case 22:
                    Idasignacion = 2;
                    break;
            }


            var model = new GeneracionCDP();
            model.CometidoId = CometidoId;
            model.Cometido = com;
            
            /*CDP Viaticos*/
            model.VtcTipoCapituloId = model.Cometido.UnidadDescripcion.Contains("Turismo") ? 2 : 1;
            model.VtcTipoPartidaId = 1;
            model.VtcCentroCostoId = programa;
            model.VtcTipoSubTituloId = Idsubtitulo;
            model.VtcTipoItemId = Iditem;
            model.VtcTipoAsignacionId = Idasignacion;
            model.VtcTipoSubAsignacionId = IdsubAsignacion;
            model.VtcIdCompromiso = "0";
            model.VtcPptoTotal = "0";
            model.VtcObligacionActual = "0";
            model.VtcSaldo = "0";
            model.VtcValorViatico = "0";
            model.VtcCodCompromiso = "0";
            model.VtcCompromisoAcumulado = "0";
            model.PsjCompromisoAcumulado = "0";

            /*CDP Pasajes*/
            model.PsjTipoPartidaId = 1;
            model.PsjVtcTipoCapituloId = model.Cometido.UnidadDescripcion.Contains("Turismo") ? 2 : 1;
            model.PsjTipoPartidaId = 1;
            model.PsjCentroCostoId = programa;
            model.PsjTipoSubTituloId = Idsubtitulo;
            model.PsjTipoItemId = Iditem;
            model.PsjTipoAsignacionId = Idasignacion;
            model.PsjTipoSubAsignacionId = IdsubAsignacion;
            model.PsjIdCompromiso = "0";
            model.PsjPptoTotal = "0";
            model.PsjObligacionActual = "0";
            model.PsjSaldo = "0";
            model.PsjValorViatico = "0";
            model.PsjCodCompromiso = "0";

            /*Obtiene el monto el viatico desde los destinos */
            if (destinos != null)
            {
                int total = 0;
                foreach (var d in destinos)
                {
                    if (d.Total != null)
                        total += d.Total.Value;
                    else
                        total += 0;
                }
                model.VtcValorViatico = total.ToString();
            }
            model.VtcObligacionActual = model.VtcValorViatico;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GeneracionCDP model)
        {
            /*Viaticos*/
            ViewBag.VtcTipoPartidaId = new SelectList(_repository.GetAll<TipoPartida>(), "TipoPartidaId", "TpaNombre");
            ViewBag.VtcTipoCapituloId = new SelectList(_repository.GetAll<TipoCapitulo>(), "TipoCapituloId", "TcaNombre");
            //ViewBag.VtcCentroCostoId = new SelectList(_repository.GetAll<CentroCosto>(), "CentroCostoId", "CCNombre");
            ViewBag.VtcCentroCostoId = new SelectList(_sigper.GetREPYTs(), "RePytCod", "RePytDes");  // programa
            ViewBag.VtcTipoSubTituloId = new SelectList(_repository.GetAll<TipoSubTitulo>(), "TipoSubTituloId", "TstNombre");
            ViewBag.VtcTipoItemId = new SelectList(_repository.GetAll<TipoItem>(), "TipoItemId", "TitNombre");
            ViewBag.VtcTipoAsignacionId = new SelectList(_repository.GetAll<TipoAsignacion>(), "TipoAsignacionId", "TasNombre");
            ViewBag.VtcTipoSubAsignacionId = new SelectList(_repository.GetAll<TipoSubAsignacion>(), "TipoSubAsignacionId", "TsaNombre");
            /*pasajes*/
            ViewBag.PsjTipoPartidaId = new SelectList(_repository.GetAll<TipoPartida>(), "TipoPartidaId", "TpaNombre");
            ViewBag.PsjVtcTipoCapituloId = new SelectList(_repository.GetAll<TipoCapitulo>(), "TipoCapituloId", "TcaNombre");
            //ViewBag.PsjCentroCostoId = new SelectList(_repository.GetAll<CentroCosto>(), "CentroCostoId", "CCNombre");
            ViewBag.PsjCentroCostoId = new SelectList(_sigper.GetREPYTs(), "RePytCod", "RePytDes");  // programa
            ViewBag.PsjTipoSubTituloId = new SelectList(_repository.GetAll<TipoSubTitulo>(), "TipoSubTituloId", "TstNombre");
            ViewBag.PsjTipoItemId = new SelectList(_repository.GetAll<TipoItem>(), "TipoItemId", "TitNombre");
            ViewBag.PsjTipoAsignacionId = new SelectList(_repository.GetAll<TipoAsignacion>(), "TipoAsignacionId", "TasNombre");
            ViewBag.PsjTipoSubAsignacionId = new SelectList(_repository.GetAll<TipoSubAsignacion>(), "TipoSubAsignacionId", "TsaNombre");

            /*Se toma usuario logeado para guardar registro del ejecutor de la tarea*/
            var persona = _sigper.GetUserByEmail(User.Email());
            model.VtcNombreId = persona.Funcionario.RH_NumInte;
            model.VtcNombre = persona.Funcionario.PeDatPerChq.Trim();
            model.PsjNombreId = persona.Funcionario.RH_NumInte;
            model.PsjNombre = persona.Funcionario.PeDatPerChq.Trim();

            Cometido com = _repository.GetById<Cometido>(model.CometidoId);
            model.CometidoId = model.CometidoId;
            model.Cometido = com;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.GeneracionCDPInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("EditPpto", "Cometido", new { model.WorkflowId, id = model.CometidoId });
                    //return RedirectToAction("Index");
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }

                //TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<GeneracionCDP>(id);

            /*Viaticos*/
            ViewBag.VtcTipoPartidaId = new SelectList(_repository.GetAll<TipoPartida>(), "TipoPartidaId", "TpaNombre",model.VtcTipoPartidaId);
            ViewBag.VtcTipoCapituloId = new SelectList(_repository.GetAll<TipoCapitulo>(), "TipoCapituloId", "TcaNombre",model.VtcTipoCapituloId);
            ViewBag.VtcCentroCostoId = new SelectList(_sigper.GetREPYTs(), "RePytCod", "RePytDes", model.VtcCentroCostoId.Value);
            ViewBag.VtcTipoSubTituloId = new SelectList(_repository.GetAll<TipoSubTitulo>(), "TipoSubTituloId", "TstNombre",model.VtcTipoSubTituloId);
            ViewBag.VtcTipoItemId = new SelectList(_repository.GetAll<TipoItem>(), "TipoItemId", "TitNombre",model.VtcTipoItemId);
            ViewBag.VtcTipoAsignacionId = new SelectList(_repository.GetAll<TipoAsignacion>(), "TipoAsignacionId", "TasNombre",model.VtcTipoAsignacionId);
            ViewBag.VtcTipoSubAsignacionId = new SelectList(_repository.GetAll<TipoSubAsignacion>(), "TipoSubAsignacionId", "TsaNombre",model.VtcTipoSubAsignacionId);
            /*pasajes*/
            ViewBag.PsjTipoPartidaId = new SelectList(_repository.GetAll<TipoPartida>(), "TipoPartidaId", "TpaNombre",model.PsjTipoPartidaId);
            ViewBag.PsjVtcTipoCapituloId = new SelectList(_repository.GetAll<TipoCapitulo>(), "TipoCapituloId", "TcaNombre",model.PsjVtcTipoCapituloId);
            ViewBag.PsjCentroCostoId = new SelectList(_sigper.GetREPYTs(), "RePytCod", "RePytDes", model.PsjCentroCostoId);
            ViewBag.PsjTipoSubTituloId = new SelectList(_repository.GetAll<TipoSubTitulo>(), "TipoSubTituloId", "TstNombre",model.PsjTipoSubTituloId);
            ViewBag.PsjTipoItemId = new SelectList(_repository.GetAll<TipoItem>(), "TipoItemId", "TitNombre",model.PsjTipoItemId);
            ViewBag.PsjTipoAsignacionId = new SelectList(_repository.GetAll<TipoAsignacion>(), "TipoAsignacionId", "TasNombre",model.PsjTipoAsignacionId);
            ViewBag.PsjTipoSubAsignacionId = new SelectList(_repository.GetAll<TipoSubAsignacion>(), "TipoSubAsignacionId", "TsaNombre",model.PsjTipoSubAsignacionId);
                                   
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GeneracionCDP model)
        {
            /*Se toma usuario logeado para guardar registro del ejecutor de la tarea*/
            var persona = _sigper.GetUserByEmail(User.Email());
            if (string.IsNullOrEmpty(model.VtcNombre))
            {
                model.VtcNombreId = persona.Funcionario.RH_NumInte;
                model.VtcNombre = persona.Funcionario.PeDatPerChq.Trim();
            }
            if (string.IsNullOrEmpty(model.PsjNombre))
            {
                model.PsjNombreId = persona.Funcionario.RH_NumInte;
                model.PsjNombre = persona.Funcionario.PeDatPerChq.Trim();
            }            

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.GeneracionCDPUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";

                    /*se traen los datos del cometido, para determinar a q tarea se debe devolver*/
                    model.Cometido = _repository.GetAll<Cometido>().Where(c =>c.CometidoId == model.CometidoId).FirstOrDefault();
                    if (model.Cometido.Workflow.DefinicionWorkflow.Secuencia == 8)
                    {
                        return RedirectToAction("EditPpto", "Cometido", new { model.WorkflowId, id = model.CometidoId });
                    }
                    else 
                    {
                        return RedirectToAction("Details", "Cometido", new { model.WorkflowId, id = model.CometidoId });
                    }
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }

            model.Cometido = _repository.GetAll<Cometido>().Where(c => c.CometidoId == model.CometidoId).FirstOrDefault();
            /*Viaticos*/
            ViewBag.VtcTipoPartidaId = new SelectList(_repository.GetAll<TipoPartida>(), "TipoPartidaId", "TpaNombre", model.VtcTipoPartidaId);
            ViewBag.VtcTipoCapituloId = new SelectList(_repository.GetAll<TipoCapitulo>(), "TipoCapituloId", "TcaNombre", model.VtcTipoCapituloId);
            ViewBag.VtcCentroCostoId = new SelectList(_sigper.GetREPYTs(), "RePytCod", "RePytDes", model.VtcCentroCostoId.Value);
            ViewBag.VtcTipoSubTituloId = new SelectList(_repository.GetAll<TipoSubTitulo>(), "TipoSubTituloId", "TstNombre", model.VtcTipoSubTituloId);
            ViewBag.VtcTipoItemId = new SelectList(_repository.GetAll<TipoItem>(), "TipoItemId", "TitNombre", model.VtcTipoItemId);
            ViewBag.VtcTipoAsignacionId = new SelectList(_repository.GetAll<TipoAsignacion>(), "TipoAsignacionId", "TasNombre", model.VtcTipoAsignacionId);
            ViewBag.VtcTipoSubAsignacionId = new SelectList(_repository.GetAll<TipoSubAsignacion>(), "TipoSubAsignacionId", "TsaNombre", model.VtcTipoSubAsignacionId);
            /*pasajes*/
            ViewBag.PsjTipoPartidaId = new SelectList(_repository.GetAll<TipoPartida>(), "TipoPartidaId", "TpaNombre", model.PsjTipoPartidaId);
            ViewBag.PsjVtcTipoCapituloId = new SelectList(_repository.GetAll<TipoCapitulo>(), "TipoCapituloId", "TcaNombre", model.PsjVtcTipoCapituloId);
            ViewBag.PsjCentroCostoId = new SelectList(_sigper.GetREPYTs(), "RePytCod", "RePytDes", model.PsjCentroCostoId);
            ViewBag.PsjTipoSubTituloId = new SelectList(_repository.GetAll<TipoSubTitulo>(), "TipoSubTituloId", "TstNombre", model.PsjTipoSubTituloId);
            ViewBag.PsjTipoItemId = new SelectList(_repository.GetAll<TipoItem>(), "TipoItemId", "TitNombre", model.PsjTipoItemId);
            ViewBag.PsjTipoAsignacionId = new SelectList(_repository.GetAll<TipoAsignacion>(), "TipoAsignacionId", "TasNombre", model.PsjTipoAsignacionId);
            ViewBag.PsjTipoSubAsignacionId = new SelectList(_repository.GetAll<TipoSubAsignacion>(), "TipoSubAsignacionId", "TsaNombre", model.PsjTipoSubAsignacionId);

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<GeneracionCDP>(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            var _UseCaseResponseMessage = _useCaseInteractor.GeneracionCDPDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index");
        }
    }
}