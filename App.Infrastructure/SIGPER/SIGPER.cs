using System;
using System.Collections.Generic;
using System.Linq;
using App.Model.SIGPER;
using App.Core.Interfaces;

namespace App.Infrastructure.SIGPER
{
    public class SIGPER : ISIGPER
    {
        private string criterioExclusionUnidad = "COMISIONADO";

        public List<PECARGOS> GetCargos()
        {
            var returnValue = new List<PECARGOS>();
            try
            {
                using (var context = new AppContextEconomia())
                {
                    returnValue.AddRange(context.PECARGOS.AsNoTracking().ToList().Select(q => new PECARGOS { Pl_CodCar = q.Pl_CodCar, Pl_DesCar = q.Pl_DesCar.Trim() + " (ECONOMIA)" }));
                }

                using (var context = new AppContextTurismo())
                {
                    returnValue.AddRange(context.PECARGOS.AsNoTracking().ToList().Select(q => new PECARGOS { Pl_CodCar = q.Pl_CodCar, Pl_DesCar = q.Pl_DesCar.Trim() + " (TURISMO)" }));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return returnValue.OrderBy(q => q.Pl_DesCar).ToList();

        }
        public PECARGOS GetCargo(int codigo)
        {
            try
            {
                using (AppContextEconomia context = new AppContextEconomia())
                {
                    if (context.PECARGOS.Any(q => q.Pl_CodCar == codigo))
                        return context.PECARGOS.AsNoTracking().FirstOrDefault(q => q.Pl_CodCar == codigo);
                }
                using (AppContextTurismo context = new AppContextTurismo())
                {
                    if (context.PECARGOS.Any(q => q.Pl_CodCar == codigo))
                        return context.PECARGOS.AsNoTracking().FirstOrDefault(q => q.Pl_CodCar == codigo);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }
        public PLUNILAB GetUnidad(int codigo)
        {
            try
            {
                using (AppContextEconomia context = new AppContextEconomia())
                {
                    if (context.PLUNILAB.AsNoTracking().Any(q => q.Pl_UndCod == codigo))
                        return context.PLUNILAB.AsNoTracking().FirstOrDefault(q => q.Pl_UndCod == codigo);
                }
                using (AppContextTurismo context = new AppContextTurismo())
                {
                    if (context.PLUNILAB.AsNoTracking().Any(q => q.Pl_UndCod == codigo))
                        return context.PLUNILAB.AsNoTracking().FirstOrDefault(q => q.Pl_UndCod == codigo);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }
        public Model.SIGPER.SIGPER GetUserByEmail(string email)
        {
            var sigper = new Model.SIGPER.SIGPER()
            {
                Funcionario = null,
                Jefatura = null,
                Secretaria = null,
                Unidad = null,
                FunDatosLaborales = null,
                SubSecretaria = null
            };

            try
            {
                using (var context = new AppContextEconomia())
                {
                    var funcionario = context.PEDATPER.AsNoTracking().FirstOrDefault(q => q.Rh_Mail == email && q.RH_EstLab == "A");
                    if (funcionario != null)
                    {
                        sigper.Funcionario = funcionario;

                        //jefatura del funcionario
                        var jefatura = (from f in context.PEFERJEFAF
                                        join j in context.PEFERJEFAJ on f.PeFerJerCod equals j.PeFerJerCod
                                        join p in context.PEDATPER on j.FyPFunARut equals p.RH_NumInte
                                        where f.FyPFunRut == funcionario.RH_NumInte
                                        where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                        where j.PeFerJerAutEst == 1
                                        select p).FirstOrDefault();

                        if (jefatura != null)
                            sigper.Jefatura = jefatura;

                        //datos laborales del funcionario
                        var PeDatLab = context.PeDatLab.AsNoTracking().Where(q => q.RH_NumInte == funcionario.RH_NumInte && q.RH_ContCod != 13 && (q.RhConIni.Value.Year == 2020 || q.RH_ContCod == 1)).OrderByDescending(q => q.RH_Correla).FirstOrDefault();
                        if (PeDatLab != null)
                        {

                            var CodUnidad = (from u in context.ReContra
                                             join p in context.PEDATPER on u.RH_NumInte equals p.RH_NumInte
                                             where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                             where p.RH_NumInte == sigper.Funcionario.RH_NumInte
                                             where u.Re_ConIni == (from ud in context.ReContra
                                                                   where ud.RH_NumInte == sigper.Funcionario.RH_NumInte
                                                                   select ud.Re_ConIni).Max()
                                             select u.Re_ConUni).FirstOrDefault();

                            /*unidad del funcionario*/
                            var unidad = context.PLUNILAB.AsNoTracking().FirstOrDefault(q => q.Pl_UndCod == CodUnidad);
                            if (unidad != null)
                            {
                                sigper.Unidad = unidad;

                                //secretaria del funcionario
                                var secretaria = context.PEDATPER.AsNoTracking().FirstOrDefault(q => q.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase) && q.PeDatPerChq == unidad.Pl_UndNomSec);
                                if (secretaria != null)
                                    sigper.Secretaria = secretaria;
                            }

                            /*datos laborales funcionario*/
                            var FunDatosLaborales = context.PeDatLab.AsNoTracking().Where(q => q.RH_NumInte == funcionario.RH_NumInte && q.RH_ContCod != 13 && (q.RhConIni.Value.Year == 2020 || q.RH_ContCod == 1)).OrderByDescending(q => q.RH_Correla).FirstOrDefault();

                            if (FunDatosLaborales != null)
                            {
                                sigper.FunDatosLaborales = FunDatosLaborales;
                            }
                        }
                        else
                        {
                            PeDatLab = context.PeDatLab.AsNoTracking().Where(q => q.RH_NumInte == funcionario.RH_NumInte && q.RH_ContCod != 13 && (q.RhConIni.Value.Year == 2020 || q.RH_ContCod == 1)).OrderByDescending(q => q.RH_Correla).FirstOrDefault();

                            var CodUnidad = (from u in context.ReContra
                                             join p in context.PEDATPER on u.RH_NumInte equals p.RH_NumInte
                                             where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                             where p.RH_NumInte == sigper.Funcionario.RH_NumInte
                                             where u.Re_ConIni == (from ud in context.ReContra
                                                                   where ud.RH_NumInte == sigper.Funcionario.RH_NumInte
                                                                   select ud.Re_ConIni).Max()
                                             select u.Re_ConUni).FirstOrDefault();


                            //unidad del funcionario
                            var unidad = context.PLUNILAB.AsNoTracking().FirstOrDefault(q => q.Pl_UndCod == CodUnidad);
                            if (unidad != null)
                            {
                                sigper.Unidad = unidad;

                                //secretaria del funcionario
                                var secretaria = context.PEDATPER.AsNoTracking().FirstOrDefault(q => q.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase) && q.PeDatPerChq == unidad.Pl_UndNomSec);
                                if (secretaria != null)
                                    sigper.Secretaria = secretaria;
                            }

                            /*datos laborales funcionario*/
                            var FunDatosLaborales = context.PeDatLab.AsNoTracking().Where(q => q.RH_NumInte == funcionario.RH_NumInte && q.RH_ContCod != 13 && (q.RhConIni.Value.Year == 2020 || q.RH_ContCod == 1)).OrderByDescending(q => q.RH_Correla).FirstOrDefault();
                            if (FunDatosLaborales != null)
                            {
                                sigper.FunDatosLaborales = FunDatosLaborales;
                            }
                        }

                        sigper.SubSecretaria = "ECONOMIA";

                        if (sigper.Unidad != null && !string.IsNullOrWhiteSpace(sigper.Unidad.Pl_UndDes) && !sigper.Unidad.Pl_UndDes.ToUpper().Contains(this.criterioExclusionUnidad))
                            return sigper;
                    }
                }

                using (var context = new AppContextTurismo())
                {
                    var funcionario = context.PEDATPER.AsNoTracking().FirstOrDefault(q => q.Rh_Mail == email);
                    if (funcionario != null)
                    {
                        sigper.Funcionario = funcionario;

                        //jefatura del funcionario
                        var jefatura = (from f in context.PEFERJEFAF
                                        join j in context.PEFERJEFAJ on f.PeFerJerCod equals j.PeFerJerCod
                                        join p in context.PEDATPER on j.FyPFunARut equals p.RH_NumInte
                                        where f.FyPFunRut == funcionario.RH_NumInte
                                        where p.RH_EstLab.Equals("A")
                                        where j.PeFerJerAutEst == 1
                                        where p.Rh_MailPer == null
                                        select p).FirstOrDefault();

                        if (jefatura != null)
                            sigper.Jefatura = jefatura;

                        //datos laborales del funcionario
                        //var PeDatLab = dbTurismo.PeDatLab.Where(q => q.RH_NumInte == funcionario.RH_NumInte).OrderByDescending(q => q.RH_Correla).FirstOrDefault();
                        var PeDatLab = context.PeDatLab.AsNoTracking().Where(q => q.RH_NumInte == funcionario.RH_NumInte && q.RH_ContCod != 13 && (q.RhConIni.Value.Year == 2020 || q.RH_ContCod == 1)).OrderByDescending(q => q.RH_Correla).FirstOrDefault();
                        if (PeDatLab != null)
                        {
                            var CodUnidad = (from u in context.ReContra
                                             join p in context.PEDATPER on u.RH_NumInte equals p.RH_NumInte
                                             where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                             where p.RH_NumInte == sigper.Funcionario.RH_NumInte
                                             where u.Re_ConIni == (from ud in context.ReContra
                                                                   where ud.RH_NumInte == sigper.Funcionario.RH_NumInte
                                                                   select ud.Re_ConIni).Max()
                                             select u.Re_ConUni).FirstOrDefault();

                            //unidad del funcionario
                            var unidad = context.PLUNILAB.AsNoTracking().FirstOrDefault(q => q.Pl_UndCod == CodUnidad);
                            if (unidad != null)
                            {
                                sigper.Unidad = unidad;

                                //secretaria del funcionario
                                var secretaria = context.PEDATPER.AsNoTracking().FirstOrDefault(q => q.RH_EstLab.Equals("A") && q.PeDatPerChq == unidad.Pl_UndNomSec);
                                if (secretaria != null)
                                    sigper.Secretaria = secretaria;
                            }

                            /*datos laborales funcionario*/
                            var FunDatosLaborales = context.PeDatLab.AsNoTracking().Where(q => q.RH_NumInte == funcionario.RH_NumInte && q.RH_ContCod != 13 && (q.RhConIni.Value.Year == 2020 || q.RH_ContCod == 1)).OrderByDescending(q => q.RH_Correla).FirstOrDefault();
                            if (FunDatosLaborales != null)
                            {
                                sigper.FunDatosLaborales = FunDatosLaborales;
                            }
                        }

                        sigper.SubSecretaria = "TURISMO";

                        return sigper;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return sigper;
        }
        public List<PLUNILAB> GetUnidades()
        {
            var returnValue = new List<PLUNILAB>();

            try
            {
                using (var context = new AppContextEconomia())
                {
                    var unidades = context.PLUNILAB.AsNoTracking().Where(q => !q.Pl_UndDes.Contains(criterioExclusionUnidad)).ToList().Select(q => new PLUNILAB { Pl_UndCod = q.Pl_UndCod, Pl_UndDes = q.Pl_UndDes.Trim() + " (ECONOMIA)" });
                    foreach (var item in unidades)
                    {
                        /*excluir las unidades sin funcionarios*/
                        var funcionarios = from PE in context.PEDATPER
                                           join r in context.PeDatLab on PE.RH_NumInte equals r.RH_NumInte
                                           where PE.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                           where r.RhConUniCod == item.Pl_UndCod
                                           where r.PeDatLabAdDocCor == (from ud in context.PeDatLab
                                                                        where ud.RH_NumInte == PE.RH_NumInte
                                                                        select ud.PeDatLabAdDocCor).Max()
                                           select PE;
                        if (funcionarios != null && funcionarios.Any())
                            returnValue.Add(item);
                    }
                }
                using (var context = new AppContextTurismo())
                {
                    //excluir las unidades sin funcionarios
                    //excluir unidad 1 de turismo
                    var unidades = context.PLUNILAB.AsNoTracking().Where(q => !q.Pl_UndDes.Contains(criterioExclusionUnidad) && q.Pl_UndCod != 1).ToList().Select(q => new PLUNILAB { Pl_UndCod = q.Pl_UndCod, Pl_UndDes = q.Pl_UndDes.Trim() + " (TURISMO)" });
                    foreach (var item in unidades)
                    {
                        //excluir las unidades sin funcionarios
                        var funcionarios = from PE in context.PEDATPER
                                           join r in context.PeDatLab on PE.RH_NumInte equals r.RH_NumInte
                                           where PE.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                           where r.RhConUniCod == item.Pl_UndCod
                                           where r.PeDatLabAdDocCor == (from ud in context.PeDatLab
                                                                        where ud.RH_NumInte == PE.RH_NumInte
                                                                        select ud.PeDatLabAdDocCor).Max()
                                           select PE;

                        if (funcionarios != null && funcionarios.Any())
                            returnValue.Add(item);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return returnValue.OrderBy(q => q.Pl_UndDes).ToList();
        }
        public Model.SIGPER.SIGPER GetUserByRut(int rut)
        {
            var sigper = new Model.SIGPER.SIGPER()
            {
                Funcionario = null,
                Jefatura = null,
                Secretaria = null,
                Unidad = null,
                FunDatosLaborales = null,
                SubSecretaria = null,
                datosLaborales = null,
            };

            try
            {
                using (var context = new AppContextEconomia())
                {
                    var funcionario = context.PEDATPER.AsNoTracking().FirstOrDefault(q => q.RH_NumInte == rut);
                    if (funcionario != null)
                    {
                        sigper.Funcionario = funcionario;

                        //jefatura del funcionario
                        var jefatura = (from f in context.PEFERJEFAF
                                        join j in context.PEFERJEFAJ on f.PeFerJerCod equals j.PeFerJerCod
                                        join p in context.PEDATPER on j.FyPFunARut equals p.RH_NumInte
                                        where f.FyPFunRut == funcionario.RH_NumInte
                                        where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                        where j.PeFerJerAutEst == 1
                                        select p).FirstOrDefault();

                        if (jefatura != null)
                            sigper.Jefatura = jefatura;

                        //datos laborales del funcionario
                        var PeDatLab = context.PeDatLab.AsNoTracking().Where(q => q.RH_NumInte == funcionario.RH_NumInte).OrderByDescending(q => q.RH_Correla).FirstOrDefault();
                        if (PeDatLab != null)
                        {
                            sigper.FunDatosLaborales = PeDatLab;
                            var CodUnidad = (from u in context.ReContra
                                             join p in context.PEDATPER on u.RH_NumInte equals p.RH_NumInte
                                             where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                             where p.RH_NumInte == sigper.Funcionario.RH_NumInte
                                             where u.Re_ConIni == (from ud in context.ReContra
                                                                   where ud.RH_NumInte == sigper.Funcionario.RH_NumInte
                                                                   select ud.Re_ConIni).Max()
                                             select u.Re_ConUni).FirstOrDefault();

                            //unidad del funcionario
                            var unidad = context.PLUNILAB.AsNoTracking().FirstOrDefault(q => q.Pl_UndCod == CodUnidad);
                            if (unidad != null)
                            {
                                sigper.Unidad = unidad;

                                //secretaria del funcionario
                                var secretaria = context.PEDATPER.AsNoTracking().FirstOrDefault(q => q.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase) && q.PeDatPerChq == unidad.Pl_UndNomSec);
                                if (secretaria != null)
                                    sigper.Secretaria = secretaria;
                            }
                        }

                        /*se obtienen datos laborales desde tabla ReContra*/
                        var datosLaborales = (from rc in context.ReContra
                                              join p in context.PEDATPER on rc.RH_NumInte equals p.RH_NumInte
                                              join pl in context.PLUNILAB on rc.Re_ConUni equals pl.Pl_UndCod
                                              join e in context.DGESCALAFONES on rc.Re_ConEsc equals e.Pl_CodEsc
                                              join co in context.DGCONTRATOS on rc.RH_ContCod equals co.RH_ContCod
                                              join pc in context.PECARGOS on rc.Re_ConCar equals pc.Pl_CodCar
                                              join es in context.DGESTAMENTOS on rc.ReContraEst equals es.DgEstCod
                                              where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                              where p.RH_NumInte == sigper.Funcionario.RH_NumInte
                                              where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                              where rc.Re_ConIni == (from ud in context.ReContra
                                                                     where ud.RH_NumInte == sigper.Funcionario.RH_NumInte
                                                                     select ud.Re_ConIni).Max()
                                              select rc).FirstOrDefault();

                        if (datosLaborales != null)
                        {
                            sigper.datosLaborales = datosLaborales;
                        }

                        sigper.SubSecretaria = "ECONOMIA";

                        if (sigper.Unidad != null && !string.IsNullOrWhiteSpace(sigper.Unidad.Pl_UndDes) && !sigper.Unidad.Pl_UndDes.ToUpper().Contains(this.criterioExclusionUnidad))
                            return sigper;
                    }
                }

                using (var context = new AppContextTurismo())
                {
                    var funcionario = context.PEDATPER.AsNoTracking().FirstOrDefault(q => q.RH_NumInte == rut);
                    if (funcionario != null)
                    {
                        sigper.Funcionario = funcionario;

                        //jefatura del funcionario
                        var jefatura = (from f in context.PEFERJEFAF
                                        join j in context.PEFERJEFAJ on f.PeFerJerCod equals j.PeFerJerCod
                                        join p in context.PEDATPER on j.FyPFunARut equals p.RH_NumInte
                                        where f.FyPFunRut == funcionario.RH_NumInte
                                        where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                        where j.PeFerJerAutEst == 1
                                        select p).FirstOrDefault();

                        if (jefatura != null)
                            sigper.Jefatura = jefatura;

                        //datos laborales del funcionario
                        var PeDatLab = context.PeDatLab.AsNoTracking().Where(q => q.RH_NumInte == funcionario.RH_NumInte).OrderByDescending(q => q.RH_Correla).FirstOrDefault();
                        if (PeDatLab != null)
                        {
                            sigper.FunDatosLaborales = PeDatLab;

                            var CodUnidad = (from u in context.ReContra
                                             join p in context.PEDATPER on u.RH_NumInte equals p.RH_NumInte
                                             where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                             where p.RH_NumInte == sigper.Funcionario.RH_NumInte
                                             where u.Re_ConIni == (from ud in context.ReContra
                                                                   where ud.RH_NumInte == sigper.Funcionario.RH_NumInte
                                                                   select ud.Re_ConIni).Max()
                                             select u.Re_ConUni).FirstOrDefault();

                            //unidad del funcionario
                            var unidad = context.PLUNILAB.AsNoTracking().FirstOrDefault(q => q.Pl_UndCod == CodUnidad);
                            if (unidad != null)
                            {
                                sigper.Unidad = unidad;

                                //secretaria del funcionario
                                var secretaria = context.PEDATPER.AsNoTracking().FirstOrDefault(q => q.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase) && q.PeDatPerChq == unidad.Pl_UndNomSec);
                                if (secretaria != null)
                                    sigper.Secretaria = secretaria;
                            }
                        }

                        /*se obtienen datos laborales desde tabla ReContra*/
                        var datosLaborales = (from rc in context.ReContra
                                              join p in context.PEDATPER on rc.RH_NumInte equals p.RH_NumInte
                                              join pl in context.PLUNILAB on rc.Re_ConUni equals pl.Pl_UndCod
                                              join e in context.DGESCALAFONES on rc.Re_ConEsc equals e.Pl_CodEsc
                                              join co in context.DGCONTRATOS on rc.RH_ContCod equals co.RH_ContCod
                                              join pc in context.PECARGOS on rc.Re_ConCar equals pc.Pl_CodCar
                                              join es in context.DGESTAMENTOS on rc.ReContraEst equals es.DgEstCod
                                              where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                              where p.RH_NumInte == sigper.Funcionario.RH_NumInte
                                              where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                              where rc.Re_ConIni == (from ud in context.ReContra
                                                                     where ud.RH_NumInte == sigper.Funcionario.RH_NumInte
                                                                     select ud.Re_ConIni).Max()
                                              select rc).FirstOrDefault();

                        if (datosLaborales != null)
                        {
                            sigper.datosLaborales = datosLaborales;
                        }

                        sigper.SubSecretaria = "TURISMO";

                        return sigper;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return sigper;

        }

        public List<PEDATPER> GetUserByUnidad(int codigoUnidad)
        {
            var returnValue = new List<PEDATPER>();

            try
            {
                using (var context = new AppContextEconomia())
                {
                    if (context.PLUNILAB.AsNoTracking().Any(q => q.Pl_UndCod == codigoUnidad))
                    {
                        var data = (from ReContra in context.ReContra
                                    join PEDATPER in context.PEDATPER on ReContra.RH_NumInte equals PEDATPER.RH_NumInte
                                    where PEDATPER.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                    where ReContra.Re_ConUni == codigoUnidad
                                    where ReContra.Re_ConIni == (from ud in context.ReContra where ud.RH_NumInte == PEDATPER.RH_NumInte select ud.Re_ConIni).Max()
                                    select PEDATPER);

                        returnValue.AddRange(data.ToList());
                    }
                }
                using (var context = new AppContextTurismo())
                {
                    if (context.PLUNILAB.AsNoTracking().Any(q => q.Pl_UndCod == codigoUnidad))
                    {
                        var data = (from ReContra in context.ReContra
                                    join PEDATPER in context.PEDATPER on ReContra.RH_NumInte equals PEDATPER.RH_NumInte
                                    where PEDATPER.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                    where ReContra.Re_ConUni == codigoUnidad
                                    where ReContra.Re_ConIni == (from ud in context.ReContra where ud.RH_NumInte == PEDATPER.RH_NumInte select ud.Re_ConIni).Max()
                                    select PEDATPER);

                        returnValue.AddRange(data.ToList());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return returnValue;

        }
        public List<PEDATPER> GetUserByUnidadForFirma(int Rut)
        {
            var returnValue = new List<PEDATPER>();

            try
            {
                if (Rut != 0)
                {
                    using (var dbE = new AppContextEconomia())
                    {
                        var unid = from PE in dbE.PEFERJEFAF
                                   where PE.FyPFunRut == Rut
                                   select PE;

                        var users = from r in dbE.PEDATPER
                                    join PER in dbE.PEFERJEFAF on r.RH_NumInte equals PER.FyPFunRut
                                    where PER.PeFerJerCod == unid.FirstOrDefault().PeFerJerCod
                                    where r.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                    select r;

                        var jefe = from p in dbE.PEDATPER
                                   join j in dbE.PEFERJEFAJ on p.RH_NumInte equals j.FyPFunARut
                                   where j.PeFerJerCod == unid.FirstOrDefault().PeFerJerCod
                                   where j.PeFerJerAutEst == 1
                                   select p;


                        returnValue.AddRange(users.ToList());
                        returnValue.AddRange(jefe.ToList());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return returnValue;
        }
        public List<PEDATPER> GetAllUsers()
        {
            var returnValue = new List<PEDATPER>();

            using (var dbE = new AppContextEconomia())
            {
                //var rE = from PE in dbE.PEDATPER
                var rE = from PE in dbE.PEDATPER
                         join r in dbE.ReContra on PE.RH_NumInte equals r.RH_NumInte
                         //from r in dbE.ReContra
                         where r.Re_ConPyt != 0
                         where r.Re_ConIni.Year >= DateTime.Now.Year || r.RH_ContCod == 1
                         //where r.ReContraSed != 0
                         where r.Re_ConCar != 21
                         //where r.Re_ConTipHon != 1
                         from PL in dbE.PLUNILAB
                         where (PL.Pl_UndCod == PE.RhSegUnd01.Value || PL.Pl_UndCod == PE.RhSegUnd02.Value || PL.Pl_UndCod == PE.RhSegUnd03.Value)
                         //where PL.Pl_UndCod == codigoUnidad
                         where PE.RH_EstLab.Equals("A")
                         select PE;
                returnValue.AddRange(rE.ToList());

            }

            using (var dbT = new AppContextTurismo())
            {
                //var rT = from PE in dbT.PEDATPER
                var rT = from PE in dbT.PEDATPER
                         join r in dbT.ReContra on PE.RH_NumInte equals r.RH_NumInte
                         where r.Re_ConPyt != 0
                         where r.Re_ConIni.Year >= DateTime.Now.Year || r.RH_ContCod == 1
                         //where r.ReContraSed != 0
                         where r.Re_ConCar != 21
                         //where r.Re_ConTipHon != 1
                         from PL in dbT.PLUNILAB
                         where (PL.Pl_UndCod == PE.RhSegUnd01.Value || PL.Pl_UndCod == PE.RhSegUnd02.Value || PL.Pl_UndCod == PE.RhSegUnd03.Value)
                         //where PL.Pl_UndCod == codigoUnidad
                         where PE.RH_EstLab.Equals("A")
                         select PE;
                returnValue.AddRange(rT.ToList());
            }

            return returnValue.OrderBy(q => q.PeDatPerChq).ToList();
        }
        public List<PEDATPER> GetUserByTerm(string term)
        {
            var returnValue = new List<PEDATPER>();

            try
            {
                using (var context = new AppContextEconomia())
                {
                    returnValue.AddRange(context.PEDATPER.AsNoTracking().Where(q => q.RH_EstLab.Equals("A") && (q.PeDatPerChq.ToLower().Contains(term.ToLower())) || (q.Rh_Mail.ToLower().Contains(term.ToLower()))));
                }
                using (var context = new AppContextTurismo())
                {
                    returnValue.AddRange(context.PEDATPER.AsNoTracking().Where(q => q.RH_EstLab.Equals("A") && (q.PeDatPerChq.ToLower().Contains(term.ToLower())) || (q.Rh_Mail.ToLower().Contains(term.ToLower()))));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return returnValue.OrderBy(q => q.PeDatPerChq).ToList();
        }
        public Model.SIGPER.SIGPER GetJefaturaByUnidad(int codigo)
        {
            try
            {
                using (var context = new AppContextEconomia())
                {
                    //traer los usuarios de la unidad
                    var users = from PEDATPER in context.PEDATPER
                                join r in context.PeDatLab on PEDATPER.RH_NumInte equals r.RH_NumInte
                                where PEDATPER.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                where r.RhConUniCod == codigo
                                where r.PeDatLabAdDocCor == (from ud in context.PeDatLab
                                                             where ud.RH_NumInte == PEDATPER.RH_NumInte
                                                             select ud.PeDatLabAdDocCor).Max()
                                select PEDATPER;

                    //iterar cada usuariode la unidad
                    foreach (var item in users)
                    {
                        // tarer los datos del usuario
                        var data = GetUserByRut(item.RH_NumInte);

                        //si el usuario tiene jefatura => retornar los detalles del jefe
                        if (data.Funcionario != null && data.Jefatura != null)
                            return GetUserByRut(data.Jefatura.RH_NumInte);

                    }
                }
                using (var context = new AppContextTurismo())
                {
                    //traer los usuarios de la unidad
                    var users = from PEDATPER in context.PEDATPER
                                join r in context.PeDatLab on PEDATPER.RH_NumInte equals r.RH_NumInte
                                where PEDATPER.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                where r.RhConUniCod == codigo
                                where r.PeDatLabAdDocCor == (from ud in context.PeDatLab
                                                             where ud.RH_NumInte == PEDATPER.RH_NumInte
                                                             select ud.PeDatLabAdDocCor).Max()
                                select PEDATPER;

                    //iterar cada usuariode la unidad
                    foreach (var item in users)
                    {
                        // tarer los datos del usuario
                        var data = GetUserByRut(item.RH_NumInte);

                        //si el usuario tiene jefatura => retornar los detalles del jefe
                        if (data.Funcionario != null && data.Jefatura != null)
                            return GetUserByRut(data.Jefatura.RH_NumInte);

                    }
                }

            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }
        public Model.SIGPER.SIGPER GetSecretariaByUnidad(int codigo)
        {
            try
            {
                using (var context = new AppContextEconomia())
                {
                    //unidad del funcionario
                    var unidad = context.PLUNILAB.AsNoTracking().FirstOrDefault(q => q.Pl_UndCod == codigo);
                    if (unidad != null && !string.IsNullOrEmpty(unidad.Pl_UndNomSec)) 
                    {
                        //secretaria del funcionario
                        var secretaria = context.PEDATPER.AsNoTracking().FirstOrDefault(q => q.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase) && q.PeDatPerChq.ToUpper().Trim() == unidad.Pl_UndNomSec.ToUpper().Trim());
                        if (secretaria != null)
                            return GetUserByRut(secretaria.RH_NumInte);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }
        public List<DGREGIONES> GetRegion()
        {
            try
            {
                using (var context = new AppContextEconomia())
                {
                    return context.DGREGIONES.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DGCOMUNAS> GetDGCOMUNAs()
        {
            try
            {
                using (var context = new AppContextEconomia())
                {
                    return context.DGCOMUNAS.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DGESCALAFONES> GetGESCALAFONEs()
        {
            try
            {
                using (var context = new AppContextEconomia())
                {
                    return context.DGESCALAFONES.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DGCONTRATOS> GetDGCONTRATOS()
        {
            try
            {
                using (var context = new AppContextEconomia())
                {
                    return context.DGCONTRATOS.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PECARGOS> GetPECARGOs()
        {
            try
            {
                using (var context = new AppContextEconomia())
                {
                    return context.PECARGOS.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DGESTAMENTOS> GetDGESTAMENTOs()
        {
            try
            {
                using (var context = new AppContextEconomia())
                {
                    return context.DGESTAMENTOS.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ReContra> GetReContra()
        {
            try
            {
                using (var context = new AppContextEconomia())
                {
                    return context.ReContra.Where(c => c.ReContraSed != 0 /*&& c.Re_ConIni >= Convert.ToDateTime("2019-01-01")*/).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<REPYT> GetREPYTs()
        {
            try
            {
                using (var context = new AppContextEconomia())
                {
                    return context.REPYT.Where(c => c.RePytEst == "S").ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string GetRegionbyComuna(string codComuna)
        {
            try
            {
                using (var dbEconomia = new AppContextEconomia())
                {
                    var region = dbEconomia.DGCOMUNAS.Where(c => c.Pl_CodCom == codComuna).FirstOrDefault().Pl_CodReg;
                    var regionnombre = dbEconomia.DGREGIONES.Where(r => r.Pl_CodReg == region).FirstOrDefault().Pl_DesReg;
                    return regionnombre;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DGCOMUNAS> GetComunasbyRegion(string IdRegion)
        {
            try
            {
                using (var context = new AppContextEconomia())
                {
                    return context.DGCOMUNAS.Where(c => c.Pl_CodReg == IdRegion).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PEDATPER> GetAllUsersForCometido()
        {
            var returnValue = new List<PEDATPER>();

            using (var dbE = new AppContextEconomia())
            {
                var rE = from PE in dbE.PEDATPER
                             //join r in dbE.ReContra on PE.RH_NumInte equals r.RH_NumInte
                             //where r.Re_ConPyt != 0
                             //where r.Re_ConIni.Year >= DateTime.Now.Year || r.RH_ContCod == 1
                             //where r.Re_ConCar != 21
                             //from PL in dbE.PLUNILAB
                             //where (PL.Pl_UndCod == PE.RhSegUnd01.Value || PL.Pl_UndCod == PE.RhSegUnd02.Value || PL.Pl_UndCod == PE.RhSegUnd03.Value)
                         where PE.RH_EstLab.Equals("A")
                         select PE;
                returnValue.AddRange(rE.ToList());

            }

            /*solo se dejan los funcionarios de economia, ya que algunos estan duplicados en turismo  22092020*/
            //using (var dbT = new AppContextTurismo())
            //{
            //    var rT = from PE in dbT.PEDATPER
            //             //join r in dbT.ReContra on PE.RH_NumInte equals r.RH_NumInte
            //             //where r.Re_ConPyt != 0
            //             //where r.Re_ConIni.Year >= DateTime.Now.Year || r.RH_ContCod == 1
            //             //where r.Re_ConCar != 21
            //             //from PL in dbT.PLUNILAB
            //             //where (PL.Pl_UndCod == PE.RhSegUnd01.Value || PL.Pl_UndCod == PE.RhSegUnd02.Value || PL.Pl_UndCod == PE.RhSegUnd03.Value)
            //             where PE.RH_EstLab.Equals("A")
            //             select PE;
            //    returnValue.AddRange(rT.ToList());
            //}

            return returnValue.OrderBy(q => q.PeDatPerChq).ToList();
        }
        public List<PLUNILAB> GetUnidadesFirmantes(List<string> listEmailFirmantes)
        {
            var returnValue = new List<PLUNILAB>();

            foreach (var email in listEmailFirmantes)
            {
                var user = GetUserByEmail(email);
                if (user != null && user.Unidad != null && !returnValue.Any(q => q.Pl_UndCod == user.Unidad.Pl_UndCod))
                    returnValue.Add(GetUserByEmail(email).Unidad);
            }

            return returnValue.OrderBy(q => q.Pl_UndDes).ToList();
        }
        public List<PEDATPER> GetUserFirmanteByUnidad(int codigoUnidad, List<string> listEmailFirmantes)
        {
            var returnValue = new List<PEDATPER>();

            var funcionarios = GetUserByUnidad(codigoUnidad);
            foreach (var funcionario in funcionarios.Where(q => listEmailFirmantes.Contains(q.Rh_Mail.Trim())))
                returnValue.Add(funcionario);

            return returnValue;
        }
        /*nuevo metodo para traer datos de los funcionarios desde SIGPER - 05112020*/
        public Model.SIGPER.SIGPER NewGetUserByEmail(string email)
        {
            var sigper = new Model.SIGPER.SIGPER()
            {
                Funcionario = null,
                Jefatura = null,
                Secretaria = null,
                Unidad = null,
                FunDatosLaborales = null,
                SubSecretaria = null,
                datosLaborales = null
            };

            try
            {
                using (var context = new AppContextEconomia())
                {
                    var funcionario = context.PEDATPER.FirstOrDefault(q => q.Rh_Mail == email && q.RH_EstLab == "A");
                    if (funcionario != null)
                    {
                        sigper.Funcionario = funcionario;

                        //jefatura del funcionario
                        var jefatura = (from f in context.PEFERJEFAF
                                        join j in context.PEFERJEFAJ on f.PeFerJerCod equals j.PeFerJerCod
                                        join p in context.PEDATPER on j.FyPFunARut equals p.RH_NumInte
                                        where f.FyPFunRut == funcionario.RH_NumInte
                                        where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                        where j.PeFerJerAutEst == 1
                                        //where p.Rh_MailPer == null
                                        select p).FirstOrDefault();

                        if (jefatura != null)
                            sigper.Jefatura = jefatura;

                        //datos laborales del funcionario
                        var PeDatLab = context.PeDatLab.Where(q => q.RH_NumInte == funcionario.RH_NumInte && q.RH_ContCod != 13 && (q.RhConIni.Value.Year == 2020 || q.RH_ContCod == 1)).OrderByDescending(q => q.RH_Correla).FirstOrDefault();
                        if (PeDatLab != null)
                        {
                            var CodUnidad = (from u in context.PeDatLab
                                             join p in context.PEDATPER on u.RH_NumInte equals p.RH_NumInte
                                             where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                             where p.RH_NumInte == sigper.Funcionario.RH_NumInte
                                             where u.PeDatLabAdDocCor == (from ud in context.PeDatLab
                                                                          where ud.RH_NumInte == sigper.Funcionario.RH_NumInte
                                                                          select ud.PeDatLabAdDocCor).Max()
                                             select u.RhConUniCod).FirstOrDefault();

                            /*unidad del funcionario*/
                            var unidad = context.PLUNILAB.FirstOrDefault(q => q.Pl_UndCod == CodUnidad);
                            if (unidad != null)
                            {
                                sigper.Unidad = unidad;

                                //secretaria del funcionario
                                var secretaria = context.PEDATPER.FirstOrDefault(q => q.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase) && q.PeDatPerChq == unidad.Pl_UndNomSec);
                                if (secretaria != null)
                                    sigper.Secretaria = secretaria;
                            }

                            /*datos laborales funcionario*/
                            var FunDatosLaborales = context.PeDatLab.Where(q => q.RH_NumInte == funcionario.RH_NumInte && q.RH_ContCod != 13 && (q.RhConIni.Value.Year == 2020 || q.RH_ContCod == 1)).OrderByDescending(q => q.RH_Correla).FirstOrDefault();

                            if (FunDatosLaborales != null)
                            {
                                sigper.FunDatosLaborales = FunDatosLaborales;
                            }

                            /*se obtienen datos laborales desde tabla ReContra*/
                            var datosLaborales = (from rc in context.ReContra
                                                  join p in context.PEDATPER on rc.RH_NumInte equals p.RH_NumInte
                                                  join pl in context.PLUNILAB on rc.Re_ConUni equals pl.Pl_UndCod
                                                  join e in context.DGESCALAFONES on rc.Re_ConEsc equals e.Pl_CodEsc
                                                  join co in context.DGCONTRATOS on rc.RH_ContCod equals co.RH_ContCod
                                                  join pc in context.PECARGOS on rc.Re_ConCar equals pc.Pl_CodCar
                                                  join es in context.DGESTAMENTOS on rc.ReContraEst equals es.DgEstCod
                                                  where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                                  where p.RH_NumInte == sigper.Funcionario.RH_NumInte
                                                  where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                                  where rc.Re_ConIni == (from ud in context.ReContra
                                                                         where ud.RH_NumInte == sigper.Funcionario.RH_NumInte
                                                                         select ud.Re_ConIni).Max()
                                                  select rc).FirstOrDefault();

                            if (datosLaborales != null)
                            {
                                sigper.datosLaborales = datosLaborales;
                            }

                        }
                        else
                        {
                            PeDatLab = context.PeDatLab.Where(q => q.RH_NumInte == funcionario.RH_NumInte && q.RH_ContCod != 13 && (q.RhConIni.Value.Year == 2020 || q.RH_ContCod == 1)).OrderByDescending(q => q.RH_Correla).FirstOrDefault();

                            var CodUnidad = (from u in context.PeDatLab
                                             join p in context.PEDATPER on u.RH_NumInte equals p.RH_NumInte
                                             where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                             where p.RH_NumInte == sigper.Funcionario.RH_NumInte
                                             where u.PeDatLabAdDocCor == (from ud in context.PeDatLab
                                                                          where ud.RH_NumInte == sigper.Funcionario.RH_NumInte
                                                                          select ud.PeDatLabAdDocCor).Max()
                                             select u.RhConUniCod).FirstOrDefault();


                            //unidad del funcionario
                            var unidad = context.PLUNILAB.FirstOrDefault(q => q.Pl_UndCod == CodUnidad);
                            if (unidad != null)
                            {
                                sigper.Unidad = unidad;

                                //secretaria del funcionario
                                var secretaria = context.PEDATPER.FirstOrDefault(q => q.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase) && q.PeDatPerChq == unidad.Pl_UndNomSec);
                                if (secretaria != null)
                                    sigper.Secretaria = secretaria;
                            }

                            /*datos laborales funcionario*/
                            var FunDatosLaborales = context.PeDatLab.Where(q => q.RH_NumInte == funcionario.RH_NumInte && q.RH_ContCod != 13 && (q.RhConIni.Value.Year == 2020 || q.RH_ContCod == 1)).OrderByDescending(q => q.RH_Correla).FirstOrDefault();
                            if (FunDatosLaborales != null)
                            {
                                sigper.FunDatosLaborales = FunDatosLaborales;
                            }
                        }

                        sigper.SubSecretaria = "ECONOMIA";

                        return sigper;
                    }
                }

                using (var context = new AppContextTurismo())
                {
                    var funcionario = context.PEDATPER.FirstOrDefault(q => q.Rh_Mail == email);
                    if (funcionario != null)
                    {
                        sigper.Funcionario = funcionario;

                        //jefatura del funcionario
                        var jefatura = (from f in context.PEFERJEFAF
                                        join j in context.PEFERJEFAJ on f.PeFerJerCod equals j.PeFerJerCod
                                        join p in context.PEDATPER on j.FyPFunARut equals p.RH_NumInte
                                        where f.FyPFunRut == funcionario.RH_NumInte
                                        where p.RH_EstLab.Equals("A")
                                        where j.PeFerJerAutEst == 1
                                        where p.Rh_MailPer == null
                                        select p).FirstOrDefault();

                        if (jefatura != null)
                            sigper.Jefatura = jefatura;

                        //datos laborales del funcionario
                        var PeDatLab = context.PeDatLab.Where(q => q.RH_NumInte == funcionario.RH_NumInte && q.RH_ContCod != 13 && (q.RhConIni.Value.Year == 2020 || q.RH_ContCod == 1)).OrderByDescending(q => q.RH_Correla).FirstOrDefault();
                        if (PeDatLab != null)
                        {
                            var CodUnidad = (from u in context.PeDatLab
                                             join p in context.PEDATPER on u.RH_NumInte equals p.RH_NumInte
                                             where p.RH_EstLab.Equals("A", StringComparison.InvariantCultureIgnoreCase)
                                             where p.RH_NumInte == sigper.Funcionario.RH_NumInte
                                             where u.PeDatLabAdDocCor == (from ud in context.PeDatLab
                                                                          where ud.RH_NumInte == sigper.Funcionario.RH_NumInte
                                                                          select ud.PeDatLabAdDocCor).Max()
                                             select u.RhConUniCod).FirstOrDefault();

                            //unidad del funcionario
                            var unidad = context.PLUNILAB.FirstOrDefault(q => q.Pl_UndCod == CodUnidad);
                            if (unidad != null)
                            {
                                sigper.Unidad = unidad;

                                //secretaria del funcionario
                                var secretaria = context.PEDATPER.FirstOrDefault(q => q.RH_EstLab.Equals("A") && q.PeDatPerChq == unidad.Pl_UndNomSec);
                                if (secretaria != null)
                                    sigper.Secretaria = secretaria;
                            }

                            /*datos laborales funcionario*/
                            var FunDatosLaborales = context.PeDatLab.Where(q => q.RH_NumInte == funcionario.RH_NumInte && q.RH_ContCod != 13 && (q.RhConIni.Value.Year == 2020 || q.RH_ContCod == 1)).OrderByDescending(q => q.RH_Correla).FirstOrDefault();
                            if (FunDatosLaborales != null)
                            {
                                sigper.FunDatosLaborales = FunDatosLaborales;
                            }
                        }

                        sigper.SubSecretaria = "TURISMO";

                        return sigper;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return sigper;
        }
        public int GetBaseCalculoHorasExtras(int rut, int mes, int anno, int calidad)
        {
            try
            {
                using (var context = new AppContextEconomia())
                {
                    Decimal monto = 0;
                    if (calidad == 10)
                    {
                        /*honorarios*/
                        monto = (from R in context.RePagHisDet
                                 where R.RH_NumInte == rut
                                 where R.Re_Hismm == mes
                                 where R.Re_Hisyy == anno
                                 where R.RehDetObjTip == "H"
                                 //where results.Contains(R.RehDetObj)
                                 select R.RehDetObjMon).Sum().Value;
                    }
                    else
                    {
                        /*plata y contrata*/
                        var results = (from l in context.LREMREP1Level1
                                       where l.lrem_codrep == 36
                                       where l.lrem_tipo == 1
                                       select l.lrem_reforcod).ToList();

                        monto = (from R in context.RePagHisDet
                                 where R.RH_NumInte == rut
                                 where R.Re_Hismm == mes
                                 where R.Re_Hisyy == anno
                                 where results.Contains(R.RehDetObj)
                                 select R.RehDetObjMon).Sum().Value;
                    }

                    return Convert.ToInt32(monto);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}