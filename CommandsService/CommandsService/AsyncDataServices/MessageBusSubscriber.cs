﻿
using CommandsService.EventProcessing;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration  _config;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration config, IEventProcessor eventProcessor)
         {
            _config = config;     
            _eventProcessor = eventProcessor;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
           var factory = new ConnectionFactory() { HostName = _config["RabbitMQHost"], Port = int.Parse(_config["RabbitMQPort"]) };

            _connection = factory.CreateConnection();

             _channel = _connection.CreateModel();
             _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
             _queueName = _channel.QueueDeclare().QueueName;
             _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");
         
              Console.WriteLine($"--> Listening on the  MessageBus.");

             _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;

        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine($"--> Event Received!");

                var body = ea.Body; 
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());  

                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck:true, consumer: consumer);

            return Task.CompletedTask;  

        }




        public void Dispose(string message)
        {
            Console.WriteLine($"--> MessageBus Disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }


        private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine($"--> RabbitMQ Connection Shut Down.");
        }



    }
}
