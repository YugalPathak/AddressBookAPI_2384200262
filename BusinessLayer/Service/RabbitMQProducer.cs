using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;

namespace BusinessLayer.Services
{
    /// <summary>
    /// This class is responsible for publishing messages to a RabbitMQ queue.
    /// It creates a connection to the RabbitMQ server and sends messages in JSON format.
    /// </summary>
    public class RabbitMQProducer
    {
        private readonly string _hostname = "localhost"; // RabbitMQ server hostname
        private readonly string _queueName; // Name of the queue to publish messages

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMQProducer"/> class with the specified queue name.
        /// </summary>
        /// <param name="queueName">The name of the RabbitMQ queue to which messages will be published.</param>
        public RabbitMQProducer(string queueName)
        {
            _queueName = queueName;
        }

        /// <summary>
        /// Publishes a message to the specified RabbitMQ queue.
        /// </summary>
        /// <typeparam name="T">The type of the message being sent.</typeparam>
        /// <param name="message">The message object to be serialized and sent to the queue.</param>
        public void PublishMessage<T>(T message)
        {
            // Create a connection factory for RabbitMQ
            var factory = new ConnectionFactory() { HostName = _hostname };

            // Establish connection and channel with RabbitMQ
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declare a queue in case it does not exist
                channel.QueueDeclare(queue: _queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Convert the message object to JSON format
                var jsonMessage = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                // Publish the message to the specified queue
                channel.BasicPublish(exchange: "",
                                     routingKey: _queueName,
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}