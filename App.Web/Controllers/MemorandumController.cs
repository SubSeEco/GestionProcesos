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
            //var IdCalidad = per.FunDatosLaborales.RH_ContCod;
            //var calidad = string.IsNullOrEmpty(per.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == per.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGrado = string.IsNullOrEmpty(per.FunDatosLaborales.RhConGra.Trim()) ? "0" : per.FunDatosLaborales.RhConGra.Trim();
            //var grado = string.IsNullOrEmpty(per.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : per.FunDatosLaborales.RhConGra.Trim();
            //var estamento = per.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == per.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgId = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var Programa = ProgId != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgId).FirstOrDefault().RePytDes : "S/A";
            //var conglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte).ReContraSed;
            var jefatura = per.Jefatura != null ? per.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreo = per.Funcionario != null ? per.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechq = per.Funcionario != null ? per.Funcionario.PeDatPerChq : "Sin nombre definido";

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
                //IdCalidad = IdCalidad,
                //CalidadJuridica = calidad,
                //IdGrado = IdGrado,
                //Grado = grado,
                //Estamento = estamento,
                //Programa = Programa.Trim(),
                //Conglomerado = conglomerado,
                Unidad = per.Unidad.Pl_UndDes.Trim(),
                Jefatura = jefatura,
                EmailRem = ecorreo,
                NombreChqRem = nombrechq

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
            //var IdCalidadDest = perdest.FunDatosLaborales.RH_ContCod;
            //var calidaddest = string.IsNullOrEmpty(perdest.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == perdest.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGradoDest = string.IsNullOrEmpty(perdest.FunDatosLaborales.RhConGra.Trim()) ? "0" : perdest.FunDatosLaborales.RhConGra.Trim();
            //var gradodest = string.IsNullOrEmpty(perdest.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : perdest.FunDatosLaborales.RhConGra.Trim();
            //var estamentodest = perdest.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == perdest.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgIdDest = _sigper.GetReContra().Where(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var ProgramaDest = ProgIdDest != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdDest).FirstOrDefault().RePytDes : "S/A";
            //var conglomeradodest = _sigper.GetReContra().Where(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == perdest.Funcionario.RH_NumInte).ReContraSed;
            var jefaturadest = perdest.Jefatura != null ? perdest.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreodest = perdest.Funcionario != null ? perdest.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechqdest = perdest.Funcionario != null ? perdest.Funcionario.PeDatPerChq : "Sin nombre definido";
            var nombresecredest = perdest.Unidad != null ? perdest.Unidad.Pl_UndNomSec : "Sin secretaria definida";


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
                //IdCalidadDest = IdCalidadDest,
                //CalidadJuridicaDest = calidaddest,
                //IdGradoDest = IdGradoDest,
                //GradoDest = gradodest,
                //EstamentoDest = estamentodest,
                ////ProgramaDest = Programa.Trim(),
                //ConglomeradoDest = conglomeradodest,
                UnidadDest = perdest.Unidad.Pl_UndDes.Trim(),
                JefaturaDest = jefaturadest,
                EmailDest = ecorreodest,
                NombreChqDest = nombrechqdest,
                NombreSecreDest = nombresecredest,
                EmailSecreDest = _sigper.GetAllUsers().Where(c => c.PeDatPerChq.Contains(nombresecredest.Trim())).FirstOrDefault().Rh_Mail.Trim(),

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioSecre(int RutSecre)
        {
            var correosecre = _sigper.GetUserByRut(RutSecre).Funcionario.Rh_Mail.Trim();
            var persecre = _sigper.GetUserByEmail(correosecre);
            //var IdCargoSecre = persecre.FunDatosLaborales.RhConCar.Value;
            //var cargosecre = string.IsNullOrEmpty(persecre.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == persecre.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            //var IdCalidadSecre = persecre.FunDatosLaborales.RH_ContCod;
            //var calidadsecre = string.IsNullOrEmpty(persecre.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == persecre.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGradoSecre = string.IsNullOrEmpty(persecre.FunDatosLaborales.RhConGra.Trim()) ? "0" : persecre.FunDatosLaborales.RhConGra.Trim();
            //var gradosecre = string.IsNullOrEmpty(persecre.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : persecre.FunDatosLaborales.RhConGra.Trim();
            //var estamentosecre = persecre.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persecre.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgIdSecre = _sigper.GetReContra().Where(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var ProgramaSecre = ProgIdSecre != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdSecre).FirstOrDefault().RePytDes : "S/A";
            //var conglomeradosecre = _sigper.GetReContra().Where(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persecre.Funcionario.RH_NumInte).ReContraSed;
            var jefaturasecre = persecre.Jefatura != null ? persecre.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreosecre = persecre.Funcionario != null ? persecre.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechqsecre = persecre.Funcionario != null ? persecre.Funcionario.PeDatPerChq : "Sin nombre definido";

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
                //IdCargoSecre = IdCargoSecre,
                //CargoSecre = cargosecre,
                //IdCalidadSecre = IdCalidadSecre,
                //CalidadJuridicaSecre = calidadsecre,
                //IdGradoSecre = IdGradoSecre,
                //GradoSecre = gradosecre,
                //EstamentoSecre = estamentosecre,
                ////ProgramaDest = Programa.Trim(),
                //ConglomeradoSecre = conglomeradosecre,
                UnidadSecre = persecre.Unidad.Pl_UndDes.Trim(),
                JefaturaSecre = jefaturasecre,
                EmailSecre = ecorreosecre,
                NombreChqSecre = nombrechqsecre

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa1(int RutVisa1)
        {
            var correovisa1 = _sigper.GetUserByRut(RutVisa1).Funcionario.Rh_Mail.Trim();
            var pervisa1 = _sigper.GetUserByEmail(correovisa1);
            //var IdCargoVisa1 = pervisa1.FunDatosLaborales.RhConCar.Value;
            //var cargovisa1 = string.IsNullOrEmpty(pervisa1.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa1.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            //var IdCalidadVisa1 = pervisa1.FunDatosLaborales.RH_ContCod;
            //var calidadvisa1 = string.IsNullOrEmpty(pervisa1.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa1.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGradoVisa1 = string.IsNullOrEmpty(pervisa1.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa1.FunDatosLaborales.RhConGra.Trim();
            //var gradovisa1 = string.IsNullOrEmpty(pervisa1.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa1.FunDatosLaborales.RhConGra.Trim();
            //var estamentovisa1 = pervisa1.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa1.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgIdVisa1 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var ProgramaVisa1 = ProgIdVisa1 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa1).FirstOrDefault().RePytDes : "S/A";
            //var conglomeradovisa1 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa1.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa1 = pervisa1.Jefatura != null ? pervisa1.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa1 = pervisa1.Funcionario != null ? pervisa1.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechqvisa1 = pervisa1.Funcionario != null ? pervisa1.Funcionario.PeDatPerChq : "Sin nombre definido";

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
                //IdCargoVisa1 = IdCargoVisa1,
                //CargoVisa1 = cargovisa1,
                //IdCalidadVisa1 = IdCalidadVisa1,
                //CalidadJuridicaVisa1 = calidadvisa1,
                //IdGradoVisa1 = IdGradoVisa1,
                //GradoVisa1 = gradovisa1,
                //EstamentoVisa1 = estamentovisa1,
                ////ProgramaDest = Programa.Trim(),
                //ConglomeradoVisa1 = conglomeradovisa1,
                UnidadVisa1 = pervisa1.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa1 = jefaturavisa1,
                EmailVisa1 = ecorreovisa1,
                NombreChqVisa1 = nombrechqvisa1

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa2(int RutVisa2)
        {
            var correovisa2 = _sigper.GetUserByRut(RutVisa2).Funcionario.Rh_Mail.Trim();
            var pervisa2 = _sigper.GetUserByEmail(correovisa2);
            //var IdCargoVisa2 = pervisa2.FunDatosLaborales.RhConCar.Value;
            //var cargovisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa2.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            //var IdCalidadVisa2 = pervisa2.FunDatosLaborales.RH_ContCod;
            //var calidadvisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa2.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGradoVisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var gradovisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var estamentovisa2 = pervisa2.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa2.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgIdVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var ProgramaVisa2 = ProgIdVisa2 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa2).FirstOrDefault().RePytDes : "S/A";
            //var conglomeradovisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa2 = pervisa2.Jefatura != null ? pervisa2.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa2 = pervisa2.Funcionario != null ? pervisa2.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechqvisa2 = pervisa2.Funcionario != null ? pervisa2.Funcionario.PeDatPerChq : "Sin nombre definido";

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
                //IdCargoVisa2 = IdCargoVisa2,
                //CargoVisa2 = cargovisa2,
                //IdCalidadVisa2 = IdCalidadVisa2,
                //CalidadJuridicaVisa2 = calidadvisa2,
                //IdGradoVisa2 = IdGradoVisa2,
                //GradoVisa2 = gradovisa2,
                //EstamentoVisa2 = estamentovisa2,
                ////ProgramaDest = Programa.Trim(),
                //ConglomeradoVisa2 = conglomeradovisa2,
                UnidadVisa2 = pervisa2.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa2 = jefaturavisa2,
                EmailVisa2 = ecorreovisa2,
                NombreChqVisa2 = nombrechqvisa2

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa3(int RutVisa3)
        {
            var correovisa3 = _sigper.GetUserByRut(RutVisa3).Funcionario.Rh_Mail.Trim();
            var pervisa3 = _sigper.GetUserByEmail(correovisa3);
            //var IdCargoVisa3 = pervisa3.FunDatosLaborales.RhConCar.Value;
            //var cargovisa3 = string.IsNullOrEmpty(pervisa3.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa3.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            //var IdCalidadVisa2 = pervisa2.FunDatosLaborales.RH_ContCod;
            //var calidadvisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa2.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGradoVisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var gradovisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var estamentovisa2 = pervisa2.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa2.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgIdVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var ProgramaVisa2 = ProgIdVisa2 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa2).FirstOrDefault().RePytDes : "S/A";
            //var conglomeradovisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa3 = pervisa3.Jefatura != null ? pervisa3.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa3 = pervisa3.Funcionario != null ? pervisa3.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechqvisa3 = pervisa3.Funcionario != null ? pervisa3.Funcionario.PeDatPerChq : "Sin nombre definido";

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
                //IdCargoVisa3 = IdCargoVisa3,
                //CargoVisa3 = cargovisa3,
                //IdCalidadVisa2 = IdCalidadVisa2,
                //CalidadJuridicaVisa2 = calidadvisa2,
                //IdGradoVisa2 = IdGradoVisa2,
                //GradoVisa2 = gradovisa2,
                //EstamentoVisa2 = estamentovisa2,
                ////ProgramaDest = Programa.Trim(),
                //ConglomeradoVisa2 = conglomeradovisa2,
                UnidadVisa3 = pervisa3.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa3 = jefaturavisa3,
                EmailVisa3 = ecorreovisa3,
                NombreChqVisa3 = nombrechqvisa3

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa4(int RutVisa4)
        {
            var correovisa4 = _sigper.GetUserByRut(RutVisa4).Funcionario.Rh_Mail.Trim();
            var pervisa4 = _sigper.GetUserByEmail(correovisa4);
            //var IdCargoVisa3 = pervisa3.FunDatosLaborales.RhConCar.Value;
            //var cargovisa3 = string.IsNullOrEmpty(pervisa3.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa3.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            //var IdCalidadVisa2 = pervisa2.FunDatosLaborales.RH_ContCod;
            //var calidadvisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa2.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGradoVisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var gradovisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var estamentovisa2 = pervisa2.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa2.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgIdVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var ProgramaVisa2 = ProgIdVisa2 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa2).FirstOrDefault().RePytDes : "S/A";
            //var conglomeradovisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa4 = pervisa4.Jefatura != null ? pervisa4.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa4 = pervisa4.Funcionario != null ? pervisa4.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechqvisa4 = pervisa4.Funcionario != null ? pervisa4.Funcionario.PeDatPerChq : "Sin nombre definido";

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
                //IdCargoVisa3 = IdCargoVisa3,
                //CargoVisa3 = cargovisa3,
                //IdCalidadVisa2 = IdCalidadVisa2,
                //CalidadJuridicaVisa2 = calidadvisa2,
                //IdGradoVisa2 = IdGradoVisa2,
                //GradoVisa2 = gradovisa2,
                //EstamentoVisa2 = estamentovisa2,
                ////ProgramaDest = Programa.Trim(),
                //ConglomeradoVisa2 = conglomeradovisa2,
                UnidadVisa4 = pervisa4.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa4 = jefaturavisa4,
                EmailVisa4 = ecorreovisa4,
                NombreChqVisa4 = nombrechqvisa4

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa5(int RutVisa5)
        {
            var correovisa5 = _sigper.GetUserByRut(RutVisa5).Funcionario.Rh_Mail.Trim();
            var pervisa5 = _sigper.GetUserByEmail(correovisa5);
            //var IdCargoVisa3 = pervisa3.FunDatosLaborales.RhConCar.Value;
            //var cargovisa3 = string.IsNullOrEmpty(pervisa3.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa3.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            //var IdCalidadVisa2 = pervisa2.FunDatosLaborales.RH_ContCod;
            //var calidadvisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa2.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGradoVisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var gradovisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var estamentovisa2 = pervisa2.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa2.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgIdVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var ProgramaVisa2 = ProgIdVisa2 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa2).FirstOrDefault().RePytDes : "S/A";
            //var conglomeradovisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa5 = pervisa5.Jefatura != null ? pervisa5.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa5 = pervisa5.Funcionario != null ? pervisa5.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechqvisa5 = pervisa5.Funcionario != null ? pervisa5.Funcionario.PeDatPerChq : "Sin nombre definido";

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
                //IdCargoVisa3 = IdCargoVisa3,
                //CargoVisa3 = cargovisa3,
                //IdCalidadVisa2 = IdCalidadVisa2,
                //CalidadJuridicaVisa2 = calidadvisa2,
                //IdGradoVisa2 = IdGradoVisa2,
                //GradoVisa2 = gradovisa2,
                //EstamentoVisa2 = estamentovisa2,
                ////ProgramaDest = Programa.Trim(),
                //ConglomeradoVisa2 = conglomeradovisa2,
                UnidadVisa5 = pervisa5.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa5 = jefaturavisa5,
                EmailVisa5 = ecorreovisa5,
                NombreChqVisa5 = nombrechqvisa5

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa6(int RutVisa6)
        {
            var correovisa6 = _sigper.GetUserByRut(RutVisa6).Funcionario.Rh_Mail.Trim();
            var pervisa6 = _sigper.GetUserByEmail(correovisa6);
            //var IdCargoVisa3 = pervisa3.FunDatosLaborales.RhConCar.Value;
            //var cargovisa3 = string.IsNullOrEmpty(pervisa3.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa3.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            //var IdCalidadVisa2 = pervisa2.FunDatosLaborales.RH_ContCod;
            //var calidadvisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa2.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGradoVisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var gradovisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var estamentovisa2 = pervisa2.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa2.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgIdVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var ProgramaVisa2 = ProgIdVisa2 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa2).FirstOrDefault().RePytDes : "S/A";
            //var conglomeradovisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa6 = pervisa6.Jefatura != null ? pervisa6.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa6 = pervisa6.Funcionario != null ? pervisa6.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechqvisa6 = pervisa6.Funcionario != null ? pervisa6.Funcionario.PeDatPerChq : "Sin nombre definido";

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
                //IdCargoVisa3 = IdCargoVisa3,
                //CargoVisa3 = cargovisa3,
                //IdCalidadVisa2 = IdCalidadVisa2,
                //CalidadJuridicaVisa2 = calidadvisa2,
                //IdGradoVisa2 = IdGradoVisa2,
                //GradoVisa2 = gradovisa2,
                //EstamentoVisa2 = estamentovisa2,
                ////ProgramaDest = Programa.Trim(),
                //ConglomeradoVisa2 = conglomeradovisa2,
                UnidadVisa6 = pervisa6.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa6 = jefaturavisa6,
                EmailVisa6 = ecorreovisa6,
                NombreChqVisa6 = nombrechqvisa6

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa7(int RutVisa7)
        {
            var correovisa7 = _sigper.GetUserByRut(RutVisa7).Funcionario.Rh_Mail.Trim();
            var pervisa7 = _sigper.GetUserByEmail(correovisa7);
            //var IdCargoVisa3 = pervisa3.FunDatosLaborales.RhConCar.Value;
            //var cargovisa3 = string.IsNullOrEmpty(pervisa3.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa3.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            //var IdCalidadVisa2 = pervisa2.FunDatosLaborales.RH_ContCod;
            //var calidadvisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa2.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGradoVisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var gradovisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var estamentovisa2 = pervisa2.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa2.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgIdVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var ProgramaVisa2 = ProgIdVisa2 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa2).FirstOrDefault().RePytDes : "S/A";
            //var conglomeradovisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa7 = pervisa7.Jefatura != null ? pervisa7.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa7 = pervisa7.Funcionario != null ? pervisa7.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechqvisa7 = pervisa7.Funcionario != null ? pervisa7.Funcionario.PeDatPerChq : "Sin nombre definido";

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
                //IdCargoVisa3 = IdCargoVisa3,
                //CargoVisa3 = cargovisa3,
                //IdCalidadVisa2 = IdCalidadVisa2,
                //CalidadJuridicaVisa2 = calidadvisa2,
                //IdGradoVisa2 = IdGradoVisa2,
                //GradoVisa2 = gradovisa2,
                //EstamentoVisa2 = estamentovisa2,
                ////ProgramaDest = Programa.Trim(),
                //ConglomeradoVisa2 = conglomeradovisa2,
                UnidadVisa7 = pervisa7.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa7 = jefaturavisa7,
                EmailVisa7 = ecorreovisa7,
                NombreChqVisa7 = nombrechqvisa7

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa8(int RutVisa8)
        {
            var correovisa8 = _sigper.GetUserByRut(RutVisa8).Funcionario.Rh_Mail.Trim();
            var pervisa8 = _sigper.GetUserByEmail(correovisa8);
            //var IdCargoVisa3 = pervisa3.FunDatosLaborales.RhConCar.Value;
            //var cargovisa3 = string.IsNullOrEmpty(pervisa3.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa3.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            //var IdCalidadVisa2 = pervisa2.FunDatosLaborales.RH_ContCod;
            //var calidadvisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa2.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGradoVisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var gradovisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var estamentovisa2 = pervisa2.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa2.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgIdVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var ProgramaVisa2 = ProgIdVisa2 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa2).FirstOrDefault().RePytDes : "S/A";
            //var conglomeradovisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa8 = pervisa8.Jefatura != null ? pervisa8.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa8 = pervisa8.Funcionario != null ? pervisa8.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechqvisa8 = pervisa8.Funcionario != null ? pervisa8.Funcionario.PeDatPerChq : "Sin nombre definido";

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
                //IdCargoVisa3 = IdCargoVisa3,
                //CargoVisa3 = cargovisa3,
                //IdCalidadVisa2 = IdCalidadVisa2,
                //CalidadJuridicaVisa2 = calidadvisa2,
                //IdGradoVisa2 = IdGradoVisa2,
                //GradoVisa2 = gradovisa2,
                //EstamentoVisa2 = estamentovisa2,
                ////ProgramaDest = Programa.Trim(),
                //ConglomeradoVisa2 = conglomeradovisa2,
                UnidadVisa8 = pervisa8.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa8 = jefaturavisa8,
                EmailVisa8 = ecorreovisa8,
                NombreChqVisa8 = nombrechqvisa8

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa9(int RutVisa9)
        {
            var correovisa9 = _sigper.GetUserByRut(RutVisa9).Funcionario.Rh_Mail.Trim();
            var pervisa9 = _sigper.GetUserByEmail(correovisa9);
            //var IdCargoVisa3 = pervisa3.FunDatosLaborales.RhConCar.Value;
            //var cargovisa3 = string.IsNullOrEmpty(pervisa3.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa3.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            //var IdCalidadVisa2 = pervisa2.FunDatosLaborales.RH_ContCod;
            //var calidadvisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa2.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGradoVisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var gradovisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var estamentovisa2 = pervisa2.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa2.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgIdVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var ProgramaVisa2 = ProgIdVisa2 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa2).FirstOrDefault().RePytDes : "S/A";
            //var conglomeradovisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa9 = pervisa9.Jefatura != null ? pervisa9.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa9 = pervisa9.Funcionario != null ? pervisa9.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechqvisa9 = pervisa9.Funcionario != null ? pervisa9.Funcionario.PeDatPerChq : "Sin nombre definido";

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
                //IdCargoVisa3 = IdCargoVisa3,
                //CargoVisa3 = cargovisa3,
                //IdCalidadVisa2 = IdCalidadVisa2,
                //CalidadJuridicaVisa2 = calidadvisa2,
                //IdGradoVisa2 = IdGradoVisa2,
                //GradoVisa2 = gradovisa2,
                //EstamentoVisa2 = estamentovisa2,
                ////ProgramaDest = Programa.Trim(),
                //ConglomeradoVisa2 = conglomeradovisa2,
                UnidadVisa9 = pervisa9.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa9 = jefaturavisa9,
                EmailVisa9 = ecorreovisa9,
                NombreChqVisa9 = nombrechqvisa9

            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioVisa10(int RutVisa10)
        {
            var correovisa10 = _sigper.GetUserByRut(RutVisa10).Funcionario.Rh_Mail.Trim();
            var pervisa10 = _sigper.GetUserByEmail(correovisa10);
            //var IdCargoVisa3 = pervisa3.FunDatosLaborales.RhConCar.Value;
            //var cargovisa3 = string.IsNullOrEmpty(pervisa3.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == pervisa3.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            //var IdCalidadVisa2 = pervisa2.FunDatosLaborales.RH_ContCod;
            //var calidadvisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == pervisa2.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            //var IdGradoVisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "0" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var gradovisa2 = string.IsNullOrEmpty(pervisa2.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : pervisa2.FunDatosLaborales.RhConGra.Trim();
            //var estamentovisa2 = pervisa2.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == pervisa2.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            //var ProgIdVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            //var ProgramaVisa2 = ProgIdVisa2 != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgIdVisa2).FirstOrDefault().RePytDes : "S/A";
            //var conglomeradovisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == pervisa2.Funcionario.RH_NumInte).ReContraSed;
            var jefaturavisa10 = pervisa10.Jefatura != null ? pervisa10.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreovisa10 = pervisa10.Funcionario != null ? pervisa10.Funcionario.Rh_Mail : "Sin correo definido";
            var nombrechqvisa10 = pervisa10.Funcionario != null ? pervisa10.Funcionario.PeDatPerChq : "Sin nombre definido";

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
                //IdCargoVisa3 = IdCargoVisa3,
                //CargoVisa3 = cargovisa3,
                //IdCalidadVisa2 = IdCalidadVisa2,
                //CalidadJuridicaVisa2 = calidadvisa2,
                //IdGradoVisa2 = IdGradoVisa2,
                //GradoVisa2 = gradovisa2,
                //EstamentoVisa2 = estamentovisa2,
                ////ProgramaDest = Programa.Trim(),
                //ConglomeradoVisa2 = conglomeradovisa2,
                UnidadVisa10 = pervisa10.Unidad.Pl_UndDes.Trim(),
                JefaturaVisa10 = jefaturavisa10,
                EmailVisa10 = ecorreovisa10,
                NombreChqVisa10 = nombrechqvisa10

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

            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");

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
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _hsm);
                //var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);
                var doc = _repository.Get<Documento>(c => c.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 5).FirstOrDefault();
                var user = User.Email();
                var _UseCaseResponseMessage = _useCaseInteractor.DocumentoSign(doc, user);



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

            //ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdDest = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa3 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa4 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa5 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa6 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa7 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa8 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa9 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa10 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");

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
                model.FechaFirma = DateTime.Now;
                model.IdUnidad = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcion = persona.Unidad.Pl_UndDes.Trim();
                model.Rut = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DV = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreId = null;
                //model.NombreId = persona.Funcionario.RH_NumInte;
                model.Nombre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargo = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidad = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGrado = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcion = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamento = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcion = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcion = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdPrograma = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcion = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailRem = persona.Funcionario.Rh_Mail;
                model.NombreChqRem = persona.Funcionario.PeDatPerChq;

                //model.FechaSolicitud = DateTime.Now;
                model.IdUnidadDest = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionDest = persona.Unidad.Pl_UndDes.Trim();
                model.RutDest = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVDest = persona.Funcionario.RH_DvNuInt.Trim();
                //model.NombreIdDest = persona.Funcionario.RH_NumInte;
                model.NombreIdDest = null;
                model.NombreDest = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargoDest = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcionDest = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidadDest = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcionDest = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGradoDest = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcionDest = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamentoDest = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcionDest = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomeradoDest = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcionDest = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdProgramaDest = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcionDest = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailDest = persona.Funcionario.Rh_Mail;
                model.NombreChqDest = persona.Funcionario.PeDatPerChq;
                model.NombreSecreDest = persona.Unidad.Pl_UndNomSec.Trim();


                //model.FechaSolicitud = DateTime.Now;
                model.IdUnidadSecre = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionSecre = persona.Unidad.Pl_UndDes.Trim();
                model.RutSecre = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVSecre = persona.Funcionario.RH_DvNuInt.Trim();
                //model.NombreIdSecre = persona.Funcionario.RH_NumInte;
                model.NombreIdSecre = null;
                model.NombreSecre = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoSecre = persona.FunDatosLaborales.RhConCar.Value;
                //model.CargoDescripcionSecre = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidadSecre = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcionSecre = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGradoSecre = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcionSecre = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamentoSecre = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcionSecre = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomeradoSecre = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcionSecre = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdProgramaSecre = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcionSecre = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailSecre = persona.Funcionario.Rh_Mail;
                model.NombreChqSecre = persona.Funcionario.PeDatPerChq;

                //model.FechaSolicitud = DateTime.Now;
                model.IdUnidadVisa1 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa1 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa1 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa1 = persona.Funcionario.RH_DvNuInt.Trim();
                //model.NombreIdVisa1 = persona.Funcionario.RH_NumInte;
                model.NombreIdVisa1 = null;
                model.NombreVisa1 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa1 = persona.FunDatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa1 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidadVisa1 = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcionVisa1 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGradoVisa1 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcionVisa1 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamentoVisa1 = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcionVisa1 = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomeradoVisa1 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcionVisa1 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdProgramaVisa1 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcionVisa1 = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailVisa1 = persona.Funcionario.Rh_Mail;
                model.NombreChqVisa1 = persona.Funcionario.PeDatPerChq;

                //model.FechaSolicitud = DateTime.Now;
                model.IdUnidadVisa2 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa2 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa2 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa2 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa2 = null;
                //model.NombreIdVisa2 = persona.Funcionario.RH_NumInte;
                model.NombreVisa2 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa2 = persona.FunDatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidadVisa2 = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGradoVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcionVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamentoVisa2 = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcionVisa2 = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomeradoVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcionVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdProgramaVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcionVisa2 = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailVisa2 = persona.Funcionario.Rh_Mail;
                model.NombreChqVisa2 = persona.Funcionario.PeDatPerChq;

                //model.FechaSolicitud = DateTime.Now;
                model.IdUnidadVisa3 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa3 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa3 = null;
                //model.NombreIdVisa2 = persona.Funcionario.RH_NumInte;
                model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa2 = persona.FunDatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidadVisa2 = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGradoVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcionVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamentoVisa2 = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcionVisa2 = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomeradoVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcionVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdProgramaVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcionVisa2 = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailVisa3 = persona.Funcionario.Rh_Mail;
                model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq;

                //model.FechaSolicitud = DateTime.Now;
                model.IdUnidadVisa4 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa4 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa3 = null;
                //model.NombreIdVisa2 = persona.Funcionario.RH_NumInte;
                model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa2 = persona.FunDatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidadVisa2 = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGradoVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcionVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamentoVisa2 = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcionVisa2 = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomeradoVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcionVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdProgramaVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcionVisa2 = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailVisa3 = persona.Funcionario.Rh_Mail;
                model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq;

                //model.FechaSolicitud = DateTime.Now;
                model.IdUnidadVisa3 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa3 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa3 = null;
                //model.NombreIdVisa2 = persona.Funcionario.RH_NumInte;
                model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa2 = persona.FunDatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidadVisa2 = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGradoVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcionVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamentoVisa2 = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcionVisa2 = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomeradoVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcionVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdProgramaVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcionVisa2 = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailVisa3 = persona.Funcionario.Rh_Mail;
                model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq;

                //model.FechaSolicitud = DateTime.Now;
                model.IdUnidadVisa3 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa3 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa3 = null;
                //model.NombreIdVisa2 = persona.Funcionario.RH_NumInte;
                model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa2 = persona.FunDatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidadVisa2 = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGradoVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcionVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamentoVisa2 = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcionVisa2 = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomeradoVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcionVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdProgramaVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcionVisa2 = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailVisa3 = persona.Funcionario.Rh_Mail;
                model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq;

                //model.FechaSolicitud = DateTime.Now;
                model.IdUnidadVisa3 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa3 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa3 = null;
                //model.NombreIdVisa2 = persona.Funcionario.RH_NumInte;
                model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa2 = persona.FunDatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidadVisa2 = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGradoVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcionVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamentoVisa2 = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcionVisa2 = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomeradoVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcionVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdProgramaVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcionVisa2 = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailVisa3 = persona.Funcionario.Rh_Mail;
                model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq;

                //model.FechaSolicitud = DateTime.Now;
                model.IdUnidadVisa3 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa3 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa3 = null;
                //model.NombreIdVisa2 = persona.Funcionario.RH_NumInte;
                model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa2 = persona.FunDatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidadVisa2 = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGradoVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcionVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamentoVisa2 = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcionVisa2 = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomeradoVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcionVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdProgramaVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcionVisa2 = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailVisa3 = persona.Funcionario.Rh_Mail;
                model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq;

                //model.FechaSolicitud = DateTime.Now;
                model.IdUnidadVisa3 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa3 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa3 = null;
                //model.NombreIdVisa2 = persona.Funcionario.RH_NumInte;
                model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa2 = persona.FunDatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidadVisa2 = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGradoVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcionVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamentoVisa2 = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcionVisa2 = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomeradoVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcionVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdProgramaVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcionVisa2 = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailVisa3 = persona.Funcionario.Rh_Mail;
                model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq;

                //model.FechaSolicitud = DateTime.Now;
                model.IdUnidadVisa3 = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcionVisa3 = persona.Unidad.Pl_UndDes.Trim();
                model.RutVisa3 = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DVVisa3 = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreIdVisa3 = null;
                //model.NombreIdVisa2 = persona.Funcionario.RH_NumInte;
                model.NombreVisa3 = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargoVisa2 = persona.FunDatosLaborales.RhConCar.Value;
                //model.CargoDescripcionVisa2 = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                //model.IdCalidadVisa2 = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcionVisa2 = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGradoVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                //model.GradoDescripcionVisa2 = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                //model.IdEstamentoVisa2 = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcionVisa2 = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                //model.IdConglomeradoVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                //model.ConglomeradoDescripcionVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                //model.IdProgramaVisa2 = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcionVisa2 = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.EmailVisa3 = persona.Funcionario.Rh_Mail;
                model.NombreChqVisa3 = persona.Funcionario.PeDatPerChq;

                model.VisaChk1 = false;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Memorandum model)
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
            ViewBag.NombreIdVisa6 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa7 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa8 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa9 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa10 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");

            model.FechaSolicitud = DateTime.Now;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseMemorandum(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.MemorandumInsert(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("GeneraDocumento2", "Memorandum", new { model.WorkflowId, id = model.MemorandumId });
                }
                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<Memorandum>(id);

            //model.Destinos = ListDestino;
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa3 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");


            //ViewBag.DestinosPasajes = _repository.Get<DestinosPasajes>(q => q.Pasaje.ProcesoId == model.ProcesoId);


            //ViewBag.Pasaje = new DestinosPasajes(); //_des; //_repository.Get<DestinosPasajes>(c => c.PasajeId == 2038).FirstOrDefault(); //_repository.Get<Dest>(c => c.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault().CometidoId;
            //ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            //ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            //ViewBag.FechaOrigen = DateTime.Now;
            //ViewBag.FechaVuelta = DateTime.Now;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Memorandum model/*, DestinosPasajes DesPasajes*/)
        {
            //var com = _repository.GetById<Cometido>(model.CometidoId);
            //model.Alojamiento = com.Alojamiento;
            //model.Alimentacion = com.Alimentacion;
            //model.Pasajes = com.Pasajes;

            if (ModelState.IsValid)
            {
                //var _useCaseInteractor = new UseCaseInteractorCustom(_repository, _sigper);
                var _useCaseInteractor = new UseCaseMemorandum(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.MemorandumUpdate(model);
                var resp = new ResponseMessage();
                var res = new ResponseMessage();


                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    /*Se ingresa informacion de pasajes*/
                    /*Se crean los pasajes, si se solicitan*/
                    //if (model.ReqPasajeAereo == true)
                    //{
                    //    if (DesPasajes != null && DesPasajes.IdRegion != null)
                    //    {
                    //        /*se crea encabezado de pasaje y destinos de pasajes*/
                    //        Pasaje _pasaje = new Pasaje();
                    //        _pasaje.FechaSolicitud = DateTime.Now;
                    //        _pasaje.Nombre = model.Nombre;
                    //        _pasaje.NombreId = model.NombreId;
                    //        _pasaje.Rut = model.Rut;
                    //        _pasaje.DV = model.DV;
                    //        _pasaje.IdCalidad = model.IdCalidad;
                    //        _pasaje.CalidadDescripcion = model.CalidadDescripcion;
                    //        _pasaje.PasajeDescripcion = model.CometidoDescripcion;
                    //        _pasaje.TipoDestino = true;
                    //        _pasaje.ProcesoId = model.ProcesoId;
                    //        _pasaje.WorkflowId = model.WorkflowId;
                    //        resp = _useCaseInteractor.PasajeInsert(_pasaje);
                    //        if (resp.Errors.Count > 0)
                    //            _UseCaseResponseMessage.Errors.Add(resp.Errors.FirstOrDefault());

                    //        /*genera resgistro en tabla destino pasaje, segun los destinos señalados en el cometido*/
                    //        //foreach (var com in DesPasajes)
                    //        //{
                    //        DestinosPasajes _destino = new DestinosPasajes();
                    //        _destino.PasajeId = resp.EntityId;
                    //        _destino.IdRegion = DesPasajes.IdRegion;
                    //        _destino.RegionDescripcion = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == DesPasajes.IdRegion).Pl_DesReg.Trim(); //DesPasajes.RegionDescripcion;
                    //        _destino.IdRegionOrigen = DesPasajes.IdRegionOrigen;
                    //        _destino.OrigenRegionDescripcion = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == DesPasajes.IdRegionOrigen).Pl_DesReg.Trim();
                    //        _destino.FechaIda = DesPasajes.FechaOrigen;
                    //        _destino.FechaVuelta = DesPasajes.FechaVuelta;
                    //        _destino.FechaOrigen = DateTime.Now;
                    //        _destino.ObservacionesOrigen = DesPasajes.ObservacionesOrigen;
                    //        _destino.ObservacionesDestinos = DesPasajes.ObservacionesDestinos;
                    //        res = _useCaseInteractor.DestinosPasajesInsert(_destino);
                    //        if (res.Errors.Count > 0)
                    //            _UseCaseResponseMessage.Errors.Add(res.Errors.FirstOrDefault());
                    //        //}
                    //    }
                    //    //else
                    //    //    TempData["Errors"] = "Debe agregar datos del pasaje";
                    //}


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

            //ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso", model.TipoReembolsoId);
            //ViewBag.Pasaje = new DestinosPasajes(); //_repository.Get<DestinosPasajes>(c => c.PasajeId == 2038).FirstOrDefault(); //_repository.Get<Dest>(c => c.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault().CometidoId;
            ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreIdVisa3 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");

            return View(model);
        }

        public ActionResult EditGP(int id)
        {
            var model = _repository.GetById<Memorandum>(id);

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            //ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.FinanciaOrganismo = model.FinanciaOrganismo;


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGP(Memorandum model)
        {

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

            //ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso", model.TipoReembolsoId);

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

            return RedirectToAction("Sign", "Memorandum", new { model.WorkflowId, id = model.MemorandumId });


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






        //public FileResult ShowDocumentoSinFirma(int id)
        //{
        //    var model = _repository.GetById<Memorandum>(id);
        //    return File(model.DocumentoSinFirma, "application/pdf");
        //}

        public ActionResult DetailsGM(int id)
        {
            //var model = _repository.GetById<Memorandum>(id);

            //ViewBag.NombreId = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdDest = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdSecre = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa1 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
            //ViewBag.NombreIdVisa2 = new SelectList(_sigper.GetAllUsers(), "RH_NumInte", "PeDatPerChq");
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

            //return Redirect(Request.UrlReferrer.PathAndQuery);
            return RedirectToAction("Sign", "Memorandum", new { model.WorkflowId, id = model.MemorandumId });
        }
    }
}