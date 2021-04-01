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
using App.Util;
using Newtonsoft.Json;
using App.Core.UseCases;
using System.ComponentModel.DataAnnotations;
using System.IO;
using OfficeOpenXml;
//using App.Infrastructure.Extensions;
//using com.sun.corba.se.spi.ior;
//using System.Net.Mail;
//using com.sun.codemodel.@internal;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
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
            public DateTime? Desde { get; set; }

            [Display(Name = "Hasta")]
            [DataType(DataType.Date)]
            public DateTime? Hasta { get; set; }

            [Display(Name = "ID")]
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
        private static List<DTODomainUser> ActiveDirectoryUsers { get; set; }
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

        public JsonResult GetUsuarioDest(int RutDest)
        {
            var correodest = _sigper.GetUserByRut(RutDest).Funcionario.Rh_Mail.Trim();
            var perdest = _sigper.GetUserByEmail(correodest);
            var IdCargoDest = perdest.DatosLaborales.RhConCar.Value;
            var cargodest = string.IsNullOrEmpty(perdest.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perdest.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadDest = perdest.DatosLaborales.RH_ContCod;
            var calidaddest = string.IsNullOrEmpty(perdest.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perdest.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoDest = string.IsNullOrEmpty(perdest.DatosLaborales.RhConGra.Trim()) ? "0" : perdest.DatosLaborales.RhConGra.Trim();
            var gradodest = string.IsNullOrEmpty(perdest.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perdest.DatosLaborales.RhConGra.Trim();
            var estamentodest = perdest.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perdest.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
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
            var IdCargoSecre = persecre.DatosLaborales.RhConCar.Value;
            var cargosecre = string.IsNullOrEmpty(persecre.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == persecre.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadSecre = persecre.DatosLaborales.RH_ContCod;
            var calidadsecre = string.IsNullOrEmpty(persecre.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == persecre.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoSecre = string.IsNullOrEmpty(persecre.DatosLaborales.RhConGra.Trim()) ? "0" : persecre.DatosLaborales.RhConGra.Trim();
            var gradosecre = string.IsNullOrEmpty(persecre.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : persecre.DatosLaborales.RhConGra.Trim();
            var estamentosecre = persecre.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persecre.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
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
            var IdCargoVisa1 = pervisa1.DatosLaborales.RhConCar.Value;
            var cargovisa1 = string.IsNullOrEmpty(pervisa1.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa1.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa1 = pervisa1.DatosLaborales.RH_ContCod;
            var calidadvisa1 = string.IsNullOrEmpty(pervisa1.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa1.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa1 = string.IsNullOrEmpty(pervisa1.DatosLaborales.RhConGra.Trim()) ? "0" : pervisa1.DatosLaborales.RhConGra.Trim();
            var gradovisa1 = string.IsNullOrEmpty(pervisa1.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa1.DatosLaborales.RhConGra.Trim();
            var estamentovisa1 = pervisa1.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa1.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
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
            var IdCargoVisa2 = pervisa2.DatosLaborales.RhConCar.Value;
            var cargovisa2 = string.IsNullOrEmpty(pervisa2.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa2.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa2 = pervisa2.DatosLaborales.RH_ContCod;
            var calidadvisa2 = string.IsNullOrEmpty(pervisa2.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa2.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa2 = string.IsNullOrEmpty(pervisa2.DatosLaborales.RhConGra.Trim()) ? "0" : pervisa2.DatosLaborales.RhConGra.Trim();
            var gradovisa2 = string.IsNullOrEmpty(pervisa2.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa2.DatosLaborales.RhConGra.Trim();
            var estamentovisa2 = pervisa2.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa2.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
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
            var IdCargoVisa3 = pervisa3.DatosLaborales.RhConCar.Value;
            var cargovisa3 = string.IsNullOrEmpty(pervisa3.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa3.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa3 = pervisa3.DatosLaborales.RH_ContCod;
            var calidadvisa3 = string.IsNullOrEmpty(pervisa3.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa3.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa3 = string.IsNullOrEmpty(pervisa3.DatosLaborales.RhConGra.Trim()) ? "0" : pervisa3.DatosLaborales.RhConGra.Trim();
            var gradovisa3 = string.IsNullOrEmpty(pervisa3.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa3.DatosLaborales.RhConGra.Trim();
            var estamentovisa3 = pervisa3.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa3.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
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
            var IdCargoVisa4 = pervisa4.DatosLaborales.RhConCar.Value;
            var cargovisa4 = string.IsNullOrEmpty(pervisa4.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa4.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa4 = pervisa4.DatosLaborales.RH_ContCod;
            var calidadvisa4 = string.IsNullOrEmpty(pervisa4.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa4.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa4 = string.IsNullOrEmpty(pervisa4.DatosLaborales.RhConGra.Trim()) ? "0" : pervisa4.DatosLaborales.RhConGra.Trim();
            var gradovisa4 = string.IsNullOrEmpty(pervisa4.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa4.DatosLaborales.RhConGra.Trim();
            var estamentovisa4 = pervisa4.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa4.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
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
            var IdCargoVisa5 = pervisa5.DatosLaborales.RhConCar.Value;
            var cargovisa5 = string.IsNullOrEmpty(pervisa5.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa5.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa5 = pervisa5.DatosLaborales.RH_ContCod;
            var calidadvisa5 = string.IsNullOrEmpty(pervisa5.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa5.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa5 = string.IsNullOrEmpty(pervisa5.DatosLaborales.RhConGra.Trim()) ? "0" : pervisa5.DatosLaborales.RhConGra.Trim();
            var gradovisa5 = string.IsNullOrEmpty(pervisa5.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa5.DatosLaborales.RhConGra.Trim();
            var estamentovisa5 = pervisa5.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa5.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
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

        public JsonResult GetUsuarioVisa6(int RutVisa6)
        {
            var correovisa6 = _sigper.GetUserByRut(RutVisa6).Funcionario.Rh_Mail.Trim();
            var pervisa6 = _sigper.GetUserByEmail(correovisa6);
            var IdCargoVisa6 = pervisa6.DatosLaborales.RhConCar.Value;
            var cargovisa6 = string.IsNullOrEmpty(pervisa6.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa6.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa6 = pervisa6.DatosLaborales.RH_ContCod;
            var calidadvisa6 = string.IsNullOrEmpty(pervisa6.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa6.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa6 = string.IsNullOrEmpty(pervisa6.DatosLaborales.RhConGra.Trim()) ? "0" : pervisa6.DatosLaborales.RhConGra.Trim();
            var gradovisa6 = string.IsNullOrEmpty(pervisa6.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa6.DatosLaborales.RhConGra.Trim();
            var estamentovisa6 = pervisa6.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa6.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdVisa6 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa6.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa6.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa6.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaVisa6 = ProgIdVisa6 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa6).FirstOrDefault().RePytDes : "S/A";
            var conglomeradovisa6 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa6.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa6.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa6.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa6.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa6 = pervisa6.Jefatura != null ? pervisa6.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa6 = pervisa6.Funcionario != null ? pervisa6.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrevisa6 = pervisa6.Funcionario != null ? pervisa6.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutvisa6;
            if (pervisa6.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = pervisa6.Funcionario.RH_NumInte.ToString();
                rutvisa6 = string.Concat("0", t);
            }
            else
            {
                rutvisa6 = pervisa6.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutVisa6 = rutvisa6,
                DVVisa6 = pervisa6.Funcionario.RH_DvNuInt.ToString(),
                IdCargoVisa6 = IdCargoVisa6,
                CargoVisa6 = cargovisa6,
                IdCalidadVisa6 = IdCalidadVisa6,
                CalidadJuridicaVisa6 = calidadvisa6,
                IdGradoVisa6 = IdGradoVisa6,
                GradoVisa6 = gradovisa6,
                EstamentoVisa6 = estamentovisa6,
                ProgramaVisa6 = ProgramaVisa6.Trim(),
                ConglomeradoVisa6 = conglomeradovisa6,
                IdUnidadVisa6 = pervisa6.Unidad.Pl_UndCod,
                UnidadVisa6 = pervisa6.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa6 = jefaturavisa6,
                EmailVisa6 = ecorreovisa6,
                NombreChqVisa6 = pervisa6.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa7(int RutVisa7)
        {
            var correovisa7 = _sigper.GetUserByRut(RutVisa7).Funcionario.Rh_Mail.Trim();
            var pervisa7 = _sigper.GetUserByEmail(correovisa7);
            var IdCargoVisa7 = pervisa7.DatosLaborales.RhConCar.Value;
            var cargovisa7 = string.IsNullOrEmpty(pervisa7.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa7.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa7 = pervisa7.DatosLaborales.RH_ContCod;
            var calidadvisa7 = string.IsNullOrEmpty(pervisa7.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa7.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa7 = string.IsNullOrEmpty(pervisa7.DatosLaborales.RhConGra.Trim()) ? "0" : pervisa7.DatosLaborales.RhConGra.Trim();
            var gradovisa7 = string.IsNullOrEmpty(pervisa7.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa7.DatosLaborales.RhConGra.Trim();
            var estamentovisa7 = pervisa7.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa7.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdVisa7 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa7.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa7.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa7.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaVisa7 = ProgIdVisa7 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa7).FirstOrDefault().RePytDes : "S/A";
            var conglomeradovisa7 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa7.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa7.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa7.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa7.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa7 = pervisa7.Jefatura != null ? pervisa7.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa7 = pervisa7.Funcionario != null ? pervisa7.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrevisa7 = pervisa7.Funcionario != null ? pervisa7.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutvisa7;
            if (pervisa7.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = pervisa7.Funcionario.RH_NumInte.ToString();
                rutvisa7 = string.Concat("0", t);
            }
            else
            {
                rutvisa7 = pervisa7.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutVisa7 = rutvisa7,
                DVVisa7 = pervisa7.Funcionario.RH_DvNuInt.ToString(),
                IdCargoVisa7 = IdCargoVisa7,
                CargoVisa7 = cargovisa7,
                IdCalidadVisa7 = IdCalidadVisa7,
                CalidadJuridicaVisa7 = calidadvisa7,
                IdGradoVisa7 = IdGradoVisa7,
                GradoVisa7 = gradovisa7,
                EstamentoVisa7 = estamentovisa7,
                ProgramaVisa7 = ProgramaVisa7.Trim(),
                ConglomeradoVisa7 = conglomeradovisa7,
                IdUnidadVisa7 = pervisa7.Unidad.Pl_UndCod,
                UnidadVisa7 = pervisa7.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa7 = jefaturavisa7,
                EmailVisa7 = ecorreovisa7,
                NombreChqVisa7 = pervisa7.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa8(int RutVisa8)
        {
            var correovisa8 = _sigper.GetUserByRut(RutVisa8).Funcionario.Rh_Mail.Trim();
            var pervisa8 = _sigper.GetUserByEmail(correovisa8);
            var IdCargoVisa8 = pervisa8.DatosLaborales.RhConCar.Value;
            var cargovisa8 = string.IsNullOrEmpty(pervisa8.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa8.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa8 = pervisa8.DatosLaborales.RH_ContCod;
            var calidadvisa8 = string.IsNullOrEmpty(pervisa8.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa8.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa8 = string.IsNullOrEmpty(pervisa8.DatosLaborales.RhConGra.Trim()) ? "0" : pervisa8.DatosLaborales.RhConGra.Trim();
            var gradovisa8 = string.IsNullOrEmpty(pervisa8.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa8.DatosLaborales.RhConGra.Trim();
            var estamentovisa8 = pervisa8.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa8.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdVisa8 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa8.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa8.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa8.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaVisa8 = ProgIdVisa8 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa8).FirstOrDefault().RePytDes : "S/A";
            var conglomeradovisa8 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa8.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa8.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa8.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa8.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa8 = pervisa8.Jefatura != null ? pervisa8.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa8 = pervisa8.Funcionario != null ? pervisa8.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrevisa8 = pervisa8.Funcionario != null ? pervisa8.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutvisa8;
            if (pervisa8.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = pervisa8.Funcionario.RH_NumInte.ToString();
                rutvisa8 = string.Concat("0", t);
            }
            else
            {
                rutvisa8 = pervisa8.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutVisa8 = rutvisa8,
                DVVisa8 = pervisa8.Funcionario.RH_DvNuInt.ToString(),
                IdCargoVisa8 = IdCargoVisa8,
                CargoVisa8 = cargovisa8,
                IdCalidadVisa8 = IdCalidadVisa8,
                CalidadJuridicaVisa8 = calidadvisa8,
                IdGradoVisa8 = IdGradoVisa8,
                GradoVisa8 = gradovisa8,
                EstamentoVisa8 = estamentovisa8,
                ProgramaVisa8 = ProgramaVisa8.Trim(),
                ConglomeradoVisa8 = conglomeradovisa8,
                IdUnidadVisa8 = pervisa8.Unidad.Pl_UndCod,
                UnidadVisa8 = pervisa8.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa8 = jefaturavisa8,
                EmailVisa8 = ecorreovisa8,
                NombreChqVisa8 = pervisa8.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa9(int RutVisa9)
        {
            var correovisa9 = _sigper.GetUserByRut(RutVisa9).Funcionario.Rh_Mail.Trim();
            var pervisa9 = _sigper.GetUserByEmail(correovisa9);
            var IdCargoVisa9 = pervisa9.DatosLaborales.RhConCar.Value;
            var cargovisa9 = string.IsNullOrEmpty(pervisa9.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa9.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa9 = pervisa9.DatosLaborales.RH_ContCod;
            var calidadvisa9 = string.IsNullOrEmpty(pervisa9.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa9.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa9 = string.IsNullOrEmpty(pervisa9.DatosLaborales.RhConGra.Trim()) ? "0" : pervisa9.DatosLaborales.RhConGra.Trim();
            var gradovisa9 = string.IsNullOrEmpty(pervisa9.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa9.DatosLaborales.RhConGra.Trim();
            var estamentovisa9 = pervisa9.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa9.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdVisa9 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa9.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa9.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa9.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaVisa9 = ProgIdVisa9 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa9).FirstOrDefault().RePytDes : "S/A";
            var conglomeradovisa9 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa9.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa9.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa9.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa9.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa9 = pervisa9.Jefatura != null ? pervisa9.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa9 = pervisa9.Funcionario != null ? pervisa9.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrevisa9 = pervisa9.Funcionario != null ? pervisa9.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutvisa9;
            if (pervisa9.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = pervisa9.Funcionario.RH_NumInte.ToString();
                rutvisa9 = string.Concat("0", t);
            }
            else
            {
                rutvisa9 = pervisa9.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutVisa9 = rutvisa9,
                DVVisa9 = pervisa9.Funcionario.RH_DvNuInt.ToString(),
                IdCargoVisa9 = IdCargoVisa9,
                CargoVisa9 = cargovisa9,
                IdCalidadVisa9 = IdCalidadVisa9,
                CalidadJuridicaVisa9 = calidadvisa9,
                IdGradoVisa9 = IdGradoVisa9,
                GradoVisa9 = gradovisa9,
                EstamentoVisa9 = estamentovisa9,
                ProgramaVisa9 = ProgramaVisa9.Trim(),
                ConglomeradoVisa9 = conglomeradovisa9,
                IdUnidadVisa9 = pervisa9.Unidad.Pl_UndCod,
                UnidadVisa9 = pervisa9.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa9 = jefaturavisa9,
                EmailVisa9 = ecorreovisa9,
                NombreChqVisa9 = pervisa9.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa10(int RutVisa10)
        {
            var correovisa10 = _sigper.GetUserByRut(RutVisa10).Funcionario.Rh_Mail.Trim();
            var pervisa10 = _sigper.GetUserByEmail(correovisa10);
            var IdCargoVisa10 = pervisa10.DatosLaborales.RhConCar.Value;
            var cargovisa10 = string.IsNullOrEmpty(pervisa10.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa10.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadVisa10 = pervisa10.DatosLaborales.RH_ContCod;
            var calidadvisa10 = string.IsNullOrEmpty(pervisa10.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa10.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoVisa10 = string.IsNullOrEmpty(pervisa10.DatosLaborales.RhConGra.Trim()) ? "0" : pervisa10.DatosLaborales.RhConGra.Trim();
            var gradovisa10 = string.IsNullOrEmpty(pervisa10.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa10.DatosLaborales.RhConGra.Trim();
            var estamentovisa10 = pervisa10.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa10.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdVisa10 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa10.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa10.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa10.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaVisa10 = ProgIdVisa10 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa10).FirstOrDefault().RePytDes : "S/A";
            var conglomeradovisa10 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa10.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa10.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa10.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa10.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa10 = pervisa10.Jefatura != null ? pervisa10.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa10 = pervisa10.Funcionario != null ? pervisa10.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrevisa10 = pervisa10.Funcionario != null ? pervisa10.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutvisa10;
            if (pervisa10.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = pervisa10.Funcionario.RH_NumInte.ToString();
                rutvisa10 = string.Concat("0", t);
            }
            else
            {
                rutvisa10 = pervisa10.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutVisa10 = rutvisa10,
                DVVisa10 = pervisa10.Funcionario.RH_DvNuInt.ToString(),
                IdCargoVisa10 = IdCargoVisa10,
                CargoVisa10 = cargovisa10,
                IdCalidadVisa10 = IdCalidadVisa10,
                CalidadJuridicaVisa10 = calidadvisa10,
                IdGradoVisa10 = IdGradoVisa10,
                GradoVisa10 = gradovisa10,
                EstamentoVisa10 = estamentovisa10,
                ProgramaVisa10 = ProgramaVisa10.Trim(),
                ConglomeradoVisa10 = conglomeradovisa10,
                IdUnidadVisa10 = pervisa10.Unidad.Pl_UndCod,
                UnidadVisa10 = pervisa10.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa10 = jefaturavisa10,
                EmailVisa10 = ecorreovisa10,
                NombreChqVisa10 = pervisa10.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioAna(int RutAna)
        {
            var correoana = _sigper.GetUserByRut(RutAna).Funcionario.Rh_Mail.Trim();
            var perana = _sigper.GetUserByEmail(correoana);
            var IdCargoAna = perana.DatosLaborales.RhConCar.Value;
            var cargoana = string.IsNullOrEmpty(perana.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perana.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadAna = perana.DatosLaborales.RH_ContCod;
            var calidadana = string.IsNullOrEmpty(perana.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perana.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoAna = string.IsNullOrEmpty(perana.DatosLaborales.RhConGra.Trim()) ? "0" : perana.DatosLaborales.RhConGra.Trim();
            var gradoana = string.IsNullOrEmpty(perana.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perana.DatosLaborales.RhConGra.Trim();
            var estamentoana = perana.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perana.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
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

        public JsonResult GetUsuarioAutorizaFirma1(int RutAutorizaFirma1)
        {
            var correoautorizafirma1 = _sigper.GetUserByRut(RutAutorizaFirma1).Funcionario.Rh_Mail.Trim();
            var perautorizafirma1 = _sigper.GetUserByEmail(correoautorizafirma1);
            var IdCargoAutorizaFirma1 = perautorizafirma1.DatosLaborales.RhConCar.Value;
            var cargoautorizafirma1 = string.IsNullOrEmpty(perautorizafirma1.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perautorizafirma1.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadAutorizaFirma1 = perautorizafirma1.DatosLaborales.RH_ContCod;
            var calidadautorizafirma1 = string.IsNullOrEmpty(perautorizafirma1.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perautorizafirma1.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoAutorizaFirma1 = string.IsNullOrEmpty(perautorizafirma1.DatosLaborales.RhConGra.Trim()) ? "0" : perautorizafirma1.DatosLaborales.RhConGra.Trim();
            var gradoautorizafirma1 = string.IsNullOrEmpty(perautorizafirma1.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perautorizafirma1.DatosLaborales.RhConGra.Trim();
            var estamentoautorizafirma1 = perautorizafirma1.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perautorizafirma1.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdAutorizaFirma1 = _sigper.GetReContra().Where(c => c.RH_NumInte == perautorizafirma1.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perautorizafirma1.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perautorizafirma1.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaAutorizaFirma1 = ProgIdAutorizaFirma1 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdAutorizaFirma1).FirstOrDefault().RePytDes : "S/A";
            var conglomeradoautorizafirma1 = _sigper.GetReContra().Where(c => c.RH_NumInte == perautorizafirma1.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perautorizafirma1.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perautorizafirma1.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perautorizafirma1.Funcionario.RH_NumInte).ReContraSed;
            var jefaturaautorizafirma1 = perautorizafirma1.Jefatura != null ? perautorizafirma1.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreoautorizafirma1 = perautorizafirma1.Funcionario != null ? perautorizafirma1.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombreautorizafirma1 = perautorizafirma1.Funcionario != null ? perautorizafirma1.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutautorizafirma1;
            if (perautorizafirma1.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perautorizafirma1.Funcionario.RH_NumInte.ToString();
                rutautorizafirma1 = string.Concat("0", t);
            }
            else
            {
                rutautorizafirma1 = perautorizafirma1.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutAutorizaFirma1 = rutautorizafirma1,
                DVAutorizaFirma1 = perautorizafirma1.Funcionario.RH_DvNuInt.ToString(),
                IdCargoAutorizaFirma1 = IdCargoAutorizaFirma1,
                CargoAutorizaFirma1 = cargoautorizafirma1,
                IdCalidadautorizaFirma1 = IdCalidadAutorizaFirma1,
                CalidadJuridicaAutorizaFirma1 = calidadautorizafirma1,
                IdGradoautorizaFirma1 = IdGradoAutorizaFirma1,
                GradoautorizaFirma1 = gradoautorizafirma1,
                EstamentoautorizaFirma1 = estamentoautorizafirma1,
                ProgramaAutorizaFirma1 = ProgramaAutorizaFirma1.Trim(),
                ConglomeradoAutorizaFirma1 = conglomeradoautorizafirma1,
                IdUnidadAutorizaFirma1 = perautorizafirma1.Unidad.Pl_UndCod,
                UnidadAutorizaFirma1 = perautorizafirma1.Unidad.Pl_UndDes.Trim(),
                JefaturaAutorizaFirma1 = jefaturaautorizafirma1,
                EmailAutorizaFirma1 = ecorreoautorizafirma1,
                NombreChqAutorizaFirma1 = perautorizafirma1.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioAutorizaFirma2(int RutAutorizaFirma2)
        {
            var correoautorizafirma2 = _sigper.GetUserByRut(RutAutorizaFirma2).Funcionario.Rh_Mail.Trim();
            var perautorizafirma2 = _sigper.GetUserByEmail(correoautorizafirma2);
            var IdCargoAutorizaFirma2 = perautorizafirma2.DatosLaborales.RhConCar.Value;
            var cargoautorizafirma2 = string.IsNullOrEmpty(perautorizafirma2.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perautorizafirma2.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadAutorizaFirma2 = perautorizafirma2.DatosLaborales.RH_ContCod;
            var calidadautorizafirma2 = string.IsNullOrEmpty(perautorizafirma2.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perautorizafirma2.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoAutorizaFirma2 = string.IsNullOrEmpty(perautorizafirma2.DatosLaborales.RhConGra.Trim()) ? "0" : perautorizafirma2.DatosLaborales.RhConGra.Trim();
            var gradoautorizafirma2 = string.IsNullOrEmpty(perautorizafirma2.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perautorizafirma2.DatosLaborales.RhConGra.Trim();
            var estamentoautorizafirma2 = perautorizafirma2.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perautorizafirma2.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdAutorizaFirma2 = _sigper.GetReContra().Where(c => c.RH_NumInte == perautorizafirma2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perautorizafirma2.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perautorizafirma2.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaAutorizaFirma2 = ProgIdAutorizaFirma2 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdAutorizaFirma2).FirstOrDefault().RePytDes : "S/A";
            var conglomeradoautorizafirma2 = _sigper.GetReContra().Where(c => c.RH_NumInte == perautorizafirma2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perautorizafirma2.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perautorizafirma2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perautorizafirma2.Funcionario.RH_NumInte).ReContraSed;
            var jefaturaautorizafirma2 = perautorizafirma2.Jefatura != null ? perautorizafirma2.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreoautorizafirma2 = perautorizafirma2.Funcionario != null ? perautorizafirma2.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombreautorizafirma2 = perautorizafirma2.Funcionario != null ? perautorizafirma2.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutautorizafirma2;
            if (perautorizafirma2.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perautorizafirma2.Funcionario.RH_NumInte.ToString();
                rutautorizafirma2 = string.Concat("0", t);
            }
            else
            {
                rutautorizafirma2 = perautorizafirma2.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutAutorizaFirma2 = rutautorizafirma2,
                DVAutorizaFirma2 = perautorizafirma2.Funcionario.RH_DvNuInt.ToString(),
                IdCargoAutorizaFirma2 = IdCargoAutorizaFirma2,
                CargoAutorizaFirma2 = cargoautorizafirma2,
                IdCalidadautorizaFirma2 = IdCalidadAutorizaFirma2,
                CalidadJuridicaAutorizaFirma2 = calidadautorizafirma2,
                IdGradoautorizaFirma2 = IdGradoAutorizaFirma2,
                GradoautorizaFirma2 = gradoautorizafirma2,
                EstamentoautorizaFirma2 = estamentoautorizafirma2,
                ProgramaAutorizaFirma2 = ProgramaAutorizaFirma2.Trim(),
                ConglomeradoAutorizaFirma2 = conglomeradoautorizafirma2,
                IdUnidadAutorizaFirma2 = perautorizafirma2.Unidad.Pl_UndCod,
                UnidadAutorizaFirma2 = perautorizafirma2.Unidad.Pl_UndDes.Trim(),
                JefaturaAutorizaFirma2 = jefaturaautorizafirma2,
                EmailAutorizaFirma2 = ecorreoautorizafirma2,
                NombreChqAutorizaFirma2 = perautorizafirma2.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioAutorizaFirma3(int RutAutorizaFirma3)
        {
            var correoautorizafirma3 = _sigper.GetUserByRut(RutAutorizaFirma3).Funcionario.Rh_Mail.Trim();
            var perautorizafirma3 = _sigper.GetUserByEmail(correoautorizafirma3);
            var IdCargoAutorizaFirma3 = perautorizafirma3.DatosLaborales.RhConCar.Value;
            var cargoautorizafirma3 = string.IsNullOrEmpty(perautorizafirma3.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == perautorizafirma3.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidadAutorizaFirma3 = perautorizafirma3.DatosLaborales.RH_ContCod;
            var calidadautorizafirma3 = string.IsNullOrEmpty(perautorizafirma3.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perautorizafirma3.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGradoAutorizaFirma3 = string.IsNullOrEmpty(perautorizafirma3.DatosLaborales.RhConGra.Trim()) ? "0" : perautorizafirma3.DatosLaborales.RhConGra.Trim();
            var gradoautorizafirma3 = string.IsNullOrEmpty(perautorizafirma3.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perautorizafirma3.DatosLaborales.RhConGra.Trim();
            var estamentoautorizafirma3 = perautorizafirma3.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perautorizafirma3.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgIdAutorizaFirma3 = _sigper.GetReContra().Where(c => c.RH_NumInte == perautorizafirma3.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perautorizafirma3.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perautorizafirma3.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var ProgramaAutorizaFirma3 = ProgIdAutorizaFirma3 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdAutorizaFirma3).FirstOrDefault().RePytDes : "S/A";
            var conglomeradoautorizafirma3 = _sigper.GetReContra().Where(c => c.RH_NumInte == perautorizafirma3.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perautorizafirma3.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perautorizafirma3.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perautorizafirma3.Funcionario.RH_NumInte).ReContraSed;
            var jefaturaautorizafirma3 = perautorizafirma3.Jefatura != null ? perautorizafirma3.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreoautorizafirma3 = perautorizafirma3.Funcionario != null ? perautorizafirma3.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombreautorizafirma3 = perautorizafirma3.Funcionario != null ? perautorizafirma3.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rutautorizafirma3;
            if (perautorizafirma3.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = perautorizafirma3.Funcionario.RH_NumInte.ToString();
                rutautorizafirma3 = string.Concat("0", t);
            }
            else
            {
                rutautorizafirma3 = perautorizafirma3.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                RutAutorizaFirma3 = rutautorizafirma3,
                DVAutorizaFirma3 = perautorizafirma3.Funcionario.RH_DvNuInt.ToString(),
                IdCargoAutorizaFirma3 = IdCargoAutorizaFirma3,
                CargoAutorizaFirma3 = cargoautorizafirma3,
                IdCalidadautorizaFirma3 = IdCalidadAutorizaFirma3,
                CalidadJuridicaAutorizaFirma3 = calidadautorizafirma3,
                IdGradoautorizaFirma3 = IdGradoAutorizaFirma3,
                GradoautorizaFirma3 = gradoautorizafirma3,
                EstamentoautorizaFirma3 = estamentoautorizafirma3,
                ProgramaAutorizaFirma3 = ProgramaAutorizaFirma3.Trim(),
                ConglomeradoAutorizaFirma3 = conglomeradoautorizafirma3,
                IdUnidadAutorizaFirma3 = perautorizafirma3.Unidad.Pl_UndCod,
                UnidadAutorizaFirma3 = perautorizafirma3.Unidad.Pl_UndDes.Trim(),
                JefaturaAutorizaFirma3 = jefaturaautorizafirma3,
                EmailAutorizaFirma3 = ecorreoautorizafirma3,
                NombreChqAutorizaFirma3 = perautorizafirma3.Funcionario.PeDatPerChq.Trim(),

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

            var model = _repository.GetById<Memorandum>(id);

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

            var model = _repository.GetById<Memorandum>(id);

            if (ModelState.IsValid)
            {
                model.FechaSolicitud = DateTime.Now;

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
            var model = _repository.GetById<Memorandum>(id);
            return View(model);
        }

        public ActionResult Create(int WorkFlowId)
        {
            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new Memorandum()
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
            };

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


            var persona = _sigper.GetUserByEmail(User.Email());
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
                //model.Nombre = persona.Funcionario.PeDatPerChq.Trim();
                model.Nombre = null;
                model.IdCargo = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailRem = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqRem = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidad = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadDest = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionDest = persona.Unidad.Pl_UndDes.Trim();
                model.RutDest = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVDest = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdDest = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                //model.NombreDest = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreDest = null;
                model.IdCargoDest = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionDest = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailDest = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqDest = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadDest = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionDest = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                model.IdUnidadSecre = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionSecre = persona.Unidad.Pl_UndDes.Trim();
                model.RutSecre = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVSecre = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdSecre = null;
                //model.NombreIdSecre = persona.Funcionario.RH_NumInte;
                //model.NombreSecre = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreSecre = null;
                model.IdCargoSecre = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionSecre = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.EmailSecre = persona.Funcionario.Rh_Mail.Trim();
                model.NombreChqSecre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadSecre = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionSecre = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa1 = persona.Unidad.Pl_UndCod;
                model.IdUnidadVisa1 = null;
                //model.UnidadDescripcionVisa1 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionVisa1 = null;
                model.RutVisa1 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa1 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa1 = null;
                //model.NombreIdVisa1 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa1 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreVisa1 = null;
                model.IdCargoVisa1 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa1 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa1 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailVisa1 = null;
                model.NombreChqVisa1 = null;
                //model.NombreChqVisa1 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa1 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa1 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa2 = persona.Unidad.Pl_UndCod;
                model.IdUnidadVisa2 = null;
                //model.UnidadDescripcionVisa2 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionVisa2 = null;
                model.RutVisa2 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa2 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa2 = null;
                //model.NombreIdVisa2 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa2 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreVisa2 = null;
                model.IdCargoVisa2 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa2 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailVisa2 = null;
                model.NombreChqVisa2 = null;
                //model.NombreChqVisa2 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa2 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa3 = persona.Unidad.Pl_UndCod;
                model.IdUnidadVisa3 = null;
                //model.UnidadDescripcionVisa3 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionVisa3 = null;
                model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa3 = null;
                //model.NombreIdVisa3 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreVisa3 = null;
                model.IdCargoVisa3 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa3 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa3 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailVisa3 = null;
                model.NombreChqVisa3 = null;
                //model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa3 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa3 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa4 = persona.Unidad.Pl_UndCod;
                model.IdUnidadVisa4 = null;
                //model.UnidadDescripcionVisa4 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionVisa4 = null;
                model.RutVisa4 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa4 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa4 = null;
                //model.NombreIdVisa4 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa4 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreVisa4 = null;
                model.IdCargoVisa4 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa4 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa4 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailVisa4 = null;
                model.NombreChqVisa4 = null;
                //model.NombreChqVisa4 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa4 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa4 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa5 = persona.Unidad.Pl_UndCod;
                model.IdUnidadVisa5 = null;
                //model.UnidadDescripcionVisa5 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionVisa5 = null;
                model.RutVisa5 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa5 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa5 = null;
                //model.NombreIdVisa5 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa5 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreVisa5 = null;
                model.IdCargoVisa5 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa5 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa5 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailVisa5 = null;
                model.NombreChqVisa5 = null;
                //model.NombreChqVisa5 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa5 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa5 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa6 = persona.Unidad.Pl_UndCod;
                model.IdUnidadVisa6 = null;
                //model.UnidadDescripcionVisa6 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionVisa6 = null;
                model.RutVisa6 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa6 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa6 = null;
                //model.NombreIdVisa6 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa6 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreVisa6 = null;
                model.IdCargoVisa6 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa6 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa6 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailVisa6 = null;
                model.NombreChqVisa6 = null;
                //model.NombreChqVisa6 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa6 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa6 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa7 = persona.Unidad.Pl_UndCod;
                model.IdUnidadVisa7 = null;
                //model.UnidadDescripcionVisa7 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionVisa7 = null;
                model.RutVisa7 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa7 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa7 = null;
                //model.NombreIdVisa7 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa7 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreVisa7 = null;
                model.IdCargoVisa7 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa7 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa7 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailVisa7 = null;
                model.NombreChqVisa7 = null;
                //model.NombreChqVisa7 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa7 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa7 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa8 = persona.Unidad.Pl_UndCod;
                model.IdUnidadVisa8 = null;
                //model.UnidadDescripcionVisa8 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionVisa8 = null;
                model.RutVisa8 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa8 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa8 = null;
                //model.NombreIdVisa8 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa8 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreVisa8 = null;
                model.IdCargoVisa8 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa8 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa8 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailVisa8 = null;
                model.NombreChqVisa8 = null;
                //model.NombreChqVisa8 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa8 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa8 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa9 = persona.Unidad.Pl_UndCod;
                model.IdUnidadVisa9 = null;
                //model.UnidadDescripcionVisa9 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionVisa9 = null;
                model.RutVisa9 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa9 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa9 = null;
                //model.NombreIdVisa9 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa9 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreVisa9 = null;
                model.IdCargoVisa9 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa9 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa9 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailVisa9 = null;
                model.NombreChqVisa9 = null;
                //model.NombreChqVisa9 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa9 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa9 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadVisa10 = persona.Unidad.Pl_UndCod;
                model.IdUnidadVisa10 = null;
                //model.UnidadDescripcionVisa10 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionVisa10 = null;
                model.RutVisa10 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa10 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa10 = null;
                //model.NombreIdVisa10 = persona.Funcionario.RH_NumInte;
                //model.NombreVisa10 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreVisa10 = null;
                model.IdCargoVisa10 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionVisa10 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailVisa10 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailVisa10 = null;
                model.NombreChqVisa10 = null;
                //model.NombreChqVisa10 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadVisa10 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionVisa10 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadAna = persona.Unidad.Pl_UndCod;
                model.IdUnidadAna = null;
                //model.UnidadDescripcionAna = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionAna = null;
                model.RutAna = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVAna = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdAna = null;
                //model.NombreIdAna = persona.Funcionario.RH_NumInte;
                //model.NombreAna = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreAna = null;
                model.IdCargoAna = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionAna = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailAna = persona.Funcionario.Rh_Mail.Trim();
                model.EmailAna = null;
                model.NombreChqAna = null;
                //model.NombreChqAna = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadAna = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionAna = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadAutorizaFirma1 = persona.Unidad.Pl_UndCod;
                model.IdUnidadAutorizaFirma1 = null;
                //model.UnidadDescripcionAutorizaFirma1 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionAutorizaFirma1 = null;
                model.RutAutorizaFirma1 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVAutorizaFirma1 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdAutorizaFirma1 = null;
                //model.NombreIdAutorizaFirma1 = persona.Funcionario.RH_NumInte;
                //model.NombreAutorizaFirma1 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreAutorizaFirma1 = null;
                model.IdCargoAutorizaFirma1 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionAutorizaFirma1 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailAutorizaFirma1 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailAutorizaFirma1 = null;
                model.NombreChqAutorizaFirma1 = null;
                //model.NombreChqAutorizaFirma1 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadAutorizaFirma1 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionAutorizaFirma1 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadAutorizaFirma2 = persona.Unidad.Pl_UndCod;
                model.IdUnidadAutorizaFirma2 = null;
                //model.UnidadDescripcionAutorizaFirma2 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionAutorizaFirma2 = null;
                model.RutAutorizaFirma2 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVAutorizaFirma2 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdAutorizaFirma2 = null;
                //model.NombreIdAutorizaFirma2 = persona.Funcionario.RH_NumInte;
                //model.NombreAutorizaFirma2 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreAutorizaFirma2 = null;
                model.IdCargoAutorizaFirma2 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionAutorizaFirma2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailAutorizaFirma2 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailAutorizaFirma2 = null;
                model.NombreChqAutorizaFirma2 = null;
                //model.NombreChqAutorizaFirma2 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadAutorizaFirma2 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionAutorizaFirma2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();

                //model.IdUnidadAutorizaFirma3 = persona.Unidad.Pl_UndCod;
                model.IdUnidadAutorizaFirma3 = null;
                //model.UnidadDescripcionAutorizaFirma3 = persona.Unidad.Pl_UndDes.Trim();
                model.UnidadDescripcionAutorizaFirma3 = null;
                model.RutAutorizaFirma3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVAutorizaFirma3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdAutorizaFirma3 = null;
                //model.NombreIdAutorizaFirma3 = persona.Funcionario.RH_NumInte;
                //model.NombreAutorizaFirma3 = persona.Funcionario.PeDatPerChq.Trim();
                model.NombreAutorizaFirma3 = null;
                model.IdCargoAutorizaFirma3 = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcionAutorizaFirma3 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.EmailAutorizaFirma3 = persona.Funcionario.Rh_Mail.Trim();
                model.EmailAutorizaFirma3 = null;
                model.NombreChqAutorizaFirma3 = null;
                //model.NombreChqAutorizaFirma3 = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCalidadAutorizaFirma3 = persona.DatosLaborales.RH_ContCod;
                model.CalidadDescripcionAutorizaFirma3 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Memorandum model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseMemorandum(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.MemorandumInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    //return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
                    return RedirectToAction("GeneraDocumento", "Memorandum", new { model.WorkflowId, id = model.MemorandumId });
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

            var model = _repository.GetById<Memorandum>(id);

            if (ModelState.IsValid)
            {
                model.FechaSolicitud = DateTime.Now;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Memorandum model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseMemorandum(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.MemorandumUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    //return Redirect(Request.UrlReferrer.PathAndQuery);
                    return RedirectToAction("GeneraDocumento", "Memorandum", new { model.WorkflowId, id = model.MemorandumId });
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

            var model = _repository.GetById<Memorandum>(id);

            if (ModelState.IsValid)
            {
                model.FechaSolicitud = DateTime.Now;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAna(Memorandum model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseMemorandum(_repository, _sigper, _file, _folio, _hsm, _email);
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
            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            int tipoDoc = 0;
            int IdDocto = 0;
            string Name = string.Empty;
            var model = _repository.GetById<Memorandum>(id);
            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
            if ((Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)) /*genera CDP, por la etapa en la que se encuentra*/
            {
                /*Se genera certificado de viatico*/
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("CDPViatico", new { id = model.MemorandumId }) { FileName = "CDP_Viatico" + ".pdf", /*Cookies = cookieCollection,*/ FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);
                tipoDoc = 2;
                Name = "CDP Viatico Cometido nro" + " " + model.MemorandumId.ToString() + ".pdf";
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
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.MemorandumId }) { FileName = "Memorandum" + ".pdf" };
                pdf = resultPdf.BuildFile(ControllerContext);
                data = _file.BynaryToText(pdf);

                tipoDoc = 8;
                Name = "Memorandum nro" + " " + model.MemorandumId.ToString() + ".pdf";

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var memorandum = _repository.GetFirst<Documento>(d => d.ProcesoId == model.ProcesoId && d.TipoDocumentoId == 8);
                if (memorandum != null)
                    IdDocto = memorandum.DocumentoId;

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
        }

        public ActionResult GeneraDocumento2(int id)
        {
            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            int tipoDoc = 0;
            int IdDocto = 0;
            string Name = string.Empty;
            var model = _repository.GetById<Memorandum>(id);
            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
            if ((Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)) /*genera CDP, por la etapa en la que se encuentra*/
            {
                /*Se genera certificado de viatico*/
                //Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("CDPViatico", new { id = model.MemorandumId }) { FileName = "CDP_Viatico" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                //pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);
                tipoDoc = 2;
                Name = "CDP Viatico Cometido nro" + " " + model.MemorandumId.ToString() + ".pdf";
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
                //Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.MemorandumId }) { FileName = "Resolucion" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.MemorandumId }) { FileName = "Memorandum" + ".pdf" };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);

                tipoDoc = 8;
                Name = "Memorandum nro" + " " + model.MemorandumId.ToString() + ".pdf";

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var resolucion = _repository.Get<Documento>(d => d.ProcesoId == model.ProcesoId);
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

        [AllowAnonymous]
        public ActionResult Pdf(int id)
        {
            var model = _repository.GetById<Memorandum>(id);

            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
            if (Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
            {
                return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);
            }
            else
            {
                model.FechaSolicitud = DateTime.Now;
                model.FechaFirma = DateTime.Now;

                /*Se valida que se encuentre en la tarea de Firma electronica para agregar folio y fecha de resolucion*/
                var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId) ?? null;
                if (workflowActual.DefinicionWorkflow.Secuencia == 12 || (workflowActual.DefinicionWorkflow.Secuencia == 12 && workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.Memorandum))
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
                        var obj = JsonConvert.DeserializeObject<DTOFolio>(result);

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


                if (model.GradoDescripcion == "0")
                {
                    return View(model);
                }
                else
                {
                    return View(model);
                }
            }
        }

        public ActionResult Anular(int id)
        {
            var model = _repository.GetById<Workflow>(id);
            return View(model);
        }

        //Firma Vieja
        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<Memorandum>(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sign(Memorandum model)
        {
            var firmante1 = _repository.GetFirst<Memorandum>(c => c.MemorandumId == model.MemorandumId);

            var listafirmantes = new List<string>();
            listafirmantes.Add(firmante1.EmailRem.ToString());

            var email = UserExtended.Email(User);

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseMemorandum(_repository, _sigper, _file, _folio, _hsm, _email);
                //var _UseCaseResponseMessage = _useCaseInteractor.Sign(model.MemorandumId, listafirmantes);
                var _UseCaseResponseMessage = _useCaseInteractor.Sign(model.MemorandumId, new List<string> { "ereyes@economia.cl", "mmontoya@economia.cl", "padiaz@economia.cl", "lcorrales@economia.cl" });

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
                //TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return View(model);
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
            var model = _repository.GetById<Memorandum>(id);

            var Workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
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
                var resolucion = _repository.Get<Documento>(d => d.ProcesoId == model.ProcesoId);
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
                var workflow = _repository.Get<Workflow>(w => w.ProcesoId == memorandum.ProcesoId);
                //var destino = _repository.GetAll<Destinos>().Where(d => d.CometidoId == cometido.CometidoId).ToList();

                //if (destino.Count > 0)
                //{
                fila++;

                if (memorandum.Proceso.EstadoProcesoId == (int)App.Util.Enum.EstadoProceso.EnProceso)
                    worksheet.Cells[fila, 1].Value = "En Curso";
                else if (memorandum.Proceso.EstadoProcesoId == (int)App.Util.Enum.EstadoProceso.Terminado)
                    worksheet.Cells[fila, 1].Value = "Terminada";
                else if (memorandum.Proceso.EstadoProcesoId == (int)App.Util.Enum.EstadoProceso.Anulado)
                    worksheet.Cells[fila, 1].Value = "Anulada";

                //worksheet.Cells[fila, 1].Value = memorandum.Proceso.Terminada == false && memorandum.Proceso.Anulada == false ? "En Proceso" : memorandum.Workflow.Terminada == true ? "Terminada" : "Anulada";
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
            excelPackageSeguimientoMemo.Workbook.Worksheets[0].Cells.AutoFitColumns();

            return File(excelPackageSeguimientoMemo.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, DateTime.Now.ToString("rptSeguimientoGP_yyyyMMddhhmmss") + ".xlsx");
        }
    }
}