using App.Core.Interfaces;
using App.Model.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace App.Infrastructure.GestionProcesos
{
    public class WorkflowService : IWorkflowService
    {
        public List<WorkflowDTO> GetPendingTask(Model.Sigper.Sigper user)
        {
            var result = new List<WorkflowDTO>();

            using (var _context = new AppContext())
            {
                _context.Configuration.AutoDetectChangesEnabled = false;
                _context.Configuration.ValidateOnSaveEnabled = false; 
                _context.Configuration.LazyLoadingEnabled = false;

                var userIsAdmin = _context.Usuario.AsNoTracking().Any(q => q.Habilitado && q.Email == user.Funcionario.Rh_Mail && q.Grupo.Nombre.Contains(Util.Enum.Grupo.Administrador.ToString()));

                if (userIsAdmin)
                {
                    result.AddRange(
                        (from w in _context.Workflow
                         join p in _context.Proceso on w.ProcesoId equals p.ProcesoId
                         join gd in _context.GD.Include(q => q.GDOrigen) on p.ProcesoId equals gd.ProcesoId into grupo
                         from x in grupo.DefaultIfEmpty()
                         where !w.Terminada
                         where w.TareaPersonal
                        select new WorkflowDTO
                        {
                            WorkflowId = w.WorkflowId,
                            FechaCreacion = w.FechaCreacion,
                            Asunto = w.Asunto,
                            Definicion = w.DefinicionWorkflow.Nombre,
                            TareaPersonal = w.TareaPersonal,
                            NombreFuncionario = w.NombreFuncionario,
                            Pl_UndDes = w.Pl_UndDes,
                            Grupo = w.Grupo != null ? w.Grupo.Nombre : string.Empty,
                            Mensaje = w.Mensaje,
                            ProcesoId = w.ProcesoId,
                            ProcesoFechaVencimiento = p.FechaVencimiento,
                            ProcesoDefinicion = p.DefinicionProceso.Nombre,
                            ProcesoNombreFuncionario = p.NombreFuncionario,
                            ProcesoEmail = p.Email,
                            ProcesoEntidad = p.DefinicionProceso.Entidad.Codigo,
                            GD = x
                        }
                        ).ToList());

                    result.AddRange(
                         (from w in _context.Workflow
                         join p in _context.Proceso on w.ProcesoId equals p.ProcesoId
                         join gd in _context.GD.Include(q => q.GDOrigen) on p.ProcesoId equals gd.ProcesoId into grupo
                         from x in grupo.DefaultIfEmpty()
                         where !w.Terminada
                         where !w.TareaPersonal
                          select new WorkflowDTO
                          {
                              WorkflowId = w.WorkflowId,
                              FechaCreacion = w.FechaCreacion,
                              Asunto = w.Asunto,
                              Definicion = w.DefinicionWorkflow.Nombre,
                              TareaPersonal = w.TareaPersonal,
                              NombreFuncionario = w.NombreFuncionario,
                              Pl_UndDes = w.Pl_UndDes,
                              Grupo = w.Grupo != null ? w.Grupo.Nombre : string.Empty,
                              Mensaje = w.Mensaje,
                              ProcesoId = w.ProcesoId,
                              ProcesoFechaVencimiento = p.FechaVencimiento,
                              ProcesoDefinicion = p.DefinicionProceso.Nombre,
                              ProcesoNombreFuncionario = p.NombreFuncionario,
                              ProcesoEmail = p.Email,
                              ProcesoEntidad = p.DefinicionProceso.Entidad.Codigo,
                              GD = x
                          }
                         ).ToList());
                }

                if (!userIsAdmin)
                {
                    var gruposEspeciales = _context.Usuario.AsNoTracking().Where(q => q.Email == user.Funcionario.Rh_Mail).Select(q => q.GrupoId).ToList();

                    result.AddRange(
                        (from w in _context.Workflow
                        join p in _context.Proceso on w.ProcesoId equals p.ProcesoId
                        join gd in _context.GD.Include(q => q.GDOrigen) on p.ProcesoId equals gd.ProcesoId into grupo
                        from x in grupo.DefaultIfEmpty()
                        where !w.Terminada
                        where w.Email == user.Funcionario.Rh_Mail
                         where w.TareaPersonal
                        select new WorkflowDTO
                        {
                            WorkflowId = w.WorkflowId,
                            FechaCreacion = w.FechaCreacion,
                            Asunto = w.Asunto,
                            Definicion = w.DefinicionWorkflow.Nombre,
                            TareaPersonal = w.TareaPersonal,
                            NombreFuncionario = w.NombreFuncionario,
                            Pl_UndDes = w.Pl_UndDes,
                            Grupo = w.Grupo != null ? w.Grupo.Nombre : string.Empty,
                            Mensaje = w.Mensaje,
                            ProcesoId = w.ProcesoId,
                            ProcesoFechaVencimiento = p.FechaVencimiento,
                            ProcesoDefinicion = p.DefinicionProceso.Nombre,
                            ProcesoNombreFuncionario = p.NombreFuncionario,
                            ProcesoEmail = p.Email,
                            ProcesoEntidad = p.DefinicionProceso.Entidad.Codigo,
                            GD = x
                        }
                        ).ToList());

                    result.AddRange(
                         (from w in _context.Workflow
                         join p in _context.Proceso on w.ProcesoId equals p.ProcesoId
                         join gd in _context.GD.Include(q => q.GDOrigen) on p.ProcesoId equals gd.ProcesoId into grupo
                         from x in grupo.DefaultIfEmpty()
                         where !w.Terminada
                         where !w.TareaPersonal
                         where w.Pl_UndCod == user.Unidad.Pl_UndCod
                         select new WorkflowDTO
                         {
                             WorkflowId = w.WorkflowId,
                             FechaCreacion = w.FechaCreacion,
                             Asunto = w.Asunto,
                             Definicion = w.DefinicionWorkflow.Nombre,
                             TareaPersonal = w.TareaPersonal,
                             NombreFuncionario = w.NombreFuncionario,
                             Pl_UndDes = w.Pl_UndDes,
                             Grupo = w.Grupo != null ? w.Grupo.Nombre : string.Empty,
                             Mensaje = w.Mensaje,
                             ProcesoId = w.ProcesoId,
                             ProcesoFechaVencimiento = p.FechaVencimiento,
                             ProcesoDefinicion = p.DefinicionProceso.Nombre,
                             ProcesoNombreFuncionario = p.NombreFuncionario,
                             ProcesoEmail = p.Email,
                             ProcesoEntidad = p.DefinicionProceso.Entidad.Codigo,
                             GD = x
                         }
                         ).ToList());

                    result.AddRange(
                        (from w in _context.Workflow
                        join p in _context.Proceso on w.ProcesoId equals p.ProcesoId
                        join gd in _context.GD.Include(q => q.GDOrigen) on p.ProcesoId equals gd.ProcesoId into grupo
                        from x in grupo.DefaultIfEmpty()
                        where !w.Terminada
                        where !w.TareaPersonal
                        where gruposEspeciales.Contains(w.GrupoId.Value)
                        select new WorkflowDTO
                        {
                            WorkflowId = w.WorkflowId,
                            FechaCreacion = w.FechaCreacion,
                            Asunto = w.Asunto,
                            Definicion = w.DefinicionWorkflow.Nombre,
                            TareaPersonal = w.TareaPersonal,
                            NombreFuncionario = w.NombreFuncionario,
                            Pl_UndDes = w.Pl_UndDes,
                            Grupo = w.Grupo != null ? w.Grupo.Nombre : string.Empty,
                            Mensaje = w.Mensaje,
                            ProcesoId = w.ProcesoId,
                            ProcesoFechaVencimiento = p.FechaVencimiento,
                            ProcesoDefinicion = p.DefinicionProceso.Nombre,
                            ProcesoNombreFuncionario = p.NombreFuncionario,
                            ProcesoEmail = p.Email,
                            ProcesoEntidad = p.DefinicionProceso.Entidad.Codigo,
                            GD = x
                        }
                        ).ToList());
                }

                return result.ToList();
            }
        }
    }
}