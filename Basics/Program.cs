using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Demo
{
    /// <summary>
    /// http://www.jarloo.com/listening-to-rabbitmq-events/
    /// https://cmatskas.com/getting-started-with-rabbitmq-on-windows/
    /// https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("RabbitMQ Integration Demo version 1.0");
                Console.WriteLine("Author: Justin Reddy");
                Console.WriteLine("Release Date: 17 March 2017");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;

                const string EXCHANGE_NAME = "RabbitMQ.Demo";

                ConnectionFactory factory = new ConnectionFactory();
                //var factory = new ConnectionFactory()
                //{
                //    HostName = "localhost",
                //    UserName = "guest",
                //    Password = "guest",
                //    VirtualHost = "/",
                //    Port = 5672
                //};

                Console.WriteLine("ConnectionFactory() - {0}", factory.HostName);

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        factory.HostName = "localhost";
                        channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Fanout, false, false, null);

                        //string queueName = channel.QueueDeclare(queue: "RabbitMQ.Demo",
                        //         durable: true,
                        //         exclusive: false,
                        //         autoDelete: false,
                        //         arguments: null);
                        //Console.WriteLine("QueueDeclare().Name - {0}", queueName);
                        Console.WriteLine("QueueDeclare().IsOpen - {0}", channel.IsOpen);

                        for (int i = 0; i < 100; i++)
                        {
                            string message = $"Hello World! [{i}]";
                            var body = Encoding.ASCII.GetBytes(message);

                            //channel.BasicPublish(exchange: EXCHANGE_NAME,
                            //    routingKey: "myTopic",
                            //    basicProperties: null,
                            //    body: body);
                            channel.BasicPublish(EXCHANGE_NAME, "myTopic", null, body);
                            Console.WriteLine("BasicPublish().message - {0}", message);
                            Console.WriteLine(" [x] {0} Sent {1}", i, message);
                            Thread.Sleep(500);
                        }
                    }

                }
                Console.WriteLine(" Press [enter] to De-Queue.");
                Console.ReadLine();

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        //channel.ExchangeDeclare(EXCHANGE_NAME, "fanout");
                        channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Fanout, false, false, null);
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
