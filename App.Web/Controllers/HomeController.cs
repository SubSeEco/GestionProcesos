using System.Linq;
using System.Web.Mvc;
using App.Core.Interfaces;
using App.Util;
using App.Model.Core;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class HomeController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IHSM _hsm;

        public class DTOUser
        {
            public bool IsAdmin { get; set; } = false;
            public bool IsConsultor { get; set; } = false;
            public bool IsCometido { get; set; } = false;

            public int Task { get; set; } = 0;
        }

        public HomeController(IGestionProcesos repository, ISIGPER sigper, IHSM hsm)
        {
            _sigper = sigper;
            _repository = repository;
            _hsm = hsm;
        }

        public ActionResult Index() {

            //foreach (var item in _repository.Get<Proceso>())
            //{
            //    var unidad = _sigper.GetUserByEmail(item.Email);
            //    if (unidad != null && unidad.Unidad != null)
            //    {
            //        item.Pl_UndCod = unidad.Unidad.Pl_UndCod;
            //        item.Pl_UndDes = unidad.Unidad.Pl_UndDes;
            //    }
            //}
            //_repository.Save();



            var email = UserExtended.Email(User);
            var model = new DTOUser()
            {
                IsAdmin = _repository.GetExists<Usuario>(q => q.Habilitado && q.Email == email && q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Administrador.ToString())),
                IsConsultor = _repository.GetExists<Usuario>(q => q.Habilitado && q.Email == email && q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Consultor.ToString())),
                IsCometido = _repository.GetExists<Usuario>(q => q.Habilitado && q.Email == email && q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Cometido.ToString()))
            };

            var user = _sigper.GetUserByEmail(email);
            var gruposEspeciales = _repository.Get<Usuario>(q => q.Habilitado && q.Email == email && !q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Administrador.ToString())).Select(q => q.GrupoId).ToList();

            if (user.Funcionario == null || user.Unidad == null)
                ModelState.AddModelError(string.Empty, "No se encontró información del funcionario en el sistema SIGPER");

            if (ModelState.IsValid)
            {
                //tareas no terminadas, personales y de mis grupos
                var predicatePersonal = PredicateBuilder.True<Workflow>();
                var predicateGrupal = PredicateBuilder.True<Workflow>();

                predicatePersonal = predicatePersonal.And(q => !q.Terminada && q.TareaPersonal);
                predicateGrupal = predicateGrupal.And(q => !q.Terminada && !q.TareaPersonal);

                //no es administrador, filtrar por grupo y tareas personales
                if (!_repository.GetExists<Usuario>(q => q.Habilitado && q.Email == email && q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Administrador.ToString())))
                {
                    predicatePersonal = predicatePersonal.And(q => q.TareaPersonal && q.Email == email);
                    predicateGrupal = predicateGrupal.And(q => !q.TareaPersonal && (q.Pl_UndCod == user.Unidad.Pl_UndCod || gruposEspeciales.Contains(q.GrupoId.Value)));
                }

                model.Task = _repository.GetCount(predicatePersonal) + _repository.GetCount(predicateGrupal);
            }

            return View(model);
        }

        public PartialViewResult Menu()
        {
            var email = UserExtended.Email(User);
            var model = new DTOUser()
            {
                IsAdmin = _repository.GetExists<Usuario>(q => q.Habilitado && q.Email == email && q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Administrador.ToString())),
                IsConsultor = _repository.GetExists<Usuario>(q => q.Habilitado && q.Email == email && q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Consultor.ToString()))
            };

            return PartialView(model);
        }
    }
}
