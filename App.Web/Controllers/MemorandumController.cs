using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using App.Model.Memorandum;
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
    public class MemorandumController : Controller
    {
        public class DTOFilterMemorandum
        {
            public DTOFilterMemorandum()
            {
                TextSearch = string.Empty;
                Select = new HashSet<DTOSelect>();
                Result = new HashSet<Memorandum>();
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
            public IEnumerable<Memorandum> Result { get; set; }
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

        public MemorandumController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm, IEmail email)
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
            var IdCargo = per.FunDatosLaborales.RhConCar.Value;
            var cargo = string.IsNullOrEmpty(per.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == per.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidad = per.FunDatosLaborales.RH_ContCod;
            var calidad = string.IsNullOrEmpty(per.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == per.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGrado = string.IsNullOrEmpty(per.FunDatosLaborales.RhConGra.Trim()) ? "0" : per.FunDatosLaborales.RhConGra.Trim();
            var grado = string.IsNullOrEmpty(per.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : per.FunDatosLaborales.RhConGra.Trim();
            var estamento = per.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == per.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
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

        public JsonResult GetUsuarioDest(int RutDest)
        {
            var correodest = _sigper.GetUserByRut(RutDest).Funcionario.Rh_Mail.Trim();
            var perdest = _sigper.GetUserByEmail(correodest);
            var IdCargoDest = perdest.FunDatosLaborales.RhConCar.Value;
            var cargodest = string.IsNullOrEmpty(perdest.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perdest.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadDest = perdest.FunDatosLaborales.RH_ContCod;
            var calidaddest = string.IsNullOrEmpty(perdest.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perdest.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoDest = string.IsNullOrEmpty(perdest.FunDatosLaborales.RhConGra.Trim()) ? "0" : perdest.FunDatosLaborales.RhConGra.Trim();
            var gradodest = string.IsNullOrEmpty(perdest.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perdest.FunDatosLaborales.RhConGra.Trim();
            var estamentodest = perdest.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perdest.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdDest = _sigper.GetReContra().Where(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaDest = ProgIdDest != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdDest).FirstOrDefault().RePytDes : "S/A";
            var conglomeradodest = _sigper.GetReContra().Where(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte).ReContraSed;
            var jefaturadest = perdest.Jefatura != null ? perdest.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreodest = perdest.Funcionario != null ? perdest.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombredest = perdest.Funcionario != null ? perdest.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutdest;
            if (perdest.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perdest.Funcionario.RH_NumInte.ToString();
                rutdest = string.Concat("0", t);
            }
            else
            {
                rutdest = perdest.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutDest = rutdest,
                DVDest = perdest.Funcionario.RH_DvNuInt.ToString(),
                IdCargoDest = IdCargoDest,
                CargoDest = cargodest,
                IdCalidadDest = IdCalidadDest,
                CalidadJuridicaDest = calidaddest,
                IdGradoDest = IdGradoDest,
                GradoDest = gradodest,
                EstamentoDest = estamentodest,
                ProgramaDest = ProgramaDest.Trim(),
                ConglomeradoDest = conglomeradodest,
                IdUnidadDest = perdest.Unidad.Pl_UndCod,
                UnidadDest = perdest.Unidad.Pl_UndDes.Trim(),
                JefaturaDest = jefaturadest,
                EmailDest = ecorreodest,
                NombreChqDest = perdest.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioSecre(int RutSecre)
        {
            var correosecre = _sigper.GetUserByRut(RutSecre).Funcionario.Rh_Mail.Trim();
            var persecre = _sigper.GetUserByEmail(correosecre);
            var IdCargoSecre = persecre.FunDatosLaborales.RhConCar.Value;
            var cargosecre = string.IsNullOrEmpty(persecre.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == persecre.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadSecre = persecre.FunDatosLaborales.RH_ContCod;
            var calidadsecre = string.IsNullOrEmpty(persecre.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == persecre.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoSecre = string.IsNullOrEmpty(persecre.FunDatosLaborales.RhConGra.Trim()) ? "0" : persecre.FunDatosLaborales.RhConGra.Trim();
            var gradosecre = string.IsNullOrEmpty(persecre.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : persecre.FunDatosLaborales.RhConGra.Trim();
            var estamentosecre = persecre.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persecre.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdSecre = _sigper.GetReContra().Where(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaSecre = ProgIdSecre != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdSecre).FirstOrDefault().RePytDes : "S/A";
            var conglomeradosecre = _sigper.GetReContra().Where(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte).ReContraSed;
            var jefaturasecre = persecre.Jefatura != null ? persecre.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreosecre = persecre.Funcionario != null ? persecre.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombresecre = persecre.Funcionario != null ? persecre.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutsecre;
            if (persecre.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = persecre.Funcionario.RH_NumInte.ToString();
                rutsecre = string.Concat("0", t);
            }
            else
            {
                rutsecre = persecre.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutSecre = rutsecre,
                DVSecre = persecre.Funcionario.RH_DvNuInt.ToString(),
                IdCargoSecre = IdCargoSecre,
                CargoSecre = cargosecre,
                IdCalidadSecre = IdCalidadSecre,
                CalidadJuridicaSecre = calidadsecre,
                IdGradoSecre = IdGradoSecre,
                GradoSecre = gradosecre,
                EstamentoSecre = estamentosecre,
                ProgramaSecre = ProgramaSecre.Trim(),
                ConglomeradoSecre = conglomeradosecre,
                IdUnidadSecre = persecre.Unidad.Pl_UndCod,
                UnidadSecre = persecre.Unidad.Pl_UndDes.Trim(),
                JefaturaSecre = jefaturasecre,
                EmailSecre = ecorreosecre,
                NombreChqSecre = persecre.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa1(int RutVisa1)
        {
            var correovisa1 = _sigper.GetUserByRut(RutVisa1).Funcionario.Rh_Mail.Trim();
            var pervisa1 = _sigper.GetUserByEmail(correovisa1);
            var IdCargoVisa1 = pervisa1.FunDatosLaborales.RhConCar.Value;
            var cargovisa1 = string.IsNullOrEmpty(pervisa1.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa1.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa1 = pervisa1.FunDatosLaborales.RH_ContCod;
            var calidadvisa1 = string.IsNullOrEmpty(pervisa1.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa1.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa1 = string.IsNullOrEmpty(pervisa1.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa1.FunDatosLaborales.RhConGra.Trim();
            var gradovisa1 = string.IsNullOrEmpty(pervisa1.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa1.FunDatosLaborales.RhConGra.Trim();
            var estamentovisa1 = pervisa1.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa1.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdVisa1 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaVisa1 = ProgIdVisa1 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa1).FirstOrDefault().RePytDes : "S/A";
            var conglomeradovisa1 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa1 = pervisa1.Jefatura != null ? pervisa1.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa1 = pervisa1.Funcionario != null ? pervisa1.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrevisa1 = pervisa1.Funcionario != null ? pervisa1.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutvisa1;
            if (pervisa1.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = pervisa1.Funcionario.RH_NumInte.ToString();
                rutvisa1 = string.Concat("0", t);
            }
            else
            {
                rutvisa1 = pervisa1.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutVisa1 = rutvisa1,
                DVVisa1 = pervisa1.Funcionario.RH_DvNuInt.ToString(),
                IdCargoVisa1 = IdCargoVisa1,
                CargoVisa1 = cargovisa1,
                IdCalidadVisa1 = IdCalidadVisa1,
                CalidadJuridicaVisa1 = calidadvisa1,
                IdGradoVisa1 = IdGradoVisa1,
                GradoVisa1 = gradovisa1,
                EstamentoVisa1 = estamentovisa1,
                ProgramaVisa1 = ProgramaVisa1.Trim(),
                ConglomeradoVisa1 = conglomeradovisa1,
                IdUnidadVisa1 = pervisa1.Unidad.Pl_UndCod,
                UnidadVisa1 = pervisa1.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa1 = jefaturavisa1,
                EmailVisa1 = ecorreovisa1,
                NombreChqVisa1 = pervisa1.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa2(int RutVisa2)
        {
            var correovisa2 = _sigper.GetUserByRut(RutVisa2).Funcionario.Rh_Mail.Trim();
            var pervisa2 = _sigper.GetUserByEmail(correovisa2);
            var IdCargoVisa2 = pervisa2.FunDatosLaborales.RhConCar.Value;
            var cargovisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa2.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa2 = pervisa2.FunDatosLaborales.RH_ContCod;
            var calidadvisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa2.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            var gradovisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            var estamentovisa2 = pervisa2.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa2.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaVisa2 = ProgIdVisa2 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa2).FirstOrDefault().RePytDes : "S/A";
            var conglomeradovisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa2 = pervisa2.Jefatura != null ? pervisa2.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa2 = pervisa2.Funcionario != null ? pervisa2.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrevisa2 = pervisa2.Funcionario != null ? pervisa2.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutvisa2;
            if (pervisa2.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = pervisa2.Funcionario.RH_NumInte.ToString();
                rutvisa2 = string.Concat("0", t);
            }
            else
            {
                rutvisa2 = pervisa2.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutVisa2 = rutvisa2,
                DVVisa2 = pervisa2.Funcionario.RH_DvNuInt.ToString(),
                IdCargoVisa2 = IdCargoVisa2,
                CargoVisa2 = cargovisa2,
                IdCalidadVisa2 = IdCalidadVisa2,
                CalidadJuridicaVisa2 = calidadvisa2,
                IdGradoVisa2 = IdGradoVisa2,
                GradoVisa2 = gradovisa2,
                EstamentoVisa2 = estamentovisa2,
                ProgramaVisa2 = ProgramaVisa2.Trim(),
                ConglomeradoVisa2 = conglomeradovisa2,
                IdUnidadVisa2 = pervisa2.Unidad.Pl_UndCod,
                UnidadVisa2 = pervisa2.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa2 = jefaturavisa2,
                EmailVisa2 = ecorreovisa2,
                NombreChqVisa2 = pervisa2.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa3(int RutVisa3)
        {
            var correovisa3 = _sigper.GetUserByRut(RutVisa3).Funcionario.Rh_Mail.Trim();
            var pervisa3 = _sigper.GetUserByEmail(correovisa3);
            var IdCargoVisa3 = pervisa3.FunDatosLaborales.RhConCar.Value;
            var cargovisa3 = string.IsNullOrEmpty(pervisa3.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa3.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa3 = pervisa3.FunDatosLaborales.RH_ContCod;
            var calidadvisa3 = string.IsNullOrEmpty(pervisa3.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa3.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa3 = string.IsNullOrEmpty(pervisa3.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa3.FunDatosLaborales.RhConGra.Trim();
            var gradovisa3 = string.IsNullOrEmpty(pervisa3.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa3.FunDatosLaborales.RhConGra.Trim();
            var estamentovisa3 = pervisa3.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa3.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdVisa3 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa3.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa3.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa3.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaVisa3 = ProgIdVisa3 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa3).FirstOrDefault().RePytDes : "S/A";
            var conglomeradovisa3 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa3.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa3.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa3.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa3.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa3 = pervisa3.Jefatura != null ? pervisa3.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa3 = pervisa3.Funcionario != null ? pervisa3.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrevisa3 = pervisa3.Funcionario != null ? pervisa3.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutvisa3;
            if (pervisa3.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = pervisa3.Funcionario.RH_NumInte.ToString();
                rutvisa3 = string.Concat("0", t);
            }
            else
            {
                rutvisa3 = pervisa3.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutVisa3 = rutvisa3,
                DVVisa3 = pervisa3.Funcionario.RH_DvNuInt.ToString(),
                IdCargoVisa3 = IdCargoVisa3,
                CargoVisa3 = cargovisa3,
                IdCalidadVisa3 = IdCalidadVisa3,
                CalidadJuridicaVisa3 = calidadvisa3,
                IdGradoVisa3 = IdGradoVisa3,
                GradoVisa3 = gradovisa3,
                EstamentoVisa3 = estamentovisa3,
                ProgramaVisa3 = ProgramaVisa3.Trim(),
                ConglomeradoVisa3 = conglomeradovisa3,
                IdUnidadVisa3 = pervisa3.Unidad.Pl_UndCod,
                UnidadVisa3 = pervisa3.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa3 = jefaturavisa3,
                EmailVisa3 = ecorreovisa3,
                NombreChqVisa3 = pervisa3.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa4(int RutVisa4)
        {
            var correovisa4 = _sigper.GetUserByRut(RutVisa4).Funcionario.Rh_Mail.Trim();
            var pervisa4 = _sigper.GetUserByEmail(correovisa4);
            var IdCargoVisa4 = pervisa4.FunDatosLaborales.RhConCar.Value;
            var cargovisa4 = string.IsNullOrEmpty(pervisa4.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa4.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa4 = pervisa4.FunDatosLaborales.RH_ContCod;
            var calidadvisa4 = string.IsNullOrEmpty(pervisa4.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa4.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa4 = string.IsNullOrEmpty(pervisa4.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa4.FunDatosLaborales.RhConGra.Trim();
            var gradovisa4 = string.IsNullOrEmpty(pervisa4.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa4.FunDatosLaborales.RhConGra.Trim();
            var estamentovisa4 = pervisa4.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa4.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdVisa4 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa4.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa4.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa4.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaVisa4 = ProgIdVisa4 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa4).FirstOrDefault().RePytDes : "S/A";
            var conglomeradovisa4 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa4.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa4.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa4.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa4.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa4 = pervisa4.Jefatura != null ? pervisa4.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa4 = pervisa4.Funcionario != null ? pervisa4.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrevisa4 = pervisa4.Funcionario != null ? pervisa4.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutvisa4;
            if (pervisa4.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = pervisa4.Funcionario.RH_NumInte.ToString();
                rutvisa4 = string.Concat("0", t);
            }
            else
            {
                rutvisa4 = pervisa4.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutVisa4 = rutvisa4,
                DVVisa4 = pervisa4.Funcionario.RH_DvNuInt.ToString(),
                IdCargoVisa4 = IdCargoVisa4,
                CargoVisa4 = cargovisa4,
                IdCalidadVisa4 = IdCalidadVisa4,
                CalidadJuridicaVisa4 = calidadvisa4,
                IdGradoVisa4 = IdGradoVisa4,
                GradoVisa4 = gradovisa4,
                EstamentoVisa4 = estamentovisa4,
                ProgramaVisa4 = ProgramaVisa4.Trim(),
                ConglomeradoVisa4 = conglomeradovisa4,
                IdUnidadVisa4 = pervisa4.Unidad.Pl_UndCod,
                UnidadVisa4 = pervisa4.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa4 = jefaturavisa4,
                EmailVisa4 = ecorreovisa4,
                NombreChqVisa4 = pervisa4.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa5(int RutVisa5)
        {
            var correovisa5 = _sigper.GetUserByRut(RutVisa5).Funcionario.Rh_Mail.Trim();
            var pervisa5 = _sigper.GetUserByEmail(correovisa5);
            var IdCargoVisa5 = pervisa5.FunDatosLaborales.RhConCar.Value;
            var cargovisa5 = string.IsNullOrEmpty(pervisa5.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa5.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa5 = pervisa5.FunDatosLaborales.RH_ContCod;
            var calidadvisa5 = string.IsNullOrEmpty(pervisa5.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa5.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa5 = string.IsNullOrEmpty(pervisa5.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa5.FunDatosLaborales.RhConGra.Trim();
            var gradovisa5 = string.IsNullOrEmpty(pervisa5.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa5.FunDatosLaborales.RhConGra.Trim();
            var estamentovisa5 = pervisa5.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa5.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdVisa5 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa5.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa5.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa5.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaVisa5 = ProgIdVisa5 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa5).FirstOrDefault().RePytDes : "S/A";
            var conglomeradovisa5 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa5.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa5.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa5.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa5.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa5 = pervisa5.Jefatura != null ? pervisa5.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa5 = pervisa5.Funcionario != null ? pervisa5.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrevisa5 = pervisa5.Funcionario != null ? pervisa5.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutvisa5;
            if (pervisa5.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = pervisa5.Funcionario.RH_NumInte.ToString();
                rutvisa5 = string.Concat("0", t);
            }
            else
            {
                rutvisa5 = pervisa5.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutVisa5 = rutvisa5,
                DVVisa5 = pervisa5.Funcionario.RH_DvNuInt.ToString(),
                IdCargoVisa5 = IdCargoVisa5,
                CargoVisa5 = cargovisa5,
                IdCalidadVisa5 = IdCalidadVisa5,
                CalidadJuridicaVisa5 = calidadvisa5,
                IdGradoVisa5 = IdGradoVisa5,
                GradoVisa5 = gradovisa5,
                EstamentoVisa5 = estamentovisa5,
                ProgramaVisa5 = ProgramaVisa5.Trim(),
                ConglomeradoVisa5 = conglomeradovisa5,
                IdUnidadVisa5 = pervisa5.Unidad.Pl_UndCod,
                UnidadVisa5 = pervisa5.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa5 = jefaturavisa5,
                EmailVisa5 = ecorreovisa5,
                NombreChqVisa5 = pervisa5.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioAna(int RutAna)
        {
            var correoana = _sigper.GetUserByRut(RutAna).Funcionario.Rh_Mail.Trim();
            var perana = _sigper.GetUserByEmail(correoana);
            var IdCargoAna = perana.FunDatosLaborales.RhConCar.Value;
            var cargoana = string.IsNullOrEmpty(perana.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perana.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadAna = perana.FunDatosLaborales.RH_ContCod;
            var calidadana = string.IsNullOrEmpty(perana.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perana.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoAna = string.IsNullOrEmpty(perana.FunDatosLaborales.RhConGra.Trim()) ? "0" : perana.FunDatosLaborales.RhConGra.Trim();
            var gradoana = string.IsNullOrEmpty(perana.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perana.FunDatosLaborales.RhConGra.Trim();
            var estamentoana = perana.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perana.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdAna = _sigper.GetReContra().Where(c => c.RH_NumInte == perana.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perana.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perana.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaAna = ProgIdAna != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdAna).FirstOrDefault().RePytDes : "S/A";
            var conglomeradoana = _sigper.GetReContra().Where(c => c.RH_NumInte == perana.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perana.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perana.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perana.Funcionario.RH_NumInte).ReContraSed;
            var jefaturaana = perana.Jefatura != null ? perana.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreoana = perana.Funcionario != null ? perana.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombreana = perana.Funcionario != null ? perana.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutana;
            if (perana.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perana.Funcionario.RH_NumInte.ToString();
                rutana = string.Concat("0", t);
            }
            else
            {
                rutana = perana.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutAna = rutana,
                DVAna = perana.Funcionario.RH_DvNuInt.ToString(),
                IdCargoAna = IdCargoAna,
                CargoAna = cargoana,
                IdCalidadAna = IdCalidadAna,
                CalidadJuridicaAna = calidadana,
                IdGradoAna = IdGradoAna,
                GradoAna = gradoana,
                EstamentoAna = estamentoana,
                ProgramaAna = ProgramaAna.Trim(),
                ConglomeradoAna = conglomeradoana,
                IdUnidadAna = perana.Unidad.Pl_UndCod,
                UnidadAna = perana.Unidad.Pl_UndDes.Trim(),
                JefaturaAna = jefaturaana,
                EmailAna = ecorreoana,
                NombreChqAna = perana.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Memorandum>();
            return View(model);
        }

        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult View(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            //ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");

            var model = _repository.GetById<Memorandum>(id);
            //var model = _repository.GetFirst<Memorandum>(q => q.ProcesoId == id);
            //if (model == null)
            //    return RedirectToAction("Details", "Proceso", new { id });

            return View(model);
        }

        public ActionResult CreaDocto(int id)
        {
            var model = _repository.GetById<Memorandum>(id);
            //if (model.GeneracionCDP.Count > 0)
            //{
            //    var cdp = _repository.GetById<GeneracionCDP>(model.GeneracionCDP.FirstOrDefault().GeneracionCDPId);
            //    model.GeneracionCDP.Add(cdp);
            //}

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Memorandum>(id);

            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa3 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa4 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa5 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //if (model.GeneracionCDP.Count > 0)
            //{
            //    var cdp = _repository.GetById<GeneracionCDP>(model.GeneracionCDP.FirstOrDefault().GeneracionCDPId);
            //    model.GeneracionCDP.Add(cdp);
            //}

            return View(model);
        }

        public ActionResult DetailsDocto(int id)
        {
            var model = _repository.GetById<Memorandum>(id);
            //if (model.GeneracionCDP.Count > 0)
            //{
            //    var cdp = _repository.GetById<GeneracionCDP>(model.GeneracionCDP.FirstOrDefault().GeneracionCDPId);
            //    model.GeneracionCDP.Add(cdp);
            //}

            return View(model);
        }

        public ActionResult DetailsFinanzas(int id)
        {
            var model = _repository.GetById<Memorandum>(id);

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DetailsFinanzas(Memorandum model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _hsm, _file,_folio, _sigper);
                //var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);
                var doc = _repository.Get<Documento>(c => c.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 5).FirstOrDefault();
                var user = User.Email();
                var _UseCaseResponseMessage = _useCaseInteractor.DocumentoSign(doc, user,null);



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

            var modelo = _repository.Get<Memorandum>(c => c.MemorandumId == model.MemorandumId).FirstOrDefault();

            return View(modelo);
            //return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<Memorandum>(id);
            return View(model);
        }

        public ActionResult Create(int? WorkFlowId, int? ProcesoId)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            //ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreId = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("subturismo")), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa3 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa4 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa5 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");

            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa3 = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa4 = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa5 = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");

            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new Memorandum
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
                model.NombreId = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.Nombre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargo = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailRem = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqRem = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidad = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadDest = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionDest = persona.Unidad.Pl_UndDes.Trim();
                model.RutDest = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVDest = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdDest = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreDest = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoDest = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionDest = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailDest = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqDest = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadDest = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionDest = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadSecre = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionSecre = persona.Unidad.Pl_UndDes.Trim();
                model.RutSecre = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVSecre = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdSecre = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreSecre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoSecre = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionSecre = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailSecre = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqSecre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadSecre = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionSecre = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa1 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa1 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa1 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa1 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa1 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa1 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa1 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa1 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa1 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailVisa1 = null;
                model.NombreChqVisa1 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa1 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa1 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa2 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa2 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa2 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa2 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa2 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa2 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa2 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa2 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa2 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa2 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa3 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa3 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa3 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa3 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa3 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa3 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa3 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa3 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa4 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa4 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa4 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa4 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa4 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa4 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa4 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa4 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa4 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa4 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa4 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa4 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa5 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa5 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa5 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa5 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa5 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa5 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa5 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa5 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa5 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa5 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa5 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa5 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Memorandum model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            //ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa3 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa4 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa5 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");

            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa3 = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa4 = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa5 = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");


            //model.FechaSolicitud = DateTime.Now;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseMemorandum(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.MemorandumInsert(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    //return RedirectToAction("Edit", "Memorandum", new { model.WorkflowId, id = model.MemorandumId });
                    return RedirectToAction("GeneraDocumento", "Memorandum", new { model.WorkflowId, id = model.MemorandumId });
                }
                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            return View(model);
        }

        //public ActionResult Edit(int id)
        //{
        //    var model = _repository.GetById<Memorandum>(id);

        //    //model.Destinos = ListDestino;
        //    var persona = _sigper.GetUserByEmail(User.Email());

        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Memorandum model/*, DestinosPasajes DesPasajes*/)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        //var _useCaseInteractor = new UseCaseInteractorCustom(_repository, _sigper);
        //        var _useCaseInteractor = new UseCaseMemorandum(_repository);
        //        var _UseCaseResponseMessage = _useCaseInteractor.MemorandumUpdate(model);
        //        var resp = new ResponseMessage();
        //        var res = new ResponseMessage();


        //        if (_UseCaseResponseMessage.Warnings.Count > 0)
        //            TempData["Warning"] = _UseCaseResponseMessage.Warnings;

        //        if (_UseCaseResponseMessage.IsValid)
        //        {
        //            /*Se ingresa informacion de pasajes*/
        //            /*Se crean los pasajes, si se solicitan*/
        //            //if (model.ReqPasajeAereo == true)
        //            //{
        //            //    if (DesPasajes != null && DesPasajes.IdRegion != null)
        //            //    {
        //            //        /*se crea encabezado de pasaje y destinos de pasajes*/
        //            //        Pasaje _pasaje = new Pasaje();
        //            //        _pasaje.FechaSolicitud = DateTime.Now;
        //            //        _pasaje.Nombre = model.Nombre;
        //            //        _pasaje.NombreId = model.NombreId;
        //            //        _pasaje.Rut = model.Rut;
        //            //        _pasaje.DV = model.DV;
        //            //        _pasaje.IdCalidad = model.IdCalidad;
        //            //        _pasaje.CalidadDescripcion = model.CalidadDescripcion;
        //            //        _pasaje.PasajeDescripcion = model.CometidoDescripcion;
        //            //        _pasaje.TipoDestino = true;
        //            //        _pasaje.ProcesoId = model.ProcesoId;
        //            //        _pasaje.WorkflowId = model.WorkflowId;
        //            //        resp = _useCaseInteractor.PasajeInsert(_pasaje);
        //            //        if (resp.Errors.Count > 0)
        //            //            _UseCaseResponseMessage.Errors.Add(resp.Errors.FirstOrDefault());

        //            //        /*genera resgistro en tabla destino pasaje, segun los destinos señalados en el cometido*/
        //            //        //foreach (var com in DesPasajes)
        //            //        //{
        //            //        DestinosPasajes _destino = new DestinosPasajes();
        //            //        _destino.PasajeId = resp.EntityId;
        //            //        _destino.IdRegion = DesPasajes.IdRegion;
        //            //        _destino.RegionDescripcion = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == DesPasajes.IdRegion).Pl_DesReg.Trim(); //DesPasajes.RegionDescripcion;
        //            //        _destino.IdRegionOrigen = DesPasajes.IdRegionOrigen;
        //            //        _destino.OrigenRegionDescripcion = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == DesPasajes.IdRegionOrigen).Pl_DesReg.Trim();
        //            //        _destino.FechaIda = DesPasajes.FechaOrigen;
        //            //        _destino.FechaVuelta = DesPasajes.FechaVuelta;
        //            //        _destino.FechaOrigen = DateTime.Now;
        //            //        _destino.ObservacionesOrigen = DesPasajes.ObservacionesOrigen;
        //            //        _destino.ObservacionesDestinos = DesPasajes.ObservacionesDestinos;
        //            //        res = _useCaseInteractor.DestinosPasajesInsert(_destino);
        //            //        if (res.Errors.Count > 0)
        //            //            _UseCaseResponseMessage.Errors.Add(res.Errors.FirstOrDefault());
        //            //        //}
        //            //    }
        //            //    //else
        //            //    //    TempData["Errors"] = "Debe agregar datos del pasaje";
        //            //}


        //            TempData["Success"] = "Operación terminada correctamente.";
        //            return Redirect(Request.UrlReferrer.PathAndQuery);
        //        }

        //        foreach (var item in _UseCaseResponseMessage.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, item);
        //        }
        //    }
        //    else
        //    {
        //        var errors = ModelState.Select(x => x.Value.Errors)
        //            .Where(y => y.Count > 0)
        //            .ToList();
        //    }

        //    return View(model);
        //}

        public ActionResult Edit(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa3 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa4 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa5 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");

            var model = _repository.GetById<Memorandum>(id);

            if (ModelState.IsValid)
            {
                model.FechaSolicitud = DateTime.Now;

                model.IdUnidad = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcion = persona.Unidad.Pl_UndDes.Trim();
                model.Rut = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DV = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreId = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.Nombre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargo = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailRem = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqRem = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidad = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadDest = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionDest = persona.Unidad.Pl_UndDes.Trim();
                model.RutDest = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVDest = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdDest = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreDest = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoDest = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionDest = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailDest = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqDest = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadDest = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionDest = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadSecre = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionSecre = persona.Unidad.Pl_UndDes.Trim();
                model.RutSecre = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVSecre = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdSecre = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreSecre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoSecre = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionSecre = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailSecre = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqSecre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadSecre = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionSecre = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa1 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa1 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa1 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa1 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa1 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa1 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa1 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa1 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa1 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa1 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa1 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa1 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa2 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa2 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa2 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa2 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa2 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa2 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa2 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa2 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa2 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa2 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa3 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa3 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa3 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa3 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa3 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa3 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa3 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa3 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa4 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa4 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa4 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa4 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa4 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa4 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa4 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa4 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa4 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa4 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa4 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa4 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa5 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa5 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa5 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa5 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa5 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa5 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa5 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa5 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa5 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa5 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa5 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa5 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Memorandum model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa3 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa4 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa5 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseMemorandum(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.MemorandumUpdate(model);

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

        public ActionResult EditAna(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa3 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa4 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa5 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdAna = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");

            var model = _repository.GetById<Memorandum>(id);

            if (ModelState.IsValid)
            {
                model.FechaSolicitud = DateTime.Now;

                model.IdUnidad = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcion = persona.Unidad.Pl_UndDes.Trim();
                model.Rut = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DV = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreId = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.Nombre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargo = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailRem = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqRem = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidad = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadDest = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionDest = persona.Unidad.Pl_UndDes.Trim();
                model.RutDest = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVDest = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdDest = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreDest = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoDest = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionDest = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailDest = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqDest = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadDest = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionDest = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadSecre = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionSecre = persona.Unidad.Pl_UndDes.Trim();
                model.RutSecre = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVSecre = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdSecre = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreSecre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoSecre = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionSecre = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailSecre = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqSecre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadSecre = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionSecre = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa1 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa1 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa1 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa1 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa1 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa1 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa1 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa1 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa1 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa1 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa1 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa1 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa2 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa2 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa2 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa2 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa2 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa2 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa2 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa2 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa2 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa2 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa3 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa3 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa3 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa3 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa3 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa3 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa3 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa3 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa4 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa4 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa4 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa4 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa4 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa4 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa4 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa4 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa4 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa4 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa4 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa4 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadVisa5 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa5 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa5 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa5 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa5 = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreVisa5 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoVisa5 = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa5 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailVisa5 = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqVisa5 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa5 = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa5 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadAna = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionAna = persona.Unidad.Pl_UndDes.Trim();
                model.RutAna = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVAna = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdAna = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.NombreAna = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoAna = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionAna = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailAna = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqAna = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadAna = persona.FunDatosLaborales.RH_ContCod;
                model.CalidadDescripcionAna = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAna(Memorandum model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa3 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa4 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa5 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdAna = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");


            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseMemorandum(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.MemorandumUpdate(model);

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
            var model = _repository.GetById<Memorandum>(id);
            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();
            if ((Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)) /*genera CDP, por la etapa en la que se encuentra*/
            {
                /*Se genera certificado de viatico*/
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("CDPViatico", new { id = model.MemorandumId }) { FileName = "CDP_Viatico" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);
                tipoDoc = 2;
                Name = "CDP Viatico Cometido nro" + " " + model.MemorandumId.ToString() + ".pdf";
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

                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.MemorandumId }) { FileName = "Resolucion" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);

                tipoDoc = 8;
                Name = "Memorandum nro" + " " + model.MemorandumId.ToString() + ".pdf";

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

            return RedirectToAction("Edit", "Memorandum", new { model.WorkflowId, id = model.MemorandumId });


            //return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult GeneraDocumento2(int id)
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
            var model = _repository.GetById<Memorandum>(id);
            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();
            if ((Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)) /*genera CDP, por la etapa en la que se encuentra*/
            {
                /*Se genera certificado de viatico*/
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("CDPViatico", new { id = model.MemorandumId }) { FileName = "CDP_Viatico" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);
                tipoDoc = 2;
                Name = "CDP Viatico Cometido nro" + " " + model.MemorandumId.ToString() + ".pdf";
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

                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.MemorandumId }) { FileName = "Resolucion" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);

                tipoDoc = 8;
                Name = "Memorandum nro" + " " + model.MemorandumId.ToString() + ".pdf";

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var resolucion = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
                if (resolucion != null)
                {
                    foreach (var res in resolucion)
                    {
                        if (res.TipoDocumentoId == 8)
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

            return RedirectToAction("Sign", "Memorandum", new { model.WorkflowId, id = model.MemorandumId });


            //return Redirect(Request.UrlReferrer.PathAndQuery);
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
            var model = _repository.GetById<Memorandum>(id);

            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();
            if (Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
            {
                //model.GeneracionCDP.FirstOrDefault().FechaFirma = DateTime.Now;
                //model.GeneracionCDP.FirstOrDefault().PsjFechaFirma = DateTime.Now;

                return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);
                //return View(model);
            }
            else
            {
                //model.Dias = (model.Destinos.LastOrDefault().FechaHasta.Date - model.Destinos.FirstOrDefault().FechaInicio.Date).Days + 1;
                //model.DiasPlural = "s";
                //model.Tiempo = model.Destinos.FirstOrDefault().FechaInicio < DateTime.Now ? "Pasado" : "Futuro";
                //model.Anno = DateTime.Now.Year.ToString();
                //model.Subscretaria = model.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIO DE TURISMO" : "SUBSECRETARIA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";
                model.FechaSolicitud = DateTime.Now;
                model.FechaFirma = DateTime.Now;

                //model.Firma = false;
                //model.NumeroResolucion = model.CometidoId;
                //model.Destinos.FirstOrDefault().TotalViaticoPalabras = ExtensionesString.enletras(model.Destinos.FirstOrDefault().TotalViatico.ToString());

                /*se traen los datos de la tabla parrafos*/
                //var parrafos = _repository.GetAll<Parrafos>();
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
                //        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistoRAE).FirstOrDefault().ParrafoTexto;
                //        var vit = _repository.Get<Parrafos>(c => c.ParrafosId == (int)App.Util.Enum.Firmas.ViaticodeVuelta && c.ParrafoActivo == true).ToList();
                //        if (vit.Count > 0)
                //            model.ViaticodeVuelta = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.ViaticodeVuelta).FirstOrDefault().ParrafoTexto;
                //        else
                //            model.ViaticodeVuelta = string.Empty;

                //        break;
                //}

                /*Se valida que se encuentre en la tarea de Firma electronica para agregar folio y fecha de resolucion*/
                var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId) ?? null;
                if (workflowActual.DefinicionWorkflow.Secuencia == 7 || (workflowActual.DefinicionWorkflow.Secuencia == 7 && workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.Memorandum))
                {
                    if (model.Folio == null)
                    {
                        #region Folio
                        /*se va a buscar el folio de testing*/
                        DTOFolio folio = new DTOFolio();
                        folio.periodo = DateTime.Now.Year.ToString();
                        folio.solicitante = "Gestion Procesos - Memorandum";/*Sistema que solicita el numero de Folio*/
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
                                    folio.tipodocumento = "MEMO";/*Resolución Administrativa Exenta*/
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
                            model.FechaSolicitud = model.FechaFirma;
                            //model.Firma = true;
                            //model.TipoActoAdministrativo = "Resolución Administrativa Exenta";

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

        public ActionResult Anular(int id)
        {
            var model = _repository.GetById<Workflow>(id);
            return View(model);
        }

        //public FileResult ShowDocumentoSinFirma(int id)
        //{
        //    var model = _repository.GetById<Memorandum>(id);
        //    return File(model.DocumentoSinFirma, "application/pdf");
        //}

        //Firma Vieja
        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<Memorandum>(id);

            /*Validar si existe un documento asociado y si se encuentra firmado*/
            var doc = _repository.GetAll<Documento>().Where(c => c.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 8);
            if (doc != null)
            {
                if (doc.FirstOrDefault().Signed != true)
                    model.Proceso.Documentos = doc.ToList();
                //else
                //    TempData["Warning"] = "Documento se encuentra firmado electronicamente";
            }

            return View(model);
        }

        //public ActionResult Sign(int id)
        //{
        //    var firma = _repository.GetById<Memorandum>(id);
        //    var email = UserExtended.Email(User);

        //    var model = new Memorandum
        //    {
        //        MemorandumId = firma.MemorandumId,
        //        ProcesoId = firma.ProcesoId,
        //        WorkflowId = firma.WorkflowId,
        //        //File = firma.DocumentoSinFirma,
        //        //Comentario = firma.Observaciones,
        //        //Firmante = email,
        //        Firmante = firma.EmailRem,
        //        //TieneFirma = _repository.GetExists<Rubrica>(q => q.Email == email),
        //        //TipoDocumentoDescripcion = firma.TipoDocumentoDescripcion,
        //        //FechaCreacion = firma.FechaCreacion,
        //        //Autor = firma.Autor,
        //        Folio = firma.Folio,
        //        //URL = firma.URL
        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Sign(Memorandum model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        model.Firmante = UserExtended.Email(User);

        //        var _useCaseInteractor = new UseCaseMemorandum(_repository, _sigper, _file, _folio, _hsm, _email);
        //        var _UseCaseResponseMessage = _useCaseInteractor.Sign(model.MemorandumId, new List<string> { model.Firmante });
        //        if (_UseCaseResponseMessage.IsValid)
        //        {
        //            TempData["Success"] = "Operación terminada correctamente.";
        //            return RedirectToAction("Sign", "Memorandum", new { id = model.MemorandumId });
        //        }

        //        TempData["Error"] = _UseCaseResponseMessage.Errors;
        //    }

        //    return RedirectToAction("Sign", "FirmaDocumento", new { id = model.MemorandumId });
        //}

        //public FileResult ShowDocumentoSinFirma(int id)
        //{
        //    var model = _repository.GetById<Memorandum>(id);
        //    return File(model.DocumentoSinFirma, "application/pdf");
        //}

        public ActionResult DetailsGM(int id)
        {
            //var model = _repository.GetById<Memorandum>(id);

            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //if (model.GeneracionCDP.Count > 0)
            //{
            //    var cdp = _repository.GetById<GeneracionCDP>(model.GeneracionCDP.FirstOrDefault().GeneracionCDPId);
            //    model.GeneracionCDP.Add(cdp);
            //}

            //return View(model);

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
            var model = _repository.GetById<Memorandum>(id);

            //model.FechaFirma = DateTime.Now;

            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();
            if ((Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)) /*genera CDP, por la etapa en la que se encuentra*/
            {
                /*Se genera certificado de viatico*/
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("CDPViatico", new { id = model.MemorandumId }) { FileName = "CDP_Viatico" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);
                tipoDoc = 2;
                Name = "CDP Viatico Cometido nro" + " " + model.MemorandumId.ToString() + ".pdf";
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

                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.MemorandumId }) { FileName = "Resolucion" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);

                tipoDoc = 8;
                Name = "Memorandum nro" + " " + model.MemorandumId.ToString() + ".pdf";

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var resolucion = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
                if (resolucion != null)
                {
                    foreach (var res in resolucion)
                    {
                        if (res.TipoDocumentoId == 8)
                            IdDocto = res.DocumentoId;
                    }
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

            //return Redirect(Request.UrlReferrer.PathAndQuery);
            return RedirectToAction("Sign", "Memorandum", new { model.WorkflowId, id = model.MemorandumId });
        }

        public ActionResult SeguimientoMemo()
        {
            var model = new DTOFilterMemorandum();

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
        public ActionResult SeguimientoMemo(DTOFilterMemorandum model)
        {
            var predicate = PredicateBuilder.True<Memorandum>();

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
                    predicate = predicate.And(q => MemorandumId.Contains(q.MemorandumId));

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
            var result = _repository.GetAll<Memorandum>();

            var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\SeguimientoMemo.xlsx");
            var fileInfo = new FileInfo(file);
            var excelPackageSeguimientoMemo = new ExcelPackage(fileInfo);

            var fila = 1;
            var worksheet = excelPackageSeguimientoMemo.Workbook.Worksheets[1];
            foreach (var memorandum in result.ToList())
            {
                var workflow = _repository.GetAll<Workflow>().Where(w => w.ProcesoId == memorandum.ProcesoId);
                //var destino = _repository.GetAll<Destinos>().Where(d => d.CometidoId == cometido.CometidoId).ToList();

                //if (destino.Count > 0)
                //{
                    fila++;
                    worksheet.Cells[fila, 1].Value = memorandum.Proceso.Terminada == false && memorandum.Proceso.Anulada == false ? "En Proceso" : memorandum.Workflow.Terminada == true ? "Terminada" : "Anulada";
                    worksheet.Cells[fila, 2].Value = memorandum.UnidadDescripcion.Contains("Turismo") ? "Turismo" : "Economía";
                    worksheet.Cells[fila, 3].Value = memorandum.MemorandumId.ToString();
                    worksheet.Cells[fila, 4].Value = memorandum.NombreId;
                    worksheet.Cells[fila, 5].Value = memorandum.UnidadDescripcion;
                    //worksheet.Cells[fila, 6].Value = destino.FirstOrDefault().RegionDescripcion != null ? destino.FirstOrDefault().RegionDescripcion : "S/A";
                    worksheet.Cells[fila, 7].Value = memorandum.FechaSolicitud.ToString();
                    //worksheet.Cells[fila, 8].Value = destino.Any() ? destino.FirstOrDefault().FechaInicio.ToString() : "S/A";
                    //worksheet.Cells[fila, 9].Value = destino.Any() ? destino.LastOrDefault().FechaHasta.ToString() : "S/A";
                    worksheet.Cells[fila, 10].Value = workflow.LastOrDefault().Email;
                    worksheet.Cells[fila, 11].Value = workflow.LastOrDefault().FechaCreacion.ToString();
                //}
            }

            return File(excelPackageSeguimientoMemo.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, DateTime.Now.ToString("rptSeguimientoGP_yyyyMMddhhmmss") + ".xlsx");
        }
    }
}