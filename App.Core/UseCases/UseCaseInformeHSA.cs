using System;
using App.Model.Core;
using App.Core.Interfaces;
using App.Model.InformeHSA;
using App.Util;
using App.Model.SIGPER;
using System.Linq;

namespace App.Core.UseCases
{
    public class UseCaseInformeHSA
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IEmail _email;

        public UseCaseInformeHSA(IGestionProcesos repositoryGestionProcesos, ISIGPER sigper, IEmail email)
        {
            _repository = repositoryGestionProcesos;
            _sigper = sigper;
            _email = email;
        }

        public ResponseMessage Insert(InformeHSA obj)
        {
            var response = new ResponseMessage();

            if (!obj.FechaDesde.HasValue)
                response.Errors.Add("Debe especificar la fecha desde");
            if (!obj.FechaHasta.HasValue)
                response.Errors.Add("Debe especificar la fecha hasta");
            if (obj.Funciones.IsNullOrWhiteSpace())
                response.Errors.Add("Debe especificar las funciones");
            if (obj.Actividades.IsNullOrWhiteSpace())
                response.Errors.Add("Debe especificar las actividades");
            if (obj.NumeroBoleta.IsNullOrWhiteSpace())
                response.Errors.Add("Debe especificar las actividades");

            if (response.IsValid)
            {
                try
                {
                    _repository.Create(obj);
                    _repository.Save();
                }
                catch (Exception ex)
                {
                    response.Errors.Add(ex.Message);
                }
            }

            return response;
        }
        public ResponseMessage Update(InformeHSA obj)
        {
            var response = new ResponseMessage();

            if (!obj.FechaDesde.HasValue)
                response.Errors.Add("Debe especificar la fecha desde");
            if (!obj.FechaHasta.HasValue)
                response.Errors.Add("Debe especificar la fecha hasta");
            if (obj.Funciones.IsNullOrWhiteSpace())
                response.Errors.Add("Debe especificar las funciones");
            if (obj.Actividades.IsNullOrWhiteSpace())
                response.Errors.Add("Debe especificar las actividades");
            if (obj.NumeroBoleta.IsNullOrWhiteSpace())
                response.Errors.Add("Debe especificar las actividades");

            if (response.IsValid)
            {
                try
                {
                    _repository.Update(obj);
                    _repository.Save();
                }
                catch (Exception ex)
                {
                    response.Errors.Add(ex.Message);
                }
            }

            return response;
        }
        public ResponseMessage InicioExterno(Proceso obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (!_repository.GetExists<DefinicionProceso>(q => q.DefinicionProcesoId == obj.DefinicionProcesoId))
                    throw new ArgumentNullException("No se encontró la definición del proceso");

                if (string.IsNullOrWhiteSpace(obj.Email))
                    throw new ArgumentNullException("No se encontró el usuario que ejecutó el workflow.");

                var definicionProceso = _repository.GetById<DefinicionProceso>(obj.DefinicionProcesoId);
                if (definicionProceso == null)
                    throw new ArgumentNullException("No se encontró la definición proceso.");

                var definicionWorkflow = _repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == obj.DefinicionProcesoId).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId).FirstOrDefault();
                if (definicionWorkflow == null)
                    throw new ArgumentNullException("No se encontró la definición de tarea del proceso asociado al workflow.");

                var persona = new SIGPER();
                persona = _sigper.GetUserByEmail(obj.Email);

                var proceso = new Proceso();
                proceso.DefinicionProcesoId = obj.DefinicionProcesoId;
                proceso.Observacion = obj.Observacion;
                proceso.FechaCreacion = DateTime.Now;
                //proceso.FechaVencimiento = DateTime.Now.AddBusinessDays(definicionProceso.DuracionHoras);
                proceso.CalcularFechaVencimiento(_repository.Get<Festivo>().Select(q=>q.Fecha).ToList());
                proceso.FechaTermino = null;
                proceso.Email = obj.Email;
                proceso.EstadoProcesoId = (int)App.Util.Enum.EstadoProceso.EnProceso;
                proceso.NombreFuncionario = persona != null && persona.Funcionario != null ? persona.Funcionario.PeDatPerChq.Trim() : null;

                var workflow = new Workflow();
                workflow.FechaCreacion = DateTime.Now;
                workflow.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.SinAprobacion;
                workflow.Terminada = false;
                workflow.Proceso = proceso;
                workflow.DefinicionWorkflow = definicionWorkflow;
                //workflow.FechaVencimiento = DateTime.Now.AddBusinessDays(definicionWorkflow.DefinicionProceso.DuracionHoras);
                workflow.FechaVencimiento = proceso.FechaVencimiento;

                switch (definicionWorkflow.TipoEjecucionId)
                {
                    case (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso:

                        persona = _sigper.GetUserByEmail(proceso.Email);
                        if (persona.Funcionario == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;

                        break;

                    case (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso:

                        persona = _sigper.GetUserByEmail(proceso.Email);
                        if (persona.Funcionario == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflow.Email = persona.Jefatura.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;

                        break;

                    case (int)App.Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico:

                        workflow.GrupoId = definicionWorkflow.GrupoId;
                        workflow.Pl_UndCod = definicionWorkflow.Pl_UndCod;
                        workflow.Pl_UndDes = definicionWorkflow.Pl_UndDes;
                        workflow.TareaPersonal = false;

                        break;


                    case (int)App.Util.Enum.TipoEjecucion.EjecutaUsuarioEspecifico:

                        persona = _sigper.GetUserByEmail(definicionWorkflow.Email);
                        if (persona.Funcionario == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");

                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes.Trim();
                        workflow.TareaPersonal = true;

                        break;
                }

                proceso.Workflows.Add(workflow);

                _repository.Create(proceso);
                _repository.Save();

                //notificar al dueño del proceso
                if (workflow.DefinicionWorkflow.NotificarAlAutor)
                    _email.NotificarInicioProceso(proceso,
                    _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(App.Util.Enum.Configuracion.plantilla_nuevo_proceso)),
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion));

                response.EntityId = proceso.ProcesoId;
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}