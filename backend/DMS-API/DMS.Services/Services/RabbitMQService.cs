using DMS.Core.Entities;
using DMS.Services.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace DMS.Services.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQService> _logger;

        public RabbitMQService(ConnectionFactory connectionFactory, ILogger<RabbitMQService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public void SendMessage(ActionLog logEntry)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(
                    queue: "actionLogsQueue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var message = JsonSerializer.Serialize(logEntry);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: "actionLogsQueue",
                    basicProperties: null,
                    body: body);

                _logger.LogInformation("✔ Message sent to RabbitMQ: {ActionType} for document {DocumentName}",
                    logEntry.ActionType, logEntry.DocumentName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send message to RabbitMQ");
                // Don't throw - allow the app to continue without RabbitMQ
            }
        }
    }
}