using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("RabbitMQ Integration CONSUMER version 1.0");
                Console.WriteLine("Author: Justin Reddy");
                Console.WriteLine("Release Date: 16 November 2017");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;

                const string EXCHANGE_NAME = "RabbitMQ.Demo.TOPIC";
                ConnectionFactory factory = new ConnectionFactory();

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        //channel.ExchangeDeclare(EXCHANGE_NAME, "fanout");
                        channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Topic, true, false, null);
                        string queueName = channel.QueueDeclare(queue: "RabbitMQ.Demo",
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);
                        Console.WriteLine("QueueDeclare().Name - {0}", queueName);
                        Console.WriteLine("QueueDeclare().IsOpen - {0}", channel.IsOpen);
                        channel.QueueBind(queueName, EXCHANGE_NAME, "");

                        var consumer = new EventingBasicConsumer(channel);

                        Console.WriteLine("EventingBasicConsumer().IsRunning - {0}", consumer.IsRunning);

                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine("consumer.Received().message - {0}", message);
                            Console.WriteLine("consumer.Received().body - {0}", body);
                            Console.WriteLine(" [x] Received {0}", message);
                        };
                        string consumerTag = channel.BasicConsume(queueName, true, consumer);
                        Console.WriteLine("consumer.consumerTag - {0}", consumerTag);
                        channel.QueueBind(queueName, EXCHANGE_NAME, "myTopic");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadKey();
                        channel.QueueUnbind(queueName, EXCHANGE_NAME, "myTopic", null);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("General Error Occured");
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadKey();
        }
    }
}
