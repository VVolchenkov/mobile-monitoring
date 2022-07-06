using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
IConnection? connection = factory.CreateConnection();
IModel? channel = connection.CreateModel();

var consumer = new EventingBasicConsumer(channel);
consumer.Received += ((sender, message) =>
{
    string content = Encoding.UTF8.GetString(message.Body.ToArray());

    Console.WriteLine($"Received message from {sender} with content: {content}");

    channel.BasicAck(message.DeliveryTag, false);
});

channel.BasicConsume("EventsSuccess", false, consumer);
channel.BasicConsume("EventsFailed", false, consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();


