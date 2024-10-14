using System;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OT.Assessment.Database;
using OT.Assessment.Models;
using OT.Assessment.Service.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using static System.Net.Mime.MediaTypeNames;

namespace OT.Assessment.Service
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IPlayerService _playerService;
        private readonly MessageQueuingSettings _settings;
        /// <summary>
        /// 
        /// </summary>
        public RabbitMQService(IOptions<MessageQueuingSettings> messageQueuingSettings, IPlayerService playerService)
        {
            _settings = messageQueuingSettings.Value;

            try
            {
                var factory = new ConnectionFactory() { HostName = _settings.HostName };
                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();

                _channel.QueueDeclare(
                    queue: _settings.CasinoWagerQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
            }
            catch(Exception) {
                CreateChannel();
            }

            _playerService = playerService;
        }

        /// <summary>
        /// Receives player casino wager events to publish to the local RabbitMQ queue - CasinoWagerQueue
        /// </summary>
        /// <param name="casinoWager"></param>
        /// <returns></returns>
        public async Task SendCasinoWagerAsync(CasinoWager casinoWager)
        {
            try
            {
                if (_channel.IsClosed)
                {
                    // Optionally, re-establish the connection and channel here
                    _channel = CreateChannel(); // Implement a method to create a new channel
                }

                var message = JsonConvert.SerializeObject(casinoWager);
                var body = Encoding.UTF8.GetBytes(message);

                // Publish the message
                _channel.BasicPublish(exchange: "", routingKey: _settings.CasinoWagerQueue, basicProperties: null, body: body);
                Console.WriteLine($"Sent {message}");
            }
            catch (AlreadyClosedException ex)
            {
                Console.WriteLine($"Error: Connection already closed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Example method to create a new channel
        private IModel CreateChannel()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = CreateConnection(); // Implement this to create a new connection
            }
            return _connection.CreateModel(); // Create a new channel
        }

        private IConnection? CreateConnection()
        {
            var factory = new ConnectionFactory() { HostName = _settings.HostName };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(
                queue: _settings.CasinoWagerQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            return connection;
        }

        /// <summary>
        /// Consume messages published to the CasinoWagerQueue and store consumed messages in the SQL database OT_Assessment_DB.
        /// </summary>
        /// <returns></returns>
        public async Task ReadCasinoWagerAsync()
        {
            try
            {
                // Create a consumer
                var wagerConsumer = new EventingBasicConsumer(_channel);

                wagerConsumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var casinoWager = JsonConvert.DeserializeObject<CasinoWager>(message);

                    if (casinoWager == null || casinoWager.WagerId == Guid.Empty)
                    {
                        // Handle invalid WagerId
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                        return;
                    }

                    bool isDuplicate = await _playerService.IsDuplicate(casinoWager.WagerId);
                    if (isDuplicate)
                    {
                        // Handle duplicate 
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                        return;
                    }

                    var saveResult = await _playerService.SaveCasinoWagerAsync(casinoWager);
                    if (!saveResult)
                    {
                        // Handle not saved
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                        return;
                    }

                    // Acknowledge the message only after successful processing
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                // Start consuming messages
                _channel.BasicConsume(queue: _settings.CasinoWagerQueue,
                                       autoAck: false, 
                                       consumer: wagerConsumer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
