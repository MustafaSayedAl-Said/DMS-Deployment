using CodeChamp.RabbitMQ;
using DMS.API.Hubs;
using DMS.Core.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DMS.API.Service
{
    public class LogConsumerService : BackgroundService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly MqConsumer _consumer;
        private readonly ConnectionFactory _connectionFactory;
        private readonly ILogger<LogConsumerService> _logger;
        private IConnection? _connection;

        public LogConsumerService(
            IHubContext<NotificationHub> hubContext,
            MqConsumer consumer,
            ConnectionFactory connectionFactory,
            ILogger<LogConsumerService> logger)
        {
            _hubContext = hubContext;
            _consumer = consumer;
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Retry logic to wait for RabbitMQ
            var retryCount = 0;
            const int maxRetries = 10;

            while (!stoppingToken.IsCancellationRequested && retryCount < maxRetries)
            {
                try
                {
                    _logger.LogInformation("Attempting to connect to RabbitMQ (attempt {Attempt}/{Max})...",
                        retryCount + 1, maxRetries);

                    // Use the injected ConnectionFactory (configured in Program.cs)
                    _connection = _connectionFactory.CreateConnection();
                    var channel = _connection.CreateModel();

                    channel.QueueDeclare(
                        queue: "actionLogsQueue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    _logger.LogInformation("✔ Successfully connected to RabbitMQ");

                    // Setup your consumer listener
                    _consumer.AddListener("actionLogsQueue", async (_, args) =>
                    {
                        var body = args.Body.ToArray();
                        var postJson = Encoding.UTF8.GetString(body);
                        _logger.LogInformation("Received notification: {Message}", postJson);
                        await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", postJson);
                    });

                    _logger.LogInformation("✔ RabbitMQ consumer is now listening on queue 'actionLogsQueue'");

                    // Keep the service running
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                    break;
                }
                catch (Exception ex) when (retryCount < maxRetries - 1)
                {
                    retryCount++;
                    _logger.LogWarning(ex,
                        "Failed to connect to RabbitMQ. Retrying in 5 seconds... ({Attempt}/{Max})",
                        retryCount, maxRetries);

                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect to RabbitMQ after {Max} attempts. Service will exit.", maxRetries);
                    throw;
                }
            }
        }

        public override void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}