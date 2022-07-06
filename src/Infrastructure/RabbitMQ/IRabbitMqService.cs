namespace Infrastructure.RabbitMQ;

public interface IRabbitMqService
{
    void SendMessage(object obj, string queueName);
    void SendMessage(string message, string queueName);

}
