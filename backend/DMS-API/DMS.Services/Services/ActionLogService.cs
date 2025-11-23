using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Core.Dto;
using DMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Reflection.Metadata.Ecma335;

namespace DMS.Services.Services
{
    public class ActionLogService : IActionLogService
    {
        private readonly IUnitOfWork _uOW;
        private readonly IMapper _mapper;

        public ActionLogService(IUnitOfWork UOW, IMapper mapper)
        {
            _uOW = UOW;
            _mapper = mapper;
            
        }

        public async Task<IReadOnlyList<ActionLogsDto>> GetAllActionLogs()
        {
            var allActionLogs = await _uOW.actionLogRepository.GetAllAsync();
            if(allActionLogs == null)
            {
                throw new Exception("No Action Logs Found");
            }
            return _mapper.Map<IReadOnlyList<ActionLogsDto>>(allActionLogs);
        }

        public async Task<bool> AddActionLogAsync(ActionLog actionLog)
        {
            if (actionLog == null)
            {
                throw new ArgumentException(nameof(actionLog));
            }

            await _uOW.actionLogRepository.AddAsync(actionLog);
            return true;
        }
    }
}
