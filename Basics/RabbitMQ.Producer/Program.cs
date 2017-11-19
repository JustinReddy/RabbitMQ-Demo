using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Producer
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("RabbitMQ Integration PRODUCER version 1.0");
                Console.WriteLine("Author: Justin Reddy");
                Console.WriteLine("Release Date: 16 November 2017");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;

                const string EXCHANGE_NAME = "RabbitMQ.Demo.TOPIC";

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
                        channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Topic, true, false, null);

                        //string queueName = channel.QueueDeclare(queue: "RabbitMQ.Demo",
                        //         durable: true,
                        //         exclusive: false,
                        //         autoDelete: false,
                        //         arguments: null);
                        //Console.WriteLine("QueueDeclare().Name - {0}", queueName);
                        Console.WriteLine("QueueDeclare().IsOpen - {0}", channel.IsOpen);
                        Console.WriteLine("Press ESC to stop");
                        do
                        {
                            string message = $"{Guid.NewGuid()} [{DateTime.Now}]";
                            var body = Encoding.ASCII.GetBytes(message);

                            //channel.BasicPublish(exchange: EXCHANGE_NAME,
                            //    routingKey: "myTopic",
                            //    basicProperties: null,
                            //    body: body);

                            Console.WriteLine("BasicPublish().message - {0}", message);
                            channel.BasicPublish(EXCHANGE_NAME, "myTopic", null, body);
                            Console.WriteLine(" [x] Sent {0}", message);
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                break;

                        }
                        while (true);
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
