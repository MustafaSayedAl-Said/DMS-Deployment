using DMS.Core.Entities;

namespace DMS.Services.Interfaces
{
    public interface IRabbitMQService
    {
        public void SendMessage(ActionLog logEntry);
    }
}
