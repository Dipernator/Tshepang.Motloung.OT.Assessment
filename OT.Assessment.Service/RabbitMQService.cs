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
using static System.Net.Mime.MediaTypeNames;

namespace OT.Assessment.Service
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ApplicationDbContext _dataBaseContext;

        /// <summary>
        /// 
        /// </summary>
        public RabbitMQService(IOptions<MessageQueuingSettings> messageQueuingSettings, ApplicationDbContext dataBaseContext)
        {
            var settings = messageQueuingSettings.Value;

            var factory = new ConnectionFactory() { HostName = settings.HostName };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(
                queue: settings.CasinoWagerQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _dataBaseContext = dataBaseContext;
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
                await Task.Run(() =>
                {
                    var message = JsonConvert.SerializeObject(casinoWager);
                    var body = Encoding.UTF8.GetBytes(message);
                    _channel.BasicPublish(exchange: "", routingKey: "CasinoWagerQueue", basicProperties: null, body: body);
                    Console.WriteLine($" [x] Sent {message}");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
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

                    bool isDuplicate = await IsDuplicate(casinoWager.WagerId);
                    if (isDuplicate)
                    {
                        // Handle duplicate 
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                        return;
                    }

                    var saveResult = await SaveCasinoWagerAsync(casinoWager);
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
                _channel.BasicConsume(queue: "CasinoWagerQueue",
                                       autoAck: false, 
                                       consumer: wagerConsumer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Save casino wager
        /// </summary>
        /// <param name="casinoWager"></param>
        /// <returns></returns>
        public async Task<bool> SaveCasinoWagerAsync(CasinoWager casinoWager) {
            try {
                _dataBaseContext.CasinoWager.AddAsync(casinoWager);
                if (_dataBaseContext.SaveChanges() > 0) {
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Check for Duplicate using wagerId
        /// </summary>
        /// <param name="wagerId"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicate(Guid wagerId)
        {
            return _dataBaseContext.CasinoWager
                .Any(w => w.WagerId == wagerId);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
