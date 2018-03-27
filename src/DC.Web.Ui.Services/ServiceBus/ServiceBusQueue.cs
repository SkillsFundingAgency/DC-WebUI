using DC.Web.Ui.Settings.Models;
using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DC.Web.Ui.Services.ServiceBus
{
    public class ServiceBusQueue : IServiceBusQueue
    {
        private readonly IQueueClient _queueClient;
        public ServiceBusQueue(IQueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        public async Task SendMessagesAsync(string messageToSend, string sessionId)
        {
            try
            {

                var message = new Message(Encoding.UTF8.GetBytes(messageToSend))
                {
                    SessionId = sessionId
                };

                // Send the message to the queue.
                await _queueClient.SendAsync(message);
                await _queueClient.CloseAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
                throw;
            }
        }
    }
}