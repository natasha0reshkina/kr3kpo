using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace SalesService.Handlers;
public class PaymentConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    public PaymentConsumer(IServiceScopeFactory scopeFactory, ILogger<PaymentConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_connection == null || !_connection.IsOpen)
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = "rabbitmq",
                        UserName = "guest",
                        Password = "guest",
                        DispatchConsumersAsync = true
                    };
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    _channel.ExchangeDeclare(exchange: "sales-status", type: ExchangeType.Fanout, durable: true);
                    _channel.QueueDeclare(queue: "settlement-events", 
                                          durable: true,
                                          exclusive: false,
                                          autoDelete: false,
                                          arguments: null);
                    _channel.QueueBind(queue: "settlement-events", exchange: "sales-status", routingKey: "");
                    var consumer = new AsyncEventingBasicConsumer(_channel);
                    consumer.Received += async (model, ea) =>
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var orderService = scope.ServiceProvider.GetRequiredService<OrderService>();
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        _logger.LogInformation("Received settlement message: {Message}", message);
                        var paymentMessage = JsonSerializer.Deserialize<PaymentEventMessage>(message);
                        if (paymentMessage != null)
                        {
                            if (paymentMessage.Type == "settlement-completed")
                            {
                                await orderService.MarkOrderAsPaidAsync(paymentMessage.OrderId);
                            }
                            else if (paymentMessage.Type == "settlement-failed")
                            {
                                await orderService.MarkOrderAsCancelledAsync(paymentMessage.OrderId);
                            }
                        }
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    _channel.BasicConsume(queue: "settlement-events", autoAck: false, consumer: consumer);
                    _logger.LogInformation("Started consuming settlement-events queue.");
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PaymentConsumer. Will retry in 5 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
public class PaymentEventMessage
{
    public Guid OrderId { get; set; }
    public string Type { get; set; } = string.Empty; 
}