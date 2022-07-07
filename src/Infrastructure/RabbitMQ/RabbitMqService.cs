using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Infrastructure.RabbitMQ;

public class RabbitMqService : IRabbitMqService
{
    private readonly RabbitMqFactory rabbitMqFactory;

    public RabbitMqService(RabbitMqFactory rabbitMqFactory) => this.rabbitMqFactory = rabbitMqFactory;

    public void SendMessage(object obj, string queueName)
    {
        string message = JsonSerializer.Serialize(obj);
        SendMessage(message, queueName);
    }

    public void SendMessage(string message, string queueName)
    {
        using IConnection connection = rabbitMqFactory.CreateConnection();
        using IModel? channel = connection.CreateModel();
        byte[] body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
    }
}
