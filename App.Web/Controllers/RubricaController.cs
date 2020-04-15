using App.Model.Core;
using App.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Infrastructure.FirmaElock;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class RubricaController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        private static List<App.Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }

        public class FileUpload
        {
            public FileUpload()
            {
            }

            [Display(Name = "Autor")]
            public string Email { get; set; }

            [Display(Name = "Funcionario")]
            public string Funcionario { get; set; }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Archivo")]
            [DataType(DataType.Upload)]
            public HttpPostedFileBase File { get; set; }

            [Display(Name = "Identificador de firma electrónica")]
            public string IdentificadorFirma { get; set; }

            [Display(Name = "Id Proceso")]
            public string IdProceso { get; set; }

            [Display(Name = "Habilitado para firmar")]
            public bool HabilitadoFirma { get; set; }


        }

        public RubricaController(IGestionProcesos repository, ISIGPER sigper)
        {
            _repository = repository;
            _sigper = sigper;

            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
        }

        public JsonResult GetUser(string term)
        {
            var result = ActiveDirectoryUsers
               .Where(q => (q.User != null && q.User.ToLower().Contains(term.ToLower())) || (q.Email != null && q.Email.ToLower().Contains(term.ToLower())))
               .Take(25)
               .Select(c => new { id = c.Email, value = string.Format("{0} ({1})", c.User, c.Email)})
               .OrderBy(q => q.value)
               .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private signerInfo GetFirmantes()
        {
            SignFileImplClient ws = new SignFileImplClient();
            //signFileResponse respuesta = new signFileResponse();
            signerInfo firmantes = new signerInfo();
            signBase64EncodedResponse respBase64 = new signBase64EncodedResponse();

            firmantes = ws.getSignerNameList();
            return firmantes;
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Rubrica>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Rubrica>(id);
            return View(model);
        }

        public ActionResult Create()
        {
            var firmas = GetFirmantes().signer.ToList();

            ViewBag.Email = new SelectList(_sigper.GetUserByUnidad(0), "Rh_Mail", "PeDatPerChq");
            ViewBag.IdProceso = new SelectList(_repository.GetAll<DefinicionProceso>(), "DefinicionProcesoId", "Nombre");
            ViewBag.IdentificadorFirma = new SelectList(firmas, "", "");
            return View();
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FileUpload model)
        {
            if (Request.Files.Count == 0)
                ModelState.AddModelError(string.Empty, "Debe adjuntar un archivo.");

            if (ModelState.IsValid)
            {
                var file = Request.Files[0];
                var target = new MemoryStream();
                file.InputStream.CopyTo(target);

                var rubrica = new Rubrica()
                {
                    Email = model.Email.Trim(),
                    IdentificadorFirma = model.IdentificadorFirma,
                    //IdProceso = model.IdProceso,
                    HabilitadoFirma = model.HabilitadoFirma,
                    //FileName = file.FileName,
                    //File = target.ToArray()
                };

                var _useCaseInteractor = new UseCaseCore(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.RubricaInsert(rubrica);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<Rubrica>(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var model = _repository.GetById<Rubrica>(id);
            var _useCaseInteractor = new UseCaseCore(_repository);
            var _UseCaseResponseMessage = _useCaseInteractor.RubricaDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var firmasEdit = GetFirmantes().signer.ToList();


            var model = _repository.GetById<Rubrica>(id);
            ViewBag.Email = new SelectList(_sigper.GetUserByUnidad(0), "Rh_Mail", "PeDatPerChq");
            ViewBag.IdProceso = new SelectList(_repository.GetAll<DefinicionProceso>(), "DefinicionProcesoId", "Nombre");
            ViewBag.IdentificadorFirma = new SelectList(firmasEdit, "", "");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Rubrica model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.RubricaUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            return View(model);
        }
    }
}
