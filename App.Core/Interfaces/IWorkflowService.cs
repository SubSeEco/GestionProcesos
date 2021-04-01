using App.Model.DTO;
using System.Collections.Generic;

namespace App.Core.Interfaces
{
    public interface IWorkflowService
    {
        List<WorkflowDTO> GetPendingTask(Model.SIGPER.SIGPER user);
    }
}