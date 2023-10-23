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
        private readonly IGestionProcesos _repository;
        private readonly ISigper _sigper;
        private readonly IFile _file;

        public class DTOUser
        {
            public bool IsAdmin { get; set; }
            public bool IsConsultor { get; set; }
            public bool IsCometido { get; set; }

            public int Task { get; set; }
        }

        public HomeController(IGestionProcesos repository, ISigper sigper, IFile file)
        {
            _sigper = sigper;
            _repository = repository;
            _file = file;
        }
       
        public ActionResult Index()
        {
            var email = UserExtended.Email(User);
            var model = new DTOUser()
            {
                IsAdmin = _repository.GetExists<Usuario>(q => q.Habilitado && q.Email == email && q.Grupo.Nombre.Contains(Enum.Grupo.Administrador.ToString())),
                IsConsultor = _repository.GetExists<Usuario>(q => q.Habilitado && q.Email == email && q.Grupo.Nombre.Contains(Enum.Grupo.Consultor.ToString())),
                IsCometido = _repository.GetExists<Usuario>(q => q.Habilitado && q.Email == email && q.Grupo.Nombre.Contains(Enum.Grupo.Cometido.ToString()))
            };

            var user = _sigper.GetUserByEmail(email);
            var gruposEspeciales = _repository.Get<Usuario>(q => q.Habilitado && q.Email == email && !q.Grupo.Nombre.Contains(Enum.Grupo.Administrador.ToString())).Select(q => q.GrupoId).ToList();

            if (user.Funcionario == null || user.Unidad == null)
                ModelState.AddModelError(string.Empty, "No se encontró información del funcionario en el sistema Sigper");

            if (ModelState.IsValid)
            {
                //tareas no terminadas, personales y de mis grupos
                var predicatePersonal = PredicateBuilder.True<Workflow>();
                var predicateGrupal = PredicateBuilder.True<Workflow>();

                predicatePersonal = predicatePersonal.And(q => !q.Terminada && q.TareaPersonal);
                predicateGrupal = predicateGrupal.And(q => !q.Terminada && !q.TareaPersonal);

                //no es administrador, filtrar por grupo y tareas personales
                if (!_repository.GetExists<Usuario>(q => q.Habilitado && q.Email == email && q.Grupo.Nombre.Contains(Enum.Grupo.Administrador.ToString())))
                {
                    predicatePersonal = predicatePersonal.And(q => q.TareaPersonal && q.Email == email);
                    predicateGrupal = predicateGrupal.And(q => !q.TareaPersonal && (q.Pl_UndCod == user.Unidad.Pl_UndCod || gruposEspeciales.Contains(q.GrupoId.Value)));
                }

                model.Task = _repository.GetCount(predicatePersonal) + _repository.GetCount(predicateGrupal);
            }

            return View(model);
        }
    }
}