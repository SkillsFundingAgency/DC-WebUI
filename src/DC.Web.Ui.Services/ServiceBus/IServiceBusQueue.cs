using System.Threading.Tasks;

namespace DC.Web.Ui.Services.ServiceBus
{
    public interface IServiceBusQueue
    {
        Task SendMessagesAsync(string messageToSend, string sessionId);
    }
}