using DMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Core.Dto
{
    public class ActionLogsDto
    {

        public int Id { get; set; }
        public int ActionType { get; set; }
        public int? UserId { get; set; }

        public int? DocumentId { get; set; }

        public string UserName { get; set; }

        public string DocumentName { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
