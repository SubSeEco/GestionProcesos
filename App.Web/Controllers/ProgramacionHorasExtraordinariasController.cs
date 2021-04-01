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
using org.mp4parser.aspectj.runtime.@internal;
//using App.Infrastructure.Extensions;
//using com.sun.corba.se.spi.ior;
//using System.Net.Mail;
//using com.sun.codemodel.@internal;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class ProgramacionHorasExtraordinariasController : Controller
    {
        public class DTOFilterProgramacionHorasExtraordinarias
        {
            public DTOFilterProgramacionHorasExtraordinarias()
            {
                TextSearch = string.Empty;
                Select = new HashSet<DTOSelect>();
                Result = new HashSet<ProgramacionHorasExtraordinarias>();
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

            [Display(Name = "Estado")]
            public int? Estado { get; set; }

            [Display(Name = "Destino")]
            public int? Destino { get; set; }

            [Display(Name = "N° Dias")]
            public int DiasDiferencia { get; set; }

            public IEnumerable<DTOSelect> Select { get; set; }
            public IEnumerable<ProgramacionHorasExtraordinarias> Result { get; set; }
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
        protected readonly IFolio _folio;
        protected readonly IHSM _hsm;
        protected readonly IEmail _email;
        private static List<App.Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }
        //public static List<Destinos> ListDestino = new List<Destinos>();

        public ProgramacionHorasExtraordinariasController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm, IEmail email)
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
            var per = _sigper.GetUserByEmail(correo);
            var IdCargo = per.DatosLaborales.RhConCar.Value;
            var cargo = string.IsNullOrEmpty(per.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == per.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidad = per.DatosLaborales.RH_ContCod;
            var calidad = string.IsNullOrEmpty(per.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == per.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGrado = string.IsNullOrEmpty(per.DatosLaborales.RhConGra.Trim()) ? "0" : per.DatosLaborales.RhConGra.Trim();
            var grado = string.IsNullOrEmpty(per.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : per.DatosLaborales.RhConGra.Trim();
            var estamento = per.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == per.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgId = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var Programa = ProgId != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgId).FirstOrDefault().RePytDes : "S/A";
            var conglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte).ReContraSed;
            var jefatura = per.Jefatura != null ? per.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreorem = per.Funcionario != null ? per.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrerem = per.Funcionario != null ? per.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

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

            return Json(new
            {
                Rut = rut,
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
                IdUnidad = per.Unidad.Pl_UndCod,
                Unidad = per.Unidad.Pl_UndDes.Trim(),
                Jefatura = jefatura,
                EmailRem = ecorreorem,
                NombreChqRem = per.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioFunc1(int RutFunc1)
        {
            var correofunc1 = _sigper.GetUserByRut(RutFunc1).Funcionario.Rh_Mail.Trim();
            var perfunc1 = _sigper.GetUserByEmail(correofunc1);
            var IdCargoFunc1 = perfunc1.DatosLaborales.RhConCar.Value;
            var cargofunc1 = string.IsNullOrEmpty(perfunc1.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perfunc1.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadFunc1 = perfunc1.DatosLaborales.RH_ContCod;
            var calidadfunc1 = string.IsNullOrEmpty(perfunc1.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perfunc1.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoFunc1 = string.IsNullOrEmpty(perfunc1.DatosLaborales.RhConGra.Trim()) ? "0" : perfunc1.DatosLaborales.RhConGra.Trim();
            var gradofunc1 = string.IsNullOrEmpty(perfunc1.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perfunc1.DatosLaborales.RhConGra.Trim();
            var estamentofunc1 = perfunc1.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perfunc1.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdFunc1 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc1.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc1.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perfunc1.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaFunc1 = ProgIdFunc1 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdFunc1).FirstOrDefault().RePytDes : "S/A";
            var conglomeradofunc1 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc1.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc1.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc1.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc1.Funcionario.RH_NumInte).ReContraSed;
            var jefaturafunc1 = perfunc1.Jefatura != null ? perfunc1.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreofunc1 = perfunc1.Funcionario != null ? perfunc1.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrefunc1 = perfunc1.Funcionario != null ? perfunc1.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutfunc1;
            if (perfunc1.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perfunc1.Funcionario.RH_NumInte.ToString();
                rutfunc1 = string.Concat("0", t);
            }
            else
            {
                rutfunc1 = perfunc1.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutFunc1 = rutfunc1,
                DVFunc1 = perfunc1.Funcionario.RH_DvNuInt.ToString(),
                IdCargoFunc1 = IdCargoFunc1,
                CargoFunc1 = cargofunc1,
                IdCalidadVisa1 = IdCalidadFunc1,
                CalidadJuridicaFunc1 = calidadfunc1,
                IdGradoFunc1 = IdGradoFunc1,
                GradoFunc1 = gradofunc1,
                EstamentoFunc1 = estamentofunc1,
                ProgramaFunc1 = ProgramaFunc1.Trim(),
                ConglomeradoFunc1 = conglomeradofunc1,
                IdUnidadFunc1 = perfunc1.Unidad.Pl_UndCod,
                UnidadFunc1 = perfunc1.Unidad.Pl_UndDes.Trim(),
                JefaturaFunc1 = jefaturafunc1,
                EmailFunc1 = ecorreofunc1,
                NombreChqFunc1 = perfunc1.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioFunc2(int RutFunc2)
        {
            var correofunc2 = _sigper.GetUserByRut(RutFunc2).Funcionario.Rh_Mail.Trim();
            var perfunc2 = _sigper.GetUserByEmail(correofunc2);
            var IdCargoFunc2 = perfunc2.DatosLaborales.RhConCar.Value;
            var cargofunc2 = string.IsNullOrEmpty(perfunc2.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perfunc2.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadFunc2 = perfunc2.DatosLaborales.RH_ContCod;
            var calidadfunc2 = string.IsNullOrEmpty(perfunc2.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perfunc2.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoFunc2 = string.IsNullOrEmpty(perfunc2.DatosLaborales.RhConGra.Trim()) ? "0" : perfunc2.DatosLaborales.RhConGra.Trim();
            var gradofunc2 = string.IsNullOrEmpty(perfunc2.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perfunc2.DatosLaborales.RhConGra.Trim();
            var estamentofunc2 = perfunc2.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perfunc2.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdFunc2 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc2.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perfunc2.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaFunc2 = ProgIdFunc2 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdFunc2).FirstOrDefault().RePytDes : "S/A";
            var conglomeradofunc2 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc2.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc2.Funcionario.RH_NumInte).ReContraSed;
            var jefaturafunc2 = perfunc2.Jefatura != null ? perfunc2.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreofunc2 = perfunc2.Funcionario != null ? perfunc2.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrefunc2 = perfunc2.Funcionario != null ? perfunc2.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutfunc2;
            if (perfunc2.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perfunc2.Funcionario.RH_NumInte.ToString();
                rutfunc2 = string.Concat("0", t);
            }
            else
            {
                rutfunc2 = perfunc2.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutFunc2 = rutfunc2,
                DVFunc2 = perfunc2.Funcionario.RH_DvNuInt.ToString(),
                IdCargoFunc2 = IdCargoFunc2,
                CargoFunc2 = cargofunc2,
                IdCalidadVisa2 = IdCalidadFunc2,
                CalidadJuridicaFunc2 = calidadfunc2,
                IdGradoFunc2 = IdGradoFunc2,
                GradoFunc2 = gradofunc2,
                EstamentoFunc2 = estamentofunc2,
                ProgramaFunc2 = ProgramaFunc2.Trim(),
                ConglomeradoFunc2 = conglomeradofunc2,
                IdUnidadFunc2 = perfunc2.Unidad.Pl_UndCod,
                UnidadFunc2 = perfunc2.Unidad.Pl_UndDes.Trim(),
                JefaturaFunc2 = jefaturafunc2,
                EmailFunc2 = ecorreofunc2,
                NombreChqFunc2 = perfunc2.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioFunc3(int RutFunc3)
        {
            var correofunc3 = _sigper.GetUserByRut(RutFunc3).Funcionario.Rh_Mail.Trim();
            var perfunc3 = _sigper.GetUserByEmail(correofunc3);
            var IdCargoFunc3 = perfunc3.DatosLaborales.RhConCar.Value;
            var cargofunc3 = string.IsNullOrEmpty(perfunc3.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perfunc3.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadFunc3 = perfunc3.DatosLaborales.RH_ContCod;
            var calidadfunc3 = string.IsNullOrEmpty(perfunc3.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perfunc3.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoFunc3 = string.IsNullOrEmpty(perfunc3.DatosLaborales.RhConGra.Trim()) ? "0" : perfunc3.DatosLaborales.RhConGra.Trim();
            var gradofunc3 = string.IsNullOrEmpty(perfunc3.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perfunc3.DatosLaborales.RhConGra.Trim();
            var estamentofunc3 = perfunc3.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perfunc3.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdFunc3 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc3.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc3.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perfunc3.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaFunc3 = ProgIdFunc3 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdFunc3).FirstOrDefault().RePytDes : "S/A";
            var conglomeradofunc3 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc3.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc3.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc3.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc3.Funcionario.RH_NumInte).ReContraSed;
            var jefaturafunc3 = perfunc3.Jefatura != null ? perfunc3.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreofunc3 = perfunc3.Funcionario != null ? perfunc3.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrefunc3 = perfunc3.Funcionario != null ? perfunc3.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutfunc3;
            if (perfunc3.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perfunc3.Funcionario.RH_NumInte.ToString();
                rutfunc3 = string.Concat("0", t);
            }
            else
            {
                rutfunc3 = perfunc3.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutFunc3 = rutfunc3,
                DVFunc3 = perfunc3.Funcionario.RH_DvNuInt.ToString(),
                IdCargoFunc3 = IdCargoFunc3,
                CargoFunc3 = cargofunc3,
                IdCalidadVisa3 = IdCalidadFunc3,
                CalidadJuridicaFunc3 = calidadfunc3,
                IdGradoFunc3 = IdGradoFunc3,
                GradoFunc3 = gradofunc3,
                EstamentoFunc3 = estamentofunc3,
                ProgramaFunc3 = ProgramaFunc3.Trim(),
                ConglomeradoFunc3 = conglomeradofunc3,
                IdUnidadFunc3 = perfunc3.Unidad.Pl_UndCod,
                UnidadFunc3 = perfunc3.Unidad.Pl_UndDes.Trim(),
                JefaturaFunc3 = jefaturafunc3,
                EmailFunc3 = ecorreofunc3,
                NombreChqFunc3 = perfunc3.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioFunc4(int RutFunc4)
        {
            var correofunc4 = _sigper.GetUserByRut(RutFunc4).Funcionario.Rh_Mail.Trim();
            var perfunc4 = _sigper.GetUserByEmail(correofunc4);
            var IdCargoFunc4 = perfunc4.DatosLaborales.RhConCar.Value;
            var cargofunc4 = string.IsNullOrEmpty(perfunc4.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perfunc4.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadFunc4 = perfunc4.DatosLaborales.RH_ContCod;
            var calidadfunc4 = string.IsNullOrEmpty(perfunc4.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perfunc4.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoFunc4 = string.IsNullOrEmpty(perfunc4.DatosLaborales.RhConGra.Trim()) ? "0" : perfunc4.DatosLaborales.RhConGra.Trim();
            var gradofunc4 = string.IsNullOrEmpty(perfunc4.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perfunc4.DatosLaborales.RhConGra.Trim();
            var estamentofunc4 = perfunc4.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perfunc4.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdFunc4 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc4.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc4.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perfunc4.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaFunc4 = ProgIdFunc4 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdFunc4).FirstOrDefault().RePytDes : "S/A";
            var conglomeradofunc4 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc4.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc4.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc4.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc4.Funcionario.RH_NumInte).ReContraSed;
            var jefaturafunc4 = perfunc4.Jefatura != null ? perfunc4.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreofunc4 = perfunc4.Funcionario != null ? perfunc4.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrefunc4 = perfunc4.Funcionario != null ? perfunc4.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutfunc4;
            if (perfunc4.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perfunc4.Funcionario.RH_NumInte.ToString();
                rutfunc4 = string.Concat("0", t);
            }
            else
            {
                rutfunc4 = perfunc4.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutFunc4 = rutfunc4,
                DVFunc4 = perfunc4.Funcionario.RH_DvNuInt.ToString(),
                IdCargoFunc4 = IdCargoFunc4,
                CargoFunc4 = cargofunc4,
                IdCalidadVisa4 = IdCalidadFunc4,
                CalidadJuridicaFunc4 = calidadfunc4,
                IdGradoFunc4 = IdGradoFunc4,
                GradoFunc4 = gradofunc4,
                EstamentoFunc4 = estamentofunc4,
                ProgramaFunc4 = ProgramaFunc4.Trim(),
                ConglomeradoFunc4 = conglomeradofunc4,
                IdUnidadFunc4 = perfunc4.Unidad.Pl_UndCod,
                UnidadFunc4 = perfunc4.Unidad.Pl_UndDes.Trim(),
                JefaturaFunc4 = jefaturafunc4,
                EmailFunc4 = ecorreofunc4,
                NombreChqFunc4 = perfunc4.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioFunc5(int RutFunc5)
        {
            var correofunc5 = _sigper.GetUserByRut(RutFunc5).Funcionario.Rh_Mail.Trim();
            var perfunc5 = _sigper.GetUserByEmail(correofunc5);
            var IdCargoFunc5 = perfunc5.DatosLaborales.RhConCar.Value;
            var cargofunc5 = string.IsNullOrEmpty(perfunc5.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perfunc5.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadFunc5 = perfunc5.DatosLaborales.RH_ContCod;
            var calidadfunc5 = string.IsNullOrEmpty(perfunc5.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perfunc5.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoFunc5 = string.IsNullOrEmpty(perfunc5.DatosLaborales.RhConGra.Trim()) ? "0" : perfunc5.DatosLaborales.RhConGra.Trim();
            var gradofunc5 = string.IsNullOrEmpty(perfunc5.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perfunc5.DatosLaborales.RhConGra.Trim();
            var estamentofunc5 = perfunc5.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perfunc5.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdFunc5 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc5.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc5.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perfunc5.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaFunc5 = ProgIdFunc5 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdFunc5).FirstOrDefault().RePytDes : "S/A";
            var conglomeradofunc5 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc5.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc5.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc5.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc5.Funcionario.RH_NumInte).ReContraSed;
            var jefaturafunc5 = perfunc5.Jefatura != null ? perfunc5.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreofunc5 = perfunc5.Funcionario != null ? perfunc5.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrefunc5 = perfunc5.Funcionario != null ? perfunc5.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutfunc5;
            if (perfunc5.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perfunc5.Funcionario.RH_NumInte.ToString();
                rutfunc5 = string.Concat("0", t);
            }
            else
            {
                rutfunc5 = perfunc5.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutFunc5 = rutfunc5,
                DVFunc5 = perfunc5.Funcionario.RH_DvNuInt.ToString(),
                IdCargoFunc5 = IdCargoFunc5,
                CargoFunc5 = cargofunc5,
                IdCalidadVisa5 = IdCalidadFunc5,
                CalidadJuridicaFunc5 = calidadfunc5,
                IdGradoFunc5 = IdGradoFunc5,
                GradoFunc5 = gradofunc5,
                EstamentoFunc5 = estamentofunc5,
                ProgramaFunc5 = ProgramaFunc5.Trim(),
                ConglomeradoFunc5 = conglomeradofunc5,
                IdUnidadFunc5 = perfunc5.Unidad.Pl_UndCod,
                UnidadFunc5 = perfunc5.Unidad.Pl_UndDes.Trim(),
                JefaturaFunc5 = jefaturafunc5,
                EmailFunc5 = ecorreofunc5,
                NombreChqFunc5 = perfunc5.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioFunc6(int RutFunc6)
        {
            var correofunc6 = _sigper.GetUserByRut(RutFunc6).Funcionario.Rh_Mail.Trim();
            var perfunc6 = _sigper.GetUserByEmail(correofunc6);
            var IdCargoFunc6 = perfunc6.DatosLaborales.RhConCar.Value;
            var cargofunc6 = string.IsNullOrEmpty(perfunc6.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perfunc6.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadFunc6 = perfunc6.DatosLaborales.RH_ContCod;
            var calidadfunc6 = string.IsNullOrEmpty(perfunc6.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perfunc6.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoFunc6 = string.IsNullOrEmpty(perfunc6.DatosLaborales.RhConGra.Trim()) ? "0" : perfunc6.DatosLaborales.RhConGra.Trim();
            var gradofunc6 = string.IsNullOrEmpty(perfunc6.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perfunc6.DatosLaborales.RhConGra.Trim();
            var estamentofunc6 = perfunc6.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perfunc6.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdFunc6 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc6.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc6.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perfunc6.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaFunc6 = ProgIdFunc6 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdFunc6).FirstOrDefault().RePytDes : "S/A";
            var conglomeradofunc6 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc6.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc6.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc6.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc6.Funcionario.RH_NumInte).ReContraSed;
            var jefaturafunc6 = perfunc6.Jefatura != null ? perfunc6.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreofunc6 = perfunc6.Funcionario != null ? perfunc6.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrefunc6 = perfunc6.Funcionario != null ? perfunc6.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutfunc6;
            if (perfunc6.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perfunc6.Funcionario.RH_NumInte.ToString();
                rutfunc6 = string.Concat("0", t);
            }
            else
            {
                rutfunc6 = perfunc6.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutFunc6 = rutfunc6,
                DVFunc6 = perfunc6.Funcionario.RH_DvNuInt.ToString(),
                IdCargoFunc6 = IdCargoFunc6,
                CargoFunc6 = cargofunc6,
                IdCalidadVisa6 = IdCalidadFunc6,
                CalidadJuridicaFunc6 = calidadfunc6,
                IdGradoFunc6 = IdGradoFunc6,
                GradoFunc6 = gradofunc6,
                EstamentoFunc6 = estamentofunc6,
                ProgramaFunc6 = ProgramaFunc6.Trim(),
                ConglomeradoFunc6 = conglomeradofunc6,
                IdUnidadFunc6 = perfunc6.Unidad.Pl_UndCod,
                UnidadFunc6 = perfunc6.Unidad.Pl_UndDes.Trim(),
                JefaturaFunc6 = jefaturafunc6,
                EmailFunc6 = ecorreofunc6,
                NombreChqFunc6 = perfunc6.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioFunc7(int RutFunc7)
        {
            var correofunc7 = _sigper.GetUserByRut(RutFunc7).Funcionario.Rh_Mail.Trim();
            var perfunc7 = _sigper.GetUserByEmail(correofunc7);
            var IdCargoFunc7 = perfunc7.DatosLaborales.RhConCar.Value;
            var cargofunc7 = string.IsNullOrEmpty(perfunc7.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perfunc7.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadFunc7 = perfunc7.DatosLaborales.RH_ContCod;
            var calidadfunc7 = string.IsNullOrEmpty(perfunc7.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perfunc7.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoFunc7 = string.IsNullOrEmpty(perfunc7.DatosLaborales.RhConGra.Trim()) ? "0" : perfunc7.DatosLaborales.RhConGra.Trim();
            var gradofunc7 = string.IsNullOrEmpty(perfunc7.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perfunc7.DatosLaborales.RhConGra.Trim();
            var estamentofunc7 = perfunc7.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perfunc7.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdFunc7 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc7.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc7.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perfunc7.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaFunc7 = ProgIdFunc7 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdFunc7).FirstOrDefault().RePytDes : "S/A";
            var conglomeradofunc7 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc7.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc7.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc7.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc7.Funcionario.RH_NumInte).ReContraSed;
            var jefaturafunc7 = perfunc7.Jefatura != null ? perfunc7.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreofunc7 = perfunc7.Funcionario != null ? perfunc7.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrefunc7 = perfunc7.Funcionario != null ? perfunc7.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutfunc7;
            if (perfunc7.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perfunc7.Funcionario.RH_NumInte.ToString();
                rutfunc7 = string.Concat("0", t);
            }
            else
            {
                rutfunc7 = perfunc7.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutFunc7 = rutfunc7,
                DVFunc7 = perfunc7.Funcionario.RH_DvNuInt.ToString(),
                IdCargoFunc7 = IdCargoFunc7,
                CargoFunc7 = cargofunc7,
                IdCalidadVisa7 = IdCalidadFunc7,
                CalidadJuridicaFunc7 = calidadfunc7,
                IdGradoFunc7 = IdGradoFunc7,
                GradoFunc7 = gradofunc7,
                EstamentoFunc7 = estamentofunc7,
                ProgramaFunc7 = ProgramaFunc7.Trim(),
                ConglomeradoFunc7 = conglomeradofunc7,
                IdUnidadFunc7 = perfunc7.Unidad.Pl_UndCod,
                UnidadFunc7 = perfunc7.Unidad.Pl_UndDes.Trim(),
                JefaturaFunc7 = jefaturafunc7,
                EmailFunc7 = ecorreofunc7,
                NombreChqFunc7 = perfunc7.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioFunc8(int RutFunc8)
        {
            var correofunc8 = _sigper.GetUserByRut(RutFunc8).Funcionario.Rh_Mail.Trim();
            var perfunc8 = _sigper.GetUserByEmail(correofunc8);
            var IdCargoFunc8 = perfunc8.DatosLaborales.RhConCar.Value;
            var cargofunc8 = string.IsNullOrEmpty(perfunc8.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perfunc8.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadFunc8 = perfunc8.DatosLaborales.RH_ContCod;
            var calidadfunc8 = string.IsNullOrEmpty(perfunc8.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perfunc8.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoFunc8 = string.IsNullOrEmpty(perfunc8.DatosLaborales.RhConGra.Trim()) ? "0" : perfunc8.DatosLaborales.RhConGra.Trim();
            var gradofunc8 = string.IsNullOrEmpty(perfunc8.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perfunc8.DatosLaborales.RhConGra.Trim();
            var estamentofunc8 = perfunc8.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perfunc8.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdFunc8 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc8.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc8.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perfunc8.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaFunc8 = ProgIdFunc8 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdFunc8).FirstOrDefault().RePytDes : "S/A";
            var conglomeradofunc8 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc8.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc8.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc8.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc8.Funcionario.RH_NumInte).ReContraSed;
            var jefaturafunc8 = perfunc8.Jefatura != null ? perfunc8.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreofunc8 = perfunc8.Funcionario != null ? perfunc8.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrefunc8 = perfunc8.Funcionario != null ? perfunc8.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutfunc8;
            if (perfunc8.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perfunc8.Funcionario.RH_NumInte.ToString();
                rutfunc8 = string.Concat("0", t);
            }
            else
            {
                rutfunc8 = perfunc8.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutFunc8 = rutfunc8,
                DVFunc8 = perfunc8.Funcionario.RH_DvNuInt.ToString(),
                IdCargoFunc8 = IdCargoFunc8,
                CargoFunc8 = cargofunc8,
                IdCalidadVisa8 = IdCalidadFunc8,
                CalidadJuridicaFunc8 = calidadfunc8,
                IdGradoFunc8 = IdGradoFunc8,
                GradoFunc8 = gradofunc8,
                EstamentoFunc8 = estamentofunc8,
                ProgramaFunc8 = ProgramaFunc8.Trim(),
                ConglomeradoFunc8 = conglomeradofunc8,
                IdUnidadFunc8 = perfunc8.Unidad.Pl_UndCod,
                UnidadFunc8 = perfunc8.Unidad.Pl_UndDes.Trim(),
                JefaturaFunc8 = jefaturafunc8,
                EmailFunc8 = ecorreofunc8,
                NombreChqFunc8 = perfunc8.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioFunc9(int RutFunc9)
        {
            var correofunc9 = _sigper.GetUserByRut(RutFunc9).Funcionario.Rh_Mail.Trim();
            var perfunc9 = _sigper.GetUserByEmail(correofunc9);
            var IdCargoFunc9 = perfunc9.DatosLaborales.RhConCar.Value;
            var cargofunc9 = string.IsNullOrEmpty(perfunc9.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perfunc9.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadFunc9 = perfunc9.DatosLaborales.RH_ContCod;
            var calidadfunc9 = string.IsNullOrEmpty(perfunc9.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perfunc9.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoFunc9 = string.IsNullOrEmpty(perfunc9.DatosLaborales.RhConGra.Trim()) ? "0" : perfunc9.DatosLaborales.RhConGra.Trim();
            var gradofunc9 = string.IsNullOrEmpty(perfunc9.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perfunc9.DatosLaborales.RhConGra.Trim();
            var estamentofunc9 = perfunc9.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perfunc9.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdFunc9 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc9.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc9.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perfunc9.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaFunc9 = ProgIdFunc9 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdFunc9).FirstOrDefault().RePytDes : "S/A";
            var conglomeradofunc9 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc9.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc9.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc9.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc9.Funcionario.RH_NumInte).ReContraSed;
            var jefaturafunc9 = perfunc9.Jefatura != null ? perfunc9.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreofunc9 = perfunc9.Funcionario != null ? perfunc9.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrefunc9 = perfunc9.Funcionario != null ? perfunc9.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutfunc9;
            if (perfunc9.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perfunc9.Funcionario.RH_NumInte.ToString();
                rutfunc9 = string.Concat("0", t);
            }
            else
            {
                rutfunc9 = perfunc9.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutFunc9 = rutfunc9,
                DVFunc9 = perfunc9.Funcionario.RH_DvNuInt.ToString(),
                IdCargoFunc9 = IdCargoFunc9,
                CargoFunc9 = cargofunc9,
                IdCalidadVisa9 = IdCalidadFunc9,
                CalidadJuridicaFunc9 = calidadfunc9,
                IdGradoFunc9 = IdGradoFunc9,
                GradoFunc9 = gradofunc9,
                EstamentoFunc9 = estamentofunc9,
                ProgramaFunc9 = ProgramaFunc9.Trim(),
                ConglomeradoFunc9 = conglomeradofunc9,
                IdUnidadFunc9 = perfunc9.Unidad.Pl_UndCod,
                UnidadFunc9 = perfunc9.Unidad.Pl_UndDes.Trim(),
                JefaturaFunc9 = jefaturafunc9,
                EmailFunc9 = ecorreofunc9,
                NombreChqFunc9 = perfunc9.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioFunc10(int RutFunc10)
        {
            var correofunc10 = _sigper.GetUserByRut(RutFunc10).Funcionario.Rh_Mail.Trim();
            var perfunc10 = _sigper.GetUserByEmail(correofunc10);
            var IdCargoFunc10 = perfunc10.DatosLaborales.RhConCar.Value;
            var cargofunc10 = string.IsNullOrEmpty(perfunc10.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perfunc10.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadFunc10 = perfunc10.DatosLaborales.RH_ContCod;
            var calidadfunc10 = string.IsNullOrEmpty(perfunc10.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perfunc10.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoFunc10 = string.IsNullOrEmpty(perfunc10.DatosLaborales.RhConGra.Trim()) ? "0" : perfunc10.DatosLaborales.RhConGra.Trim();
            var gradofunc10 = string.IsNullOrEmpty(perfunc10.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perfunc10.DatosLaborales.RhConGra.Trim();
            var estamentofunc10 = perfunc10.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perfunc10.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdFunc10 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc10.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc10.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perfunc10.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaFunc10 = ProgIdFunc10 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdFunc10).FirstOrDefault().RePytDes : "S/A";
            var conglomeradofunc10 = _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc10.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc10.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perfunc10.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perfunc10.Funcionario.RH_NumInte).ReContraSed;
            var jefaturafunc10 = perfunc10.Jefatura != null ? perfunc10.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreofunc10 = perfunc10.Funcionario != null ? perfunc10.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrefunc10 = perfunc10.Funcionario != null ? perfunc10.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutfunc10;
            if (perfunc10.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perfunc10.Funcionario.RH_NumInte.ToString();
                rutfunc10 = string.Concat("0", t);
            }
            else
            {
                rutfunc10 = perfunc10.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutFunc10 = rutfunc10,
                DVFunc10 = perfunc10.Funcionario.RH_DvNuInt.ToString(),
                IdCargoFunc10 = IdCargoFunc10,
                CargoFunc10 = cargofunc10,
                IdCalidadVisa10 = IdCalidadFunc10,
                CalidadJuridicaFunc10 = calidadfunc10,
                IdGradoFunc10 = IdGradoFunc10,
                GradoFunc10 = gradofunc10,
                EstamentoFunc10 = estamentofunc10,
                ProgramaFunc10 = ProgramaFunc10.Trim(),
                ConglomeradoFunc10 = conglomeradofunc10,
                IdUnidadFunc10 = perfunc10.Unidad.Pl_UndCod,
                UnidadFunc10 = perfunc10.Unidad.Pl_UndDes.Trim(),
                JefaturaFunc10 = jefaturafunc10,
                EmailFunc10 = ecorreofunc10,
                NombreChqFunc10 = perfunc10.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<ProgramacionHorasExtraordinarias>();
            return View(model);
        }

        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult View(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            var usuarios = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");

            ViewBag.NombreId = usuarios;
            ViewBag.NombreIdDest = usuarios;
            ViewBag.NombreIdSecre = usuarios;
            ViewBag.NombreIdVisa1 = usuarios;
            ViewBag.NombreIdVisa2 = usuarios;
            ViewBag.NombreIdVisa3 = usuarios;
            ViewBag.NombreIdVisa4 = usuarios;
            ViewBag.NombreIdVisa5 = usuarios;
            ViewBag.NombreIdVisa6 = usuarios;
            ViewBag.NombreIdVisa7 = usuarios;
            ViewBag.NombreIdVisa8 = usuarios;
            ViewBag.NombreIdVisa9 = usuarios;
            ViewBag.NombreIdVisa10 = usuarios;
            ViewBag.NombreIdAna = usuarios;
            ViewBag.NombreIdAutorizaFirma1 = usuarios;
            ViewBag.NombreIdAutorizaFirma2 = usuarios;
            ViewBag.NombreIdAutorizaFirma3 = usuarios;

            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);

            if (ModelState.IsValid)
            {
                //model.Fecha = DateTime.Now;

                //model.IdUnidad = persona.Unidad.Pl_UndCod;
                //model.UnidadDescripcion = persona.Unidad.Pl_UndDes.Trim();
                //model.Rut = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                //model.DV = persona.Funcionario.RH_DvNuInt.Trim();
                //model.NombreId = persona.Funcionario.RH_NumInte;
                //model.Nombre = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargo = persona.DatosLaborales.RhConCar.Value;
                //model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailRem = persona.Funcionario.Rh_Mail.Trim();
                //model.NombreChqRem = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCalidad = persona.DatosLaborales.RH_ContCod;
                //model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadDest = persona.Unidad.Pl_UndCod;
                //model.UnidadDescripcionDest = persona.Unidad.Pl_UndDes.Trim();
                //model.RutDest = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                //model.DVDest = persona.Funcionario.RH_DvNuInt.Trim();
                //model.NombreIdDest = persona.Funcionario.RH_NumInte;
                //model.NombreDest = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoDest = persona.DatosLaborales.RhConCar.Value;
                //model.CargoDescripcionDest = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailDest = persona.Funcionario.Rh_Mail.Trim();
                //model.NombreChqDest = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCalidadDest = persona.DatosLaborales.RH_ContCod;
                //model.CalidadDescripcionDest = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadSecre = persona.Unidad.Pl_UndCod;
                //model.UnidadDescripcionSecre = persona.Unidad.Pl_UndDes.Trim();
                //model.RutSecre = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                //model.DVSecre = persona.Funcionario.RH_DvNuInt.Trim();
                //model.NombreIdSecre = persona.Funcionario.RH_NumInte;
                //model.NombreSecre = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoSecre = persona.DatosLaborales.RhConCar.Value;
                //model.CargoDescripcionSecre = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailSecre = persona.Funcionario.Rh_Mail.Trim();
                //model.NombreChqSecre = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCalidadSecre = persona.DatosLaborales.RH_ContCod;
                //model.CalidadDescripcionSecre = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa1 = persona.Unidad.Pl_UndCod;
                //model.UnidadDescripcionVisa1 = persona.Unidad.Pl_UndDes.Trim();
                //model.RutVisa1 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                //model.DVVisa1 = persona.Funcionario.RH_DvNuInt.Trim();
                //model.NombreIdVisa1 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa1 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa1 = persona.DatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa1 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa1 = persona.Funcionario.Rh_Mail.Trim();
                //model.NombreChqVisa1 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCalidadVisa1 = persona.DatosLaborales.RH_ContCod;
                //model.CalidadDescripcionVisa1 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa2 = persona.Unidad.Pl_UndCod;
                //model.UnidadDescripcionVisa2 = persona.Unidad.Pl_UndDes.Trim();
                //model.RutVisa2 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                //model.DVVisa2 = persona.Funcionario.RH_DvNuInt.Trim();
                //model.NombreIdVisa2 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa2 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa2 = persona.DatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa2 = persona.Funcionario.Rh_Mail.Trim();
                //model.NombreChqVisa2 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCalidadVisa2 = persona.DatosLaborales.RH_ContCod;
                //model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa3 = persona.Unidad.Pl_UndCod;
                //model.UnidadDescripcionVisa3 = persona.Unidad.Pl_UndDes.Trim();
                //model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                //model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                //model.NombreIdVisa3 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa3 = persona.DatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa3 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa3 = persona.Funcionario.Rh_Mail.Trim();
                //model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCalidadVisa3 = persona.DatosLaborales.RH_ContCod;
                //model.CalidadDescripcionVisa3 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa4 = persona.Unidad.Pl_UndCod;
                //model.UnidadDescripcionVisa4 = persona.Unidad.Pl_UndDes.Trim();
                //model.RutVisa4 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                //model.DVVisa4 = persona.Funcionario.RH_DvNuInt.Trim();
                //model.NombreIdVisa4 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa4 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa4 = persona.DatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa4 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa4 = persona.Funcionario.Rh_Mail.Trim();
                //model.NombreChqVisa4 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCalidadVisa4 = persona.DatosLaborales.RH_ContCod;
                //model.CalidadDescripcionVisa4 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa5 = persona.Unidad.Pl_UndCod;
                //model.UnidadDescripcionVisa5 = persona.Unidad.Pl_UndDes.Trim();
                //model.RutVisa5 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                //model.DVVisa5 = persona.Funcionario.RH_DvNuInt.Trim();
                //model.NombreIdVisa5 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa5 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa5 = persona.DatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa5 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa5 = persona.Funcionario.Rh_Mail.Trim();
                //model.NombreChqVisa5 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCalidadVisa5 = persona.DatosLaborales.RH_ContCod;
                //model.CalidadDescripcionVisa5 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            }

            return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);
            return View(model);
        }

        public ActionResult Create(int WorkFlowId)
        {
            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new ProgramacionHorasExtraordinarias()
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
            };

            var usuarios = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");

            ViewBag.NombreId = usuarios;
            ViewBag.NombreIdFunc1 = usuarios;
            ViewBag.NombreIdFunc2 = usuarios;
            ViewBag.NombreIdFunc3 = usuarios;
            ViewBag.NombreIdFunc4 = usuarios;
            ViewBag.NombreIdFunc5 = usuarios;
            ViewBag.NombreIdFunc6 = usuarios;
            ViewBag.NombreIdFunc7 = usuarios;
            ViewBag.NombreIdFunc8 = usuarios;
            ViewBag.NombreIdFunc9 = usuarios;
            ViewBag.NombreIdFunc10 = usuarios;

            var persona = _sigper.GetUserByEmail(User.Email());


            if (ModelState.IsValid)
            {
                model.IdUnidad = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcion = persona.Unidad.Pl_UndDes.Trim();
                model.Rut = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DV = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreId = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                //model.Nombre = persona.Funcionario.PeDatPerChq.Trim();
                model.Nombre = null;
                model.IdCargo = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailRem = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqRem = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidad = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadFunc1 = persona.Unidad.Pl_UndCod;
                model.IdUnidadFunc1 = null;
                //model.UnidadDescripcionFunc1 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionFunc1 = null;
                model.RutFunc1 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVFunc1 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdFunc1 = null;
                //model.NombreIdFunc1 = persona.Funcionario.RH_NumInte;
                //model.NombreFunc1 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreFunc1 = null;
                model.IdCargoFunc1 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionFunc1 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailFunc1 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailFunc1 = null;
                model.NombreChqFunc1 = null;
                //model.NombreChqFunc1 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadFunc1 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionFunc1 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadFunc2 = persona.Unidad.Pl_UndCod;
                model.IdUnidadFunc2 = null;
                //model.UnidadDescripcionFunc2 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionFunc2 = null;
                model.RutFunc2 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVFunc2 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdFunc2 = null;
                //model.NombreIdFunc2 = persona.Funcionario.RH_NumInte;
                //model.NombreFunc2 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreFunc2 = null;
                model.IdCargoFunc2 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionFunc2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailFunc2 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailFunc2 = null;
                model.NombreChqFunc2 = null;
                //model.NombreChqFunc2 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadFunc2 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionFunc2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadFunc3 = persona.Unidad.Pl_UndCod;
                model.IdUnidadFunc3 = null;
                //model.UnidadDescripcionFunc3 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionFunc3 = null;
                model.RutFunc3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVFunc3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdFunc3 = null;
                //model.NombreIdFunc3 = persona.Funcionario.RH_NumInte;
                //model.NombreFunc3 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreFunc3 = null;
                model.IdCargoFunc3 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionFunc3 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailFunc3 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailFunc3 = null;
                model.NombreChqFunc3 = null;
                //model.NombreChqFunc3 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadFunc3 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionFunc3 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadFunc4 = persona.Unidad.Pl_UndCod;
                model.IdUnidadFunc4 = null;
                //model.UnidadDescripcionFunc4 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionFunc4 = null;
                model.RutFunc4 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVFunc4 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdFunc4 = null;
                //model.NombreIdFunc4 = persona.Funcionario.RH_NumInte;
                //model.NombreFunc4 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreFunc4 = null;
                model.IdCargoFunc4 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionFunc4 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailFunc4 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailFunc4 = null;
                model.NombreChqFunc4 = null;
                //model.NombreChqFunc4 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadFunc4 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionFunc4 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadFunc5 = persona.Unidad.Pl_UndCod;
                model.IdUnidadFunc5 = null;
                //model.UnidadDescripcionFunc5 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionFunc5 = null;
                model.RutFunc5 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVFunc5 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdFunc5 = null;
                //model.NombreIdFunc5 = persona.Funcionario.RH_NumInte;
                //model.NombreFunc5 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreFunc5 = null;
                model.IdCargoFunc5 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionFunc5 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailFunc5 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailFunc5 = null;
                model.NombreChqFunc5 = null;
                //model.NombreChqFunc5 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadFunc5 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionFunc5 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadFunc6 = persona.Unidad.Pl_UndCod;
                model.IdUnidadFunc6 = null;
                //model.UnidadDescripcionFunc6 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionFunc6 = null;
                model.RutFunc6 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVFunc6 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdFunc6 = null;
                //model.NombreIdFunc6 = persona.Funcionario.RH_NumInte;
                //model.NombreFunc6 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreFunc6 = null;
                model.IdCargoFunc6 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionFunc6 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailFunc6 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailFunc6 = null;
                model.NombreChqFunc6 = null;
                //model.NombreChqFunc6 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadFunc6 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionFunc6 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadFunc7 = persona.Unidad.Pl_UndCod;
                model.IdUnidadFunc7 = null;
                //model.UnidadDescripcionFunc7 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionFunc7 = null;
                model.RutFunc7 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVFunc7 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdFunc7 = null;
                //model.NombreIdFunc7 = persona.Funcionario.RH_NumInte;
                //model.NombreFunc7 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreFunc7 = null;
                model.IdCargoFunc7 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionFunc7 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailFunc7 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailFunc7 = null;
                model.NombreChqFunc7 = null;
                //model.NombreChqFunc7 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadFunc7 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionFunc7 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadFunc8 = persona.Unidad.Pl_UndCod;
                model.IdUnidadFunc8 = null;
                //model.UnidadDescripcionFunc8 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionFunc8 = null;
                model.RutFunc8 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVFunc8 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdFunc8 = null;
                //model.NombreIdFunc8 = persona.Funcionario.RH_NumInte;
                //model.NombreFunc8 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreFunc8 = null;
                model.IdCargoFunc8 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionFunc8 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailFunc8 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailFunc8 = null;
                model.NombreChqFunc8 = null;
                //model.NombreChqFunc8 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadFunc8 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionFunc8 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadFunc9 = persona.Unidad.Pl_UndCod;
                model.IdUnidadFunc9 = null;
                //model.UnidadDescripcionFunc9 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionFunc9 = null;
                model.RutFunc9 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVFunc9 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdFunc9 = null;
                //model.NombreIdFunc9 = persona.Funcionario.RH_NumInte;
                //model.NombreFunc9 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreFunc9 = null;
                model.IdCargoFunc9 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionFunc9 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailFunc9 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailFunc9 = null;
                model.NombreChqFunc9 = null;
                //model.NombreChqFunc9 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadFunc9 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionFunc9 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadFunc10 = persona.Unidad.Pl_UndCod;
                model.IdUnidadFunc10 = null;
                //model.UnidadDescripcionFunc10 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionFunc10 = null;
                model.RutFunc10 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVFunc10 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdFunc10 = null;
                //model.NombreIdFunc10 = persona.Funcionario.RH_NumInte;
                //model.NombreFunc10 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreFunc10 = null;
                model.IdCargoFunc10 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionFunc10 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailFunc10 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailFunc10 = null;
                model.NombreChqFunc10 = null;
                //model.NombreChqFunc10 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadFunc10 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionFunc10 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProgramacionHorasExtraordinarias model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new Core.UseCases.UseCaseProgramacionHorasExtraordinarias(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.ProgramacionHorasExtraordinariasInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    //return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
                    return RedirectToAction("GeneraDocumento", "ProgramacionHorasExtraordinarias", new { model.WorkflowId, id = model.ProgramacionHorasExtraordinariasId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            var usuarios = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");

            ViewBag.NombreId = usuarios;
            ViewBag.NombreIdFunc1 = usuarios;
            ViewBag.NombreIdFunc2 = usuarios;

            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);

            if (ModelState.IsValid)
            {
                //model.Fecha = DateTime.Now;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProgramacionHorasExtraordinarias model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseProgramacionHorasExtraordinarias(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.ProgramacionHorasExtraordinariasUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    //return Redirect(Request.UrlReferrer.PathAndQuery);
                    return RedirectToAction("GeneraDocumento", "ProgramacionHorasExtraordinarias", new { model.WorkflowId, id = model.ProgramacionHorasExtraordinariasId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            return View(model);
        }

        public ActionResult EditAnalista(int id)
        {
            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);

            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.To = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc1 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc2 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc3 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc4 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc5 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc6 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc7 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc8 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc9 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc10 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc11 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc12 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc13 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc14 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc15 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc16 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc17 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc18 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc19 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc20 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");

            if (ModelState.IsValid)
            {
                //model.Fecha = DateTime.Now;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAnalista(ProgramacionHorasExtraordinarias model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseProgramacionHorasExtraordinarias(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.ProgramacionHorasExtraordinariasUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                    //return RedirectToAction("GeneraDocumento", "ProgramacionHorasExtraordinarias", new { model.WorkflowId, id = model.ProgramacionHorasExtraordinariasId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            return View(model);
        }

        public ActionResult EditVisador(int id)
        {
            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);

            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.To = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc1 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc2 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc3 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc4 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc5 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc6 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc7 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc8 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc9 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc10 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc11 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc12 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc13 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc14 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc15 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc16 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc17 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc18 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc19 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            ViewBag.ToFunc20 = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");

            if (ModelState.IsValid)
            {
                //model.Fecha = DateTime.Now;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVisador(ProgramacionHorasExtraordinarias model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseProgramacionHorasExtraordinarias(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.ProgramacionHorasExtraordinariasUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                    //return RedirectToAction("GeneraDocumento", "ProgramacionHorasExtraordinarias", new { model.WorkflowId, id = model.ProgramacionHorasExtraordinariasId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            return View(model);
        }

        public ActionResult GeneraDocumento(int id)
        {
            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            int tipoDoc = 0;
            int IdDocto = 0;
            string Name = string.Empty;
            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);
            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
            if ((Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)) /*genera CDP, por la etapa en la que se encuentra*/
            {
                /*Se genera certificado de viatico*/
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("CDPViatico", new { id = model.ProgramacionHorasExtraordinariasId }) { FileName = "CDP_Viatico" + ".pdf", /*Cookies = cookieCollection,*/ FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);
                tipoDoc = 2;
                Name = "CDP Viatico Cometido nro" + " " + model.ProgramacionHorasExtraordinariasId.ToString() + ".pdf";
                int idDoctoViatico = 0;

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var cdpViatico = _repository.GetFirst<Documento>(d => d.ProcesoId == model.ProcesoId);
                if (cdpViatico != null)
                {
                    if (cdpViatico.TipoDocumentoId == 2)
                        idDoctoViatico = cdpViatico.DocumentoId;
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
                    docOld.File = pdf;
                    docOld.Signed = false;
                    docOld.Texto = data.Text;
                    docOld.Metadata = data.Metadata;
                    docOld.Type = data.Type;
                    _repository.Update(docOld);
                    _repository.Save();
                }
            }
            else
            {
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.ProgramacionHorasExtraordinariasId }) { FileName = "Programacion Horas Extraordinarias" + ".pdf" };
                pdf = resultPdf.BuildFile(ControllerContext);
                data = _file.BynaryToText(pdf);

                tipoDoc = 9;
                Name = "Programacion Horas Extraordinarias nro" + " " + model.ProgramacionHorasExtraordinariasId.ToString() + ".pdf";

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                //var resolucion = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
                var programacion = _repository.GetFirst<Documento>(d => d.ProcesoId == model.ProcesoId && d.TipoDocumentoId == 9);
                if (programacion != null)
                    IdDocto = programacion.DocumentoId;

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

            return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
        }

        public ActionResult GeneraDocumento2(int id)
        {
            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            int tipoDoc = 0;
            int IdDocto = 0;
            string Name = string.Empty;
            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);
            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
            if ((Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId !=
                    (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) ||
                (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId ==
                    (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
            ) /*genera CDP, por la etapa en la que se encuentra*/
            {
                /*Se genera certificado de viatico*/
                Rotativa.ActionAsPdf resultPdf =
                    new Rotativa.ActionAsPdf("CDPViatico", new { id = model.ProgramacionHorasExtraordinariasId })
                    {
                        FileName = "CDP_Viatico" + ".pdf", /*Cookies = cookieCollection,*/
                        FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName
                    };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);
                tipoDoc = 2;
                Name = "CDP Viatico Cometido nro" + " " + model.ProgramacionHorasExtraordinariasId.ToString() + ".pdf";
                int idDoctoViatico = 0;

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var cdpViatico = _repository.GetFirst<Documento>(d => d.ProcesoId == model.ProcesoId);
                if (cdpViatico != null)
                {
                    if (cdpViatico.TipoDocumentoId == 2)
                        idDoctoViatico = cdpViatico.DocumentoId;
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
                    docOld.File = pdf;
                    docOld.Signed = false;
                    docOld.Texto = data.Text;
                    docOld.Metadata = data.Metadata;
                    docOld.Type = data.Type;
                    _repository.Update(docOld);
                    _repository.Save();
                }
            }
            else
            {
                Rotativa.ActionAsPdf resultPdf =
                    new Rotativa.ActionAsPdf("ResolucionProgramacion",
                            new { id = model.ProgramacionHorasExtraordinariasId })
                    { FileName = "Resolución Programacion Horas Extraordinarias" + ".pdf" };
                pdf = resultPdf.BuildFile(ControllerContext);
                data = _file.BynaryToText(pdf);

                tipoDoc = 10;
                Name = "Resolución Programacion Horas Extraordinarias nro" + " " +
                       model.ProgramacionHorasExtraordinariasId.ToString() + ".pdf";

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                //var resolucion = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
                var resolucionprogramacion =
                    _repository.GetFirst<Documento>(d => d.ProcesoId == model.ProcesoId && d.TipoDocumentoId == 10);
                if (resolucionprogramacion != null)
                    IdDocto = resolucionprogramacion.DocumentoId;

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

            return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
        }

        [AllowAnonymous]
        public ActionResult Pdf(int id)
        {
            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);

            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
            if (Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResolucionProgramacion(int id)
        {
            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);

            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
            if (Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);

            return View(model);
        }

        public ActionResult Anular(int id)
        {
            var model = _repository.GetById<Workflow>(id);
            return View(model);
        }

        //Firma Vieja
        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sign(ProgramacionHorasExtraordinarias model)
        {
            if (ModelState.IsValid)
            {
                model.Firmante = UserExtended.Email(User);

                var _useCaseInteractor = new UseCaseProgramacionHorasExtraordinarias(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.Sign(model.ProgramacionHorasExtraordinariasId, new List<string> { model.Firmante });
                //var _UseCaseResponseMessage = _useCaseInteractor.Sign(model.ProgramacionHorasExtraordinariasId, new List<string> { model.Firmante }, model.Firmante);

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Sign", "ProgramacionHorasExtraordinarias", new { id = model.ProgramacionHorasExtraordinariasId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return RedirectToAction("Sign", "FirmaDocumento", new { id = model.ProgramacionHorasExtraordinariasId });
        }

        public ActionResult DetailsGM(int id)
        {
            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");

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
            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);

            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
            if ((Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)) /*genera CDP, por la etapa en la que se encuentra*/
            {
                /*Se genera certificado de viatico*/
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("CDPViatico", new { id = model.ProgramacionHorasExtraordinariasId }) { FileName = "CDP_Viatico" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);
                tipoDoc = 2;
                Name = "CDP Viatico Cometido nro" + " " + model.ProgramacionHorasExtraordinariasId.ToString() + ".pdf";
                int idDoctoViatico = 0;

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var cdpViatico = _repository.GetFirst<Documento>(d => d.ProcesoId == model.ProcesoId);
                if (cdpViatico != null)
                {
                    if (cdpViatico.TipoDocumentoId == 2)
                        idDoctoViatico = cdpViatico.DocumentoId;
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
                //if (model.CalidadDescripcion.Contains("HONORARIOS"))/*valida si es contrata u honorario*/
                //{
                //    //if (model.IdGrado != "0" && model.GradoDescripcion != "0")
                //    //{
                //    Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Orden", new { id = model.MemorandumId }) { FileName = "Orden_Pago" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                //    pdf = resultPdf.BuildFile(ControllerContext);
                //    //data = GetBynary(pdf);
                //    data = _file.BynaryToText(pdf);

                //    tipoDoc = 8;
                //    Name = "Orden de Pago Cometido nro" + " " + model.MemorandumId.ToString() + ".pdf";
                //    //}
                //    //else
                //    //{
                //    //    //TempData["Error"] = "No existen antecedentes del grado del funcionario";
                //    //    TempData["Success"] = "No existen antecedentes del grado del funcionario.";
                //    //    return Redirect(Request.UrlReferrer.PathAndQuery);
                //    //}
                //}
                //else if (model.CalidadDescripcion.Contains("TITULAR"))/*valida si es autoridad*/
                //{
                //    Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Resolucion", new { id = model.MemorandumId }) { FileName = "Resolucion Ministerial Exenta" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                //    pdf = resultPdf.BuildFile(ControllerContext);
                //    //data = GetBynary(pdf);
                //    data = _file.BynaryToText(pdf);
                //    tipoDoc = 8;
                //    Name = "Resolucion Ministerial Exenta nro" + " " + model.MemorandumId.ToString() + ".pdf";
                //}
                //else
                //{
                //    Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.MemorandumId }) { FileName = "Resolucion" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                //    pdf = resultPdf.BuildFile(ControllerContext);
                //    //data = GetBynary(pdf);
                //    data = _file.BynaryToText(pdf);

                //    tipoDoc = 8;
                //    Name = "Memorandum nro" + " " + model.MemorandumId.ToString() + ".pdf";
                //}

                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.ProgramacionHorasExtraordinariasId }) { FileName = "Resolucion" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);

                tipoDoc = 8;
                Name = "Memorandum nro" + " " + model.ProgramacionHorasExtraordinariasId.ToString() + ".pdf";

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var resolucion = _repository.GetFirst<Documento>(d => d.ProcesoId == model.ProcesoId);
                if (resolucion != null)
                {
                    if (resolucion.TipoDocumentoId == 8)
                        IdDocto = resolucion.DocumentoId;
                }

                /*se guarda el pdf generado como documento adjunto -- se valida si ya existe el documento para actualizar*/
                if (IdDocto == 8)
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

            return RedirectToAction("Sign", "Memorandum", new { model.WorkflowId, id = model.ProgramacionHorasExtraordinariasId });
        }

        public ActionResult SignResolucion(int id)
        {
            var model = _repository.GetById<ProgramacionHorasExtraordinarias>(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult SignResolucion(int? DocumentoId)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseProgramacionHorasExtraordinarias(_repository, _sigper, _file, _folio, _hsm, _email);
                var obj = _repository.GetFirst<ProgramacionHorasExtraordinarias>(c => c.ProgramacionHorasExtraordinariasId == DocumentoId);
                var doc = _repository.GetFirst<Documento>(c => c.ProcesoId == obj.ProcesoId && c.TipoDocumentoId == 10);
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

        public ActionResult SeguimientoMemo()
        {
            var model = new DTOFilterProgramacionHorasExtraordinarias();

            List<SelectListItem> Estado = new List<SelectListItem>
            {
            new SelectListItem {Text = "En Curso", Value = "1"},
            new SelectListItem {Text = "Terminado", Value = "2"},
            new SelectListItem {Text = "Anulado", Value = "3"},
            };

            ViewBag.Estado = new SelectList(Estado, "Value", "Text");
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdUnidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes").Where(c => c.Text.Contains("ECONOMIA"));
            ViewBag.Destino = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            return View(model);
        }

        [HttpPost]
        public ActionResult SeguimientoMemo(DTOFilterProgramacionHorasExtraordinarias model)
        {
            var predicate = PredicateBuilder.True<ProgramacionHorasExtraordinarias>();

            if (ModelState.IsValid)
            {
                if (model.Estado.HasValue)
                {
                    //if (model.Estado == 1)
                    //    predicate = predicate.And(q => q.Proceso.Terminada == false && q.Proceso.Anulada == false);
                    //if (model.Estado == 2)
                    //    predicate = predicate.And(q => q.Proceso.Anulada == true);
                    //if (model.Estado == 3)
                    //    predicate = predicate.And(q => q.Proceso.Terminada == true);

                    if (model.Estado == 1)
                        predicate = predicate.And(q => q.Proceso.EstadoProcesoId != (int)App.Util.Enum.EstadoProceso.Terminado && q.Proceso.EstadoProcesoId != (int)App.Util.Enum.EstadoProceso.Anulado);
                    if (model.Estado == 2)
                        predicate = predicate.And(q => q.Proceso.EstadoProcesoId == (int)App.Util.Enum.EstadoProceso.Anulado);
                    if (model.Estado == 3)
                        predicate = predicate.And(q => q.Proceso.EstadoProcesoId == (int)App.Util.Enum.EstadoProceso.Terminado);
                }

                //if (model.NombreId.HasValue)
                //    predicate = predicate.And(q => q.NombreId == model.NombreId);

                //if (model.IdUnidad.HasValue)
                //    predicate = predicate.And(q => q.IdUnidad == model.IdUnidad);

                //if (model.Destino.HasValue)
                //    predicate = predicate.And(q => q.Destinos.FirstOrDefault().IdRegion == model.Destino.Value.ToString());

                //if (model.FechaInicio.HasValue)
                //    predicate = predicate.And(q =>
                //        q.Destinos.FirstOrDefault().FechaInicio.Year >= model.FechaInicio.Value.Year &&
                //        q.Destinos.FirstOrDefault().FechaInicio.Month >= model.FechaInicio.Value.Month &&
                //        q.Destinos.FirstOrDefault().FechaInicio.Day >= model.FechaInicio.Value.Day);

                //if (model.FechaTermino.HasValue)
                //    predicate = predicate.And(q =>
                //        q.Destinos.LastOrDefault().FechaInicio.Year <= model.FechaTermino.Value.Year &&
                //        q.Destinos.LastOrDefault().FechaInicio.Month <= model.FechaTermino.Value.Month &&
                //        q.Destinos.LastOrDefault().FechaInicio.Day <= model.FechaTermino.Value.Day);

                var MemorandumId = model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
                if (MemorandumId.Any())
                    predicate = predicate.And(q => MemorandumId.Contains(q.ProgramacionHorasExtraordinariasId));

                model.Result = _repository.Get(predicate);
            }

            //foreach (var res in model.Result)
            //{
            //    res.Subscretaria = res.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIO DE TURISMO" : "SUBSECRETARIA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";
            //    if (res.Proceso.Anulada == false && res.Proceso.Terminada == false)
            //        model.Estado = 1; // "En Proceso";
            //    else if (res.Proceso.Terminada == true)
            //        model.Estado = 3; //Terminado
            //    else if (res.Proceso.Anulada == true)
            //        model.Estado = 2; //Anulado

            //    //model.Result.FirstOrDefault().Subscretaria = res.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIO DE TURISMO" : "SUBSECRETARIA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";

            //    //var workflow = _repository.Get<Workflow>(c => c.ProcesoId == res.ProcesoId);
            //    //foreach(var w in workflow)
            //    //{
            //    //    res.Workflow.Email = w.Email;
            //    //}

            //    //switch (res.Proceso.DefinicionProcesoId)
            //    //{
            //    //    case 13:
            //    //        var com = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId);
            //    //        if (com.Count() > 0)
            //    //        {
            //    //            model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId;
            //    //        }
            //    //        break;
            //    //    case 10:
            //    //        var come = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId);
            //    //        if (come.Count() > 0)
            //    //        {
            //    //            model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId;
            //    //        }
            //    //        break;
            //    //    case 12:
            //    //        var comision = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId);
            //    //        if (comision.Count() > 0)
            //    //        {
            //    //            model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId;
            //    //        }
            //    //        break;
            //    //}
            //}



            List<SelectListItem> Estado = new List<SelectListItem>
            {
                new SelectListItem {Text = "En Curso", Value = "1"},
                new SelectListItem {Text = "Terminado", Value = "2"},
                new SelectListItem {Text = "Anulado", Value = "3"},
            };

            ViewBag.Estado = new SelectList(Estado, "Value", "Text");
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdUnidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes").Where(c => c.Text.Contains("ECONOMIA"));
            ViewBag.Destino = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            return View(model);

        }

        public FileResult DownloadGP()
        {
            var result = _repository.GetAll<ProgramacionHorasExtraordinarias>();

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\SeguimientoMemo.xlsx");
            var fileInfo = new FileInfo(file);
            var excelPackageSeguimientoMemo = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackageSeguimientoMemo.Workbook.Worksheets[1];
            foreach (var memorandum in result.ToList())
            {
                var workflow = _repository.Get<Workflow>(w => w.ProcesoId == memorandum.ProcesoId);
                fila++;
                if (memorandum.Proceso.EstadoProcesoId == (int)App.Util.Enum.EstadoProceso.EnProceso)
                    worksheet.Cells[fila, 1].Value = "En Curso";
                else if (memorandum.Proceso.EstadoProcesoId == (int)App.Util.Enum.EstadoProceso.Terminado)
                    worksheet.Cells[fila, 1].Value = "Terminada";
                else if (memorandum.Proceso.EstadoProcesoId == (int)App.Util.Enum.EstadoProceso.Anulado)
                    worksheet.Cells[fila, 1].Value = "Anulada";

                worksheet.Cells[fila, 10].Value = workflow.LastOrDefault().Email;
                worksheet.Cells[fila, 11].Value = workflow.LastOrDefault().FechaCreacion.ToString();
            }

            return File(excelPackageSeguimientoMemo.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, DateTime.Now.ToString("rptSeguimientoGP_yyyyMMddhhmmss") + ".xlsx");
        }
    }
}