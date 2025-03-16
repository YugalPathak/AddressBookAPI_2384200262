using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    /// <summary>
    /// This class is responsible for consuming messages from a RabbitMQ queue.
    /// It listens for new messages and processes them asynchronously.
    /// </summary>
    public class RabbitMQConsumer
    {
        private readonly string _hostname = "localhost"; // RabbitMQ server hostname
        private readonly string _queueName; // Name of the queue to consume messages from

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMQConsumer"/> class with the specified queue name.
        /// </summary>
        /// <param name="queueName">The name of the RabbitMQ queue from which messages will be consumed.</param>
        public RabbitMQConsumer(string queueName)
        {
            _queueName = queueName;
        }

        /// <summary>
        /// Starts listening for messages from the specified RabbitMQ queue.
        /// Messages are processed asynchronously.
        /// </summary>
        public void StartListening()
        {
            // Create a connection factory for RabbitMQ
            var factory = new ConnectionFactory() { HostName = _hostname };

            // Establish connection and channel with RabbitMQ
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            // Declare a queue in case it does not exist
            channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Create a consumer to receive messages
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                // Read and decode the message body
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Received Message: {message}");

                // Process the message asynchronously
                Task.Run(() => ProcessMessage(message));
            };

            // Start consuming messages from the queue
            channel.BasicConsume(queue: _queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        /// <summary>
        /// Processes the received message. This can include sending an email or other business logic.
        /// </summary>
        /// <param name="message">The received message in JSON format.</param>
        private void ProcessMessage(string message)
        {
            Console.WriteLine($"Processing Message: {message}");
            // Add email sending logic here
        }
    }
}