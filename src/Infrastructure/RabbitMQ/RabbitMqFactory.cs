using Infrastructure.Configuration;
using RabbitMQ.Client;

namespace Infrastructure.RabbitMQ;

public class RabbitMqFactory
{
    private readonly RabbitMqConfiguration rabbitMqConfiguration;

    public RabbitMqFactory(RabbitMqConfiguration rabbitMqConfiguration)
    {
        this.rabbitMqConfiguration = rabbitMqConfiguration;
        InitializeQueues();
    }

    public IConnection CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = rabbitMqConfiguration.HostName,
            UserName = rabbitMqConfiguration.UserName,
            Password = rabbitMqConfiguration.Password,
        };

        return factory.CreateConnection();
    }

    private void InitializeQueues()
    {
        if (rabbitMqConfiguration.HostName == "test")
        {
            return;
        }

        using IConnection connection = CreateConnection();
        using IModel? channel = connection.CreateModel();
        channel.QueueDeclare(queue: "EventsSuccess", durable: false, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueDeclare(queue: "EventsFailed", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }
}
