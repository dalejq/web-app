using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            const string queueName = "testqueue";
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "172.18.0.2",
                UserName = "guest",
                Password = "guest",
                Port = 5672,
            };

            using (var rabbitConnection = connectionFactory.CreateConnection())
            {
                using (var channel = rabbitConnection.CreateModel())
                {
                    // Declaring a queue is idempotent 
                    channel.QueueDeclare(
                        queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    string body = $"A nice random message: {DateTime.Now.Ticks}";
                    channel.BasicPublish(
                        exchange: string.Empty,
                        routingKey: queueName,
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(body));
                    Console.WriteLine("Message sent");

                }
            }
            return View();
        }
 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
