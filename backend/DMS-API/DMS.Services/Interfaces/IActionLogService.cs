using DMS.Core.Dto;
using DMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Services.Interfaces
{
    public interface IActionLogService
    {

        public Task<IReadOnlyList<ActionLogsDto>> GetAllActionLogs();

        public Task<bool> AddActionLogAsync(ActionLog actionLog);
    }
}
